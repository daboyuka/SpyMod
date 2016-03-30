SoundData SoundFireAGP84 {
   wavFileName = "sniper.wav";//"ricoche2.wav";
   profile = Profile3dMedium;
};
SoundData SoundLoadAGP84 {
   wavFileName = "mortar_reload.wav";
   profile = Profile3dNear;
};
SoundData SoundAGP84BoltExp {
   wavFileName = "debris_small.wav";
   profile = Profile3dMedium;
};

ItemData AGP84Ammo {
	description = "AGP84 Ammo";
	heading = $InvHeading[Ammo];
	className = "Ammo";
	shapeFile = "plasammo";
	shadowDetailMask = 4;
	price = 2;
};

ExplosionData AGP84BoltExp {
   shapeName = "tumult_small.dts";
   soundId   = SoundAGP84BoltExp;

   faceCamera = true;
   randomSpin = true;
   hasLight   = true;
   lightRange = 3.0;

//   timeScale = 1;

   timeZero = 0.250;
   timeOne  = 0.850;

   colors[0]  = { 0.5, 0.4,  0.4 };
   colors[1]  = { 1.0, 1.0,  1.0 };
   colors[2]  = { 1.0, 0.4, 0.3 };
   radFactors = { 0.5, 1.0, 1.0 };
};

BulletData AGP84Bolt {
   bulletShapeName    = "bullet.dts";
   explosionTag       = AGP84BoltExp;

   damageClass        = 1;                 // 0 = impact, 1 = radius
   damageValue        = 30;
   damageType         = $AGPDamageType;

   explosionRadius    = 2.5;
   kickBackStrength   = 50;

   muzzleVelocity     = 400.0;
   totalTime          = 1.25;
   liveTime           = 1.25;
   lightRange         = 2.0;
   lightColor         = { 1, 0.8, 0 };
   inheritedVelocityScale = 0.3;
   isVisible          = True;

   soundId = SoundJetLight;
};

ItemImageData AGP84Image2 {
	shapeFile = "force";
	mountPoint = 0;
	mountRotation = "-1.57 0 0";
	mountOffset = "0 0.03 0.085";
};

ItemData AGP842 {
	shapeFile = "force";
	imageType = AGP84Image2;
};

ItemImageData AGP84Image {
	shapeFile = "paintgun";
	mountPoint = 0;

	weaponType = 0;
	reloadTime = 0; 
	fireTime = 0.25;

	ammoType = LoadedAmmo;

	accuFire = false;

	lightType = 3;  // 1 = sustained, 2 = pulsing, 3 = weapon fire
	lightRadius = 5;
	lightTime = 0.25;
	lightColor = { 1, 1, 0.8 };

	sfxFire = SoundFireAGP84;
	sfxActivate = SoundPickUpWeapon;
};

ItemData AGP84 {
	description = "AGP84";
	className = "Weapon";
	shapeFile = "paintgun";
	hudIcon = "mortar";
	heading = $InvHeading[Weapon];
	shadowDetailMask = 4;
	imageType = AGP84Image;
	price = 125;
	showWeaponBar = false;//true;
};



ItemImageData AGP84DisplayImage {
	shapeFile = "breath";
	firstPerson = false;
	ammoType = AGP84Ammo;
};

ItemData AGP84Display {
	description = "";
	className = "Tool";
	shapeFile = "breath";
	hudIcon = "ammopack";
	shadowDetailMask = 4;
	imageType = AGP84DisplayImage;
	showWeaponBar = true;
	showInventory = false;
};



$ClipSize[AGP84] = 1;
$AmmoDisplay[AGP84] = AGP84Display;
$ItemDescription[AGP84] = "<jc><f1>AGP84:<f0> Fires small explosive bolts; use to dislodge grappler hooks";

function AGP84Image::onFire(%player, %slot) {
  if (Player::getItemCount(%player, LoadedAmmo) == 0) return;
  Weapon::fireProjectile(%player, AGP84Bolt);
  Player::decItemCount(%player, LoadedAmmo);

  if (Player::getItemCount(%player, LoadedAmmo) == 0 && Player::getMountedItem(%player, 3) == "AGP842")
    Player::unmountItem(%player, 3);
}

function AGP84::onMount(%player, %slot) {
  Weapon::displayDescription(%player, AGP84);
  Weapon::standardMount(%player, AGP84);
  if (Player::getItemCount(%player, LoadedAmmo) > 0)
    Player::mountItem(%player, AGP842, 3);
}
function AGP84::onUnmount(%player, %slot) {
  Weapon::standardUnmount(%player, AGP84);
  if (Player::getMountedItem(%player, 3) == "AGP842")
    Player::unmountItem(%player, 3);
}

function AGP84::onNoAmmo(%player) {
  AGP84::reload(%player);
}

function AGP84::reload(%player) {
  Weapon::standardReload2(%player, AGP84,
                          "GameBase::playSound("@%player@",SoundLoadAGP84,0);Player::mountItem("@%player@",AGP842,3);",
                          3);
}

registerWeapon(AGP84, AGP84Ammo, 1);
linkWeapon(AGP84);
setArmorAllowsItem(larmor, AGP84, 5);
setArmorAllowsItem(lfemale, AGP84, 5);
setArmorAllowsItem(SpyMale, AGP84, 5);
setArmorAllowsItem(SpyFemale, AGP84, 5);
setArmorAllowsItem(DMMale, AGP84, 5);
setArmorAllowsItem(DMFemale, AGP84, 5);