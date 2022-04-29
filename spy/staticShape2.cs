StaticShapeData PoweredDamagingElectricalBeam
{
	shapeFile = "zap";
	maxDamage = 10000.0;
	isTranslucent = true;
    description = "Damaging Electrical Beam";
   disableCollision = false;
	triggerRadius = 2;
};

//function to fade in electrical beam based on base power.
function PoweredDamagingElectricalBeam::onPower(%this, %power, %generator)
{
   if(%power)
	  GameBase::startFadeIn(%this);
   else
      GameBase::startFadeOut(%this);
}

function PoweredDamagingElectricalBeam::onCollision(%this, %object) {
  if (GameBase::isPowered(%this)) {
    %center = getBoxCenter(%this);
    %objCenter = getBoxCenter(%object);
    
    %mom = Vector::resize(Vector::sub(%objCenter, %center), 100);
    GameBase::applyDamage(%object, $ElectricityDamageType, 30,
                         %objCenter, %mom, %mom, %this);
  }
}


$LaserBeamEmitter::callRate = 0.125;
$LaserBeamEmitter::callsPerLaser = 4;

LaserData DetectLaser {
   laserBitmapName   = "repairAdd.bmp";//"laserPulse.bmp";

   damageConversion  = 0;
   baseDamageType    = 0;

   beamTime = $LaserBeamEmitter::callsPerLaser * $LaserBeamEmitter::callRate;

   lightRange        = 3.0;
   lightColor        = { 1, 1, 0.5 };

   detachFromShooter = false;
};

StaticShapeData LaserBeamEmitter {
	shapeFile = "breath";
	maxDamage = 10000.0;
	isTranslucent = true;
    description = "Laser Beam Emitter";
   disableCollision = true;
};

function LaserBeamEmitter::onAdd(%this) {
  GameBase::setActive(%this, true);
  LaserBeamEmitter::emit(%this);
}

function LaserBeamEmitter::emit(%this) {
  if (GameBase::isPowered(%this) && GameBase::isActive(%this)) {
    if (%this.laser == 0) {
      Projectile::spawnProjectile(DetectLaser, GameBase::getMuzzleTransform(%this), %this, 0);
    }
    %this.laser++;
    %this.laser %= $LaserBeamEmitter::callsPerLaser;

    $los::object = "";
    GameBase::getLOSInfo(%this, 20);
    if (%this.normalObject == "") {
      %this.normalObject = $los::object;
    } else if (%this.normalObject != $los::object) {
      GameBase::applyDamage($los::object, $DetectDamageType, 10, getBoxCenter($los::object), "0 0 1", "0 0 0", %this);
    }
  }
  schedule("LaserBeamEmitter::emit("@%this@");", $LaserBeamEmitter::callRate);
}


StaticShapeData BigGenerator {
   description = "Generator";
   shapeFile = "generator";
	className = "Generator";
   sfxAmbient = SoundGeneratorPower;
	debrisId = flashDebrisLarge;
	explosionId = LargeShockwave;
   maxDamage = 200;
	visibleToSensor = true;
	mapFilter = 4;
	mapIcon = "M_generator";
	damageSkinData = "objectDamageSkins";
	shadowDetailMask = 16;
};

function BigGenerator::onDamage(%this, %type, %a, %b, %c, %d, %e, %f, %g, %h) {
  if (%type != $BombDamageType) return;
  StaticShape::onDamage(%this, %type, %a, %b, %c, %d, %e, %f, %g, %h);
}

StaticShapeData BigSolarPanel {
   description = "Solar Panel";
	shapeFile = "solar";
	className = "Generator";
	debrisId = flashDebrisLarge;
	maxDamage = 120;
	visibleToSensor = true;
	mapFilter = 4;
	mapIcon = "M_generator";
    damageSkinData = "objectDamageSkins";
	shadowDetailMask = 16;
	explosionId = flashExpLarge;
};

StaticShapeData BigSatDish {
   description = "Big Satellite Dish";
   shapeFile = "sat_big";
	className = "StaticShape";
	debrisId = flashDebrisLarge;
	explosionId = LargeShockwave;
   maxDamage = 200;
	visibleToSensor = false;
	mapFilter = 4;
	mapIcon = "M_generator";
	damageSkinData = "objectDamageSkins";
	shadowDetailMask = 16;
};

function BigSatDish::onPower(%this, %power) {
	GameBase::playSequence(%this,0,"activate");
}

function BigSatDish::onDeactivate(%this) {
	GameBase::stopSequence(%this,0);
}


SoundData SoundShatter {
   wavFileName = "float_explode.wav";
   profile = Profile3dMedium;
};
ExplosionData WindowExp {
   shapeName = "breath.dts";
   soundId   = SoundShatter;

   faceCamera = false;
   randomSpin = false;
   hasLight   = false;

   timeScale = 0.01;
};

StaticShapeData Window {
	shapeFile = "forcefield";
	debrisId = defaultDebrisSmall;
	explosionId = WindowExp;
	maxDamage = 1.0;
	isTranslucent = true;
   description = "Force Field";
};

StaticShapeData TreeShape {
	shapeFile = "tree1";
	className = "Tree";
	maxDamage = 200.0;
	isTranslucent = "True";
   description = "Tree";
};

StaticShapeData TreeShapeTwo {
	shapeFile = "tree2";
	className = "Tree";
	maxDamage = 200.0;
	isTranslucent = "True";
   description = "Tree";
};

function Tree::onDamage(%this,%type,%value,%pos,%vec,%mom,%object) {
  if (%type == $PlasmaDamageType) Effects::burn(%this, "-4 -4 7", "4 4 9", 10);
  StaticShape::onDamage(%this,%type,%value,%pos,%vec,%mom,%object);
}








$DiscoEmitter::laserRate = 0.5;
$DiscoEmitter::laserTime = 3;

LaserData DiscoLaser {
   laserBitmapName   = "repairadd.bmp";

   damageConversion  = 0;
   baseDamageType    = 0;

   beamTime = $DiscoEmitter::laserTime;

   lightRange        = 3.0;
   lightColor        = { 1, 0, 0 };

   detachFromShooter = true;
};

StaticShapeData DiscoEmitter {
	shapeFile = "breath";
	maxDamage = 10000.0;
	isTranslucent = true;
    description = "Disco Beam Emitter";
   //disableCollision = true;
};

function DiscoEmitter::onAdd(%this) {
  GameBase::setActive(%this, true);
//  %this.minRot = "3.14 0 -3.14";
//  %this.maxRot = "6.28 0 3.14";
  DiscoEmitter::emit(%this);
}

function DiscoEmitter::emit(%this) {
  if (GameBase::isPowered(%this) && GameBase::isActive(%this)) {
    GameBase::setRotation(%this,
                          Vector::randomVec(getWord(%this.minRot, 0), getWord(%this.maxRot, 0),
                                            getWord(%this.minRot, 1), getWord(%this.maxRot, 1),
                                            getWord(%this.minRot, 2), getWord(%this.maxRot, 2)));
    Projectile::spawnProjectile(DiscoLaser, GameBase::getMuzzleTransform(%this), %this, 0);
  }
  schedule("DiscoEmitter::emit("@%this@");", $DiscoEmitter::laserRate);
}






LaserData DiscoLaser2 {
   laserBitmapName   = "repairadd.bmp";

   damageConversion  = 0;
   baseDamageType    = 0;

   beamTime = $DiscoEmitter::laserTime;

   lightRange        = 3.0;
   lightColor        = { 1, 0, 0 };

   detachFromShooter = false;
};

LaserData DiscoLaser3 {
   laserBitmapName   = "forcefield.bmp";

   damageConversion  = 0;
   baseDamageType    = 0;

   beamTime = $DiscoEmitter::laserTime;

   lightRange        = 3.0;
   lightColor        = { 1, 0, 0 };

   detachFromShooter = false;
};

LaserData DiscoLaser4 {
   laserBitmapName   = "olite03.bmp";

   damageConversion  = 0;
   baseDamageType    = 0;

   beamTime = $DiscoEmitter::laserTime;

   lightRange        = 3.0;
   lightColor        = { 1, 0, 0 };

   detachFromShooter = false;
};

LaserData DiscoLaser5 {
   laserBitmapName   = "plite01.bmp";

   damageConversion  = 0;
   baseDamageType    = 0;

   beamTime = $DiscoEmitter::laserTime;

   lightRange        = 3.0;
   lightColor        = { 1, 0, 0 };

   detachFromShooter = false;
};

LaserData DiscoLaser6 {
   laserBitmapName   = "glite01.bmp";

   damageConversion  = 0;
   baseDamageType    = 0;

   beamTime = $DiscoEmitter::laserTime;

   lightRange        = 3.0;
   lightColor        = { 1, 0, 0 };

   detachFromShooter = false;
};

StaticShapeData DiscoEmitter2 {
	shapeFile = "breath";
	maxDamage = 10000.0;
	isTranslucent = true;
    description = "Disco Beam Emitter";
   //disableCollision = true;
};

function DiscoEmitter2::onAdd(%this) {
  GameBase::setActive(%this, true);
//  %this.minRot = "3.14 0 -3.14";
//  %this.maxRot = "6.28 0 3.14";
  DiscoEmitter2::emit(%this);
}

function DiscoEmitter2::emit(%this) {
  if (!isObject(%this)) return;
  if (GameBase::isPowered(%this) && GameBase::isActive(%this)) {
    GameBase::setRotation(%this,
                          Vector::randomVec(getWord(%this.minRot, 0), getWord(%this.maxRot, 0),
                                            getWord(%this.minRot, 1), getWord(%this.maxRot, 1),
                                            getWord(%this.minRot, 2), getWord(%this.maxRot, 2)));
    Projectile::spawnProjectile(tern(%this.rainbow, radnomItems(5, DiscoLaser2, DiscoLaser3, DiscoLaser4, DiscoLaser5, DiscoLaser6), DiscoLaser2), GameBase::getMuzzleTransform(%this), %this, 0);
  }
  schedule("DiscoEmitter2::emit("@%this@");", tern(%this.laserRate != "", %this.laserRate, $DiscoEmitter::laserRate));
}

StaticShapeData InfoSwitch {
	shapeFile = "tower";
	maxDamage = 10000.0;
    description = "Info tower";
};

function InfoSwitch::onDamage() {}

function InfoSwitch::onCollision(%this, %object) {
  if (getObjectType(%object) != "Player" || %object.infoing[%this]) return;
  %object.infoing[%this] = true;

  %client = Player::getClient(%object);

  %time = 0;
  for (%i = 0; %this.infos[%i] != ""; %i++) {
    schedule("bottomprint("@%client@",\""@escapeString(%this.infos[%i])@"\", "@%this.delayTime@");", %time);
    echo("bottomprint("@%client@",\""@escapeString(%this.infos[%i])@"\", "@%this.delayTime@");");
    %time += %this.delayTime;
  }

  schedule(%object @ ".infoing["@%this@"] = \"\";", %time);
}

StaticShapeData LOSer {
	shapeFile = "breath";
	maxDamage = 10000.0;
	disableCollision = true;
};
function LOSer::onDamage() {}

StaticShapeData Octa6x6 {
	shapeFile = "elevator_6x6_octagon";
	debrisId = defaultDebrisSmall;
	explosionId = flashExpSmall;
	maxDamage = 500.0;
   description = "Octagon 6x6";
};

StaticShapeData MainStation {
	shapeFile = "mainpad";
	debrisId = defaultDebrisSmall;
	explosionId = flashExpSmall;
	maxDamage = 1000.0;
   description = "Main Station";
};



DebrisData flashDebrisLarge {
   type      = 0;
   imageType = 0;
   
   mass       = 100.0;
   elasticity = 0.25;
   friction   = 0.5;
   center     = { 0, 0, 0 };

   animationSequence = -1;

   minTimeout = 3.0;
   maxTimeout = 6.0;

   explodeOnBounce = 0.3;

   damage          = 1000.0;
   damageThreshold = 100.0;

   spawnedDebrisMask     = 1;
   spawnedDebrisStrength = 90;
   spawnedDebrisRadius   = 0.2;

   spawnedExplosionID = turretExp;

   p = 1;

   explodeOnRest   = True;
   collisionDetail = 0;
};

StaticShapeData ServerRack {
   description = "Server Rack";
   shapeFile = "generator";
	className = "ServerRack";
	debrisId = flashDebrisLarge;
	explosionId = turretExp;
    maxDamage = 200;
	visibleToSensor = true;
	mapFilter = 4;
	mapIcon = "M_generator";
	damageSkinData = "objectDamageSkins";
	shadowDetailMask = 16;
};

function ServerRack::onEnabled(%this)   { GameBase::setActive(%this,true); }
function ServerRack::onDisabled(%this)  { GameBase::stopSequence(%this, 0); }
function ServerRack::onDestroyed(%this)  { ServerRack::onDisabled(%this); }
function ServerRack::onActivate(%this)   { GameBase::playSequence(%this,0,"power"); }
function ServerRack::onDeactivate(%this) { GameBase::stopSequence(%this,0); }

