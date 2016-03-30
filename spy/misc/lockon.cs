LightningData LockOnCharge {
   bitmapName       = "zap04.bmp";

   damageType       = 0;
   boltLength       = 400.0;
   coneAngle        = 10.0;
   damagePerSec      = 0;
   energyDrainPerSec = 0;
   segmentDivisions = 0;
   numSegments      = 1;
   beamWidth        = 0.00001;

   updateTime   = 10;
   skipPercent  = 0.01;
   displaceBias = 0.01;
};

function LockOnCharge::damageTargetOld(%target, %timeSlice, %damPerSec, %enDrainPerSec, %pos, %vec, %mom, %shooterId) {
  %veh = Client::getControlObject(%shooterId);
  if (getObjectType(%veh) != "Flier") return;

  %name = GameBase::getDataName(%veh);
  %wep = Vehicle::getCurrentWeapon(%veh);
  %time = getSimTime();

  if (%target == %veh.lockonTarget) {
    if (GameBase::virtual(%veh, "verifyTarget", %target)) {
      %veh.lockonUpdateTime = %time;
      %veh.lockonTargetDistract = 0;
    } else LockOn::targetLost(%veh, %target);
  } else if (((%veh.lockonTarget != %target && %veh.lockonTargetDistract >= 2) || %veh.lockonTarget == 0) &&
             GameBase::virtual(%veh, "verifyTarget", %target)) {

    %veh.lockonTarget = %target;
    %veh.lockonTargetLocked = false;
    %veh.lockonStartTime = %time;
    %veh.lockonTargetDistract = 0;
    %veh.lockonUpdateTime = %time;
    GameBase::virtual(%veh, "onTargetLockAcquired", %target);
  } else {
    %veh.lockonTargetDistract++;
  }


  if (%veh.lockonTarget && (%time - %veh.lockonStartTime >= $Vehicle::lockonTime[%name, Vehicle::getCurrentWeapon(%veh)]) && !%veh.lockonTargetLocked) {
    %veh.lockonTargetLocked = true;
    %veh.lockonTargetDistract = -5;
    GameBase::virtual(%veh, "onTargetLockLocked", %target);
  }

  if (%veh.lockonTarget)
    schedule("if (" @ %veh @ ".lockonUpdateTime == " @ %veh.lockonUpdateTime @ ")" @
             "LockOn::targetLost(" @ %veh @ "," @ %target @ ");", $Vehicle::lockExpireTime[%name, %wep] + 0.01);

}

function LockOnCharge::damageTarget(%target, %timeSlice, %damPerSec, %enDrainPerSec, %pos, %vec, %mom, %shooterId) {
  %this = Client::getControlObject(%shooterId);

  %lockWep = Client::getOwnedObject(%shooterId).lockOnWep;//GameBase::getDataName(%this);
  if (%lockWep == "False" || %lockWep == "") return;
  %time = getSimTime();

  if (%target == %this.lockonTarget) {
    if (eval(%lockWep @ "::verifyTarget(" @ %this @ "," @ %target @ ");")) {
      %this.lockonUpdateTime = %time;
      %this.lockonTargetDistract = 0;
    } else { LockOn::targetLost(%this, %target); }
  } else if (((%this.lockonTarget != %target && %this.lockonTargetDistract >= 10) || %this.lockonTarget == 0) &&
             eval(%lockWep @ "::verifyTarget(" @ %this @ "," @ %target @ ");")) {

    %this.lockonTarget = %target;
    %this.lockonTargetLocked = false;
    %this.lockonStartTime = %time;
    %this.lockonTargetDistract = 0;
    %this.lockonUpdateTime = %time;
    eval(%lockWep @ "::onTargetLockAcquired("@%this@","@%target@");");
  } else {
    %this.lockonTargetDistract++;
  }


  if (%this.lockonTarget && (%time - %this.lockonStartTime >= $LockOn::lockonTime[%lockWep]) && !%this.lockonTargetLocked && %target == %this.lockonTarget) {
    %this.lockonTargetLocked = true;
    %this.lockonTargetDistract = -20;
    eval(%lockWep @ "::onTargetLockLocked("@%this@","@%target@");");
  }

  if (%this.lockonTarget) {
    schedule("if (" @ %this @ ".lockonUpdateTime == " @ %this.lockonUpdateTime @ ")" @
             "LockOn::targetLost(" @ %this @ "," @ %target @ ");", $LockOn::lockExpireTime[%lockWep] + 0.01);
  }
}

function LockOn::targetLostOld(%veh, %target) {
  if (%target == %veh.lockonTarget) {
    %veh.lockonTarget = 0;
    %veh.lockonStartTime = -1;
    %veh.lockonTargetLocked = false;
    %veh.lockonUpdateTime = -1;
    %veh.lockonTargetDistract = 0;
  }
  GameBase::virtual(%veh, "onTargetLockLost", %target);
}

function LockOn::targetLost(%this, %target) {
  if (%target == %this.lockonTarget) {
    %this.lockonTarget = 0;
    %this.lockonStartTime = -1;
    %this.lockonTargetLocked = false;
    %this.lockonUpdateTime = -1;
    %this.lockonTargetDistract = 0;
  }
  %player = Client::getOwnedObject(GameBase::getControlClient(%this));
  if (%player.lockOnWep == "False" || %player.lockOnWep == "") return;
  echo(%player.lockOnWep @ "::onTargetLockLost("@%this@","@%target@");");
  eval(%player.lockOnWep @ "::onTargetLockLost("@%this@","@%target@");");
}

function LockOn::enableLockon(%this, %wepName) {
  %client = GameBase::getControlClient(%this);
  if (%client == -1) return;
  %player = Client::getOwnedObject(%client);
  Player::mountItem(%player, LockOn, 7);
  Player::trigger(%player, 7, true);
  %player.lockOnWep = %wepName;
}

function LockOn::disableLockon(%this, %wepName) {
  %client = GameBase::getControlClient(%this);
  if (%client == -1) return;

  %player = Client::getOwnedObject(%client);
  if (%player.lockOnWep != %wepName) return;

  if (Player::getMountedItem(%player, 7) == LockOn) {
    Player::trigger(%player, 7, false);
    Player::unmountItem(%player, 7);
  }
  %this.lockOnWep = "";
}

function LockOn::enableLockon2(%this, %wepName) {
  %client = GameBase::getControlClient(%this);
  if (%client == -1) return;
  %player = Client::getOwnedObject(%client);
  Player::mountItem(%player, LockOn2, 7);
  Player::trigger(%player, 7, true);
  %player.lockOnWep = %wepName;
}

function LockOn::disableLockon2(%this, %wepName) {
  %client = GameBase::getControlClient(%this);
  if (%client == -1) return;

  %player = Client::getOwnedObject(%client);
  if (%player.lockOnWep != %wepName) return;

  if (Player::getMountedItem(%player, 7) == LockOn2) {
    Player::trigger(%player, 7, false);
    Player::unmountItem(%player, 7);
  }
  %this.lockOnWep = "";
}

ItemImageData LockOnImage {
	shapeFile = "breath";
	mountPoint = 4;
	mountOffset = "-0.55 0 0";

	weaponType = 2;
	projectileType = LockOnCharge;
	reloadTime = 0;

	minEnergy = 0;
	maxEnergy = 0;

	firstPerson = false;
};

ItemData LockOn {
   description = "";
	shapeFile = "breath";
	hudIcon = "energyRifle";
   className = "LockOn";
   heading = "bWeapons";
   shadowDetailMask = 4;
   imageType = LockOnImage;
	showWeaponBar = false;
	showInventory = false;
   price = 125;
};

ItemImageData LockOn2Image {
	shapeFile = "breath";
	mountPoint = 0;
	mountOffset = "0 0 0";

	weaponType = 2;
	projectileType = LockOnCharge;
	reloadTime = 0;

	minEnergy = 0;
	maxEnergy = 0;

	firstPerson = false;
};

ItemData LockOn2 {
   description = "";
	shapeFile = "breath";
	hudIcon = "energyRifle";
   className = "LockOn";
   heading = "bWeapons";
   shadowDetailMask = 4;
   imageType = LockOn2Image;
	showWeaponBar = false;
	showInventory = false;
   price = 125;
};