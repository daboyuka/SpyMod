SoundData SoundFireZipline {
   wavFileName = "sniper.wav";//"ricoche2.wav";
   profile = Profile3dMedium;
};
SoundData SoundLoadZipline {
   wavFileName = "mortar_reload.wav";
   profile = Profile3dNear;
};

RepairEffectData ZiplineTriggerBolt {
   bitmapName       = "bar00.bmp";
   boltLength       = 0;
   segmentDivisions = 1;
   beamWidth        = 0;

   updateTime   = 1000;
   skipPercent  = 0.6;
   displaceBias = 0.15;
};

ItemImageData ZiplineImage {
	shapeFile = "paintgun";
	mountPoint = 0;

	weaponType = 2;
	reloadTime = 0.1;
	fireTime = 0;
	projectileType = ZiplineTriggerBolt;
	minEnergy = 0;
	maxEnergy = 0;
	
	accuFire = false;

	sfxActivate = SoundPickUpWeapon;
};

ItemData Zipline {
	description = "Zipline";
	className = "Tool";
	shapeFile = "paintgun";
	hudIcon = "mortar";
   heading = $InvHeading[Gadget];
	shadowDetailMask = 4;
	imageType = ZiplineImage;
	price = 125;
	showWeaponBar = false;
};

StaticShapeData ZiplineClaw {
   description = "";
	shapeFile = "mine";
	visibleToSensor = false;
};

function ZiplineClaw::onDamage(%this,%type,%value,%pos,%vec,%mom,%object) {
  if (%this.zipGuy.zipping && %value > 0.001) {
    %this.zipGuy.zipDislodger = %object;
    Zipline::unzip(%this.zipGuy, true);
  }
}

BulletData ZiplineRope2 {
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

$ItemDescription[Zipline, 0] = "<jc><f1>Zipline, Set Hook Mode:<f0> Sets the zipline hooks, or releases them when both are set.";
$ItemDescription[Zipline, 1] = "<jc><f1>Zipline, Tighten Mode:<f0> Tightens the zipline.";
$ItemDescription[Zipline, 2] = "<jc><f1>Zipline, Move Mode:<f0> Moves up and down the rope, or slows a slide.";
$NumModes[Zipline] = 3;

$Zipline::maxRange = 500;
$Zipline::callRate = 0.07;
$Zipline::tightenSpeed = 1.5;
$Zipline::moveStrength = 15;
$Zipline::springStrength = 45;
$Zipline::recoilStrength = 0.9;

function ZiplineTriggerBolt::onAcquire(%blah, %this, %target) {
  %mode = Weapon::getMode(%this, Zipline);

  if (%mode == 0) {
    if (%this.zipHook1 == "" || %this.zipHook2 == "") Zipline::setZipHook(%this);
    else Zipline::unzip(%this);
  }

  %this.holding = true;
}
function ZiplineTriggerBolt::onRelease(%blah, %this, %target) {
  %this.holding = false;
}
function ZiplineTriggerBolt::checkDone(%blah, %this) {}

function Zipline::onMount(%player, %slot) {
  Weapon::displayDescription(%player, Zipline);
}
function Zipline::onUnmount(%player, %slot) {}

function Zipline::onModeChanged(%player) {
  Weapon::displayDescription(%player, Zipline);
}

function Zipline::setZipHook(%this) {
  if (%this.ziplineShotOff) {
    Client::sendMessage(Player::getClient(%this), 1, "Your zipline hook was shot.~wLeftMissionArea.wav");
    return;
  }
  if (GameBase::getLOSInfo(%this, $Zipline::maxRange)) {
    %type = getObjectType($los::object);
    %pos = GameBase::getPosition(%this);

    if (%type == "InteriorShape" || %type == "StaticShape" || %type == "SimTerrain") {
      if (%this.zipHook1 == "") {
        %hookNum = 1;
        %hook = (%this.zipHook1 = newObject("", StaticShape, ZiplineClaw, true));
      } else if (%this.zipHook2 == "") {
        %distTo1 = Vector::getDistance(%pos, GameBase::getPosition(%this.zipHook1));
        %distTo2 = Vector::getDistance(%pos, $los::position);
        if (%distTo1 + %distTo2 > $Zipline::maxRange) return;
        %hookNum = 2;
        %hook = (%this.zipHook2 = newObject("", StaticShape, ZiplineClaw, true));
      } else return;

      addToSet(MissionCleanup, %hook);
      %this.zipping = true;

      GameBase::setPosition(%hook, Vector::add(Vector::resize($los::normal, 0.25), $los::position));
      GameBase::setRotation(%hook, Vector::getRotation($los::normal));
      GameBase::playSequence(%hook, 1, "deploy");

      %hook.ziplineGuy = %this;

      if (%hookNum == 1) {
        Zipline::doZiplineRope(%this);
      } else {
        %this.zipLen = Vector::getDistance(%pos, GameBase::getPosition(%this.zipHook1)) + Vector::getDistance(%pos, GameBase::getPosition(%this.zipHook2));
        %this.taut = false;
        Zipline::doZipline(%this);
      }

      Client::sendMessage(GameBase::getControlClient(%this), 1, "Zipline hook " @ %hookNum @ " attached");
    }
  }
  GameBase::playSound(%this, SoundFireZipline, 0);
}

function Zipline::unzip(%this, %shotOff) {
  if (!%shotOff) {
    Client::sendMessage(GameBase::getControlClient(%this), 1, "Zipline detached");
  } else {
    Client::sendMessage(GameBase::getControlClient(%this), 1, "Zipline hook dislodged");
    %this.ziplineShotOff = true;
    schedule(%this @ ".ziplineShotOff = false;", 3);
  }
  %this.zipping = false;
  GameBase::playSound(%this, SoundLoadZipline, 0);
  schedule("deleteObject("@%this.zipHook1@");", 0.1);
  schedule("deleteObject("@%this.zipHook2@");", 0.1);
  %this.zipHook1 = "";
  %this.zipHook2 = "";
}

function Zipline::rope(%this) {
  %pos = Matrix::subMatrix(GameBase::getMuzzleTransform(%this), 3, 4, 3, 1, 0, 3);

  if (%this.zipHook1 != "") {
    %vec = Vector::sub(GameBase::getPosition(%this.zipHook1), %pos);
    %vec = Vector::resize(%vec, 80);
    Projectile::spawnProjectile(ZiplineRope2, "1 0 0 0 1 0 0 0 1 " @ %pos,
                                %this, %vec);
  }

  if (%this.zipHook2 != "") {
    %vec = Vector::sub(GameBase::getPosition(%this.zipHook2), %pos);
    %vec = Vector::resize(%vec, 80);
    Projectile::spawnProjectile(ZiplineRope2, "1 0 0 0 1 0 0 0 1 " @ %pos,
                                %this, %vec);
  }
}

function Zipline::doZipline(%this) {
  if (!%this.zipping || Player::isDead(%this)) return;

  %client = Player::getClient(%this);
  %mode = Weapon::getMode(%this, Zipline);

  %pos1 = GameBase::getPosition(%this.zipHook1);
  %pos2 = GameBase::getPosition(%this.zipHook2);
  %pos = Vector::add(GameBase::getPosition(%this), "0 0 2");

  %lineVec = Vector::sub(%pos2, %pos1); // line vec from pos1, which is the start point
  %zip1ToYou = Vector::sub(%pos, %pos1); // from hook 1 to you
  %zip2ToYou = Vector::sub(%pos, %pos2); // from hook 2 to you
  %lineVecLen = Vector::length(%lineVec);
  %zip1ToYouLen = Vector::length(%zip1ToYou);
  %zip2ToYouLen = Vector::length(%zip2ToYou);

  if (Weapon::getMode(%this, Zipline) == 1 && %this.holding) {
    if (%this.zipLen > %lineVecLen*1.005) %this.zipLen = max(%this.zipLen - $Zipline::tightenSpeed*$Zipline::callRate, %lineVecLen*1.005);
    else if (!%this.taut) { Client::sendMessage(%client, 1, "Zipline fully tightened"); %this.taut = true; }
  }
  %lineLength = %this.zipLen;

  %vel = Item::getVelocity(%this);

  %tension = $Zipline::springStrength*$Zipline::callRate*max(%zip1ToYouLen + %zip2ToYouLen - %lineLength, 0);
  if (%tension > 0) {
    echo("TENSION: " @ %tension);
    if ((Vector::dot(%zip1ToYou, %lineVec)/%zip1ToYouLen/%lineVecLen < -0.8 || Vector::dot(%zip2ToYou, %lineVec)/%zip2ToYouLen/%lineVecLen > 0.8 && %tension> 2)) { ZipLine::unzip(%this, false); }
    else {

    %endStretch = !(Vector::dot(%zip1ToYou, %lineVec) > 0 && Vector::dot(%zip2ToYou, %lineVec) < 0);
    %pullVec = Vector::add(Vector::resize(%zip1ToYou, -%tension), Vector::resize(%zip2ToYou, -%tension));
    %vel = Vector::add(%pullVec, %vel);

    if (Vector::dot(%vel, %pullVec) < 0) {
      %pullVecN = Vector::normalize(%pullVec);
      %recoil = Vector::mul(%pullVecN, -Vector::dot(%vel, %pullVecN) * $Zipline::recoilStrength);
      %vel = Vector::add(%vel, %recoil);
    }

    }
  } else {
    %pullVec = "0 0 0";
  }

  if (Weapon::getMode(%this, Zipline) == 2 && %this.holding) {
    %lookVec = Matrix::subMatrix(GameBase::getMuzzleTransform(%this), 3, 4, 3, 1, 0, 1);
    if (Vector::dot(%lookVec, %zip1ToYou) < 0) {
      %vel = Vector::add(%vel, Vector::resize(%zip1ToYou, -max($Zipline::moveStrength + min(Vector::dot(%vel, %zip1ToYou)/%zip1ToYouLen, 0), 0) * $Zipline::callRate));
    } else {
      %vel = Vector::add(%vel, Vector::resize(%zip2ToYou, -max($Zipline::moveStrength + min(Vector::dot(%vel, %zip2ToYou)/%zip2ToYouLen, 0), 0) * $Zipline::callRate));
    }
  }

  Item::setVelocity(%this, %vel);

  schedule("Zipline::doZipline("@%this@");", $Zipline::callRate);
}

function Zipline::doZiplineRope(%this) {
  if (!%this.zipping || Player::isDead(%this)) return;
  Zipline::rope(%this);
  schedule("Zipline::doZiplineRope("@%this@");", 0.125);
}

registerGadget(Zipline);
linkGadget(Zipline);
setArmorAllowsItem(larmor, Zipline);
setArmorAllowsItem(lfemale, Zipline);
setArmorAllowsItem(SpyMale, Zipline);
setArmorAllowsItem(SpyFemale, Zipline);
setArmorAllowsItem(DMMale, Zipline);
setArmorAllowsItem(DMFemale, Zipline);