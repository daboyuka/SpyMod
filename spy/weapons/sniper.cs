SoundData SoundSniperFire
{
   wavFileName = "mine_exp.wav";
   profile = Profile3dNear;
};

TargetLaserData SniperTargetLaser
{
   laserBitmapName   = "laserPulse.bmp";

   damageConversion  = 0.0;
   baseDamageType    = 0;

   lightRange        = 1.0;
   lightColor        = { 1, 0, 0 };

   detachFromShooter = false;
};

LaserData SniperBullet
{
   laserBitmapName   = "bar00.bmp";
   hitName           = "chainspk.dts";

   damageConversion  = 10000.0;
   baseDamageType    = $LaserDamageType;

   beamTime = 0.5;

   lightRange        = 3.0;
   lightColor        = { 1, 1, 0.5 };

   detachFromShooter = true;
   hitSoundID = ricochet1;
};

BulletData OldSniperBullet
{
   bulletShapeName    = "bullet.dts";
   explosionTag       = bulletExp0;
   expRandCycle       = 3;

   damageClass        = 0;
   damageValue        = 1.2;
   damageType         = $BulletDamageType;

   muzzleVelocity     = 1000.0;
   totalTime          = 3.0;
   liveTime           = 2.0;
   lightRange         = 3.0;
   lightColor         = { 1, 1, 0 };
   inheritedVelocityScale = 0.3;
   isVisible          = True;

   soundId = SoundJetLight;
};

ItemImageData SniperImage2
{
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

	lightType = 3;  // 1 = sustained, 2 = pulsing, 3 = weapon fire
	lightRadius = 0;
	lightTime = 1;
	lightColor = { 0.6, 1, 1 };
};

ItemData Sniper2
{
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

ItemImageData SniperImage
{
	shapeFile = "sniper";
	mountPoint = 0;

	weaponType = 0;

	reloadTime = 0.2; 
	fireTime = 0.2;

//	ammoType = SniperAmmo;

        minEnergy = 0.01;
        maxEnergy = 0.01;

	projectileType = SniperBullet;
	accuFire = false;

	lightType = 3;  // 1 = sustained, 2 = pulsing, 3 = weapon fire
	lightRadius = 3;
	lightTime = 1;
	lightColor = { 0.6, 1, 1 };

	sfxFire = SoundSniperFire;
	sfxActivate = SoundPickUpWeapon;
};

ItemData Sniper
{
	description = "Sniper";
	className = "Weapon";
	shapeFile = "sniper";
	hudIcon = "chain";
   heading = "bWeapons";
	shadowDetailMask = 4;
	imageType = SniperImage;
	price = 125;
	showWeaponBar = true;
};

// %this = player object, %slot = gun's image slot
// Note: only works if the projectileType field of the ItemImageData is undefined
function SniperImage::onFire(%this, %slot) {}

// %player = client id, %slot = gun's image slot
function Sniper::onMount(%player, %slot) { Player::mountItem(%player, Sniper2, 3); Player::trigger(%player, 3); }

// %player = client id, %slot = gun's image slot
function Sniper::onUnmount(%player, %slot) { Player::trigger(%player, 3, false); Player::unmountItem(%player, 3); }

registerWeapon(Sniper);
linkWeapon(Sniper);
setArmorAllowsItem(larmor, Sniper, 10);
setArmorAllowsItem(lfemale, Sniper, 10);
