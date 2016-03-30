// Object mounts
ItemImageData Tank2Object1Image {
	shapeFile = "gunturet";
	mountPoint = 4;
	mountOffset = "-2.5 0 -1.25";
	firstPerson = true;
};

ItemData Tank2Object1 {
	shapeFile = "breath";
	imageType = Tank2Object1Image;
};

ItemImageData Tank2Object2Image {
	shapeFile = "gunturet";
	mountPoint = 4;
	mountOffset = "-2.5 1.3 -1.25";
	firstPerson = true;
};

ItemData Tank2Object2 {
	shapeFile = "breath";
	imageType = Tank2Object2Image;
};

ItemImageData Tank2Object3Image {
	shapeFile = "magcargo";
	mountPoint = 4;
	mountOffset = "0 0 0";
	firstPerson = true;
};

ItemData Tank2Object3 {
	shapeFile = "magcargo";
	imageType = Tank2Object3Image;
};





// Tank gunner's cannon
SoundData SoundTank2CannonFire {
   wavFileName = "BXplo2.wav";
   profile = Profile3dMedium;
};

BulletData Tank2CannonBullet {
   bulletShapeName    = "bullet.dts";
   explosionTag       = bulletExp0;
   expRandCycle       = 3;
   bulletHoleIndex    = 0;

   damageClass        = 0;                 // 0 = impact, 1 = radius
   damageValue        = 20;
   damageType         = $BulletDamageType;

   muzzleVelocity     = 600.0;
   totalTime          = 3;
   liveTime           = 3;
   lightRange         = 1.0;
   lightColor         = { 1, 1, 0 };
   inheritedVelocityScale = 0;
   isVisible          = false;

   aimDeflection      = 0.005;
   tracerLength       = 10;
   tracerPercentage   = 1;

   soundId = SoundJetLight;
};

ItemImageData Tank2CannonImage2 {
	shapeFile = "plasma";
	mountPoint = 0;
	mountOffset = "-0.45 0 -0.1";
	mountRotation = "0 "@($PI/4)@" 0";
};

ItemData Tank2Cannon2 {
	shapeFile = "plasma";
	imageType = Tank2CannonImage2;
};

ItemImageData Tank2CannonImage3 {
	shapeFile = "plasma";
	mountPoint = 0;
	mountOffset = "-0.45 0 -0.1";
	mountRotation = "0 "@(3*$PI/4)@" 0";
};

ItemData Tank2Cannon3 {
	shapeFile = "plasma";
	imageType = Tank2CannonImage3;
};

ItemImageData Tank2CannonImage4 {
	shapeFile = "plasma";
	mountPoint = 0;
	mountOffset = "-0.45 0 -0.1";
	mountRotation = "0 "@(5*$PI/4)@" 0";
};

ItemData Tank2Cannon4 {
	shapeFile = "plasma";
	imageType = Tank2CannonImage4;
};

ItemImageData Tank2CannonImage5 {
	shapeFile = "plasma";
	mountPoint = 0;
	mountOffset = "-0.45 0 -0.1";
	mountRotation = "0 "@(7*$PI/4)@" 0";
};

ItemData Tank2Cannon5 {
	shapeFile = "plasma";
	imageType = Tank2CannonImage5;
};

ItemImageData Tank2CannonImage {
	shapeFile = "breath";
	mountPoint = 0;
	mountOffset = "-0.45 0 -0.1";

	weaponType = 0;
	reloadTime = 0.07; 
	fireTime = 0;

	maxEnergy = 0;
	minEnergy = 0;

	accuFire = true;

	sfxFire = SoundTank2CannonFire;
	sfxActivate = SoundPickUpWeapon;
};

ItemData Tank2Cannon {
	description = "Tank Machine Gun";
	className = "Weapon";
	shapeFile = "breath";
	hudIcon = "mortar";
	heading = $InvHeading[Weapon];
	shadowDetailMask = 4;
	imageType = Tank2CannonImage;
	price = 125;
	showWeaponBar = false;
	showInventory = false;
};

$ItemDescription[Tank2Cannon] = "<jc><f1>Tank Machine Gun:<f0> A powerful machine gun turret mounted on the tank";

function Tank2CannonImage::onFire(%player, %slot) {
  Weapon::fireProjectile(%player, Tank2CannonBullet);
}

function Tank2Cannon::onMount(%player, %slot) {
  Weapon::displayDescription(%player, Tank2Cannon);
  Player::mountItem(%player, Tank2Cannon2, 3);
  Player::mountItem(%player, Tank2Cannon3, 4);
  Player::mountItem(%player, Tank2Cannon4, 5);
  Player::mountItem(%player, Tank2Cannon5, 6);
  Weapon::standardMount(%player, Tank2Cannon);
}
function Tank2Cannon::onUnmount(%player, %slot) {
  Player::unmountItem(%player, 3);
  Player::unmountItem(%player, 4);
  Player::unmountItem(%player, 5);
  Player::unmountItem(%player, 6);
  Weapon::standardUnmount(%player, Tank2Cannon);
}

//setArmorAllowsItem(larmor, JuggernaughtCannon, 0);
//setArmorAllowsItem(lfemale, JuggernaughtCannon, 0);
//setArmorAllowsItem(SpyMale, JuggernaughtCannon, 0);
//setArmorAllowsItem(SpyFemale, JuggernaughtCannon, 0);
//setArmorAllowsItem(DMMale, JuggernaughtCannon, 0);
//setArmorAllowsItem(DMFemale, JuggernaughtCannon, 0);