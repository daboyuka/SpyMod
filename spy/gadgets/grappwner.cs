ItemData GrappwnerAmmo {
	description = "Grappwner Ammo";
   heading = "xAmmunition";
	className = "Ammo";
	shapeFile = "plasammo";
	shadowDetailMask = 4;
	price = 2;
};

RepairEffectData GrappwnerTriggerBolt {
   bitmapName       = "bar00.bmp";
   boltLength       = 0;
   segmentDivisions = 1;
   beamWidth        = 0;

   updateTime   = 1000;
   skipPercent  = 0.6;
   displaceBias = 0.15;
};

ItemImageData GrappwnerImage
{
	shapeFile = "paintgun";
	mountPoint = 0;

	weaponType = 2;
	reloadTime = 1; 
	fireTime = 0;
	projectileType = GrappwnerTriggerBolt;
	minEnergy = 0;
	maxEnergy = 0;
	
	ammoType = LoadedAmmo;
	
	accuFire = false;

//	sfxFire = SoundFireGrappler;
//	sfxReload = SoundLoadGrappler;
	sfxActivate = SoundPickUpWeapon;
};

ItemData Grappwner
{
	description = "Grappwner";
	className = "Tool";
	shapeFile = "paintgun";
	hudIcon = "mortar";
   heading = $InvHeading[Gadget];
	shadowDetailMask = 4;
	imageType = GrappwnerImage;
	price = 125;
	showWeaponBar = false;//true;
};

StaticShapeData GrappwnerClaw {
   description = "";
	shapeFile = "mine";
	visibleToSensor = false;
};

StaticShapeData GrappwnerClaw2 {
   description = "";
	shapeFile = "mine";
	visibleToSensor = false;
	disableCollision = true;
};

function GrappwnerClaw::onDamage(%this,%type,%value,%pos,%vec,%mom,%object) {
	echo("BANG",%this,%type,%value,%pos,%vec,%mom,%object);
  %gg = %this.grappwnGuy;
  if ((%type != $SmokeBlindDamageType && %type != $PoisonGasDamageType) && %value > 0.001) {
    if (%gg.grappwning) {
	  %gg.grappwnerDislodger = %object;
      Grappwner::ungrappwn(%gg, true);
	} else {
      schedule("deleteObject("@%this@");", 0.1);
    }
  }
}

function GrappwnerClaw2::onDamage(%this,%type,%value,%pos,%vec,%mom,%object) {
  GrappwnerClaw::onDamage(%this,%type,%value,%pos,%vec,%mom,%object);
}

ItemImageData GrappwnerDisplayImage {
	shapeFile = "breath";
	ammoType = GrappwnerAmmo;
};

ItemData GrappwnerDisplay {
	className = "Tool";
	shapeFile = "breath";
	hudIcon = "ammopack";
	imageType = GrappwnerDisplayImage;
	showWeaponBar = true;
	showInventory = false;
};

$ClipSize[Grappwner] = 1;
$AmmoDisplay[Grappwner] = GrappwnerDisplay;
$ItemDescription[Grappwner] = "<jc><f1>Grappwner:<f0> Grapple a wall, then grapple an enemy, then laugh. Hard.";

function GrappwnerTriggerBolt::onAcquire(%blah, %this, %target) {
  GameBase::playSound(%this, SoundFireGrappler, 0);
  if (!%this.grappwnerPrepHookSet) {
	  Grappwner::setHook(%this);
  } else {
	  if (Grappwner::grappwn(%this)) {
		  Player::decItemCount(%this, LoadedAmmo);
	  }
  }
}
function GrappwnerTriggerBolt::onRelease(%blah, %this, %target) {}
function GrappwnerTriggerBolt::checkDone(%blah, %this) {}

function Grappwner::onMount(%player, %slot) {
  Weapon::displayDescription(%player, Grappwner);
  Weapon::standardMount(%player, Grappwner);
}
function Grappwner::onUnmount(%player, %slot) {
	Weapon::standardUnmount(%player, Grappwner);
}

function Grappwner::onNoAmmo(%player) {
  Grappwner::reload(%player);
}

function Grappwner::reload(%player) {
  Weapon::standardReload(%player, Grappwner, SoundLoadGrappler, 2);
}

$Grappwner::slackRatio = 1.03;
$Grappwner::slackAdd = 0.3;
$Grappwner::minDist = 1;

function Grappwner::setHook(%this) {
  if (!GameBase::getLOSInfo(%this, $Grappler::maxRange))
	return;
  
  %type = getObjectType($los::object);
  if (%type != "InteriorShape" && %type != "StaticShape" && %type != "SimTerrain" && !(%type == "Moveable" && GameBase::getDataName($los::object).className == "Elevator"))
	return;

  %this.grappwnerPrepHookSet = true;
  %this.grappwnerPrepObj = $los::object;
  %this.grappwnerPrepOffset = Vector::sub($los::position, GameBase::getPosition($los::object));
  %this.grappwnerPrepRot = GameBase::getRotation($los::object);

  %this.grappwnerPrepClaw = newObject("", StaticShape, tern(%type == "Moveable", GrappwnerClaw2, GrappwnerClaw), true);
  addToSet(MissionCleanup, %this.grappwnerPrepClaw);

  %this.grappwnerPrepClaw.hookOffset = Vector::resize($los::normal, 0.25);
  GameBase::setPosition(%this.grappwnerPrepClaw, Vector::add($los::position, %this.grappwnerPrepClaw.hookOffset));
  GameBase::setRotation(%this.grappwnerPrepClaw, Vector::getRotation($los::normal));
  GameBase::playSequence(%this.grappwnerPrepClaw, 1, "deploy");

  GameBase::playSound(%this, SoundLoadGrappler, 1);
  Client::sendMessage(GameBase::getControlClient(%this), 1, "Grappwner hook set");
}

function Grappwner::grappwn(%this) {
  if (!%this.grappwnerPrepHookSet)
	return false;
	
  if (!GameBase::getLOSInfo(%this, $Grappler::maxRange))
	return false;
  
  %type = getObjectType($los::object);
  if (%type != "Player")
	return false;

  %enemy = $los::object;
  
  if (%enemy.grappwning)
	Grappwner::ungrappwn(%enemy, false);
  
  %enemy.grappwning = true;
  %enemy.grappwnerObj = %this.grappwnerPrepObj;
  %enemy.grappwnerOffset = %this.grappwnerPrepOffset;
  %enemy.grappwnerRot = %this.grappwnerPrepRot;
  %enemy.grappwnerClaw = %this.grappwnerPrepClaw;

  %grapPos = Vector::add(GameBase::getPosition(%enemy.grappwnerObj), %enemy.grappwnerOffset);
  %enemy.grappwnerDist = Vector::getDistance(GameBase::getPosition(%enemy), %grapPos) * $Grappwner::slackRatio + $Grappwner::slackAdd;
  %enemy.grappwnerDist = max(%enemy.grappwnerDist, $Grappwner::minDist);

  %enemy.grappwnerClaw.grappwnGuy = %enemy;
  
  %enemy.grappwnerPwner = %this;
  
  %this.grappwnerPrepObj = %this.grappwnerPrepOffset = %this.grappwnerPrepRot = %this.grappwnerPrepClaw = "";
  %this.grappwnerPrepHookSet = "";
  
  Grappwner::doGrappwn(%enemy);
  //Grappwner::doGrappwnRope(%enemy);
  
  GameBase::playSound(%this, SoundLoadGrappler, 1);
  Client::sendMessage(GameBase::getControlClient(%this), 1, "Grappwner attached to player");
  
  return true;
}

function Grappwner::ungrappwn(%enemy, %shotOff) {
  if (%shotOff) {
    Client::sendMessage(GameBase::getControlClient(%enemy), 1, "Grappwner dislodged");
    %this.grappwnerShotOff++;
    schedule(%this @ ".grappwnerShotOff--;", 3);
  } else {
    Client::sendMessage(GameBase::getControlClient(%enemy), 1, "Grappwner detached");
  }
  %enemy.grappwning = false;
  GameBase::playSound(%enemy, SoundLoadGrappler, 0);
  schedule("deleteObject("@%enemy.grappwnerClaw@");", 0.1);
}

function Grappwner::doGrappwn(%this) {
  if (!%this.grappwning || !isObject(%this) || !isObject(%this.grappwnerObj)) return;

  %this.grappwnerDist = max(%this.grappwnerDist - $Grappler::ascendSpeed, $Grappwner::minDist); // Always tightening
  
  %target = Vector::add(GameBase::getPosition(%this.grappwnerObj), %this.grappwnerOffset);
  %pos = GameBase::getPosition(%this);
  %dist = Vector::getDistance(%pos, %target);
  %vel = Vector::add(Item::getVelocity(%this), "0 0.00001 0");
  if (%dist > %this.grappwnerDist) {
    %speed = Vector::getDistance("0 0 0", %vel);
    %velNorm = Vector::normalize(%vel);
    %dir = Vector::sub(%target, %pos);
    %dirNorm = Vector::normalize(%dir);

    %stretch = (%dist - %this.grappwnerDist) * $Grappler::stretchElasticity;

    %cosAngle = Vector::dot(%velNorm, %dirNorm);
    %speedTowardTarget = %speed * %cosAngle;
    if (%speedTowardTarget < 0) {
      %recoil = Vector::mul(%dirNorm, -%speedTowardTarget * $Grappler::recoilElasticity);
      %vel = Vector::add(%vel, %recoil); //Item::setVelocity(%this, Vector::add(Item::getVelocity(%this), %recoil));
    }
    %vel = Vector::add(%vel, Vector::mul(%dirNorm, %stretch));
  }
  Item::setVelocity(%this, %vel);

  // Move the hook to right place
  %hookPos = Vector::add(Vector::add(%this.grappwnerOffset, GameBase::getPosition(%this.grappwnerObj)), %this.grappwnerClaw.hookOffset);
  GameBase::setPosition(%this.grappwnerClaw, %hookPos);  

  schedule("Grappwner::doGrappwn("@%this@");", $Grappler::callRate);
}

//function Grappler::doGrappleRope(%this) {
//  if (!%this.grappling || Player::isDead(%this) || !isObject(%this.grappleObj)) return;
//  Grappler::rope(%this);
//  schedule("Grappler::doGrappleRope("@%this@");", 0.125);
//}

registerGadget(Grappwner, GrappwnerAmmo, 1);
linkGadget(Grappwner);
setArmorAllowsItem(larmor, Grappwner, 5);
setArmorAllowsItem(lfemale, Grappwner, 5);
setArmorAllowsItem(SpyMale, Grappwner, 5);
setArmorAllowsItem(SpyFemale, Grappwner, 5);
setArmorAllowsItem(DMMale, Grappwner, 5);
setArmorAllowsItem(DMFemale, Grappwner, 5);