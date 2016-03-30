SoundData SoundDoomSlayerFire {
   wavFileName = "BXplo2.wav";
   profile = Profile3dMedium;
};

SoundData SoundDoomSlayerReload {
   wavFileName = "turretturn4.wav";
   profile = Profile3dMedium;
};

SoundData SoundDoomSlayerSpinUp {
   wavFileName = "forceopen.wav";
   profile = Profile3dMedium;
};

SoundData SoundDoomSlayerSpinDown {
   wavFileName = "forceclose.wav";
   profile = Profile3dMedium;
};

ItemData DoomSlayerAmmo {
	description = "DoomSlayer Ammo";
	heading = $InvHeading[Ammo];
	className = "Ammo";
	shapeFile = "plasammo";
	shadowDetailMask = 4;
	price = 2;
};

BulletData DoomSlayerBolt {
   bulletShapeName    = "bullet.dts";//"plasmabolt.dts";
   explosionTag       = plasmaExp;

   damageClass        = 1;                 // 0 = impact, 1 = radius
   damageValue        = 50;
   damageType         = $PlasmaDamageType;

   explosionRadius    = 4.0;

   muzzleVelocity     = 100.0;
   totalTime          = 3;
   liveTime           = 3;
   lightRange         = 1.0;
   lightColor         = { 1, 1, 0 };
   inheritedVelocityScale = 0;
   isVisible          = true;

   aimDeflection      = 0.02;
   tracerLength       = 20;
   tracerPercentage   = 1;

   soundId = SoundJetLight;
};

ItemImageData DoomSlayerImage2 {
	shapeFile = "sniper";
	mountPoint = 0;
};

ItemData DoomSlayer2 {
	shapeFile = "sniper";
	imageType = DoomSlayerImage2;
};

ItemImageData DoomSlayerImage3 {
	shapeFile = "sniper";
	mountPoint = 0;
	mountRotation = "0 1.57 0";
};

ItemData DoomSlayer3 {
	shapeFile = "sniper";
	imageType = DoomSlayerImage3;
};

ItemImageData DoomSlayerImage4 {
	shapeFile = "sniper";
	mountPoint = 0;
	mountRotation = "0 3.14 0";
};

ItemData DoomSlayer4 {
	shapeFile = "sniper";
	imageType = DoomSlayerImage4;
};

ItemImageData DoomSlayerImage5 {
	shapeFile = "sniper";
	mountPoint = 0;
	mountRotation = "0 -1.57 0";
};

ItemData DoomSlayer5 {
	shapeFile = "sniper";
	imageType = DoomSlayerImage5;
};

ItemImageData DoomSlayerImage {
	shapeFile = "breath";
	mountPoint = 0;

	weaponType = 1;
	reloadTime = 0; 
	fireTime = 0.2;
	spinUpTime = 1;
	spinDownTime = 0.1;

	ammoType = LoadedAmmo;

	accuFire = true;

	firstPerson = false;

	lightType = 3;  // 1 = sustained, 2 = pulsing, 3 = weapon fire
	lightRadius = 10;
	lightTime = 2;
	lightColor = { 1, 0.7, 0.3 };

	sfxSpinUp = SoundDoomSlayerSpinUp;
	sfxSpinDown = SoundDoomSlayerSpinDown;
	sfxFire = SoundDoomSlayerFire;
	sfxActivate = SoundPickUpWeapon;
};

ItemData DoomSlayer {
	description = "DoomSlayer";
	className = "Weapon";
	shapeFile = "sniper";
	hudIcon = "mortar";
	heading = $InvHeading[Weapon];
	shadowDetailMask = 4;
	imageType = DoomSlayerImage;
	price = 125;
	showWeaponBar = false;//true;
};



ItemImageData DoomSlayerDisplayImage {
	shapeFile = "breath";
	firstPerson = false;
	ammoType = DoomSlayerAmmo;
};

ItemData DoomSlayerDisplay {
	description = "";
	className = "Tool";
	shapeFile = "breath";
	hudIcon = "ammopack";
	shadowDetailMask = 4;
	imageType = DoomSlayerDisplayImage;
	showWeaponBar = true;
	showInventory = false;
};

$ClipSize[DoomSlayer] = 1;
$AmmoDisplay[DoomSlayer] = DoomSlayerDisplay;
$ItemDescription[DoomSlayer] = "<jc><f1>DoomSlayer:<f0> Fires explosive shotgun bolts for maximum annihilation";

function DoomSlayerImage::onFire(%player, %slot) {
  if (Player::getItemCount(%player, LoadedAmmo) == 0) return;
  Weapon::fireProjectile(%player, DoomSlayerBolt);
  Weapon::fireProjectile(%player, DoomSlayerBolt);
  Weapon::fireProjectile(%player, DoomSlayerBolt);
  Weapon::fireProjectile(%player, DoomSlayerBolt);
  Weapon::fireProjectile(%player, DoomSlayerBolt);
  Weapon::fireProjectile(%player, DoomSlayerBolt);
  Weapon::fireProjectile(%player, DoomSlayerBolt);
  Weapon::fireProjectile(%player, DoomSlayerBolt);
  Player::decItemCount(%player, LoadedAmmo);//DragonAmmo);
}

function DoomSlayer::onMount(%player, %slot) {
  Weapon::displayDescription(%player, DoomSlayer);
  Player::mountItem(%player, DoomSlayer2, 3);
  Player::mountItem(%player, DoomSlayer3, 4);
  Player::mountItem(%player, DoomSlayer4, 5);
  Player::mountItem(%player, DoomSlayer5, 6);
  Weapon::standardMount(%player, DoomSlayer);
}
function DoomSlayer::onUnmount(%player, %slot) {
  Player::unmountItem(%player, 3);
  Player::unmountItem(%player, 4);
  Player::unmountItem(%player, 5);
  Player::unmountItem(%player, 6);
  Weapon::standardUnmount(%player, DoomSlayer);
}

function DoomSlayer::onNoAmmo(%player) {
  DoomSlayer::reload(%player);
}

function DoomSlayer::reload(%player) {
  Weapon::standardReload(%player, DoomSlayer, SoundDoomSlayerReload, 3.5);
}

registerWeapon(DoomSlayer, DoomSlayerAmmo, 1);
linkWeapon(DoomSlayer);
setArmorAllowsItem(larmor, DoomSlayer, 2);
setArmorAllowsItem(lfemale, DoomSlayer, 2);
setArmorAllowsItem(SpyMale, DoomSlayer, 2);
setArmorAllowsItem(SpyFemale, DoomSlayer, 2);
setArmorAllowsItem(DMMale, DoomSlayer, 2);
setArmorAllowsItem(DMFemale, DoomSlayer, 2);