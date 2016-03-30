RepairEffectData FlashLiteTriggerBolt {
   bitmapName       = "bar00.bmp";
   boltLength       = 3;
   segmentDivisions = 1;
   beamWidth        = 0;

   updateTime   = 1000;
   skipPercent  = 0.6;
   displaceBias = 0.15;
};

TargetLaserData FlashLiteBeam {
   laserBitmapName   = "ammoSh.bmp";//"paintPulse.bmp";

   damageConversion  = 0.0;
   baseDamageType    = 0;

   lightRange        = 10.0;
   lightColor        = { 1.0, 1.0, 1.0 };

   detachFromShooter = false;
};



ItemImageData FlashLiteLightImage {
   shapeFile  = "breath";
	mountPoint = 0;
	mountOffset = "0 0 0";
	mountRotation = "0 0 0";

	weaponType = 2; // Sustained
	projectileType = FlashLiteBeam;
	accuFire = true;
	reloadTime = 0.05;

	lightType = 3;  // Weapon Fire
	lightRadius = 3;
	lightTime = 1;
	lightColor = { 1, 1, 1 };

	minEnergy = 0;
	maxEnergy = 0;
};

ItemData FlashLiteLight {
  shapeFile = "breath";
  imageType = FlashLiteLightImage;
};

ItemImageData FlashLiteImage {
   shapeFile  = "paintgun";
	mountPoint = 0;
	mountOffset = "0 0 0";
	mountRotation = "0 0 0";

	weaponType = 2; // Sustained
	reloadTime = 0;
	fireTime = 0.1;

	projectileType = FlashLiteTriggerBolt;

	minEnergy = 0;
	maxEnergy = 0;

	sfxActivate = SoundPickUpWeapon;
};

ItemData FlashLite {
  description = "FlashLite";

  shapeFile = "paintgun";
	shadowDetailMask = 4;

	lightType = 2;   // Pulsing
	lightRadius = 4;
	lightTime = 1.5;
	lightColor = { 0.3, 1, 0 };

	className = "Tool";
	imageType = FlashLiteImage;
	heading = $InvHeading[Gadget];

	price = 100;
};

$ItemDescription[FlashLite] = "<jc><f1>FlashLite:<f0> A high-powered flashlight that can light up even the darkest nights.";

function FlashLite::onMount(%player, %item) {
  Weapon::displayDescription(%player, FlashLite);
  Player::mountItem(%player, FlashLiteLight, 3);
}

function FlashLite::onUnmount(%player, %item) {
  Player::unmountItem(%player, 3);
  Weapon::clearDescription(%player);
}

function FlashLiteTriggerBolt::onAcquire(%blah, %this, %target) {
  if (Player::getMountedItem(%this, 3) == FlashLiteLight)
    Player::trigger(%this, 3, true);
}

function FlashLiteTriggerBolt::onRelease(%blah, %this, %target) {
  if (Player::getMountedItem(%this, 3) == FlashLiteLight)
    Player::trigger(%this, 3, false);
}

function FlashLiteTriggerBolt::checkDone(%blah, %this) {}

registerGadget(FlashLite, "", 1);
linkGadget(FlashLite);
setArmorAllowsItem(larmor, FlashLite);
setArmorAllowsItem(lfemale, FlashLite);
setArmorAllowsItem(SpyMale, FlashLite);
setArmorAllowsItem(SpyFemale, FlashLite);
setArmorAllowsItem(DMMale, FlashLite);
setArmorAllowsItem(DMFemale, FlashLite);