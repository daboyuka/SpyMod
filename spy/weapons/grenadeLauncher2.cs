SoundData SoundFireGrenadeLauncher {
   wavFileName = "Grenade.wav";
   profile = Profile3dNear;
};
SoundData SoundGrenadeShellExp {
   wavFileName = "BXplo1.wav";
   profile = Profile3dFar;
};

ExplosionData GrenadeShellExp {
   shapeName = "fiery.dts";
   soundId   = SoundGrenadeShellExp;

   faceCamera = true;
   randomSpin = true;
   hasLight   = true;
   lightRange = 10.0;

   timeScale = 2;//1.5;

   timeZero = 0.150;
   timeOne  = 0.500;

   colors[0]  = { 0.0, 0.0,  0.0 };
   colors[1]  = { 1.0, 0.63, 0.0 };
   colors[2]  = { 1.0, 0.63, 0.0 };
   radFactors = { 0.0, 1.0, 0.9 };
};

GrenadeData GrenadeLauncherShell {
   bulletShapeName    = "grenade.dts";
   explosionTag       = GrenadeShellExp;
   collideWithOwner   = True;
   ownerGraceMS       = 250;
   collisionRadius    = 0.2;
   mass               = 1.0;
   elasticity         = 0.01;

   damageClass        = 1;       // 0 impact, 1, radius
   damageValue        = 140;
   damageType         = $ExplosionDamageType;

   explosionRadius    = 10;
   kickBackStrength   = 150.0;
   maxLevelFlightDist = 75;
   totalTime          = 30.0;    // special meaning for grenades...
   liveTime           = 0;//1.0;
   projSpecialTime    = 0.05;

   inheritedVelocityScale = 0.5;

   smokeName              = "breath.dts";
};

GrenadeData GrenadeLauncherShell2 {
   bulletShapeName    = "grenade.dts";
   explosionTag       = noExp;
   collideWithOwner   = True;
   ownerGraceMS       = 250;
   collisionRadius    = 0.2;
   mass               = 1.0;
   elasticity         = 0.2;

   damageClass        = 0;       // 0 impact, 1, radius
   damageValue        = 0;
   damageType         = 0;

   maxLevelFlightDist = 75;
   totalTime          = 100.0;    // special meaning for grenades...
   liveTime           = 100;//1.0;
   projSpecialTime    = 0.05;

   inheritedVelocityScale = 0.5;

   smokeName              = "breath.dts";
};


ItemData GrenadeLauncherAmmo {
	description = "Grenade Ammo";
	heading = $InvHeading[Ammo];
	className = "Ammo";
	shapeFile = "plasammo";
	shadowDetailMask = 4;
	price = 2;
};

ItemImageData GrenadeLauncherImage {
	shapeFile = "grenadeL";
	mountPoint = 0;

	weaponType = 0;
	reloadTime = 0; 
	fireTime = 1.5;

	ammoType = LoadedAmmo;

	accuFire = false;

	lightType = 3;  // 1 = sustained, 2 = pulsing, 3 = weapon fire
	lightRadius = 8;
	lightTime = 0.75;
	lightColor = { 1, 1, 0.5 };

	sfxFire = SoundFireGrenadeLauncher;
	sfxActivate = SoundPickUpWeapon;
};

ItemData GrenadeLauncher2 {
	description = "V7 Grenade Launcher";
	className = "Weapon";
	shapeFile = "grenadeL";
	hudIcon = "mortar";
	heading = $InvHeading[Weapon];
	shadowDetailMask = 4;
	imageType = GrenadeLauncherImage;
	price = 125;
	showWeaponBar = false;
};

ItemImageData GrenadeLauncherDisplayImage {
	shapeFile = "breath";
	firstPerson = false;
	ammoType = GrenadeLauncherAmmo;
};

ItemData GrenadeLauncherDisplay {
	className = "Tool";
	shapeFile = "breath";
	hudIcon = "ammopack";
	imageType = GrenadeLauncherDisplayImage;
	showWeaponBar = true;
	showInventory = false;
};

ItemImageData GrenadeLauncherSmokeDisplayImage {
	shapeFile = "breath";
	firstPerson = false;
	ammoType = SmokeGrenadeItem;
};

ItemData GrenadeLauncherSmokeDisplay {
	className = "Tool";
	shapeFile = "breath";
	hudIcon = "ammopack";
	imageType = GrenadeLauncherSmokeDisplayImage;
	showWeaponBar = true;
	showInventory = false;
};

ItemImageData GrenadeLauncherDistractDisplayImage {
	shapeFile = "breath";
	firstPerson = false;
	ammoType = DistractionGrenadeItem;
};

ItemData GrenadeLauncherDistractDisplay {
	className = "Tool";
	shapeFile = "breath";
	hudIcon = "ammopack";
	imageType = GrenadeLauncherDistractDisplayImage;
	showWeaponBar = true;
	showInventory = false;
};

ItemImageData GrenadeLauncherPoisonDisplayImage {
	shapeFile = "breath";
	firstPerson = false;
	ammoType = GasGrenadeItem;
};

ItemData GrenadeLauncherPoisonDisplay {
	className = "Tool";
	shapeFile = "breath";
	hudIcon = "ammopack";
	imageType = GrenadeLauncherPoisonDisplayImage;
	showWeaponBar = true;
	showInventory = false;
};

$ClipSize[GrenadeLauncher2] = 1;
$AmmoDisplay[GrenadeLauncher2, 0] = GrenadeLauncherDisplay;
$AmmoDisplay[GrenadeLauncher2, 1] = GrenadeLauncherSmokeDisplay;
$AmmoDisplay[GrenadeLauncher2, 2] = GrenadeLauncherDistractDisplay;
$AmmoDisplay[GrenadeLauncher2, 3] = GrenadeLauncherPoisonDisplay;
$ItemDescription[GrenadeLauncher2, 0] = "<jc><f1>Grenade Launcher, Normal Grenades:<f0> Lobs explosive grenades at enemies";
$ItemDescription[GrenadeLauncher2, 1] = "<jc><f1>Grenade Launcher, Smoke Grenades:<f0> Fires smoke grenades";
$ItemDescription[GrenadeLauncher2, 2] = "<jc><f1>Grenade Launcher, Distraction Grenades:<f0> Launches distraction grenades";
$ItemDescription[GrenadeLauncher2, 3] = "<jc><f1>Grenade Launcher, Poison Grenades:<f0> Lobs poison grenades";

$NumModes[GrenadeLauncher2] = 4;

function GrenadeLauncherImage::onFire(%player, %slot) {
  if (Player::getItemCount(%player, LoadedAmmo) == 0) return;

  %mode = Weapon::getMode(%player, GrenadeLauncher2);
  if (%mode == 0) {
    Weapon::fireProjectile(%player, GrenadeLauncherShell);
  } else if (%mode == 1) {
    if ($waitingForSmokers) {
      Client::sendMessage(Player::getClient(%player),1,"Sorry, no one can use smoke grenades until the next mission starts");
      return;
    } else {
      %team = Client::getApparentTeam(Player::getClient(%player));
      if (!SmokeGrenade::checkMax(%team)) {
        %client = Player::getClient(%player);
        if (getNumTeams() == 1)
          Client::sendMessage(%client, 1, "Too many smoke grenades on the field. Try again later.");
        else
          Client::sendMessage(%client, 1, "Too many smoke grenades from your team on the field. Try again later.");

        return;
      }
      $SmokeGrenade::numSmokers[%team]++;
      %p = Weapon::fireProjectile(%player, GrenadeLauncherShell2);
      %p.throwerTeam = %team;
      schedule("SmokeGrenade::onAdd("@%p@");", 0);
    }
  } else if (%mode == 2) {
    %p = Weapon::fireProjectile(%player, GrenadeLauncherShell2);
    %obj = newObject("", Mine, DistractionGrenade);
    addToSet(MissionCleanup, %obj);
    GameBase::setPosition(%obj, "0 0 -100000");
    schedule("Item::setVelocity("@%obj@", Item::getVelocity("@%p@")); GameBase::setPosition("@%obj@",Vector::add(GameBase::getPosition("@%p@"),\"0 0 0.3\")); deleteObject("@%p@");", 3);
  } else if (%mode == 3) {
    %p = Weapon::fireProjectile(%player, GrenadeLauncherShell2);
    %p.throwerClient = Player::getClient(%player);
    schedule("GasGrenade::onAdd("@%p@");", 0);
  }
  Player::decItemCount(%player, LoadedAmmo);
}

function GrenadeLauncher2::onMount(%player, %slot) {
  Weapon::displayDescription(%player, GrenadeLauncher2);

  if (%player.mountedItem == GrenadeLauncher2) return;
  %mode = Weapon::getMode(%player, GrenadeLauncher2);
  Player::setItemCount(%player, $AmmoDisplay[GrenadeLauncher2, %mode], 1);

  GrenadeLauncher::loadPartialClip(%player, min(%player.loadedAmmo[GrenadeLauncher2], Player::getItemCount(%player, GrenadeLauncher::getAmmoType(%mode))));
  %player.mountedItem = GrenadeLauncher2;
}

function GrenadeLauncher2::onUnmount(%player, %slot) {
  if (%player.mountedItem != GrenadeLauncher2) return;
  %mode = Weapon::getMode(%player, GrenadeLauncher2);
  Player::setItemCount(%player, $AmmoDisplay[GrenadeLauncher2, %mode], 0);

  %player.loadedAmmo[GrenadeLauncher2] = Player::getItemCount(%player, LoadedAmmo);
  GrenadeLauncher::returnClipAmmo(%player);
  %player.lastUnmount[GrenadeLauncher2]++;
  %player.mountedItem = "";
}

function GrenadeLauncher2::onNoAmmo(%player) {
  GrenadeLauncher2::reload(%player);
}

function GrenadeLauncher2::reload(%player) {
  if (%player.reloading[GrenadeLauncher2]) return;
  %player.reloading[GrenadeLauncher2] = true;

  GrenadeLauncher::returnClipAmmo(%player);

  %seconds = tern(Weapon::getMode(%player, GrenadeLauncher2) == 0, 3, 1.5);

  if (%player.lastUnmount[GrenadeLauncher2] == "") %player.lastUnmount[GrenadeLauncher2] = 0;

  %command = %player @ ".reloading[GrenadeLauncher2] = false;" @
             "if ("@%player@".lastUnmount[GrenadeLauncher2] == "@%player.lastUnmount[GrenadeLauncher2]@" && !Player::isDead("@%player@"))" @
             "{";

  %command = %command @ "GrenadeLauncher::loadClip("@%player@"); }";

  schedule(%command, %seconds, %player);
}

function GrenadeLauncher::returnClipAmmo(%player) {
  if (%player.loadedAmmoForGun != "GrenadeLauncher2") return;
  %ammoType = GrenadeLauncher::getAmmoType(Weapon::getMode(%player, GrenadeLauncher2));
  %player.ammoLoaded[%ammoType] -= Player::getItemCount(%player, LoadedAmmo);
  Player::incItemCount(%player, %ammoType, Player::getItemCount(%player, LoadedAmmo));
  Player::setItemCount(%player, LoadedAmmo, 0);
  %player.loadedAmmoForGun = "";
}


function GrenadeLauncher::loadClip(%player) {
  %ammoType = GrenadeLauncher::getAmmoType(Weapon::getMode(%player, GrenadeLauncher2));
  %num = min(Player::getItemCount(%player, %ammoType), $ClipSize[GrenadeLauncher2]);
  %num = min($ClipSize[GrenadeLauncher2] - Player::getItemCount(%player, LoadedAmmo), %num);
  Player::decItemCount(%player, %ammoType, %num);
  Player::incItemCount(%player, LoadedAmmo, %num);
  %player.loadedAmmo[GrenadeLauncher2] = %num;
  %player.ammoLoaded[%ammoType] += %num;
  %player.loadedAmmoForGun = GrenadeLauncher2;
}

function GrenadeLauncher::loadPartialClip(%player, %num) {
  %ammoType = GrenadeLauncher::getAmmoType(Weapon::getMode(%player, GrenadeLauncher2));
  Player::decItemCount(%player, %ammoType, %num);
  Player::incItemCount(%player, LoadedAmmo, %num);
  %player.loadedAmmo[GrenadeLauncher2] = %num;
  %player.ammoLoaded[%ammoType] += %num;
  %player.loadedAmmoForGun = GrenadeLauncher2;
}

function GrenadeLauncher2::onModeChanged(%player, %mode, %oldMode) {
  Weapon::displayDescription(%player, GrenadeLauncher2);

  Player::setItemCount(%player, $AmmoDisplay[GrenadeLauncher2, %mode], 1);
  Player::setItemCount(%player, $AmmoDisplay[GrenadeLauncher2, %oldMode], 0);

  %ammoType = GrenadeLauncher::getAmmoType(%oldMode);
  %player.ammoLoaded[%ammoType] -= Player::getItemCount(%player, LoadedAmmo);
  Player::incItemCount(%player, %ammoType, Player::getItemCount(%player, LoadedAmmo));
  Player::setItemCount(%player, LoadedAmmo, 0);

  %player.reloading[GrenadeLauncher2] = false;
  %player.lastUnmount[GrenadeLauncher2]++;

  GrenadeLauncher2::reload(%player);
}

function GrenadeLauncher::getAmmoType(%mode) {
  if (%mode == 0) return GrenadeLauncherAmmo;
  if (%mode == 1) return SmokeGrenadeItem;
  if (%mode == 2) return DistractionGrenadeItem;
  if (%mode == 3) return GasGrenadeItem;
}

function GrenadeLauncher::isSelectable(%player) { return true; }

registerWeapon(GrenadeLauncher2, GrenadeLauncherAmmo, 1);
linkWeapon(GrenadeLauncher2);
setArmorAllowsItem(larmor, GrenadeLauncher2, 4);
setArmorAllowsItem(lfemale, GrenadeLauncher2, 4);
setArmorAllowsItem(SpyMale, GrenadeLauncher2, 4);
setArmorAllowsItem(SpyFemale, GrenadeLauncher2, 4);
setArmorAllowsItem(DMMale, GrenadeLauncher2, 4);
setArmorAllowsItem(DMFemale, GrenadeLauncher2, 4);

// All weapons have LoadedAmmo as their ammo type, but is not shown on the weapon list
// All weapons have an accompanying item which has the weapon's normal ammo as its ammo type
// So the weapon relies on LoadedAmmo, but the weapons real ammo type is displayed on the GUI