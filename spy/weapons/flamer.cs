SoundData SoundFireFlamer {
   wavFileName = "dryfire1.wav";//"ricoche2.wav";
   profile = Profile3dMedium;
};
SoundData SoundLoadFlamer {
   wavFileName = "Pku_weap.wav";
   profile = Profile3dNear;
};

ItemData FlamerAmmo {
	description = "Flamer Ammo";
	heading = $InvHeading[Ammo];
	className = "Ammo";
	shapeFile = "plasammo";
	shadowDetailMask = 4;
	price = 2;
};

ItemImageData FlamerImage2 {
	shapeFile = "mortar";
	mountPoint = 0;
	mountOffset = "-0.12 -0.05 0";
};

ItemData Flamer2 {
	shapeFile = "mortar";
	imageType = FlamerImage2;
};

ItemImageData FlamerImage3 {
	shapeFile = "mortar";
	mountPoint = 0;
	mountOffset = "0.12 -0.05 0";
};

ItemData Flamer3 {
	shapeFile = "mortar";
	imageType = FlamerImage3;
};

ItemImageData FlamerImage4 {
	shapeFile = "grenade";
	mountPoint = 0;
	mountOffset = "0 -0.15 0.15";
	mountRotation = "1.57 0 0";
};

ItemData Flamer4 {
	shapeFile = "grenade";
	imageType = FlamerImage4;
};

GrenadeData FlamerFire {
   bulletShapeName    = "plasmatrail.dts";
   explosionTag       = PlasmaExp;
   collideWithOwner   = True;
   ownerGraceMS       = 250;
   collisionRadius    = 0.2;
   mass               = 1.0;
   elasticity         = 0.05;

   damageClass        = 1;       // 0 impact, 1, radius
   damageValue        = 7;
   damageType         = $PlasmaDamageType;

   explosionRadius    = 5;
   kickBackStrength   = 0;
   maxLevelFlightDist = 0;
   totalTime          = 5.0;    // special meaning for grenades...
   liveTime           = 0;//1.0;
   projSpecialTime    = 0.05;

   inheritedVelocityScale = 1;

   smokeName              = "plasmatrail.dts";
};

ItemImageData FlamerImage {
	shapeFile = "plasma";
	mountPoint = 0;
	mountOffset = "0 -0.1 0";

	weaponType = 0;
	reloadTime = 0.33; 
	fireTime = 0;

	ammoType = LoadedAmmo;

	accuFire = false;

	lightType = 3;  // 1 = sustained, 2 = pulsing, 3 = weapon fire
	lightRadius = 3;
	lightTime = 0.25;
	lightColor = { 1, 0.5, 0.1 };

	sfxFire = SoundFireFlamer;
	sfxActivate = SoundPickUpWeapon;
};

ItemData Flamer {
	description = "Flamer";
	className = "Weapon";
	shapeFile = "plasma";
	hudIcon = "mortar";
	heading = $InvHeading[Weapon];
	shadowDetailMask = 4;
	imageType = FlamerImage;
	price = 125;
	showWeaponBar = false;//true;
};



ItemImageData FlamerDisplayImage {
	shapeFile = "breath";
	firstPerson = false;
	ammoType = FlamerAmmo;
};

ItemData FlamerDisplay {
	description = "";
	className = "Tool";
	shapeFile = "breath";
	hudIcon = "ammopack";
	shadowDetailMask = 4;
	imageType = FlamerDisplayImage;
	showWeaponBar = true;
	showInventory = false;
};



$ClipSize[Flamer] = 20;
$AmmoDisplay[Flamer] = FlamerDisplay;
$ItemDescription[Flamer] = "<jc><f1>Flamer:<f0> A flame thrower, keep clear of the area";

function FlamerImage::onFire(%player, %slot) {
  if (Player::getItemCount(%player, LoadedAmmo) == 0) return;

  %vel = Matrix::mul(GameBase::getMuzzleTransform(%player),3,3,"0 18 0",3,1);
  %p = Projectile::spawnProjectile(FlamerFire, GameBase::getMuzzleTransform(%player), %player, %vel);
  schedule("Flamer::doSplitFire("@%player@","@%p@");", 0.35);
  Player::decItemCount(%player, LoadedAmmo);
}

function Flamer::doSplitFire(%player, %p) {
  if (!isObject(%p)) return;

  %vel = Item::getVelocity(%p);
  if (%vel == "0 0 0") {
    Projectile::spawnProjectile(FlamerFire, "1 0 0 0 1 0 0 0 1 " @ GameBase::getPosition(%p), %player, "0 0 0");
    Projectile::spawnProjectile(FlamerFire, "1 0 0 0 1 0 0 0 1 " @ GameBase::getPosition(%p), %player, "0 0 0");
    Projectile::spawnProjectile(FlamerFire, "1 0 0 0 1 0 0 0 1 " @ GameBase::getPosition(%p), %player, "0 0 0");
  } else {
    %forwardVec = Vector::normalize(getWord(%vel, 0) @ " " @ getWord(%vel, 1) @ " 0");//Vector::mul(%forwardVec, 2)
    %sideVec = -getWord(%forwardVec, 1) @ " " @ getWord(%forwardVec, 0) @ " 0";
    %va1 = Vector::add(Vector::add("0 0 1.5", Vector::mul(%forwardVec, 1)), Vector::mul(%sideVec, getRandom() * 4 - 2));
    %va2 = Vector::add(Vector::add("0 0 3", Vector::mul(%forwardVec, 2)), Vector::mul(%sideVec, getRandom() * 4 - 2));
    %va3 = Vector::add(Vector::add("0 0 4.5", Vector::mul(%forwardVec, 3)), Vector::mul(%sideVec, getRandom() * 4 - 2));
    Projectile::spawnProjectile(FlamerFire, "1 0 0 0 1 0 0 0 1 " @ GameBase::getPosition(%p), %player, Vector::add(%vel, %va1));
    Projectile::spawnProjectile(FlamerFire, "1 0 0 0 1 0 0 0 1 " @ GameBase::getPosition(%p), %player, Vector::add(%vel, %va2));
    Projectile::spawnProjectile(FlamerFire, "1 0 0 0 1 0 0 0 1 " @ GameBase::getPosition(%p), %player, Vector::add(%vel, %va3));
  }
}

function Flamer::onMount(%player, %slot) {
  Weapon::displayDescription(%player, Flamer);
  Weapon::standardMount(%player, Flamer);
  Player::mountItem(%player, Flamer2, 3);
  Player::mountItem(%player, Flamer3, 4);
  Player::mountItem(%player, Flamer4, 5);
}
function Flamer::onUnmount(%player, %slot) {
  Weapon::clearDescription(%player);
  Player::unmountItem(%player, 3);
  Player::unmountItem(%player, 4);
  Player::unmountItem(%player, 5);
  Weapon::standardUnmount(%player, Flamer);
}

function Flamer::onNoAmmo(%player) {
  Flamer::reload(%player);
}

function Flamer::reload(%player) {
  Weapon::standardReload(%player, Flamer, SoundLoadFlamer, 1);
}

registerWeapon(Flamer, FlamerAmmo, 20);
linkWeapon(Flamer);
setArmorAllowsItem(larmor, Flamer, 80);
setArmorAllowsItem(lfemale, Flamer, 80);
setArmorAllowsItem(SpyMale, Flamer, 80);
setArmorAllowsItem(SpyFemale, Flamer, 80);
setArmorAllowsItem(DMMale, Flamer, 80);
setArmorAllowsItem(DMFemale, Flamer, 80);