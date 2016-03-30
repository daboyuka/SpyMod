LaserData UberSniperLaser {
   laserBitmapName   = "bar00.bmp";
   hitName           = "chainspk.dts";

   damageConversion  = 1000000.0;
   baseDamageType    = $LaserDamageType;

   beamTime = 0.5;

   lightRange        = 3.0;
   lightColor        = { 1, 1, 0.5 };

   detachFromShooter = false;
   hitSoundID = ricochet1;
};

ItemImageData UberSniperImage {
	shapeFile = "sniper";
	mountPoint = 0;

	weaponType = 0;

	reloadTime = 0.2; 
	fireTime = 0.2;

	minEnergy = 0.01;
	maxEnergy = 0.01;

	projectileType = UberSniperLaser;
	accuFire = true;

	lightType = 3;  // 1 = sustained, 2 = pulsing, 3 = weapon fire
	lightRadius = 3;
	lightTime = 1;
	lightColor = { 0.6, 1, 1 };

	sfxFire = SoundFireSniper;
	sfxActivate = SoundPickUpWeapon;
};

ItemData UberSniper {
	description = "Sniper";
	className = "Weapon";
	shapeFile = "sniper";
	hudIcon = "chain";
   heading = "bWeapons";
	shadowDetailMask = 4;
	imageType = UberSniperImage;
	price = 125;
	showWeaponBar = false;//true;
};

function UberSniper::onMount(%player, %slot) { Player::mountItem(%player, Sniper2, 4); Player::trigger(%player, 4); }
function UberSniper::onUnmount(%player, %slot) { Player::trigger(%player, 4, false); Player::unmountItem(%player, 4); }