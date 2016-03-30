function Effects::burn(%obj, %offset1, %offset2, %time) {
  %callDo = (%obj.effects::burntime <= 0);
  %obj.effects::burntime = %time;
  %obj.effects::burnoffset1 = %offset1;
  %obj.effects::burnoffset2 = %offset2;

$worked1 = %callDo @ "," @ %obj @ "," @ %obj.effects::burntime;
  if (%callDo) Effects::doBurn(%obj);
}

function Effects::doBurn(%obj) {
  if (%obj.effects::burntime <= 0 || !isObject(%obj)) { %obj.effects::burntime = 0; return; }

  %obj.effects::burntime -= 0.2;

  %offset = Vector::randomVec2(%obj.effects::burnoffset1, %obj.effects::burnoffset2);
  Projectile::spawnProjectile(FlamingFlameOfFire, "1 0 0 0 0 1 0 -1 0 " @ Vector::add(GameBase::getPosition(%obj),%offset), -1, 0);
  schedule("Effects::doBurn("@%obj@");", 0.2);

$worked2 = true;
}