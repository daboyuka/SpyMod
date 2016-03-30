SoundData SoundFireMG27 {
   wavFileName = "sniper.wav";//"ricoche2.wav";
   profile = Profile3dMedium;
};
SoundData SoundLoadMG27 {
   wavFileName = "Pku_weap.wav";
   profile = Profile3dNear;
};

ItemData MG27Ammo {
	description = "MG27 Ammo";
	heading = $InvHeading[Ammo];
	className = "Ammo";
	shapeFile = "plasammo";
	shadowDetailMask = 4;
	price = 2;
};

BulletData MG27Bullet {
   bulletShapeName    = "bullet.dts";
   explosionTag       = bulletExp0;
   expRandCycle       = 3;
   bulletHoleIndex    = 0;

   damageClass        = 0;                 // 0 = impact, 1 = radius
   damageValue        = 27;
   damageType         = $BulletDamageType;

   muzzleVelocity     = 600.0;
   totalTime          = 0.25;
   liveTime           = 0.25;
   lightRange         = 1.0;
   lightColor         = { 1, 1, 0 };
   inheritedVelocityScale = 0;
   isVisible          = False;

   aimDeflection      = 0.0005;
   tracerPercentage   = 1.0;
   tracerLength       = 10;

   soundId = SoundJetLight;
};

ItemImageData MG27Image {
	shapeFile = "paintgun";
	mountPoint = 0;

	weaponType = 0;
	reloadTime = 0.4; 
	fireTime = 0;

	ammoType = LoadedAmmo;//DragonAmmo;

	accuFire = true;

	lightType = 3;  // 1 = sustained, 2 = pulsing, 3 = weapon fire
	lightRadius = 5;
	lightTime = 0.25;
	lightColor = { 1, 1, 0.8 };

	sfxFire = SoundFireMG27;
	sfxActivate = SoundPickUpWeapon;
};

ItemData MG27 {
	description = "MG27";
	className = "Weapon";
	shapeFile = "paintgun";
	hudIcon = "mortar";
	heading = $InvHeading[Weapon];
	shadowDetailMask = 4;
	imageType = MG27Image;
	price = 125;
	showWeaponBar = false;//true;
};



ItemImageData MG27DisplayImage {
	shapeFile = "breath";
	firstPerson = false;
	ammoType = MG27Ammo;
};

ItemData MG27Display {
	description = "";
	className = "Tool";
	shapeFile = "breath";
	hudIcon = "ammopack";
	shadowDetailMask = 4;
	imageType = MG27DisplayImage;
	showWeaponBar = true;
	showInventory = false;
};



$ClipSize[MG27] = 8;
$AmmoDisplay[MG27] = MG27Display;
$ItemDescription[MG27] = "<jc><f1>MG27:<f0> A basic pistol; fires straight for medium damage, with a medium reload rate";

function MG27Image::onFire(%player, %slot) {
  if (Player::getItemCount(%player, LoadedAmmo) == 0) return;
  Weapon::fireProjectile(%player, MG27Bullet);
  Player::decItemCount(%player, LoadedAmmo);//DragonAmmo);
}

function MG27::onMount(%player, %slot) {
  Weapon::displayDescription(%player, MG27);
  Weapon::standardMount(%player, MG27);
}
function MG27::onUnmount(%player, %slot) {
  Weapon::standardUnmount(%player, MG27);
}

function MG27::onNoAmmo(%player) {
  MG27::reload(%player);
}

function MG27::reload(%player) {
//  if (%player.reloading[MG27]) return;
//  %player.reloading[MG27] = true;

//  Weapon::returnClipAmmo(%player, MG27);

//  schedule(%player @ ".reloading[MG27] = false;" @
//           "if (getSimTime() - "@%player@".lastUnmount[MG27] > 2 && !Player::isDead("@%player@")) Weapon::loadClip("@%player@", MG27);", 1);
  Weapon::standardReload(%player, MG27, SoundLoadMG27, 1);
}

registerWeapon(MG27, MG27Ammo, 8);
linkWeapon(MG27);
setArmorAllowsItem(larmor, MG27, 40);
setArmorAllowsItem(lfemale, MG27, 40);
setArmorAllowsItem(SpyMale, MG27, 40);
setArmorAllowsItem(SpyFemale, MG27, 40);
setArmorAllowsItem(DMMale, MG27, 40);
setArmorAllowsItem(DMFemale, MG27, 40);