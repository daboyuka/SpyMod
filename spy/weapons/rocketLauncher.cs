SoundData SoundRocketLauncherFire {
   wavFileName = "turretfire1.wav";
   profile = Profile3dMedium;
};

SoundData SoundRocketLauncherRocketExp {
   wavFileName = "EXPLO3.wav";
   profile = Profile3dLudicrouslyFar;
};

ExplosionData RocketLauncherRocketExp {
   shapeName = "fiery.dts";
   soundId   = SoundRocketLauncherRocketExp;

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

ItemData RocketLauncherAmmo {
	description = "Rockets";
	heading = $InvHeading[Ammo];
	className = "Ammo";
	shapeFile = "plasammo";
	shadowDetailMask = 4;
	price = 2;
};

RocketData RocketLauncherRocket {
   bulletShapeName = "rocket.dts";
   explosionTag    = RocketLauncherRocketExp;

   collisionRadius = 0.0;
   mass            = 2.0;

   damageClass      = 1;       // 0 impact, 1, radius
   damageValue      = 100;
   damageType       = $ExplosionDamageType;

   explosionRadius  = 9;
   kickBackStrength = 100.0;

   muzzleVelocity   = 60.0;
   terminalVelocity = 60.0;
   acceleration     = 5;

   totalTime        = 5;
   liveTime         = 5;

   lightRange       = 5.0;
   lightColor       = { 1, 0.8, 0 };

   inheritedVelocityScale = 0;

   // rocket specific
//   trailType   = 1;
//   trailLength = 15;
//   trailWidth  = 0.3;

   soundId = SoundJetHeavy;
};

SeekingMissileData RocketLauncherSeekingRocket {
   bulletShapeName = "rocket.dts";
   explosionTag    = RocketLauncherRocketExp;
   collisionRadius = 0.0;
   mass            = 2.0;

   damageClass      = 1;       // 0 impact, 1, radius
   damageValue      = 100;
   damageType       = $ExplosionDamageType;
   explosionRadius  = 9;
   kickBackStrength = 100.0;

   muzzleVelocity    = 60.0;
   totalTime         = 5;
   liveTime          = 5;
   seekingTurningRadius    = 9;
   nonSeekingTurningRadius = 75.0;
   proximityDist     = 1.0;
   smokeDist         = 1000;

   lightRange       = 5.0;
   lightColor       = { 1, 0.8, 0 };

   inheritedVelocityScale = 0;

   soundId = SoundJetHeavy;
};

function RocketLauncherSeekingRocket::updateTargetPercentage(%target) {
   return tern(%target.lockDiffusion != "", 1-%target.lockDiffusion, 0.9);
}

ItemImageData RocketLauncherImage2 {
	shapeFile = "mortargun";
	mountPoint = 0;
	mountOffset = "0 -0.85 0.25";
};

ItemData RocketLauncher2 {
	shapeFile = "mortargun";
	imageType = RocketLauncherImage2;
};

ItemImageData RocketLauncherImage3 {
	shapeFile = "mortargun";
	mountPoint = 0;
	mountOffset = "0 -0.15 0.25";
};

ItemData RocketLauncher3 {
	shapeFile = "mortargun";
	imageType = RocketLauncherImage3;
};

ItemImageData RocketLauncherImage4 {
	shapeFile = "mortar";
	mountPoint = 0;
	mountRotation = "1 0 0";
	mountOffset = "0 -0.15 0.1";
};

ItemData RocketLauncher4 {
	shapeFile = "mortar";
	imageType = RocketLauncherImage4;
};

ItemImageData RocketLauncherImage {
	shapeFile = "breath";
	mountPoint = 0;

	weaponType = 0;
	reloadTime = 1; 
	fireTime = 0;

	ammoType = LoadedAmmo;

	accuFire = true;

	firstPerson = false;

	lightType = 3;  // 1 = sustained, 2 = pulsing, 3 = weapon fire
	lightRadius = 8;
	lightTime = 0.5;
	lightColor = { 1, 1, 1 };

	sfxFire = SoundRocketLauncherFire;
	sfxActivate = SoundPickUpWeapon;
};

ItemData RocketLauncher {
	description = "Rocket Launcher";
	className = "Weapon";
	shapeFile = "mortargun";
	hudIcon = "mortar";
	heading = $InvHeading[Weapon];
	shadowDetailMask = 4;
	imageType = RocketLauncherImage;
	price = 125;
	showWeaponBar = false;//true;
};



ItemImageData RocketLauncherDisplayImage {
	shapeFile = "breath";
	firstPerson = false;
	ammoType = RocketLauncherAmmo;
};

ItemData RocketLauncherDisplay {
	description = "";
	className = "Tool";
	shapeFile = "breath";
	hudIcon = "ammopack";
	shadowDetailMask = 4;
	imageType = RocketLauncherDisplayImage;
	showWeaponBar = true;
	showInventory = false;
};

$ClipSize[RocketLauncher] = 1;
$AmmoDisplay[RocketLauncher] = RocketLauncherDisplay;
$ItemDescription[RocketLauncher] = "<jc><f1>Rocket Launcher:<f0> Fires guided or unguided missiles, depending on whether a lock has been acquired";

function RocketLauncherImage::onFire(%player, %slot) {
  if (Player::getItemCount(%player, LoadedAmmo) == 0) return;

  if (%player.lockonTargetLocked && %player.lockonTarget)
    Weapon::fireOffsetProjectile(%player, RocketLauncherSeekingRocket, "0 0.5 0.25", %player.lockonTarget);
  else
    Weapon::fireOffsetProjectile(%player, RocketLauncherRocket, "0 0.5 0.25", -1);

  Player::decItemCount(%player, LoadedAmmo);
}

function RocketLauncher::onMount(%player, %slot) {
  Weapon::displayDescription(%player, RocketLauncher);
  Player::mountItem(%player, RocketLauncher2, 3);
  Player::mountItem(%player, RocketLauncher3, 4);
  Player::mountItem(%player, RocketLauncher4, 5);
  LockOn::enableLockOn2(%player, RocketLauncher);
  Weapon::standardMount(%player, RocketLauncher);
}
function RocketLauncher::onUnmount(%player, %slot) {
  Player::unmountItem(%player, 3);
  Player::unmountItem(%player, 4);
  Player::unmountItem(%player, 5);
  LockOn::disableLockOn2(%player, RocketLauncher);
  Weapon::standardUnmount(%player, RocketLauncher);
}

function RocketLauncher::onNoAmmo(%player) {
  RocketLauncher::reload(%player);
}

function RocketLauncher::reload(%player) {
  Weapon::standardReload(%player, RocketLauncher, SoundRocketLauncherReload, 4);
}


$LockOn::lockonTime[RocketLauncher] = 1.5;
$LockOn::lockExpireTime[RocketLauncher] = 1;
function RocketLauncher::onTargetLockAcquired(%this, %target) {
  %client = GameBase::getControlClient(%this);
  %name = GameBase::getDataName(%target);
  %type = getObjectType(%target);
  if (%type == "Flier" && GameBase::getControlClient(%target) != -1) {
    bottomprint(%client, "<jc>Target acquired: " @ %name.description @
                         " (" @ Client::getName(GameBase::getControlClient(%target)) @ ")", 2);
  } else {
    bottomprint(%client, "<jc>Target acquired: " @ %name.description, 2);
  }
}

function RocketLauncher::onTargetLockLost(%this, %target) {
  %client = GameBase::getControlClient(%this);
  bottomprint(%client, "<jc>Target lost", 2);
}

function RocketLauncher::onTargetLockLocked(%this, %target) {
  %client = GameBase::getControlClient(%this);
  %name = GameBase::getDataName(%target);
  %type = getObjectType(%target);
  if (%type == "Flier" && GameBase::getControlClient(%target) != -1) {
    bottomprint(%client, "<jc>Target lock acquired: " @ %name.description @
                         " (" @ Client::getName(GameBase::getControlClient(%target)) @ ")", 2);
  } else {
    bottomprint(%client, "<jc>Target lock acquired: " @ %name.description, 2);
  }

  %client2 = GameBase::getControlClient(%target);
  if (%client2 != -1) {
    bottomprint(%client2, "<JC><F1>WARNING! You have been locked onto!", 3);
    Client::sendMessage(%client2, 1, "~wfloat_target.wav");
  }
}

function RocketLauncher::verifyTarget(%this, %target) {
  if (%this == %target) return false; // lol, ya this might be good to have here...

  if (GameBase::getDamageState(%target) == "Destroyed") return false;

  if (getObjectType(%target) != "Flier") return false;

  return true;
}



registerWeapon(RocketLauncher, RocketLauncherAmmo, 1);
linkWeapon(RocketLauncher);
setArmorAllowsItem(larmor, RocketLauncher, 3);
setArmorAllowsItem(lfemale, RocketLauncher, 3);
setArmorAllowsItem(SpyMale, RocketLauncher, 3);
setArmorAllowsItem(SpyFemale, RocketLauncher, 3);
setArmorAllowsItem(DMMale, RocketLauncher, 3);
setArmorAllowsItem(DMFemale, RocketLauncher, 3);