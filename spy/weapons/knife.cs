SoundData SoundFireKnife {
   wavFileName = "throwitem.wav";
   profile = Profile3dNear;
};

SoundData SoundKnifeExplosion {
   wavFileName = "dryfire1.wav";
   profile     = Profile3dNear;
};

ExplosionData KnifeExplosion {
   shapeName = "laserhit.dts";
   //soundId   = SoundKnifeExplosion;

   faceCamera = true;
   randomSpin = true;

   timeScale = 0.4;

   timeZero = 0.250;
   timeOne  = 0.850;
};

BulletData KnifeSlash {
   bulletShapeName    = "bullet.dts";
   explosionTag       = KnifeExplosion;

   damageClass        = 0;                 // 0 = impact, 1 = radius
   damageValue        = 60;
   damageType         = $KnifeDamageType;

   muzzleVelocity     = 30.0;
   totalTime          = 0.09; //0.08 too short
   liveTime           = 0.09; //0.08 too short
   inheritedVelocityScale = 1;
   isVisible          = False;
};

BulletData KnifeStab {
   bulletShapeName    = "bullet.dts";
   explosionTag       = KnifeExplosion;

   damageClass        = 0;                 // 0 = impact, 1 = radius
   damageValue        = 12;//4.3;
   damageType         = $KnifeStabDamageType;

   muzzleVelocity     = 27;
   totalTime          = 0.15; //0.19 too far
   liveTime           = 0.15; //0.19 too far
   inheritedVelocityScale = 1.3; //1.3
   isVisible         = False;
};

BulletData KnifeThrow {
   bulletShapeName    = "force.dts";
   explosionTag       = noExp;//KnifeExplosion;

   damageClass        = 0;                 // 0 = impact, 1 = radius
   damageValue        = 50;
   damageType         = $KnifeDamageType;

   muzzleVelocity     = 50;
   totalTime          = 10;
   liveTime           = 10;
   inheritedVelocityScale = 0.5;

   rotationPeriod = 0.2;
};

RepairEffectData KnifeTriggerBolt {
   bitmapName       = "bar00.bmp";
   boltLength       = 0;
   segmentDivisions = 1;
   beamWidth        = 0;

   updateTime   = 4000;
   skipPercent  = 0.6;
   displaceBias = 0.15;
};

ItemImageData KnifeStab1Image {
	shapeFile = "force";
	mountPoint = 0;
	mountRotation = "1.57 0 0";
	mountOffset = "-0.1 0 -0.05";
};

ItemData KnifeStab1 {
	shapeFile = "breath";
	imageType = KnifeStab1Image;
};

ItemImageData KnifeStab2Image {
	shapeFile = "force";
	mountPoint = 0;
	mountRotation = "1.57 0 0";
	mountOffset = "-0.1 0.3 -0.05";
};

ItemData KnifeStab2 {
	shapeFile = "breath";
	imageType = KnifeStab2Image;
};

ItemImageData KnifeM1Image {
	shapeFile = "force";
	mountPoint = 0;
	mountRotation = "0 3.14 0";
	mountOffset = "-0.1 -0.15 0.1";

	weaponType = 0;
	reloadTime = 0; 
	fireTime = 0.1;//0.25;//0.45;

	maxEnergy = 0;
	minEnergy = 0;

	projectileType = KnifeSlash;

	accuFire = true;

//	sfxFire = SoundFireKnife;
};

ItemData KnifeM1 {
	shapeFile = "breath";
	imageType = KnifeM1Image;
};

ItemImageData KnifeM2Image {
	shapeFile = "breath";
	mountPoint = 0;

	weaponType = 0;
	reloadTime = 0; 
	fireTime = 0.2;

	maxEnergy = 0;
	minEnergy = 0;

	projectileType = KnifeStab;

	accuFire = true;

	firstPerson = false;
};

ItemData KnifeM2 {
	shapeFile = "breath";
	imageType = KnifeM2Image;
};

ItemImageData KnifeImage {
	shapeFile = "breath";
	mountPoint = 0;
	mountOffset = "-0.1 0.85 0";

	weaponType = 2;
	reloadTime = 0;
	fireTime = 0;
	projectileType = KnifeTriggerBolt;

        minEnergy = 0;
        maxEnergy = 0;

	accuFire = true;

	firstPerson = false;
	isVisible = false;
};

ItemData Knife {
	description = "Knife";
	className = "Weapon";
	shapeFile = "force";
	hudIcon = "mortar";
	heading = $InvHeading[Weapon];
	shadowDetailMask = 4;
	imageType = KnifeImage;
	price = 125;
	showWeaponBar = false;
};

// This goes here 'cause it needs 'Knife' to be defined first, for ammo
ItemImageData KnifeM3Image {
	shapeFile = "force";
	mountPoint = 0;
	mountRotation = "0 3.14 0";
	mountOffset = "-0.1 -0.15 0.1";

	weaponType = 0;
	reloadTime = 0; 
	fireTime = 3;

	maxEnergy = 0;
	minEnergy = 0;

	//projectileType = KnifeThrow;
//	ammoType = Knife;

	accuFire = true;
};

ItemData KnifeM3 {
	shapeFile = "breath";
	imageType = KnifeM3Image;
};

$ItemDescription[Knife, 0] = "<jc><f1>Knife, Slash Mode:<f0> Hacks and slashes your opponent to pieces";
$ItemDescription[Knife, 1] = "<jc><f1>Knife, Stab Mode:<f0> Stab your enemy; the faster you are moving, the more it hurts";
$ItemDescription[Knife, 2] = "<jc><f1>Knife, Throw Mode:<f0> Throw your knife for extreme damage";

$NumModes[Knife] = 2;

function KnifeTriggerBolt::onAcquire(%this, %player, %target) {
  Player::trigger(%player, 3, true);
  if (Weapon::getMode(%player, Knife) == 1) Player::mountItem(%player, KnifeStab2, 4);
}
function KnifeTriggerBolt::onRelease(%this, %player) {
  Player::trigger(%player, 3, false);
  if (Weapon::getMode(%player, Knife) == 1) Player::mountItem(%player, KnifeStab1, 4);
}
function KnifeTriggerBolt::checkDone(%this, %player) {}

function KnifeM3Image::onFire(%player, %slot) {
  if (Player::getItemCount(%player, Knife) > 0) {
    Weapon::fireProjectile(%player, KnifeThrow);
    Player::decItemCount(%player, Knife);
  }
}

function Knife::onMount(%player, %slot) {
  Weapon::displayDescription(%player, Knife);
  %mode = Weapon::getMode(%player, Knife);
  if (%mode == 0) Player::mountItem(%player, KnifeM1, 3);
  if (%mode == 1) {
    Player::mountItem(%player, KnifeM2, 3);
    Player::mountItem(%player, KnifeStab1, 4);
  }
  if (%mode == 2) Player::mountItem(%player, KnifeM3, 3);
  Weapon::standardMount(%player, Knife);
}
function Knife::onUnmount(%player, %slot) {
  Weapon::clearDescription(%player);
  Player::trigger(%player, 0, false);
  Player::trigger(%player, 3, false);
  Player::unmountItem(%player, 3);
  Player::unmountItem(%player, 4);
  Weapon::standardUnmount(%player, Knife);
}

function Knife::onNoAmmo(%player) {}

function Knife::onModeChanged(%player, %mode) {
  Weapon::displayDescription(%player, Knife);
  if (%mode == 0) {
    Player::mountItem(%player, KnifeM1, 3);
    Player::unmountItem(%player, 4);
  }
  if (%mode == 1) {
    Player::mountItem(%player, KnifeM2, 3);
    Player::mountItem(%player, KnifeStab1, 4);
  }
  if (%mode == 2) {
    Player::mountItem(%player, KnifeM3, 3);
    Player::unmountItem(%player, 4);
  }
}

registerWeapon(Knife, Knife, 1);
linkWeapon(Knife);
setArmorAllowsItem(larmor, Knife, 1);
setArmorAllowsItem(lfemale, Knife, 1);
setArmorAllowsItem(SpyMale, Knife, 1);
setArmorAllowsItem(SpyFemale, Knife, 1);
setArmorAllowsItem(DMMale, Knife, 1);
setArmorAllowsItem(DMFemale, Knife, 1);