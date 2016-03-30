SoundData SoundFireMagnum {
   wavFileName = "mine_exp.wav";
   profile = Profile3dNear;
};

ItemData MagnumAmmo {
	description = "Magnum Ammo";
	heading = $InvHeading[Ammo];
	className = "Ammo";
	shapeFile = "plasammo";
	shadowDetailMask = 4;
	price = 2;
};

BulletData MagnumBullet {
   bulletShapeName    = "bullet.dts";
   explosionTag       = bulletExp0;
   expRandCycle       = 3;
   bulletHoleIndex    = 0;

   damageClass        = 0;                 // 0 = impact, 1 = radius
   damageValue        = 55;
   damageType         = $BulletDamageType;

   muzzleVelocity     = 400.0;
   totalTime          = 0.5;
   liveTime           = 0.5;
   lightRange         = 3.0;
   lightColor         = { 1, 1, 0 };
   inheritedVelocityScale = 0.3;
   isVisible          = False;

   aimDeflection      = 0.005;
   tracerPercentage   = 1.0;
   tracerLength       = 30;

   soundId = SoundJetLight;
};

ItemImageData MagnumImage {
	shapeFile = "energygun";
	mountPoint = 0;

	weaponType = 0;
	reloadTime = 0; 
	fireTime = 0.95;

	ammoType = LoadedAmmo;//MagnumAmmo;

//	projectileType = MagnumBullet;
	accuFire = false;

	lightType = 3;  // 1 = sustained, 2 = pulsing, 3 = weapon fire
	lightRadius = 5;
	lightTime = 0.25;
	lightColor = { 1, 1, 0.8 };

	sfxFire = SoundFireMagnum;
	sfxActivate = SoundPickUpWeapon;
};

ItemData Magnum {
	description = "Magnum";
	className = "Weapon";
	shapeFile = "energygun";
	hudIcon = "mortar";
	heading = $InvHeading[Weapon];
	shadowDetailMask = 4;
	imageType = MagnumImage;
	price = 125;
	showWeaponBar = false;
};

ItemImageData MagnumDisplayImage {
	shapeFile = "breath";
	ammoType = MagnumAmmo;
};

ItemData MagnumDisplay {
	className = "Tool";
	shapeFile = "breath";
	hudIcon = "ammopack";
	imageType = MagnumDisplayImage;
	showWeaponBar = true;
	showInventory = false;
};

$ClipSize[Magnum] = 6;
$AmmoDisplay[Magnum] = MagnumDisplay;
$ItemDescription[Magnum] = "<jc><f1>Magnum:<f0> A high-caliber pistol that can inflict large amounts of damage.";

function MagnumImage::onFire(%player, %slot) {
  if (Player::getItemCount(%player, LoadedAmmo) == 0) return;
  Weapon::fireProjectile(%player, MagnumBullet);
  Player::decItemCount(%player, LoadedAmmo);//MagnumAmmo);
}

function Magnum::onMount(%player, %slot) {
  Weapon::displayDescription(%player, Magnum);
  Weapon::standardMount(%player, Magnum);
}
function Magnum::onUnmount(%player, %slot) {
  Weapon::standardUnmount(%player, Magnum);
}

function Magnum::onNoAmmo(%player) {
  Magnum::reload(%player);
}

function Magnum::reload(%player) {
  Weapon::standardReload(%player, Magnum, "", 2);
}

registerWeapon(Magnum, MagnumAmmo, 5);
linkWeapon(Magnum);
setArmorAllowsItem(larmor, Magnum, 30);
setArmorAllowsItem(lfemale, Magnum, 30);
setArmorAllowsItem(SpyMale, Magnum, 30);
setArmorAllowsItem(SpyFemale, Magnum, 30);
setArmorAllowsItem(DMMale, Magnum, 30);
setArmorAllowsItem(DMFemale, Magnum, 30);

// All weapons have LoadedAmmo as their ammo type, but is not shown on the weapon list
// All weapons have an accompanying item which has the weapon's normal ammo as its ammo type
// So the weapon relies on LoadedAmmo, but the weapons real ammo type is displayed on the GUI
