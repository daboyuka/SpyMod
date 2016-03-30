// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
// W00000000T! PAINTBALL EXPS!!! WARNING, MAY CAUSE L33TNESS OVERFLOW ERROR ON THE SERVER!!!
// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
ExplosionData pbShotgunBulletExp0 {
   shapeName = "shotgunex.dts";
   soundId   = ricochet1;
   validateShape = true;
   validateMaterials = true;

   faceCamera = false;
   randomSpin = true;
   hasLight   = true;
   lightRange = 6.0;

   timeZero = 0.100;
   timeOne  = 0.900;

   timeScale = 50;

   colors[0]  = { 0.0, 0.0, 0.0 };
   colors[1]  = { 1.0, 0.0, 0.0 };
   colors[2]  = { 1.0, 0.0, 0.0 };
   radFactors = { 0.0, 1.0, 0.0 };

//   shiftPosition = True;
};

ExplosionData pbShotgunBulletExp1 {
   shapeName = "plasmabolt.dts";
   soundId   = ricochet2;

   faceCamera = false;
   randomSpin = true;
   hasLight   = true;
   lightRange = 6.0;

   timeZero = 0.100;
   timeOne  = 0.900;

   timeScale = 50;

   colors[0]  = { 0.0, 0.0, 0.0 };
   colors[1]  = { 0.0, 1.0, 0.5 };
   colors[2]  = { 0.0, 1.0, 0.5 };
   radFactors = { 0.0, 1.0, 0.0 };

//   shiftPosition = True;
};

BulletData PBShotgun2Bullet {
   bulletShapeName    = "bullet.dts";
   explosionTag       = pbShotgunBulletExp0;
   expRandCycle       = 2;
   bulletHoleIndex    = 0;

   damageClass        = 0;                 // 0 = impact, 1 = radius
   damageValue        = 12;
   damageType         = $BulletDamageType;

   muzzleVelocity     = 400.0;
   totalTime          = 0.25;
   liveTime           = 0.25;
   lightRange         = 1.0;
   lightColor         = { 1, 1, 0 };
   inheritedVelocityScale = 0;
   isVisible          = False;

   aimDeflection      = 0.008;
   tracerPercentage   = 1.0;
   tracerLength       = 30;

   soundId = SoundJetLight;
};

$Powers::POWER_PRIZE3 = Powers::addNewPower(1024, "PBShotgun", false);

$ItemDescription[Shotgun2, 0] = "<jc><f1>Shotgun M2:<f0> A larger model of the Shotgun M1; it can hold 4 shells in one clip";
$ItemDescription[Shotgun2, 1] = "<jc><f1>Shotgun M2, Plasglow Mode:<f0> LIK W00T ANOTHER PLASGLOW WEAPON!!! D00000D!!!";
$NumModes[Shotgun2] = 2;

function Shotgun2Image::onFire(%player, %slot) {
  if (Player::getItemCount(%player, LoadedAmmo) == 0) return;

  if (Weapon::getMode(%player, Shotgun2) == 0) {
    Weapon::fireProjectile(%player, Shotgun2Bullet);
    Weapon::fireProjectile(%player, Shotgun2Bullet);
    Weapon::fireProjectile(%player, Shotgun2Bullet);
    Weapon::fireProjectile(%player, Shotgun2Bullet);
    Weapon::fireProjectile(%player, Shotgun2Bullet);
    Weapon::fireProjectile(%player, Shotgun2Bullet);
  } else {
    Weapon::fireProjectile(%player, PBShotgun2Bullet);
    Weapon::fireProjectile(%player, PBShotgun2Bullet);
    Weapon::fireProjectile(%player, PBShotgun2Bullet);
    Weapon::fireProjectile(%player, PBShotgun2Bullet);
    Weapon::fireProjectile(%player, PBShotgun2Bullet);
    Weapon::fireProjectile(%player, PBShotgun2Bullet);
  }
  Player::decItemCount(%player, LoadedAmmo);
}

function Shotgun2::onModeChanged(%player, %mode, %oldMode) {
  if ((Player::getClient(%player).powers & $Powers::POWER_PRIZE3) == 0 && %mode == 1)
    Weapon::setMode(%player, Shotgun2, 0);

  Weapon::displayDescription(%player, Shotgun2);
}

