ItemData DistractionGrenadeItem {
   heading = $InvHeading[Ammo];
	description = "Distraction Grenade";
	className = "Ammo";
   shapeFile  = "grenade";
	hudIcon = "blaster";
	shadowDetailMask = 4;
	price = 85;
	showWeaponBar = false;
};

function DistractionGrenadeItem::onUse(%player,%item) {
	if($matchStarted) {
		if(%player.throwTime < getSimTime()) {
			Player::decItemCount(%player,%item);
			%obj = newObject("", Mine, DistractionGrenade);
 	 	 	addToSet("MissionCleanup", %obj);
			%client = Player::getClient(%player);
			GameBase::throw(%obj,%player, 9 * %client.throwStrength, false);
			%player.throwTime = getSimTime() + 0.5;
		}
	}
}

$ItemDescription[DistractionGrenadeItem] = "<jc><f1>Distraction Grenade:<f0> Generates noise patters the confuse enemies";

registerGrenade(DistractionGrenadeItem, 1);
linkGrenade(DistractionGrenadeItem);
setArmorAllowsItem(larmor, DistractionGrenadeItem, 2);
setArmorAllowsItem(lfemale, DistractionGrenadeItem, 2);
setArmorAllowsItem(DMMale, DistractionGrenadeItem, 2);
setArmorAllowsItem(DMFemale, DistractionGrenadeItem, 2);

SoundData ricochet12 {
   wavFileName = "ricoche1.wav";
   profile     = Profile3dMedium;
};

SoundData ricochet22 {
   wavFileName = "ricoche2.wav";
   profile     = Profile3dMedium;
};

SoundData ricochet32 {
   wavFileName = "ricoche3.wav";
   profile     = Profile3dMedium;
};

MineData DistractionGrenade {
   mass = 0.3;
   drag = 1.0;
   density = 2.0;
	elasticity = 0.15;
	friction = 1.0;
	className = "Grenade";
   description = "Distraction Grenade";
   shapeFile = "grenade";
   shadowDetailMask = 4;
	explosionRadius = 0;
	damageValue = 0;
	damageType = 0;
	kickBackStrength = 0;
	triggerRadius = 0.5;
	maxDamage = 200;
};

function DistractionGrenade::onAdd(%this) {
  %data = GameBase::getDataName(%this);
  schedule("DistractionGrenade::doDistraction(" @ %this @ ");",3.0,%this);
  addToSet(MissionCleanup, %this);
}

function DistractionGrenade::onDamage(%this,%type,%value,%pos,%vec,%mom,%object) {}

function DistractionGrenade::doDistraction(%this) {
  %time = 0;
  %dset = 1;//floor(getRandom() * 3);
  for (%i = 0; %i < $DistractionGrenade::callNum[%dset]; %i++) {
    schedule("DistractionGrenade::distraction"@%dset@"("@%this@","@%i@");", %time, %this);
    %time += $DistractionGrenade::callTime[%dset];// + getRandom() / 5;
  }
  schedule("deleteObject("@%this@");",%time, %this);
}

function DistractionGrenade::distractionOld(%this) {
  schedule("GameBase::playSound("@%this@", "@radnomItems(3,ricochet12,ricochet22,ricochet32)@", 0);", 0.03);
}



// Walking sound, then hit ground sound, then walk some more
$DistractionGrenade::callTime[0] = 0.15;
$DistractionGrenade::callNum[0] = 100;
function DistractionGrenade::distraction0(%this, %i) {
  if (%i < 20 || (%i > 30 && %i < 46) || (%i > 52))
    schedule("GameBase::playSound("@%this@", "@tern(%i%2,SoundLFootRSoft,SoundLFootLSoft)@", 0);", 0.03);
  if (%i == 26)
    schedule("GameBase::playSound("@%this@", SoundLandOnGround, 0);", 0.03);
}



// 2 sets of footsteps, gunfight (challenger and magnum), death sound, 1 set of footsteps
$DistractionGrenade::callTime[1] = 0.25;
$DistractionGrenade::callNum[1] = 70;
function DistractionGrenade::distraction1(%this, %i) {
  if (%i < 6) {
    schedule("GameBase::playSound("@%this@", "@tern(%i%2,SoundLFootRSoft,SoundLFootLSoft)@", 0);", 0.03);
    schedule("GameBase::playSound("@%this@", "@tern(%i%2,SoundLFootRSoft,SoundLFootLSoft)@", 1);", 0.1);
  }
  if (%i >= 6 && %i < 40) {
    schedule("GameBase::playSound("@%this@", SoundFireChallenger, 0);", 0.03);
    if (%i%3 && (%i <= 27 || %i > 35)) schedule("GameBase::playSound("@%this@", SoundFireMagnum, 1);", 0.1);
    schedule("GameBase::playSound("@%this@", "@radnomItems(3,ricochet12,ricochet22,ricochet32)@", 2);", 0.03);
  }
  if (%i == 40) {
    schedule("GameBase::playSound("@%this@", SoundPlayerDeath, 0);", 0.03);
  }
  if (%i > 50) {
    schedule("GameBase::playSound("@%this@", "@tern(%i%2,SoundLFootRSoft,SoundLFootLSoft)@", 0);", 0.03);
  }
}



// Complete havoc :)
$DistractionGrenade::callTime[2] = 0.25;
$DistractionGrenade::callNum[2] = 70;
function DistractionGrenade::distraction2(%this, %i) {
  schedule("GameBase::playSound("@%this@", "@radnomItems(6,ricochet12,ricochet22,ricochet32,SoundFireMagnum,SoundFireChallenger,SoundGrenadeShellExp)@", "@%i%4@");", 0.03);
}
