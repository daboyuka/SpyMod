// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
// W00000000T! PAINTBALL EXPS!!! WARNING, MAY CAUSE L33TNESS OVERFLOW ERROR ON THE SERVER!!!
// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
ExplosionData pbBulletExp0 {
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

ExplosionData pbBulletExp1 {
   shapeName = "shotgunex.dts";
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

ExplosionData pbBulletExp2 {
   shapeName = "plasmabolt.dts";
   soundId   = ricochet3;

   faceCamera = false;
   randomSpin = true;
   hasLight   = true;
   lightRange = 6.0;

   timeZero = 0.100;
   timeOne  = 0.900;

   timeScale = 10;

   colors[0]  = { 0.0,  0.0, 0.0 };
   colors[1]  = { 0.75, 1.0, 0.0 };
   colors[2]  = { 0.75, 1.0, 0.0 };
   radFactors = { 0.0, 1.0, 0.0 };

//   shiftPosition = True;
};

BulletData TornadoBulletPB {
   bulletShapeName    = "bullet.dts";
   explosionTag       = pbBulletExp0;
   expRandCycle       = 3;
   bulletHoleIndex    = 0;

   damageClass        = 0;                 // 0 = impact, 1 = radius
   damageValue        = TornadoBullet.damageValue;
   damageType         = $BulletDamageType;

   muzzleVelocity     = 400.0;
   totalTime          = 5.25;
   liveTime           = 5.25;
   lightRange         = 1.0;
   lightColor         = { 1, 1, 0 };
   inheritedVelocityScale = 0.3;
   isVisible          = False;

   aimDeflection      = 0.01;
   tracerPercentage   = 1.0;
   tracerLength       = 15;

   soundId = SoundJetLight;
};

$Powers::POWER_PRIZE2 = Powers::addNewPower(512, "PBTornado", false);

$ItemDescription[Tornado, 0] = "<jc><f1>Tornado:<f0> A twin machine gun that can empty its large magazine in seconds";
$ItemDescription[Tornado, 1] = "<jc><f1>Tornado, Plasglow Mode:<f0> PLASGLOW POWERED TORNADO!!! W000T!!";
$NumModes[Tornado] = 2;

function TornadoImage::onFire(%player, %slot) {
  if (Player::getItemCount(%player, LoadedAmmo) == 0) return;
  if (Weapon::getMode(%player, Tornado) == 0) {
    Weapon::fireProjectile(%player, TornadoBullet);
    Weapon::fireProjectile(%player, TornadoBullet);
  } else {
    Weapon::fireProjectile(%player, TornadoBulletPB);
    Weapon::fireProjectile(%player, TornadoBulletPB);
  }
  if (!Player::isTriggered(%player, 4) && !Player::isTriggered(%player, 5)) {
    Weapon::linkSlots(%player, 0.1, 4, 5);
  }
  Player::decItemCount(%player, LoadedAmmo, 2);//TornadoAmmo, 2);
}

function Tornado::onModeChanged(%player, %mode, %oldMode) {
  if ((Player::getClient(%player).powers & $Powers::POWER_PRIZE2) == 0 && %mode == 1)
    Weapon::setMode(%player, Tornado, 0);

  Weapon::displayDescription(%player, Tornado);
}

