ExplosionData paintExp {
   shapeName = "paint.dts";
   soundId   = energyExplosion;

   faceCamera = true;
   randomSpin = true;
   hasLight   = true;
   lightRange = 3.0;

   timeZero = 0.450;
   timeOne  = 0.750;

   colors[0]  = { 0.25, 1.0, 0.25 };
   colors[1]  = { 0.25, 1.0, 0.25 };
   colors[2]  = { 0.25, 1.0, 0.25 };
   radFactors = { 1.0, 1.0, 1.0 };

   shiftPosition = True;
};

BulletData LaserDragonBullet1 {
   bulletShapeName    = "paint.dts";
   explosionTag       = paintExp;

   damageClass        = 0;                 // 0 = impact, 1 = radius
   damageValue        = 13;
   damageType         = $BulletDamageType;

   muzzleVelocity     = 400.0;
   totalTime          = 5.25;
   liveTime           = 5.25;
   lightRange         = 1.0;
   lightColor         = { 1, 1, 0 };
   inheritedVelocityScale = 0.3;

   aimDeflection      = 0.007;

   soundId = SoundJetLight;
};

BulletData LaserDragonBullet2 {
   bulletShapeName    = "shotgunex.dts";
   explosionTag       = blasterExp;

   damageClass        = 0;                 // 0 = impact, 1 = radius
   damageValue        = 13;
   damageType         = $BulletDamageType;

   muzzleVelocity     = 400.0;
   totalTime          = 5.25;
   liveTime           = 5.25;
   lightRange         = 1.0;
   lightColor         = { 1, 1, 0 };
   inheritedVelocityScale = 0.3;

   aimDeflection      = 0.007;

   soundId = SoundJetLight;
};

$Powers::POWER_LASERDRAGON = Powers::addNewPower(4096, "LaserDragon", false);
$AdminPrivileges["modifyPower", $Powers::POWER_LASERDRAGON] = $Powers::POWER_UBERADMIN;

$NumModes[Dragon] = 2;
$ItemDescription[Dragon, 0] = "<jc><f1>Dragon:<f0> A fairly accurate machine gun";
$ItemDescription[Dragon, 1] = "<jc><f1>Laser Dragon:<f0> Rapidly fires blasts of pwnsomeness";

function DragonImage::onFire(%player, %slot) {
  if (Player::getItemCount(%player, LoadedAmmo) == 0) return;

  if (Weapon::getMode(%player, Dragon) == 0) {
    Weapon::fireProjectile(%player, DragonBullet);
  } else if (Weapon::getMode(%player, Dragon) == 1) {
    Weapon::fireProjectile(%player, radnomItems(2, LaserDragonBullet1, LaserDragonBullet2));
    GameBase::playSound(%player, SoundFireBlaster, 1);
  }
  Player::decItemCount(%player, LoadedAmmo);
}

function Dragon::onModeChanged(%player, %mode, %oldMode) {
  if ((Player::getClient(%player).powers & $Powers::POWER_LASERDRAGON) == 0 && %mode == 1)
    Weapon::setMode(%player, Dragon, 0);

  Weapon::displayDescription(%player, Dragon);
}
