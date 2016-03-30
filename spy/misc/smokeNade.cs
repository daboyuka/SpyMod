ItemData SmokeGrenadeItem {
   heading = $InvHeading[Ammo];
	description = "Smoke Grenade";
	className = "Ammo";
   shapeFile  = "grenade";
	hudIcon = "blaster";
	shadowDetailMask = 4;
	price = 85;
	showWeaponBar = false;
};

function SmokeGrenadeItem::onUse(%player,%item) {
	if($matchStarted) {
		if(%player.throwTime < getSimTime()) {
			%client = Player::getClient(%player);
			%team = Client::getApparentTeam(%client);
			if (%item == "SmokeGrenadeItem") {
                          if (!SmokeGrenade::checkMax(%team)) {
			    if (getNumTeams() == 1)
                              Client::sendMessage(%client, 1, "Too many smoke grenades on the field. Try again later.");
			    else
                              Client::sendMessage(%client, 1, "Too many smoke grenades from your team on the field. Try again later.");

			    return;
			  }
                          $SmokeGrenade::numSmokers[%team]++;
                        }

			Player::decItemCount(%player,%item);
			%obj = newObject("", Mine, SmokeGrenade);
			%obj.throwerTeam = %team;
 	 	 	addToSet("MissionCleanup", %obj);
			GameBase::throw(%obj,%player, 9 * %client.throwStrength, false);
			%player.throwTime = getSimTime() + 0.5;
		}
	}
}

$ItemDescription[SmokeGrenadeItem] = "<jc><f1>Smoke Grenade:<f0> Creates plumes of smoke for cover";

registerGrenade(SmokeGrenadeItem, 1);
linkGrenade(SmokeGrenadeItem);
setArmorAllowsItem(larmor, SmokeGrenadeItem, 2);
setArmorAllowsItem(lfemale, SmokeGrenadeItem, 2);
//setArmorAllowsItem(SpyMale, SmokeGrenadeItem, SmokeGrenadeItem, 0);
//setArmorAllowsItem(SpyFemale, SmokeGrenadeItem, SmokeGrenadeItem, 0);
setArmorAllowsItem(DMMale, SmokeGrenadeItem, 2);
setArmorAllowsItem(DMFemale, SmokeGrenadeItem, 2);


$lastSmokeNadeTime = -70;
MineData SmokeGrenade {
   mass = 0.3;
   drag = 1.0;
   density = 2.0;
	elasticity = 0.3;//0.15;
	friction = 1.0;
	className = "Grenade";
   description = "Smoke Grenade";
   shapeFile = "grenade";
   shadowDetailMask = 4;
	explosionRadius = 0;
	damageValue = 0;
	damageType = 0;
	kickBackStrength = 0;
	triggerRadius = 0.5;
	maxDamage = 200;
};

ExplosionData SmokeExp {
   shapeName = "shockwave.dts";

   faceCamera = true;
   randomSpin = false;//true;
   hasLight   = false;

   timeScale = 40;

   timeZero = 0.0;
   timeOne  = 0.500;

   colors[0]  = { 0.0, 0.0, 0.0 };
   colors[1]  = { 1.0, 1.0, 1.0 };
   colors[2]  = { 1.0, 1.0, 1.0 };
   radFactors = { 0.0, 1.0, 1.0 };

   className = "w0000T!";
};

BulletData Smoke {
   bulletShapeName    = "shockwave.dts";
   explosionTag       = SmokeExp;

   damageClass        = 0;
   damageValue        = 0;
   damageType         = 0;

   muzzleVelocity     = 0.7;
   totalTime          = 7.0;
   liveTime           = 7.0;
   inheritedVelocityScale = 0;
   isVisible          = True;
   aimDeflection = $PI;

   soundId = SoundJetLight;
};

function SmokeGrenade::onAdd(%this) {
  %data = GameBase::getDataName(%this);
  schedule("SmokeGrenade::doSmoke(" @ %this @ ");",2.0,%this);
  addToSet(MissionCleanup, %this);
  $lastSmokeNadeTime = getSimTime();
}

function SmokeGrenade::onDamage(%this,%type,%value,%pos,%vec,%mom,%object) {}

function SmokeGrenade::doSmoke(%this) {
  for (%i = 0; %i < 30; %i++) {
    schedule("SmokeGrenade::smoke("@%this@");", %i / 1.5, %this);
    schedule("SmokeGrenade::blind("@%this@");", %i / 1.5, %this);
  }
  %t = %this.throwerTeam;
  schedule("deleteObject("@%this@");$SmokeGrenade::numSmokers["@%t@"]--;", 20.5, %this);
}

function SmokeGrenade::smoke(%this) {
  %pos = GameBase::getPosition(%this);
  %pos = Vector::add(%pos, (getRandom()*8-4) @ " " @ (getRandom()*8-4) @ " " @ (getRandom()*8-4));
  %proj = Projectile::spawnProjectile(Smoke, "1 0 0 0 1 0 0 0 1 " @ %pos, -1, 0);//%this, 0);
  addToSet(MissionCleanup, %proj);
}

function SmokeGrenade::blind(%this) {
  %pos = GameBase::getPosition(%this);
  GameBase::applyRadiusDamage($SmokeBlindDamageType, Vector::add(%pos, "0 0 0.5"),
                              4, 0.01, 0, %this);
}

function SmokeGrenade::checkMax(%team) {
  if (getNumTeams() == 1) {
    return $SmokeGrenade::numSmokers[%team] < $Game::maxSmokersForDM;
  } else {
    return $SmokeGrenade::numSmokers[%team] < $Game::maxSmokersPerTeam;
  }
}
