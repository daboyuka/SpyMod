ExplosionData FlamingFlameOfFireExp {
   shapeName = "plasmatrail.dts";

   faceCamera = true;
   randomSpin = true;

   timeScale = 1.5;
};

GrenadeData FlamingFlameOfFire {
   bulletShapeName    = "plasmatrail.dts";
   explosionTag       = FlamingFlameOfFireExp;
   collideWithOwner   = True;
   ownerGraceMS       = 250;
   collisionRadius    = 0.2;
   mass               = 1.0;
   elasticity         = 0;

   damageClass        = 0;       // 0 impact, 1, radius
   damageValue        = 0;
   damageType         = 0;

   maxLevelFlightDist = 0;
   totalTime          = 0.01;    // special meaning for grenades...
   liveTime           = 0.0;
   projSpecialTime    = 0.05;

   inheritedVelocityScale = 1;

   smokeName              = "plasmatrail.dts";
};

function Effects::burn(%obj, %offset1, %offset2, %time) {
  %callDo = (%obj.effects::burntime <= 0);
  %obj.effects::burntime = %time;
  %obj.effects::burnoffset1 = %offset1;
  %obj.effects::burnoffset2 = %offset2;

  if (%callDo) Effects::doBurn(%obj);
}

function Effects::doBurn(%obj) {
  if (%obj.effects::burntime <= 0 || !isObject(%obj)) { %obj.effects::burntime = ""; return; }

  %obj.effects::burntime -= 0.2;

  %offset = Vector::randomVec2(%obj.effects::burnoffset1, %obj.effects::burnoffset2);
  Projectile::spawnProjectile(FlamingFlameOfFire, "1 0 0 0 0 1 0 -1 0 " @ Vector::add(GameBase::getPosition(%obj),%offset), -1, 0);
  schedule("Effects::doBurn("@%obj@");", 0.2);
}

function Effects::stopBurn(%obj) {
  %obj.effects::burntime = "";
}