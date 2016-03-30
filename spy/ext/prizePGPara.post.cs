$Powers::POWER_PGPARA = Powers::addNewPower(8192, "Plasglow Parachute", false);
$AdminPrivileges["modifyPower", $Powers::POWER_PGPARA] = $Powers::POWER_UBERADMIN;

GrenadeData PlasglowRedShell {
   bulletShapeName    = "shotgunex.dts";
   explosionTag       = noExp;
   collideWithOwner   = True;
   ownerGraceMS       = 250;
   collisionRadius    = 0.2;
   mass               = 1.0;
   elasticity         = 0.2;

   damageClass        = 0;
   damageValue        = 00;
   damageType         = 0;

   maxLevelFlightDist = 0;
   totalTime          = 0.01;    // special meaning for grenades...
   liveTime           = 0;//3.0;
   projSpecialTime    = 0.05;

   lightRange         = 5.0;
   lightColor         = { 1.0, 0.25, 0.25 };

   inheritedVelocityScale = 1;
   smokeName              = "shotgunex.dts";
};


GrenadeData PlasglowGreenShell {
   bulletShapeName    = "paint.dts";
   explosionTag       = noExp;
   collideWithOwner   = True;
   ownerGraceMS       = 250;
   collisionRadius    = 0.2;
   mass               = 1.0;
   elasticity         = 0.2;

   damageClass        = 0;
   damageValue        = 00;
   damageType         = 0;

   maxLevelFlightDist = 0;
   totalTime          = 0.01;    // special meaning for grenades...
   liveTime           = 0;//3.0;
   projSpecialTime    = 0.05;

   lightRange         = 5.0;
   lightColor         = { 1.0, 0.25, 0.25 };

   inheritedVelocityScale = 1;
   smokeName              = "paint.dts";
};

function ParachutePack::checkParachute(%player) {
  if (!%player.parachuting || Player::getMountObject(%player) != -1 || Player::getLastContactCount(%player) <= 4) {
    ParachutePack::unparachute(%player);
    return;
  }

  %vel = Item::getVelocity(%player);
  if (Vector::dot(%vel, "0 0 -1") > 0) {
    %speed = Vector::length(%vel);
    %x = pow(%speed, 0.6) * -0.125 * $Parachute::slowFactor;
    %vel = Vector::add(%vel, Vector::resize(Vector::add(%vel, "0 0 "@getWord(%vel, 2)*20), %x));
    Item::setVelocity(%player, %vel);
  }
  if (Player::isJetting(%player)) {
    %move = Vector::mul(Vector::getFromRot(GameBase::getRotation(%player)), $Parachute::moveSpeed * 0.125);
    Item::setVelocity(%player, Vector::add(Item::getVelocity(%player), %move));
  }

  if (Player::getClient(%player).powers & $Powers::POWER_PGPARA || Player::isAIControlled(%player)) {
    echo("YAY");
    Projectile::spawnProjectile(radnomItems(2, PlasglowRedShell, PlasglowGreenShell), "1 0 0 0 1 0 0 0 1 " @ Vector::add(GameBase::getPosition(%player), Vector::randomVec2("-7 -7 10","7 7 10")), -1, 0);
  }

  schedule("ParachutePack::checkParachute("@%player@");", 0.125);
}