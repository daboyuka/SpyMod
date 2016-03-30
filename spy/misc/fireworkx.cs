ExplosionData FireworkRedExp {
   shapeName = "shotgunex.dts";

   faceCamera = true;
   randomSpin = true;
   hasLight   = true;
   lightRange = 5.0;

   timeScale = 3;

   timeZero = 0.450;
   timeOne  = 0.750;

   colors[0]  = { 1.0, 0.25, 0.25 };
   colors[1]  = { 1.0, 0.25, 0.25 };
   colors[2]  = { 1.0, 0.25, 0.25 };
   radFactors = { 1.0, 1.0, 1.0 };

   shiftPosition = True;
};

GrenadeData FireworkRedShell {
   bulletShapeName    = "shotgunex.dts";
   explosionTag       = FireworkRedExp;
   collideWithOwner   = True;
   ownerGraceMS       = 250;
   collisionRadius    = 0.2;
   mass               = 1.0;
   elasticity         = 0.2;

   damageClass        = 1;
   damageValue        = 0.05;
   damageType         = 20;

	explosionRadius = 3;

   maxLevelFlightDist = 1;
   totalTime          = 3.0;    // special meaning for grenades...
   liveTime           = 0;//3.0;
   projSpecialTime    = 0.05;

   lightRange         = 5.0;
   lightColor         = { 1.0, 0.25, 0.25 };

   inheritedVelocityScale = 30;
   smokeName              = "shotgunex.dts";
};

ExplosionData FireworkGreenExp {
   shapeName = "paint.dts";

   faceCamera = true;
   randomSpin = true;
   hasLight   = true;
   lightRange = 5.0;

   timeScale = 3;

   timeZero = 0.450;
   timeOne  = 0.750;

   colors[0]  = { 0, 1, 0 };
   colors[1]  = { 0, 1, 0 };
   colors[2]  = { 0, 1, 0 };
   radFactors = { 1.0, 1.0, 1.0 };

   shiftPosition = True;
};

GrenadeData FireworkGreenShell {
   bulletShapeName    = "paint.dts";
   explosionTag       = FireworkGreenExp;
   collideWithOwner   = True;
   ownerGraceMS       = 250;
   collisionRadius    = 0.2;
   mass               = 1.0;
   elasticity         = 0.2;

   damageClass        = 1;
   damageValue        = 0.05;
   damageType         = 20;
	explosionRadius = 3;

   maxLevelFlightDist = 1;
   totalTime          = 3.0;    // special meaning for grenades...
   liveTime           = 0;//3.0;
   projSpecialTime    = 0.05;

   lightRange         = 5.0;
   lightColor         = { 0, 1, 0 };

   inheritedVelocityScale = 30;
   smokeName              = "paint.dts";
};

ExplosionData FireworkBlueExp {
   shapeName = "fusionbolt.dts";

   faceCamera = true;
   randomSpin = true;
   hasLight   = true;
   lightRange = 5.0;

   timeScale = 3;

   timeZero = 0.450;
   timeOne  = 0.750;

   colors[0]  = { 0, 0, 1 };
   colors[1]  = { 0, 0, 1 };
   colors[2]  = { 0, 0, 1 };
   radFactors = { 1.0, 1.0, 1.0 };

   shiftPosition = True;
};

GrenadeData FireworkBlueShell {
   bulletShapeName    = "fusionbolt.dts";
   explosionTag       = FireworkBlueExp;
   collideWithOwner   = True;
   ownerGraceMS       = 250;
   collisionRadius    = 0.2;
   mass               = 1.0;
   elasticity         = 0.2;

   damageClass        = 1;
   damageValue        = 0.05;
   damageType         = 20;
	explosionRadius = 3;

   maxLevelFlightDist = 1;
   totalTime          = 3.0;    // special meaning for grenades...
   liveTime           = 0;//3.0;
   projSpecialTime    = 0.05;

   lightRange         = 5.0;
   lightColor         = { 0, 1, 0 };

   inheritedVelocityScale = 30;
   smokeName              = "fusionbolt.dts";
};

ExplosionData FireworkWhiteExp {
   shapeName = "flash_small.dts";

   faceCamera = true;
   randomSpin = true;
   hasLight   = true;
   lightRange = 5.0;

   timeScale = 3;

   timeZero = 0.450;
   timeOne  = 0.750;

   colors[0]  = { 0, 0, 1 };
   colors[1]  = { 0, 0, 1 };
   colors[2]  = { 0, 0, 1 };
   radFactors = { 1.0, 1.0, 1.0 };

   shiftPosition = True;
};

GrenadeData FireworkWhiteShell {
   bulletShapeName    = "flash_large.dts";
   explosionTag       = FireworkWhiteExp;
   collideWithOwner   = True;
   ownerGraceMS       = 250;
   collisionRadius    = 0.2;
   mass               = 1.0;
   elasticity         = 0.2;

   damageClass        = 1;
   damageValue        = 0.05;
   damageType         = 20;
	explosionRadius = 3;

   maxLevelFlightDist = 1;
   totalTime          = 3.0;    // special meaning for grenades...
   liveTime           = 0;//3.0;
   projSpecialTime    = 0.05;

   lightRange         = 5.0;
   lightColor         = { 0, 1, 0 };

   inheritedVelocityScale = 30;
   smokeName              = "flash_small.dts";
};

ExplosionData FireworkOrangeExp {
   shapeName = "plasmaex.dts";

   faceCamera = true;
   randomSpin = true;
   hasLight   = true;
   lightRange = 5.0;

   timeScale = 1.5;

   timeZero = 0.450;
   timeOne  = 0.750;

   colors[0]  = { 0, 0, 1 };
   colors[1]  = { 0, 0, 1 };
   colors[2]  = { 0, 0, 1 };
   radFactors = { 1.0, 1.0, 1.0 };

   shiftPosition = True;
};

GrenadeData FireworkOrangeShell {
   bulletShapeName    = "plasmabolt.dts";
   explosionTag       = FireworkOrangeExp;
   collideWithOwner   = True;
   ownerGraceMS       = 250;
   collisionRadius    = 0.2;
   mass               = 1.0;
   elasticity         = 0.2;

   damageClass        = 1;
   damageValue        = 0.05;
   damageType         = 20;
	explosionRadius = 3;

   maxLevelFlightDist = 1;
   totalTime          = 3.0;    // special meaning for grenades...
   liveTime           = 0;//3.0;
   projSpecialTime    = 0.05;

   lightRange         = 5.0;
   lightColor         = { 1, 0.5, 0 };

   inheritedVelocityScale = 30;
   smokeName              = "plasmatrail.dts";
};

ExplosionData FireworkUberExp {
   shapeName = "shockwave_large.dts";

   faceCamera = true;
   randomSpin = true;
   hasLight   = true;
   lightRange = 5.0;

   timeScale = 3;

   timeZero = 0.450;
   timeOne  = 0.750;

   colors[0]  = { 0, 0, 1 };
   colors[1]  = { 0, 0, 1 };
   colors[2]  = { 0, 0, 1 };
   radFactors = { 1.0, 1.0, 1.0 };

   shiftPosition = True;
};

GrenadeData FireworkUberShell {
   bulletShapeName    = "shockwave_large.dts";
   explosionTag       = FireworkUberExp;
   collideWithOwner   = True;
   ownerGraceMS       = 250;
   collisionRadius    = 0.2;
   mass               = 1.0;
   elasticity         = 0.2;

   damageClass        = 1;
   damageValue        = 0.05;
   damageType         = 20;
	explosionRadius = 3;

   maxLevelFlightDist = 1;
   totalTime          = 3.0;    // special meaning for grenades...
   liveTime           = 0;//3.0;
   projSpecialTime    = 0.05;

   lightRange         = 5.0;
   lightColor         = { 0, 1, 0 };

   inheritedVelocityScale = 30;
   smokeName              = "shockwave_large.dts";
};

ExplosionData FireworkExp1 {
   shapeName = "shockwave_large.dts";
   soundId   = shockExplosion;

   faceCamera = true;
   randomSpin = false;
   hasLight   = true;
   lightRange = 1000.0;

   timeZero = 0;
   timeOne  = 1;

   colors[0]  = { 1.0, 1.0, 1.0 };
   colors[1]  = { 1.0, 1.0, 1.0 };
   colors[2]  = { 1.0, 1.0, 1.0 };
   radFactors = { 1, 1, 1 };
};

RocketData Firework1 {
   bulletShapeName = "rocket.dts";
   explosionTag    = FireworkExp1;

   collisionRadius = 0.0;
   mass            = 2.0;

   damageClass      = 0;
   damageValue      = 0;
   damageType       = 0;

   muzzleVelocity   = 60.0;
   terminalVelocity = 60.0;
   acceleration     = 0;

   totalTime        = 4.0;
   liveTime         = 4.0;

   inheritedVelocityScale = 0;

   // rocket specific
   trailType   = 1;
   trailLength = 30;
   trailWidth  = 0.3;
};

function Fireworks::fireChristmasFireworks(%startDelay, %delay, %num) {
  for (%i = 0; %i < %num; %i++) {
    schedule("Fireworks::fireChristmasFirework($fireworkPos["@floor(getRandom()*$numFireworkPositions)@"]);", %delay * %i + %startDelay);
  }
}

function Fireworks::fireChristmasFirework(%pos) {
  %firework = Projectile::spawnProjectile(Firework1, "1 0 0 0 0 1 0 -1 0 " @ %pos, -1, 0);
  schedule("Fireworks::spawnParticles(GameBase::getPosition("@%firework@"), FireworkRedShell, 30);", 3.95);
  schedule("Fireworks::spawnParticles(GameBase::getPosition("@%firework@"), FireworkGreenShell, 30);", 3.95);
}


function Fireworks::fireIndependenceFirework(%pos) {
  %firework = Projectile::spawnProjectile(Firework1, "1 0 0 0 0 1 0 -1 0 " @ %pos, -1, 0);
  schedule("Fireworks::spawnParticles(GameBase::getPosition("@%firework@"), FireworkRedShell, 20);", 3.95);
  schedule("Fireworks::spawnParticles(GameBase::getPosition("@%firework@"), FireworkWhiteShell, 20);", 3.95);
  schedule("Fireworks::spawnParticles(GameBase::getPosition("@%firework@"), FireworkBlueShell, 20);", 3.95);
}

function Fireworks::fireTestFirework(%pos) {
  %firework = Projectile::spawnProjectile(Firework1, "1 0 0 0 0 1 0 -1 0 " @ %pos, -1, 0);
  schedule("Fireworks::spawnParticles(GameBase::getPosition("@%firework@"), FireworkUberShell, 13);", 3.95);
//  schedule("Fireworks::spawnParticles(GameBase::getPosition("@%firework@"), FireworkRedShell, 25);", 3.95);
//  schedule("Fireworks::spawnParticles(GameBase::getPosition("@%firework@"), FireworkGreenShell, 25);", 3.95);
//  schedule("Fireworks::spawnParticles(GameBase::getPosition("@%firework@"), FireworkBlueShell, 25);", 3.95);
//  schedule("Fireworks::spawnParticles(GameBase::getPosition("@%firework@"), FireworkWhiteShell, 25);", 3.95);
//  schedule("Fireworks::spawnParticles(GameBase::getPosition("@%firework@"), FireworkOrangeShell, 25);", 3.95);
}

function Fireworks::fireFirework1(%pos) {
  %firework = Projectile::spawnProjectile(Firework1, "1 0 0 0 0 1 0 -1 0 " @ %pos, -1, 0);
  schedule("Fireworks::spawnParticles(GameBase::getPosition("@%firework@"), FireworkGreenShell, 30);", 3.95);
  schedule("Fireworks::spawnParticles(GameBase::getPosition("@%firework@"), FireworkOrangeShell, 30);", 3.95);
}

function Fireworks::fireFirework2(%pos) {
  %firework = Projectile::spawnProjectile(Firework1, "1 0 0 0 0 1 0 -1 0 " @ %pos, -1, 0);
  schedule("Fireworks::spawnParticles(GameBase::getPosition("@%firework@"), FireworkBlueShell, 30);", 3.95);
  schedule("Fireworks::spawnParticles(GameBase::getPosition("@%firework@"), FireworkRedShell, 30);", 3.95);
}

function Fireworks::fireFirework3(%pos) {
  %firework = Projectile::spawnProjectile(Firework1, "1 0 0 0 0 1 0 -1 0 " @ %pos, -1, 0);
  schedule("Fireworks::spawnParticles(GameBase::getPosition("@%firework@"), FireworkRedShell, 30);", 3.95);
  schedule("Fireworks::spawnParticles(GameBase::getPosition("@%firework@"), FireworkGreenShell, 30);", 3.95);
  schedule("Fireworks::spawnParticles(GameBase::getPosition("@%firework@"), FireworkBlueShell, 30);", 3.95);
  schedule("Fireworks::spawnParticles(GameBase::getPosition("@%firework@"), FireworkOrangeShell, 30);", 3.95);
}

function Fireworks::fireDummyFirework(%pos) {
  %firework = Projectile::spawnProjectile(Firework1, "1 0 0 0 0 1 0 -1 0 " @ %pos, -1, 0);
}

function Fireworks::spawnParticles(%pos, %type, %num) {
  for (%i = 0; %i < %num; %i++) {
    %vec = Vector::randomVec(-1, 1, -1, 1, -1, 1);
    schedule("Projectile::spawnProjectile("@%type@", \"1 0 0 0 1 0 0 0 1 "@%pos@"\", -1, \""@%vec@"\");", %i * 0.02);
  }
}

function Fireworks::doEveryHour(%now, %howMany) {
  if (%now) {
    Fireworks::fireChristmasFireworks(0, 5, 30);
    %howMany--;
  }

  for (%i = 0; %i < %howMany; %i++) {
    schedule("Fireworks::fireChristmasFireworks(0, 5, 30);", (%i + 1) * 3600);
  }
}