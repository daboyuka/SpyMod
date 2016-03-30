LightningData AutoaimCharge {
   bitmapName       = "zap04.bmp";

   damageType       = 0;
   boltLength       = 400.0;
   coneAngle        = 30.0;
   damagePerSec      = 0;
   energyDrainPerSec = 0;
   segmentDivisions = 0;
   numSegments      = 1;
   beamWidth        = 0.00001;

   updateTime   = 10;
   skipPercent  = 0.01;
   displaceBias = 0.01;
};

function AutoaimCharge::damageTarget(%target, %timeSlice, %damPerSec, %enDrainPerSec, %pos, %vec, %mom, %shooterId) {
  if (getObjectType(%target) != "Player") return;
  Client::getOwnedObject(%shooterId).autoaimTarget = %target;
  Client::getOwnedObject(%shooterId).autoaimTargetTime = getSimTime() + 0.25;
  Player::trigger(%shooterId, 0, true);
  schedule("if (" @ Client::getOwnedObject(%shooterId) @ ".autoaimTargetTime < getSimTime()) Player::trigger("@%shooterId@", 0, false);", 0.26);
}

ItemImageData AutoAimerImage {
	shapeFile = "breath";
	mountPoint = 0;

	weaponType = 2;
	projectileType = AutoaimCharge;
	reloadTime = 0;

	minEnergy = 0;
	maxEnergy = 0;

	firstPerson = false;
};

ItemData AutoAimer {
	shapeFile = "shotgun";
   className = "Weapon";
   imageType = AutoAimerImage;
};

ItemImageData MG28Image {
	shapeFile = "paintgun";
	mountPoint = 0;

	weaponType = 0;
	reloadTime = 0.4; 
	fireTime = 0;

	minEnergy = 0;
	maxEnergy = 0;

	accuFire = false;

	sfxFire = SoundFireMG27;
	sfxActivate = SoundPickUpWeapon;
};

ItemData MG28 {
	description = "MG28";
	className = "Weapon";
	shapeFile = "paintgun";
	hudIcon = "mortar";
	heading = $InvHeading[Weapon];
	shadowDetailMask = 4;
	imageType = MG28Image;
	price = 125;
	showWeaponBar = false;//true;
};

$ClipSize[MG28] = "";
$ItemDescription[MG28] = "<jc><f1>MG28:<f0> Totally R0X with an ubzor auto-aiming MG27! W000000T!";


BulletData MG28Bullet
{
   bulletShapeName    = "bullet.dts";
   explosionTag       = bulletExp0;
   expRandCycle       = 3;
   bulletHoleIndex    = 0;

   damageClass        = 0;                 // 0 = impact, 1 = radius
   damageValue        = 27;
   damageType         = $BulletDamageType;

   muzzleVelocity     = 0.0;
   totalTime          = 10.25;
   liveTime           = 10.25;
   lightRange         = 1.0;
   lightColor         = { 1, 1, 0 };
   inheritedVelocityScale = 1;
   isVisible          = False;

   aimDeflection      = 0;
   tracerPercentage   = 1.0;
   tracerLength       = 10;

   soundId = SoundJetLight;
};

function MG28Image::onFire(%player, %slot) {
  if (%player.autoaimTargetTime > getSimTime()) {
    %gunPos = Matrix::subMatrix(GameBase::getMuzzleTransform(%player), 3, 4, 3, 1, 0, 3);
    %offset = tern(Player::isCrouching(%player.autoaimTarget), "0 0 1.5", "0 0 2");
    %vec = Vector::sub(Vector::add(GameBase::getPosition(%player.autoaimTarget), %offset), %gunPos);

    %vel = Item::getVelocity(%player.autoaimTarget);
    %vel = Vector::mul(%vel, (Vector::getDistance(%vec, 0) / 600 * 1.00485));

    %vec = Vector::add(%vec, %vel);
    %vec = Vector::resize(%vec, 600);


    Projectile::spawnProjectile(MG28Bullet, "1 0 0 0 1 0 0 0 1 " @ %gunPos,
                                %player, %vec);

  } else
    Weapon::fireProjectile(%player, MG27Bullet);
}

function MG28::onMount(%player, %slot) {
  Weapon::displayDescription(%player, MG28);
  Player::mountItem(%player, AutoAimer, 3);
  Player::trigger(%player, 3, true);
}
function MG28::onUnmount(%player, %slot) {
  Player::trigger(%player, 3, false);
  Player::unmountItem(%player, 3);
}

function MG28::onNoAmmo(%player) {}
//registerWeapon(MG28);
linkWeapon(MG28);