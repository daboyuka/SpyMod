SoundData SoundFireGrappler {
   wavFileName = "sniper.wav";//"ricoche2.wav";
   profile = Profile3dMedium;
};
SoundData SoundLoadGrappler {
   wavFileName = "mortar_reload.wav";
   profile = Profile3dNear;
};

RepairEffectData GrapplerTriggerBolt {
   bitmapName       = "bar00.bmp";
   boltLength       = 0;
   segmentDivisions = 1;
   beamWidth        = 0;

   updateTime   = 1000;
   skipPercent  = 0.6;
   displaceBias = 0.15;
};

ItemImageData GrapplerImage
{
	shapeFile = "paintgun";
	mountPoint = 0;

	weaponType = 2;
	reloadTime = 0.1;//1; 
	fireTime = 0;
	projectileType = GrapplerTriggerBolt;
	minEnergy = 0;
	maxEnergy = 0;
	
	accuFire = false;

//	sfxFire = SoundFireGrappler;
//	sfxReload = SoundLoadGrappler;
	sfxActivate = SoundPickUpWeapon;
};

ItemData Grappler
{
	description = "Grappler";
	className = "Tool";
	shapeFile = "paintgun";
	hudIcon = "mortar";
   heading = $InvHeading[Gadget];
	shadowDetailMask = 4;
	imageType = GrapplerImage;
	price = 125;
	showWeaponBar = false;//true;
};

StaticShapeData GrapplerClaw {
   description = "";
	shapeFile = "mine";
	visibleToSensor = false;
};

StaticShapeData GrapplerClaw2 {
   description = "";
	shapeFile = "mine";
	visibleToSensor = false;
	disableCollision = true;
};

function GrapplerClaw::onDamage(%this,%type,%value,%pos,%vec,%mom,%object) {
  if (%this.grappleGuy.grappling && (%type != $SmokeBlindDamageType && %type != $PoisonGasDamageType) && %value > 0.001) {
    %this.grappleGuy.grapplerDislodger = %object;
    Grappler::ungrapple(%this.grappleGuy, true);
  }
}
function GrapplerClaw2::onDamage(%this,%type,%value,%pos,%vec,%mom,%object) {
  if (%this.grappleGuy.grappling && %value > 0.001) {
    %this.grappleGuy.grapplerDislodger = %object;
    echo(%this,%type,%value,%pos,%vec,%mom,%vertPos,%quadrant,%object);
    Grappler::ungrapple(%this.grappleGuy, true);
  }
}

BulletData GrapplerRope2 {
   bulletShapeName    = "force.dts";
   explosionTag       = noExp;

   damageClass        = 0;                 // 0 = impact, 1 = radius
   damageValue        = 0;
   damageType         = 0;

   muzzleVelocity     = 0.0;
   totalTime          = 10;
   liveTime           = 10;
   inheritedVelocityScale = 1;
   isVisible          = true;

   tracerPercentage   = 1.0;
   tracerLength       = 2.5;
};

$ItemDescription[Grappler, 0] = "<jc><f1>Grappler, Grapple Mode:<f0> Grapples onto hard surfaces.";
$ItemDescription[Grappler, 1] = "<jc><f1>Grappler, Ascend Mode:<f0> Fire and hold ascend";
$ItemDescription[Grappler, 2] = "<jc><f1>Grappler, Descend Mode:<f0> Fire and hold descend";
$ItemDescription[Grappler, 3] = "<jc><f1>Grappler, Swing Mode:<f0> Fire and hold swing forward";
$ItemDescription[Grappler, 4] = "<jc><f1>Grappler, Expert Mode:<f0> For advanced players; see training for info";
$NumModes[Grappler] = 5;

$Grappler::maxRange = 75;
$Grappler::callRate = 0.07;
$Grappler::ascendSpeed = 2.5 * $Grappler::callRate;
$Grappler::descendSpeed = 4 * $Grappler::callRate;
$Grappler::recoilElasticity = 1.2;
$Grappler::stretchElasticity = 1;
$Grappler::swingStrength = 0.3;
//$Grapper::drag = 10;

function GrapplerTriggerBolt::onAcquire(%blah, %this, %target) {
  %mode = Weapon::getMode(%this, Grappler);
  if (%mode == 4) {  // Expert mode
    if (!%this.grappling) {
      Grappler::grapple(%this);
      GameBase::playSound(%this, SoundFireGrappler, 0);
    }
  }

  if (%mode == 0) {
    if (!%this.grappling) {
      Grappler::grapple(%this);
    } else {
      Grappler::ungrapple(%this);
    }
  }
  %this.holding = true;
}
function GrapplerTriggerBolt::onRelease(%blah, %this, %target) {
  %this.holding = false;
}
function GrapplerTriggerBolt::checkDone(%blah, %this) {
}

function Grappler::onMount(%player, %slot) {
  Weapon::displayDescription(%player, Grappler);
}
function Grappler::onUnmount(%player, %slot) {
}

function Grappler::onModeChanged(%player) {
  Weapon::displayDescription(%player, Grappler);
}

function Grappler::grapple(%this) {
  if (%this.grapplerShotOff) {
    Client::sendMessage(Player::getClient(%this), 1, "Your grappler hook was shot.~wLeftMissionArea.wav");
    return;
  }
  if (GameBase::getLOSInfo(%this, $Grappler::maxRange)) {
    %type = getObjectType($los::object);
    if (%type == "InteriorShape" || %type == "StaticShape" || %type == "SimTerrain" || (%type == "Moveable" && GameBase::getDataName($los::object).className == "Elevator")) {
      Client::sendMessage(GameBase::getControlClient(%this), 1, "Grappler attached");
      %this.grappling = true;
      %this.grappleObj = $los::object;
      %this.grappleOffset = Vector::sub($los::position, GameBase::getPosition($los::object));
      %this.grappleRot = GameBase::getRotation($los::object);
      %this.grappleDist = Vector::getDistance($los::position, GameBase::getPosition(%this));

      %this.grappleClaw = newObject("", StaticShape, tern(%type == "Moveable", GrapplerClaw2, GrapplerClaw), true);
      addToSet(MissionCleanup, %this.grappleClaw);

      %this.grappleClaw.hookOffset = Vector::resize($los::normal, 0.25);
      GameBase::setPosition(%this.grappleClaw, Vector::add($los::position, %this.grappleClaw.hookOffset));
      GameBase::setRotation(%this.grappleClaw, Vector::getRotation($los::normal));
      GameBase::playSequence(%this.grappleClaw, 1, "deploy");

      %this.grappleClaw.grappleGuy = %this;

      Grappler::doGrapple(%this);
      Grappler::doGrappleRope(%this);
    }
  }
  GameBase::playSound(%this, SoundFireGrappler, 0);
}

function Grappler::ungrapple(%this, %shotOff) {
  if (!%shotOff) {
    Client::sendMessage(GameBase::getControlClient(%this), 1, "Grappler detached");
  } else {
    Client::sendMessage(GameBase::getControlClient(%this), 1, "Grappler dislodged");
    %this.grapplerShotOff = true;
    schedule(%this @ ".grapplerShotOff = false;", 3);
  }
  %this.grappling = false;
  GameBase::playSound(%this, SoundLoadGrappler, 0);
  schedule("deleteObject("@%this.grappleClaw@");", 0.1);
}

function Grappler::ascend(%this) {
  %this.grappleDist = max(%this.grappleDist - $Grappler::ascendSpeed, 0);
}
function Grappler::descend(%this) {
  %this.grappleDist = min(%this.grappleDist + $Grappler::descendSpeed, $Grappler::maxRange);
}
function Grappler::swing(%this) {
  %vel = Item::getVelocity(%this);
  %vel = Vector::add(%vel, Vector::getFromRot(GameBase::getRotation(%this), $Grappler::swingStrength));
  Item::setVelocity(%this, %vel);
}

function Grappler::rope(%this) {
//  %this.grappleRope = Weapon::fireAutoAimVel(%this, GrapplerRope, %this.grappleClaw, 0, 200);
  %pos = Matrix::subMatrix(GameBase::getMuzzleTransform(%this), 3, 4, 3, 1, 0, 3);
  %vec = Vector::sub(GameBase::getPosition(%this.grappleClaw), %pos);
  %vec = Vector::resize(%vec, 80);
  Projectile::spawnProjectile(GrapplerRope2, "1 0 0 0 1 0 0 0 1 " @ %pos,
                              %this, %vec);
}

function Grappler::doGrapple(%this) {
  if (!%this.grappling || Player::isDead(%this) || !isObject(%this.grappleObj)) return;

  %mode = Weapon::getMode(%this, Grappler);

  if (%this.holding) {
    if (%mode == 1)
      Grappler::ascend(%this);
    if (%mode == 2)
      Grappler::descend(%this);
    if (%mode == 3)
      Grappler::swing(%this);
    if (%mode == 4) // expert
      Grappler::ascend(%this);
  }

  if (%mode == 4 && Player::isJetting(%this)) {
    if (Player::getMountedItem(%this, 0) == Grappler) {
      if (%this.holding) {
        Grappler::ungrapple(%this);
        return;
      } else Grappler::descend(%this);
    } else {
      Grappler::ungrapple(%this);
      return;
    }
  }

//  %hookOff = Vector::rotate(%this.grappleOffset, Vector::sub(GameBase::getRotation(%this.grappleObj), %this.grappleRot));
//  %hookPos = Vector::add(%hookOff, GameBase::getPosition(%this.grappleObj));

  %target = Vector::add(GameBase::getPosition(%this.grappleObj), %this.grappleOffset);//%this.grapplePoint;
  %pos = GameBase::getPosition(%this);
  %dist = Vector::getDistance(%pos, %target);
  %vel = Vector::add(Item::getVelocity(%this), "0 0.00001 0");
  if (%dist > %this.grappleDist) {
    %speed = Vector::getDistance("0 0 0", %vel);
    %velNorm = Vector::normalize(%vel);
    %dir = Vector::sub(%target, %pos);
    %dirNorm = Vector::normalize(%dir);

    %stretch = (%dist - %this.grappleDist) * $Grappler::stretchElasticity;

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
//  %hookOff = Vector::rotate(%this.grappleOffset, Vector::sub(GameBase::getRotation(%this.grappleObj), %this.grappleRot));
  %hookPos = Vector::add(Vector::add(%this.grappleOffset, GameBase::getPosition(%this.grappleObj)), %this.grappleClaw.hookOffset);
  GameBase::setPosition(%this.grappleClaw, %hookPos);  

  schedule("Grappler::doGrapple("@%this@");", $Grappler::callRate);
}

function Grappler::doGrappleRope(%this) {
  if (!%this.grappling || Player::isDead(%this) || !isObject(%this.grappleObj)) return;
  Grappler::rope(%this);
  schedule("Grappler::doGrappleRope("@%this@");", 0.125);
}

function Grappler::reload(%this) {
//  echo("YAY!!!!");
}

registerGadget(Grappler);
linkGadget(Grappler);
setArmorAllowsItem(larmor, Grappler);
setArmorAllowsItem(lfemale, Grappler);
setArmorAllowsItem(SpyMale, Grappler);
setArmorAllowsItem(SpyFemale, Grappler);
setArmorAllowsItem(DMMale, Grappler);
setArmorAllowsItem(DMFemale, Grappler);