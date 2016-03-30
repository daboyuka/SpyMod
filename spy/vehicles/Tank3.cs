exec("vehicles\\tank3Stuff.cs");

FlierData Tank3 {
	explosionId = LargeShockwave;
	debrisId = flashDebrisLarge;
	className = "Vehicle";
   shapeFile = "hover_apc_sml";
   shieldShapeName = "shield_large";
   mass = 38.0;
   drag = 1.0;
   density = 1.2;
   maxBank = 0.3;//0.15;
   maxPitch = 0.5;//0.01;
   maxSpeed = 15;
   minSpeed = -5;
	lift = 0.1;
	maxAlt = 1500;
	maxVertical = 3;
	maxDamage = 500;
	damageLevel = {1.0, 1.0};
	maxEnergy = 100;
	accel = 0.08;

	groundDamageScale = 0;

	reloadDelay = 2.5;
	repairRate = 0;
//	fireSound = SoundFireFlierRocket;
	damageSound = SoundFlierCrash;
	ramDamage = 0;
	ramDamageType = -1;
	mapFilter = 2;
	mapIcon = "M_vehicle";
	visibleToSensor = true;
	shadowDetailMask = 2;

	mountSound = SoundFlyerMount;
	dismountSound = SoundFlyerDismount;
	idleSound = SoundFlyerIdle;
	moveSound = SoundFlyerActive;

	visibleDriver = true;
	driverPose = 0;
	description = "Tank";
};

setVehicleAllowsArmorToPilot(Tank3, larmor);
setVehicleAllowsArmorToPilot(Tank3, lfemale);
setVehicleAllowsArmorToPilot(Tank3, SpyMale);
setVehicleAllowsArmorToPilot(Tank3, SpyFemale);
setVehicleAllowsArmorToPilot(Tank3, DMMale);
setVehicleAllowsArmorToPilot(Tank3, DMFemale);
setVehicleAllowsArmorToRide(Tank3, larmor);
setVehicleAllowsArmorToRide(Tank3, lfemale);
setVehicleAllowsArmorToRide(Tank3, marmor);
setVehicleAllowsArmorToRide(Tank3, mfemale);
setVehicleAllowsArmorToRide(Tank3, harmor);
setVehicleAllowsArmorToRide(Tank3, SpyMale);
setVehicleAllowsArmorToRide(Tank3, SpyFemale);
setVehicleAllowsArmorToRide(Tank3, DMMale);
setVehicleAllowsArmorToRide(Tank3, DMFemale);
addSpecialSeatToVehicle(Tank3, 1, Tank3Turret);

$Vehicle::numSeats[Tank3] = 2;
$Vehicle::driverSeat[Tank3] = 2;
//$Vehicle::crashProjectile[Tank3] = Tank3Crash;
$Vehicle::explodeDamage[%name] = 200;

//$Vehicle::numWeps[Tank3] = 0;
//$Vehicle::weaponDescription[Tank3, 0] = "<jc><f1>Tank Cannon: <f0>Fires massive explosive anti-ground vehicle shells";

$DamageScales[Tank3, $ImpactDamageType] = 0;
$DamageScales[Tank3, $MissileDamageType] = 2;
$DamageScales[Tank3, $ExplosionDamageType] = 2;

//function Tank3::onNextWeapon(%this, %wep) {
//  Vehicle::displayWeaponDescription(%this);
//}

//function Tank3::onPrevWeapon(%this, %wep) {
//  Vehicle::displayWeaponDescription(%this);
//}

function Tank3::mountStuff(%this, %player) {
  Player::mountItem(%player, Tank3Object1, 3);
  Player::mountItem(%player, Tank3Object2, 4);
//  Player::mountItem(%player, Tank3Object3, 5);
//  Player::mountItem(%player, Tank3Object4, 6);
//  Player::mountItem(%player, Tank3Object5, 7);
}
function Tank3::unmountStuff(%this, %player) {
  Player::unmountItem(%player, 3);
  Player::unmountItem(%player, 4);
//  Player::unmountItem(%player, 5);
//  Player::unmountItem(%player, 6);
//  Player::unmountItem(%player, 7);
}

function Tank3::onAdd(%this) {
  %objMounter = newObject("", Player, VehicleObjectMounter);
  Player::setMountObject(%objMounter, %this, 3);
  Tank3::mountStuff(%this, %objMounter);

  %objMounter.damageVehicle = %this;

  %this.objMounter = %objMounter;

  %this.LOSer = newObject("", StaticShape, LOSer, true);
  GameBase::setPosition(%this.LOSer, "0 0 -10000");

  Tank3::update(%this);
}

function Tank3::onDestroyed(%this) {
  schedule("deleteObject("@%this.objMounter@");", 0);
  schedule("deleteObject("@%this.LOSer@");", 0);
  Vehicle::onDestroyed(%this);
}

function Tank3::onDamage(%this,%type,%value,%pos,%vec,%mom,%object) {
  Vehicle::onDamage(%this, %type, %value, %pos, %vec, %mom, %object);
  if (GameBase::getDamageLevel(%this) / Tank3.maxDamage > 0.6) Effects::burn(%this, "-2 -2 1", "2 3 1", 5);
}

function GameBase::addPosition(%this, %x) { GameBase::setPosition(%this, Vector::add(GameBase::getPosition(%this), %x)); }

function Tank3::updateOld(%this) {
  if (!isObject(%this)) return;

  if (GameBase::getLOSInfo(%this, 4, "-1 0 0") || GameBase::getLOSInfo(%this, 4, "-2 0 0") || GameBase::getLOSInfo(%this, 4, "-0.6 0 0")) {
    GameBase::addPosition(%this, "0 0 " @ ((1.2 - getWord($los::normal, 2)) * Vector::length(Item::getVelocity(%this)) / 2));
  } else if (!GameBase::getLOSInfo(%this, 5, "-1 0 0") && !GameBase::getLOSInfo(%this, 5, "-2 0 0")) {
    %pos = Vector::add(GameBase::getPosition(%this), "0 0 -0.25");
    GameBase::setPosition(%this, %pos);
  }
  schedule("Tank3::update("@%this@");", 0.0625);
}

$Tank3::updateTime = 0.0625;
$Tank3::minHoverHeight = 4;
$Tank3::xRotAscendSpeed = 0.5;//1.5;
$Tank3::xRotDescendSpeed = 0.1;
$Tank3::fallSpeed = 8;
$Tank3::fallXRotSpeed = 0.3;
function Tank3::update(%this) {
  if (GameBase::getControlClient(%this) == -1) {
    schedule("Tank3::update("@%this@");", $Tank3::updateTime);
    GameBase::setPosition(%this.LOSer, "0 0 -10000");
    return;
  }

  %pos = GameBase::getPosition(%this);
  %trans = GameBase::getMuzzleTransform(%this);
  %mat = Matrix::subMatrix(%trans, 3, 4, 3, 3);
  //%vel = Item::getVelocity(%this);
  //%velS = Vector::mul(%vel, $Tank3::updateTime);

  %wheel[0] = Vector::add(Matrix::mul(%mat, 3, 3, "0 5 0.5", 3, 1), %pos);
  %wheel[1] = Vector::add(Matrix::mul(%mat, 3, 3, "-3 -3 0.5", 3, 1), %pos);
  %wheel[2] = Vector::add(Matrix::mul(%mat, 3, 3, "3 -3 0.5", 3, 1), %pos);

  for (%i = 0; %i < 3; %i++) {
    GameBase::setPosition(%this.LOSer, %wheel[%i]);
    if (GameBase::getLOSInfo(%this.LOSer, $Tank3::minHoverHeight+4, "-1.57 0 0")) {
      %ground[%i] = $los::position;
      %groundDist[%i] = Vector::getDistance($los::position, %wheel[%i]);
    } else {
      if (!%this.midair) {
        %this.midair = true;
        %this.midairXRot = getWord(GameBase::GetRotation(%this), 0);
        Tank3::w00tAll(%this);
      }
      //if (GameBase::testPosition(%this, Vector::add(GameBase::getPosition(%this), "0 0 " @ -10*$Tank3::updateTime)))
      GameBase::setPosition(%this.LOSer, %pos);
      if (GameBase::getLOSInfo(%this, 1000, "-1.57 0 0")) {
        if (getWord(%pos, 2) - getWord($los::position, 2) > $Tank3::minHoverHeight+2) {
          GameBase::addPosition(%this, "0 0 " @ -$Tank3::fallSpeed*$Tank3::updateTime);
          %this.midairXRot -= $Tank3::fallXRotSpeed*$Tank3::updateTime;
          GameBase::setRotation(%this, %this.midairXRot @ " " @ getWord(GameBase::getRotation(%this), 1) @ " " @ getWord(GameBase::getRotation(%this), 2));
        }
      }
      schedule("Tank3::update("@%this@");", $Tank3::updateTime);
      return;
    }
  }
  %this.midair = "";

  %xdot = Vector::dot(Vector::normalize(Vector::sub(%ground[0], Vector::mul(Vector::add(%ground[1], %ground[2]),0.5))), "0 0 1");
  %xrot = -atan(sqrt(1-pow(%xdot,2))/%xdot) + $PI/2 - tern(%xdot < 0, $PI, 0);
//  %ydot = Vector::dot(Vector::normalize(Vector::sub(%ground[2], %ground[1])), "0 0 1");
//  %yrot = atan(sqrt(1-pow(%ydot,2))/%ydot) - $PI/2 + tern(%ydot < 0, $PI, 0);

  %crot = GameBase::getRotation(%this);
  %crotx = getWord(%crot, 0);
//  %croty = getWord(%crot, 1);
//  %crotz = getWord(%crot, 2);

  if (%xrot - %crotx > $Tank3::xRotAscendSpeed*$Tank3::updateTime)
    %xrot = tern(%xrot - %crotx > 0, %crotx + $Tank3::xRotAscendSpeed*$Tank3::updateTime, %crotx - $Tank3::xRotAscendSpeed*$Tank3::updateTime);
  if (%xrot - %crotx < $Tank3::xRotDescendSpeed*$Tank3::updateTime)
    %xrot = tern(%xrot - %crotx > 0, %crotx + $Tank3::xRotDescendSpeed*$Tank3::updateTime, %crotx - $Tank3::xRotDescendSpeed*$Tank3::updateTime);
//  if (abs(%yrot - %croty) > 0.5*$Tank3::updateTime)
//    %yrot = tern(%yrot - %croty > 0, %croty + 0.1*$Tank3::updateTime, %croty - 0.1*$Tank3::updateTime);

  GameBase::setRotation(%this, %xrot @ " " @ getWord(GameBase::getRotation(%this), 1) @ " " @ getWord(GameBase::getRotation(%this), 2));

  if ((%x = min(min(%groundDist[0], %groundDist[1]), %groundDist[2])) < $Tank3::minHoverHeight)
    GameBase::addPosition(%this, "0 0 " @ ($Tank3::minHoverHeight-%x));

  GameBase::setPosition(%this.LOSer, "0 0 -10000");
  schedule("Tank3::update("@%this@");", $Tank3::updateTime);
}

function Tank3::w00tAll(%this) {
  %name = GameBase::getDataName(%this);
  for (%i = 0; %i < $Vehicle::numSeats[%name]+1; %i++) {
    if (%this.Seat[%i] != "") {
      %pl = Client::getOwnedObject(%this.Seat[%i]);
      Player::setAnimation(%pl, floor(getRandom()*3)+43);
      schedule("playVoice("@Player::getClient(%pl)@", radnomItems(3,cheer1,cheer2,cheer3));", getRandom()/2);
    }
  }
}

function moreold() {
  for (%i = 0; %i < 3; %i++) {
    GameBase::setPosition(%this.LOSer, %wheel[%i]);
    if (GameBase::getLOSInfo(%this.LOSer, 20, "-1.57 0 0")) {
      %ground[%i] = $los::position;
    } else {
    }
  }

  %p1 = Vector::sub(%ground[1], %ground[0]);
  %p2 = Vector::sub(%ground[2], %ground[0]);


  %x1 = getWord(%p1, 0);
  %x2 = getWord(%p2, 0);
  %y1 = getWord(%p1, 1);
  %y2 = getWord(%p2, 1);
  %z1 = getWord(%p1, 2);
  %z2 = getWord(%p2, 2);

  %c = 1;
  %b = (-%z1 + %z2*%x1/%x2) / (%y1 - %y2*%x1/%x2);
  %a = (-%z1 - %b*%y1) / %x1;

  %normal = Vector::normalize(%a @ " " @ %b @ " " @ %c);
  %normRot = Vector::getRotation(%normal); // Yes, I know this aligns the Z axis to the vector, which is what I want.
}

function Tank3::onPlayerMount(%this, %player, %slot) {
  %player.damageVehicle = %this;

  if (%slot == 1)
    Tank3::unmountStuff(%this, %this.objMounter);
}

function Tank3::onPlayerDismount(%this, %player) {
  if (%player.damageVehicle == %this)
    %player.damageVehicle = "";

  if (%this.seat[0] == "")
    Tank3::mountStuff(%this, %this.objMounter);
}
