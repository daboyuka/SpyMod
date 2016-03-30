SoundData SoundFireSniper {
   wavFileName = "mine_exp.wav";
   profile = Profile3dNear;
};

ItemData SniperAmmo {
	description = "Sniper Ammo";
	heading = $InvHeading[Ammo];
	className = "Ammo";
	shapeFile = "plasammo";
	shadowDetailMask = 4;
	price = 2;
};

TargetLaserData SniperTargetLaser {
   laserBitmapName   = "laserPulse.bmp";

   damageConversion  = 0.0;
   baseDamageType    = 0;

   lightRange        = 1.0;
   lightColor        = { 1, 0, 0 };

   detachFromShooter = false;
};

BulletData SniperBullet {
   bulletShapeName    = "bullet.dts";
   explosionTag       = bulletExp0;
   expRandCycle       = 3;
   bulletHoleIndex    = 0;

   damageClass        = 0;
   damageValue        = 100;
   damageType         = $BulletDamageType;

   muzzleVelocity     = 2000.0;
   totalTime          = 2.0;
   liveTime           = 2.0;

   lightRange         = 3.0;
   lightColor         = { 1, 1, 0.5 };
   isVisible          = false;

   inheritedVelocityScale = 0;

   tracerPercentage   = 1.0;
   tracerLength       = 100;
};

ItemImageData SniperImage2 {
	shapeFile = "paintgun";
	mountPoint = 0;
        mountOffset = "0.1 0 0.1";
        mountRotation = "0 0.785 0";

	weaponType = 2;

	reloadTime = 0; 
	fireTime = 0.2;

        minEnergy = 0;
        maxEnergy = 0;

	projectileType = SniperTargetLaser;
	accuFire = false;
};

ItemData Sniper2 {
	description = "Sniper";
	className = "Weapon";
	shapeFile = "paintgun";
	hudIcon = "chain";
   heading = "bWeapons";
	shadowDetailMask = 4;
	imageType = SniperImage2;
	price = 0;
	showWeaponBar = false;
};

ItemImageData SniperImage {
	shapeFile = "sniper";
	mountPoint = 0;

	weaponType = 0;

	reloadTime = 0.5; 
	fireTime = 0.5;

	ammoType = LoadedAmmo;//SniperAmmo;

	minEnergy = 1;
	maxEnergy = 1;

//	projectileType = SniperBullet;
	accuFire = true;

	lightType = 3;  // 1 = sustained, 2 = pulsing, 3 = weapon fire
	lightRadius = 3;
	lightTime = 1;
	lightColor = { 0.6, 1, 1 };

	sfxFire = SoundFireSniper;
	sfxActivate = SoundPickUpWeapon;
};

ItemData Sniper {
	description = "Sniper";
	className = "Weapon";
	shapeFile = "sniper";
	hudIcon = "chain";
	heading = $InvHeading[Weapon];
	shadowDetailMask = 4;
	imageType = SniperImage;
	price = 125;
	showWeaponBar = false;//true;
};

ItemImageData SniperDisplayImage {
	shapeFile = "breath";
	firstPerson = false;
	ammoType = SniperAmmo;
};

ItemData SniperDisplay {
	description = "";
	className = "Tool";
	shapeFile = "breath";
	hudIcon = "ammopack";
	shadowDetailMask = 4;
	imageType = SniperDisplayImage;
	showWeaponBar = true;
	showInventory = false;
};

$ClipSize[Sniper] = 1;
$AmmoDisplay[Sniper] = SniperDisplay;
$ItemDescription[Sniper] = "<jc><f1>Sniper:<f0> An accurate, long range, slow reloading rifle";

function SniperImage::onFire(%this, %slot) {
  if (Player::getItemCount(%this, LoadedAmmo) == 0) return;

  if (Vector::length(Item::getVelocity(%this)) > 0.01) {
    Client::sendMessage(Player::GetClient(%this), 1, "Cannot use sniper while moving.");
    Player::trigger(%this, 0, false);
    return;
  }

  Weapon::fireProjectile(%this, SniperBullet);
  Player::decItemCount(%this, LoadedAmmo);
}

function Sniper::onMount(%player, %slot) {
  Weapon::displayDescription(%player, Sniper);
  Player::mountItem(%player, Sniper2, 3);
  Player::trigger(%player, 3);

  Weapon::standardMount(%player, Sniper);
}

function Sniper::onUnmount(%player, %slot) {
  Player::trigger(%player, 3, false);
  Player::unmountItem(%player, 3);

  Weapon::standardUnmount(%player, Sniper);
}

function Sniper::onNoAmmo(%player) {
  Sniper::reload(%player);
}

function Sniper::reload(%player) {
  Weapon::standardReload(%player, Sniper, "", 4.5);
}

registerWeapon(Sniper, SniperAmmo, 1);
linkWeapon(Sniper);
setArmorAllowsItem(larmor, Sniper, 5);
setArmorAllowsItem(lfemale, Sniper, 5);
setArmorAllowsItem(SpyMale, Sniper, 5);
setArmorAllowsItem(SpyFemale, Sniper, 5);
setArmorAllowsItem(DMMale, Sniper, 5);
setArmorAllowsItem(DMFemale, Sniper, 5);