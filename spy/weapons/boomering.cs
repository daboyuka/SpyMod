SoundData SoundFireBoomering {
   wavFileName = "sniper.wav";//"ricoche2.wav";
   profile = Profile3dMedium;
};
SoundData SoundLoadBoomering {
   wavFileName = "Pku_weap.wav";
   profile = Profile3dNear;
};

function comment() {
BulletData BoomeringDisc {
   bulletShapeName    = "discb.dts";
   explosionTag       = bulletExp0;
   expRandCycle       = 3;
   bulletHoleIndex    = 0;

   damageClass        = 0;                 // 0 = impact, 1 = radius
   damageValue        = 70;
   damageType         = $BulletDamageType;

   muzzleVelocity     = 5.0;
   totalTime          = 10;
   liveTime           = 10;
   lightRange         = 1.0;
   lightColor         = { 1, 1, 0 };
   inheritedVelocityScale = 1;
   isVisible          = True;

   soundId = SoundJetLight;
};
}

function c() {
RocketData BoomeringDisc {
   bulletShapeName = "discb.dts";
   explosionTag    = rocketExp;

   collisionRadius = 0.0;
   mass            = 2.0;

   damageClass      = 0;       // 0 impact, 1, radius
   damageValue      = 1;
   damageType       = $BoomeringDamageType;

   muzzleVelocity   = 5.0;
   terminalVelocity = 5.0;
   acceleration     = 0.0;

   totalTime        = 10;
   liveTime         = 10;

   lightRange       = 5.0;
   lightColor       = { 0.4, 0.4, 1.0 };

   inheritedVelocityScale = 1;

   // rocket specific
   trailType   = 1;
   trailLength = 15;
   trailWidth  = 0.3;

   soundId = SoundDiscSpin;
};
}

SeekingMissileData BoomeringDisc {
   bulletShapeName = "discb.dts";
   explosionTag    = flashExpSmall;
   collisionRadius = 0.0;
   mass            = 2.0;

   damageClass      = 1;       // 0 impact, 1, radius
   damageValue      = 100*10;
   damageType       = $BoomeringDamageType;

   explosionRadius = 1.1;
   kickBackStrength = 0;

   muzzleVelocity    = 35.0;
   totalTime         = 10;
   liveTime          = 10;
   seekingTurningRadius    = 1000;
   nonSeekingTurningRadius = 1000;
   proximityDist     = 0;
   smokeDist         = 100000;

   lightRange       = 5.0;
   lightColor       = { 0.4, 0.4, 1.0 };

   inheritedVelocityScale = 0;

   soundId = SoundJetHeavy;
};


ItemData Boomering;

ItemImageData BoomeringImage {
	shapeFile = "breath";
	mountPoint = 0;

	weaponType = 0;
	reloadTime = 1; 
	fireTime = 0;

	ammoType = Boomering;//DragonAmmo;

	accuFire = true;

	sfxFire = SoundFireBoomering;
	sfxActivate = SoundPickUpWeapon;
	firstPerson = false;
};

ItemData Boomering {
	description = "Boomering";
	className = "Weapon";
	shapeFile = "discb";
	hudIcon = "mortar";
	heading = $InvHeading[Weapon];
	shadowDetailMask = 4;
	imageType = BoomeringImage;
	price = 125;
	showWeaponBar = false;
};



ItemImageData BoomeringDisplayImage {
	shapeFile = "breath";
	firstPerson = false;
	ammoType = Boomering;
};

ItemData BoomeringDisplay {
	description = "";
	className = "Tool";
	shapeFile = "breath";
	hudIcon = "ammopack";
	shadowDetailMask = 4;
	imageType = BoomeringDisplayImage;
	showWeaponBar = true;
	showInventory = false;
};



$ClipSize[Boomering] = "";
$AmmoDisplay[Boomering] = BoomeringDisplay;
$ItemDescription[Boomering] = "<jc><f1>Boomering:<f0> Blade-rimmed disc that returns when thrown (blades retract :D)";

function BoomeringImage::onFire(%player, %slot) {
  if (Player::getItemCount(%player, Boomering) == 0) return;

  %p = Projectile::spawnProjectile(BoomeringDisc, GameBase::getMuzzleTransform(%player), %player, 0, -1);//Weapon::fireProjectile(%player, BoomeringDisc);
  %p.timeLeft = 0.5;
  Boomering::doDisc(%player, %p);
  Player::decItemCount(%player, Boomering);
}

function Boomering::onMount(%player, %slot) {
  Weapon::displayDescription(%player, Boomering);
  Weapon::standardMount(%player, Boomering);
}
function Boomering::onUnmount(%player, %slot) {
  Weapon::standardUnmount(%player, Boomering);
}

function Boomering::onNoAmmo(%player) {}

function Boomering::reload(%player) {}

$Boomering::updateInterval = 0.125;
$Boomering::turnPower = 7;
function Boomering::doDisc(%player, %disc) {
  if (!isObject(%disc)) { messageAll(1,"Boomering dead..."); return; }

  if (%disc.timeLeft <= 0) {
    %vel = Item::getVelocity(%disc);
    %yourPos = Vector::add(getBoxCenter(%player), "0 0 0.5");//Matrix::subMatrix(GameBase::getMuzzleTransform(%player), 3, 4, 3, 1, 0, 3);
    %discPos = GameBase::getPosition(%disc);
    %posDiff = Vector::sub(%yourPos, %discPos);
    %posDiffLen = Vector::length(%posDiff);

    if (%posDiffLen < 2) {
      deleteObject(%disc);
      Player::incItemCount(%player, Boomering);
      messageAll(1,"YOU CAUGHT IT!!");
      return;
    }

    %rightDir = Vector::mul(%posDiff, 35 / %posDiffLen);

    %diff = Vector::sub(%rightDir, %vel);
    %len = Vector::length(%diff);

    %vel = Vector::add(%vel, Vector::mul(%diff, min($Boomering::turnPower, %len) / %len));

    Item::setVelocity(%disc,%vel);
    echo("YAY!");
  } else %disc.timeLeft -= $Boomering::updateInterval;

  echo(%player, ",", %disc);

  schedule("Boomering::doDisc("@%player@","@%disc@");", $Boomering::updateInterval);
}

registerWeapon(Boomering, Boomering, 1);
linkWeapon(Boomering);
setArmorAllowsItem(larmor, Boomering, 2);
setArmorAllowsItem(lfemale, Boomering, 2);
setArmorAllowsItem(SpyMale, Boomering, 2);
setArmorAllowsItem(SpyFemale, Boomering, 2);
setArmorAllowsItem(DMMale, Boomering, 2);
setArmorAllowsItem(DMFemale, Boomering, 2);