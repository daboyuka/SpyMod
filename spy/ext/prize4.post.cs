GrenadeData XmasFlamerFire1 {
   bulletShapeName    = "shotgunex.dts";
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

   smokeName              = "shotgunex.dts";
};

GrenadeData XmasFlamerFire2 {
   bulletShapeName    = "paint.dts";
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

   smokeName              = "paint.dts";
};

$Powers::POWER_PRIZE4 = Powers::addNewPower(2048, "XmasFlamer", false);

$ItemDescription[Flamer, 0] = "<jc><f1>Flamer:<f0> A flame thrower, keep clear of the area";
$ItemDescription[Flamer, 1] = "<jc><f1>Flamer, Christmas Mode:<f0> Spread Christmas cheer with your modified flamer!";
$NumModes[Flamer] = 2;

function FlamerImage::onFire(%player, %slot) {
  if (Player::getItemCount(%player, LoadedAmmo) == 0) return;

  %proj = tern(Weapon::getMode(%player, Flamer) == 0, FlamerFire, radnomItems(2, XmasFlamerFire1, XmasFlamerFire2));

  %vel = Matrix::mul(GameBase::getMuzzleTransform(%player),3,3,"0 18 0",3,1);
  %p = Projectile::spawnProjectile(%proj, GameBase::getMuzzleTransform(%player), %player, %vel);
  schedule("Flamer::doSplitFire("@%player@","@%p@");", 0.35);
  Player::decItemCount(%player, LoadedAmmo);
}

function Flamer::doSplitFire(%player, %p) {
  if (!isObject(%p)) return;

  %proj = tern(Weapon::getMode(%player, Flamer) == 0, FlamerFire, radnomItems(2, XmasFlamerFire1, XmasFlamerFire2));

  %vel = Item::getVelocity(%p);
  if (%vel == "0 0 0") {
    Projectile::spawnProjectile(%proj, "1 0 0 0 1 0 0 0 1 " @ GameBase::getPosition(%p), %player, "0 0 0");
    Projectile::spawnProjectile(%proj, "1 0 0 0 1 0 0 0 1 " @ GameBase::getPosition(%p), %player, "0 0 0");
    Projectile::spawnProjectile(%proj, "1 0 0 0 1 0 0 0 1 " @ GameBase::getPosition(%p), %player, "0 0 0");
  } else {
    %forwardVec = Vector::normalize(getWord(%vel, 0) @ " " @ getWord(%vel, 1) @ " 0");//Vector::mul(%forwardVec, 2)
    %sideVec = -getWord(%forwardVec, 1) @ " " @ getWord(%forwardVec, 0) @ " 0";
    %va1 = Vector::add(Vector::add("0 0 1.5", Vector::mul(%forwardVec, 1)), Vector::mul(%sideVec, getRandom() * 4 - 2));
    %va2 = Vector::add(Vector::add("0 0 3", Vector::mul(%forwardVec, 2)), Vector::mul(%sideVec, getRandom() * 4 - 2));
    %va3 = Vector::add(Vector::add("0 0 4.5", Vector::mul(%forwardVec, 3)), Vector::mul(%sideVec, getRandom() * 4 - 2));
    Projectile::spawnProjectile(%proj, "1 0 0 0 1 0 0 0 1 " @ GameBase::getPosition(%p), %player, Vector::add(%vel, %va1));
    Projectile::spawnProjectile(%proj, "1 0 0 0 1 0 0 0 1 " @ GameBase::getPosition(%p), %player, Vector::add(%vel, %va2));
    Projectile::spawnProjectile(%proj, "1 0 0 0 1 0 0 0 1 " @ GameBase::getPosition(%p), %player, Vector::add(%vel, %va3));
  }
}

function Flamer::onModeChanged(%player, %mode, %oldMode) {
  if ((Player::getClient(%player).powers & $Powers::POWER_PRIZE4) == 0 && %mode == 1)
    Weapon::setMode(%player, Flamer, 0);

  Weapon::displayDescription(%player, Flamer);
}

