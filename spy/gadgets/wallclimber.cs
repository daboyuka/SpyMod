RepairEffectData WallClimberTriggerBolt {
   bitmapName       = "bar00.bmp";
   boltLength       = 0;
   segmentDivisions = 1;
   beamWidth        = 0;

   updateTime   = 1000;
   skipPercent  = 0.6;
   displaceBias = 0.15;
};

ItemImageData WallClimberImage {
   shapeFile  = "breath";
	mountPoint = 0;
	mountOffset = "0 0 0";
	mountRotation = "0 0 3.14";

	weaponType = 2; // Single Shot
	reloadTime = 0.5;
	fireTime = 0;
	projectileType = WallClimberTriggerBolt;

	firstPerson = false;

	sfxActivate = SoundPickUpWeapon;
};

ItemData WallClimber {
  description = "Wall Climber";

  shapeFile = "sensor_small";
	shadowDetailMask = 4;

	lightType = 2;   // Pulsing
	lightRadius = 4;
	lightTime = 1.5;
	lightColor = { 0.3, 1, 0 };

	className = "Tool";
	imageType = WallClimberImage;
	heading = $InvHeading[Gadget];

	price = 150;
};

StaticShapeData WallClimberThingy {
	shapeFile = "grenade";
	maxDamage = 20000;
	disableCollision = true;
};

$NumModes[WallClimber] = 3;
$ItemDescription[WallClimber, 0] = "<jc><f1>Wall Climber, Climb Mode:<f0> Fire to set each climbing pad, then fire repeatedly to climb.";
$ItemDescription[WallClimber, 1] = "<jc><f1>Wall Climber, Detach Mode:<f0> Fire to let go of the pads.";
$ItemDescription[WallClimber, 2] = "<jc><f1>Wall Climber, Swing Mode:<f0> Fire and hold to swing forward a bit.";

$WallClimber::callRate = 0.1;
$WallClimber::maxRange = 2;
$WallClimber::maxDist = 0.3;
$WallClimber::stretchStrength = 30;
$WallClimber::recoilStrength = 30;
$WallClimber::swingStrength = 30;
$WallClimber::drag = 3;

function WallClimber::onMount(%player, %item) {
  Weapon::displayDescription(%player, WallClimber);
  Weapon::standardMount(%player, WallClimber);
}
function WallClimber::onUnmount(%player, %item) {
  Weapon::standardUnmount(%player, WallClimber);
}

function WallClimberTriggerBolt::onAcquire(%blah, %this, %targetIsBlahToo) {
  %this.holding = true;
  if (Weapon::getMode(%this, WallClimber) == 0) {
    if (GameBase::getLOSInfo(%this, $WallClimber::maxRange)) {
      WallClimber::setThingy(%this, $los::position, $los::normal);
      if (%this.wallClimberThingy[0] != "" && %this.wallClimberThingy[1] != "") WallClimber::wallClimb(%this);
    } else {
      Client::sendMessage(Player::getClient(%this), 1, "~wLeftMissionArea.wav");
    }
  } else if (Weapon::getMode(%this, WallClimber) == 1) {
    WallClimber::unWallClimb(%this);
  }
}

function WallClimberTriggerBolt::onRelease(%blah, %this) {
  %this.holding = "";
}

function WallClimberTriggerBolt::checkDone(%blah, %this) {}

function WallClimber::wallClimb(%this) {
  if (%this.wallClimbing) return;

  Player::deactivateVelocityModdingItems(%this);

  %this.wallClimbing = true;
  WallClimber::doWallClimb(%this);
}


function WallClimber::unWallClimb(%this) {
//  if (!%this.wallClimbing) return;

  %this.wallClimbing = false;
  for (%i = 0; %i < 2; %i++) {
    if (%this.wallClimberThingy[%i] != "") {
      deleteObject(%this.wallClimberThingy[%i]);
      %this.wallClimberThingy[%i] = "";
    }
  }
}

function WallClimber::swing(%this) {
  %vel = Item::getVelocity(%this);
  %vel = Vector::add(%vel, Vector::getFromRot(GameBase::getRotation(%this), $WallClimber::swingStrength));
  Item::setVelocity(%this, %vel);
}

function WallClimber::doWallClimb(%this) {
  if (!%this.wallClimbing || Player::isDead(%this)) return;  

  if (%this.wallClimberThingy[0] == "" || %this.wallClimberThingy[1] == "") {
    schedule("WallClimber::doWallClimb("@%this@");", $WallClimber::callRate);
    return;
  }

  %maxDist = $WallClimber::maxDist;

  if (Weapon::getMode(%this, WallClimber) == 2 && %this.holding) {
    %maxDist += 1;
  }

  %pos1 = Vector::add(GameBase::getPosition(%this.wallClimberThingy[0]), %this.wallClimberNormal[0]);
  %pos2 = Vector::add(GameBase::getPosition(%this.wallClimberThingy[1]), %this.wallClimberNormal[1]);

  %avgPos = Vector::mul(Vector::add(%pos1, %pos2), 0.5);
  %yourPos = Vector::add(getBoxCenter(%this), "0 0 0.5");
  %you2Avg = Vector::sub(%avgPos, %yourPos);

  %dist = Vector::getDistance(%yourPos, %avgPos);

  if (%dist > %maxDist) {
    %vel = Item::getVelocity(%this);
    %speed2Targ = Vector::dot(%vel, %you2Avg) / %dist;
    if (%speed2Targ < 0) %vel = Vector::add(Vector::mul(%you2Avg, -%speed2Targ * $WallClimber::recoilStrength * $WallClimber::callRate), %vel);
    %speed = Vector::length(%vel);
    %vel = Vector::add(%vel, Vector::resize(%you2Avg, $WallClimber::stretchStrength * $WallClimber::callRate));
    %vel = Vector::add(%vel, Vector::mul(%vel, -min(1, %speed*$WallClimber::drag*$WallClimber::callRate)));
    //Vector::add(Vector::resize(%you2Avg, (%dist - %maxDist) * $WallClimber::stretchStrength * $WallClimber::callRate), %vel);
    Item::setVelocity(%this, %vel);

    if (Weapon::getMode(%this, WallClimber) == 2 && %this.holding) WallClimber::swing(%this);
  }

  schedule("WallClimber::doWallClimb("@%this@");", $WallClimber::callRate);
}

function WallClimber::setThingy(%this, %pos, %normal) {
  if (%this.currentWallClimberThingy == "") %this.currentWallClimberThingy = 0;

  if (%this.wallClimberThingy[%this.currentWallClimberThingy^1] != "") {
    if (Vector::getDistance(%pos, GameBase::getPosition(%this.wallClimberThingy[%this.currentWallClimberThingy^1])) > $WallClimber::maxRange*2) {
      Client::sendMessage(Player::getClient(%this), 1, "~wLeftMissionArea.wav");
      return;
    }
  }

  if (%this.wallClimberThingy[%this.currentWallClimberThingy] == "") {
    %this.wallClimberThingy[%this.currentWallClimberThingy] = newObject("", StaticShape, WallClimberThingy, true);
    addToSet(MissionCleanup, %this.wallClimberThingy[%this.currentWallClimberThingy]);
  }

  GameBase::setPosition(%this.wallClimberThingy[%this.currentWallClimberThingy], %pos);
  %this.wallClimberNormal[%this.currentWallClimberThingy] = %normal;
  %this.currentWallClimberThingy ^= 1;
  GameBase::playSound(%this, SoundPickupWeapon, 0);
}

function WallClimber::onModeChanged(%this, %mode, %oldMode) {
  Weapon::displayDescription(%this, WallClimber);
}

registerGadget(WallClimber);
linkGadget(WallClimber);
setArmorAllowsItem(larmor, WallClimber, 0);
setArmorAllowsItem(lfemale, WallClimber, 0);
setArmorAllowsItem(SpyMale, WallClimber, 0);
setArmorAllowsItem(SpyFemale, WallClimber, 0);
setArmorAllowsItem(DMMale, WallClimber, 0);
setArmorAllowsItem(DMFemale, WallClimber, 0);