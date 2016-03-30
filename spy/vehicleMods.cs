//$Vehicle::driverSeat[Vehicle] = %seat;
//$Vehicle::playersDamageVehicle[Vehicle] = true/false;

function Vehicle::onDamage(%this,%type,%value,%pos,%vec,%mom,%object) {
   if (%value == 0) return;
	if (GameBase::getDamageLevel(%this) < GameBase::getDataName(%this).maxDamage)
		%this.lastDamager = %object;
	StaticShape::onDamage(%this,%type,%value,%pos,%vec,%mom,%object);
}

function Vehicle::onCollision(%this, %object, %nov) {
	%name = GameBase::getDataName(%this);
	if ($Vehicle::customCollisionScript[%name] && !%nov) {
          eval(%name @ "::onCollision(" @ %this @ "," @ %object @ ");");
          return;
        }
	if (GameBase::getDamageLevel(%this) < %name.maxDamage) {
		if (getObjectType (%object) == "Player" && %object.vehicle == "" &&
                    (getSimTime() > %object.newMountTime || %object.lastMount != %this) && %this.fading == "") {

	        	if (Player::isAiControlled(%object)) return;

			%armor = Player::getArmor(%object);
			%client = Player::getClient(%object);


			%mountSlot = Vehicle::findEmptySeat(%this, %client, Vehicle::canArmorRide(%name, %armor),
                                                            Vehicle::canArmorPilot(%name, %armor) && Vehicle::canMount(%this, %object));

			%driverSeat = $Vehicle::driverSeat[%name];

			if (%mountSlot == %driverSeat) {
				if (%object.grappling) Grappler::ungrapple(%object, false);
				if (%object.parachuting) ParachutePack::unparachute(%object);
				%weapon = Player::getMountedItem(%object,$WeaponSlot);
				if (%weapon != -1) {
					if (%weapon.className == "Weapon") %object.lastWeapon = %weapon;
					else if (%weapon.className == "Tool") %object.lastGadget = %weapon;
					Player::unMountItem(%object,$WeaponSlot);
				}
				Player::setMountObject(%object, %this, %driverSeat);
				Client::setControlObject(%client, %this);
				playSound(%name.mountSound, GameBase::getPosition(%this));
				%object.driver = 1;
				%object.vehicle = %this;
				%this.clLastMount = %client;
				GameBase::virtual(%this, "onPlayerMount", %object, %mountSlot);
			} else if (%mountSlot > 0) {
				if (%object.grappling) Grappler::ungrapple(%object, false);
				if (%object.parachuting) ParachutePack::unparachute(%object);
				%object.vehicleSlot = %mountSlot;
				%object.vehicle = %this;
				Player::setMountObject(%object, %this, %mountSlot);
				playSound(%name.mountSound, GameBase::getPosition(%this));
				if ($Vehicle::specialSeats[%name, %mountSlot] != "") 
					Vehicle::giveSpecialWeapon(%object, %this, %mountSlot);
				GameBase::virtual(%this, "onPlayerMount", %object, %mountSlot);
			} else if (GameBase::getControlClient(%this) == -1)
				Client::sendMessage(Player::getClient(%object),0,"You cannot pilot a "@%name@" while wearing "@%armor@" armor.~wError_Message.wav");
		}
	}
}

function Vehicle::onDestroyed (%this,%mom) {
  %name = GameBase::getDataName(%this);
  if ($Vehicle::crashProjectile[%name] != "")
    Projectile::spawnProjectile($Vehicle::crashProjectile[%name],
                                GameBase::getMuzzleTransform(%this), -1, Item::getVelocity(%this));

  $TeamItemCount[GameBase::getTeam(%this) @ $VehicleToItem[GameBase::getDataName(%this)]]--;

  %cl = GameBase::getControlClient(%this);
  %pl = Client::getOwnedObject(%cl);
  if (%pl != -1) {
    Player::setMountObject(%pl, -1, 0);
    Client::setControlObject(%cl, %pl);
    if (%pl.lastWeapon != "") {
      Player::useItem(%pl,%pl.lastWeapon);		 	
      %pl.lastWeapon = "";
    }
    %pl.driver = "";
    %pl.vehicle= "";
    if (Player::getMountedItem(%pl, 7) == LockOn) {
      LockOn::disableLockon(%pl, GameBase::getDataName(%this));
    }
    if (Player::getMountedItem(%pl, 7) == LockOn2) {
      LockOn::disableLockon2(%pl, GameBase::getDataName(%this));
    }
    GameBase::virtual(%this, "onPlayerDismount", %pl);
  }
  for (%i = 0; %i < $Vehicle::numSeats[%name]+1; %i++) {
    if (%this.Seat[%i] != "") {
      %pl = Client::getOwnedObject(%this.Seat[%i]);
      if (%pl.vehicleWep != "") Vehicle::takeSpecialWeapon(%pl, %this);
      Player::setMountObject(%pl, -1, 0);
      Client::setControlObject(%this.Seat[%i], %pl);
      %pl.vehicleSlot = "";
      %pl.vehicle= "";
      GameBase::virtual(%this, "onPlayerDismount", %pl);
    }
  }
  %dmg = $Vehicle::explodeDamage[%name];
  // %this, %type, %radiusRatio, %damageRatio, %forceRatio, %rMax, %rMin, %dMax, %dMin, %fMax, %fMin, %d00d
  calcRadiusDamage(%this, $DebrisDamageType, 2.5, 50, 25, 13, 2,
                   tern(%dmg!="",%dmg,200), tern(%dmg!="",%dmg,100), // fixed damage if specified
                   225, 100, %this.lastDamager);
}

function Vehicle::findEmptySeat(%this,%client,%includePassengers,%includeDriver) {
  %name = GameBase::getDataName(%this);
  %numSeats = $Vehicle::numSeats[%name];
  %driverSeat = $Vehicle::driverSeat[%name];
  %count = 0;

  for (%i = 0; %i < %numSeats+1; %i++) {
    if (%i+1 == %driverSeat) {
      if (GameBase::getControlClient(%this) == -1) {
        %slotPos[%count] = Vehicle::getMountPoint(%this, %driverSeat);
        %slotVal[%count] = %driverSeat;
        %lastEmpty = %driverSeat;
        %count++;
      }
    } else if (%this.Seat[%i] == "") {
      %slotPos[%count] = Vehicle::getMountPoint(%this, %i + 1);
      %slotVal[%count] = %i + 1;
      %lastEmpty = %i + 1;
      %count++;
    }
  }
  if (%count == 1) {
    if (%lastEmpty != %driverSeat) %this.Seat[%lastEmpty - 1] = %client;
    return %lastEmpty;
  } else if (%count > 1) {
    %freeSlot = %slotVal[getClosestPosition(%count, GameBase::getPosition(%client),
                                            %slotPos[0], %slotPos[1], %slotPos[2], %slotPos[3], %slotPos[4])];

    if (%freeSlot != %driverSeat) %this.Seat[%freeSlot - 1] = %client;
    return %freeSlot;
  } else return "False";
}

function getClosestPosition(%num,%playerPos,%slotPos0,%slotPos1,%slotPos2,%slotPos3,%slotPos4) {
	%playerX = getWord(%playerPos,0);
	%playerY = getWord(%playerPos,1);
	for(%i = 0 ;%i<%num;%i++) {
		%x = (getWord(%slotPos[%i],0)) - %playerX;
		%y = (getWord(%slotPos[%i],1)) - %playerY;
		if(%x < 0)
			%x *= -1;
		if(%y < 0)
			%y *= -1;
		%newDistance = sqrt((%x*%x)+(%y*%y));
		if(%newDistance < %distance || %distance == "") {
	  		%distance = %newDistance;			
			%closePos = %i;	
		}
	}		
	return %closePos;
}

function Vehicle::dismount(%this,%mom) {
  %name = GameBase::getDataName(%this);
  %driverSeat = $Vehicle::driverSeat[%name];

  %time = getSimTime();

  %cl = GameBase::getControlClient(%this);
  if (%cl != -1) {
    %pl = Client::getOwnedObject(%cl);
    if (getObjectType(%pl) == "Player") {
      // dismount the player
      if (GameBase::testPosition(%pl, Vector::add(Vehicle::getMountPoint(%this,%driverSeat), "0 0 3"))) {
        %pl.lastMount = %this;
        %pl.lastVehicleUnmountTime = %time;
        %pl.newMountTime = %time + 3.0;

        Player::setMountObject(%pl, -1, 0);
        GameBase::setPosition(%pl, Vector::add(Vehicle::getMountPoint(%this,%driverSeat), "0 0 3"));

        %rot = GameBase::getRotation(%this);
        %rotZ = getWord(%rot, 2);
        GameBase::setRotation(%pl, "0 0 " @ %rotZ);

        Item::setVelocity(%pl, Vector::mul(Item::getVelocity(%this), 0.5));
        Player::applyImpulse(%pl, %mom);

        Client::setControlObject(%cl, %pl);

	playSound(GameBase::getDataName(%this).dismountSound, GameBase::getPosition(%this));

        if (%cl.lastWeapon != "") {
          Player::useItem(%cl, %cl.lastWeapon);		 	
        } else if (%cl.lastGadget != "") {
          Player::useItem(%cl, %cl.lastGadget);		 	
        }

        if (Player::getMountedItem(%pl, 7) == LockOn) {
          LockOn::disableLockon(%pl, GameBase::getDataName(%this));
        }
        if (Player::getMountedItem(%pl, 7) == LockOn2) {
          LockOn::disableLockon2(%pl, GameBase::getDataName(%this));
        }

        %pl.driver = "";
        %pl.vehicle = "";

	GameBase::virtual(%this, "onPlayerDismount", %pl);
      } else Client::sendMessage(%cl,0,"Can not dismount - Obstacle in the way.~wError_Message.wav");
    }
  }
}

function Vehicle::passengerJump(%this,%passenger,%mom, %v) {
	if (!%v) {
		GameBase::virtual(%this, "passengerJump", %passenger, %mom, true);
		return;
	}

	%armor = Player::getArmor(%passenger);
	if (%armor == "larmor" || %armor == "lfemale" || %armor == "SpyMale" || %armor == "SpyFemale" || %armor == "DMMale" || %armor == "DMFemale") {
		%height = 2;
		%velocity = 50;
		%zVec = 60;
	} else if (%armor == "marmor" || %armor == "mfemale") {
		%height = 2;
		%velocity = 100;
		%zVec = 100;
	} else if (%armor == "harmor") {
		%height = 2;
		%velocity = 140;
		%zVec = 110;
	}

	%pos = GameBase::getPosition(%passenger);
	%posX = getWord(%pos,0);
	%posY	= getWord(%pos,1);
	%posZ	= getWord(%pos,2);

	if (GameBase::testPosition(%passenger,%posX @ " " @ %posY @ " " @ (%posZ + %height))) {	
		if (%passenger.vehicleWep != "") Vehicle::takeSpecialWeapon(%passenger, %this);
		%client = Player::getClient(%passenger);
		%this.Seat[%passenger.vehicleSlot-1] = "";
		%passenger.vehicleSlot = "";
		%passenger.vehicle = "";

		%time = getSimTime();
	        %pl.lastMount = %this;
        	%pl.lastVehicleUnmountTime = %time;
	        %pl.newMountTime = %time + 0.0; // Allow immediate remount

		Player::setMountObject(%passenger, -1, 0);
		%rotZ = getWord(GameBase::getRotation(%passenger),2);
		GameBase::setRotation(%passenger, "0 0 " @ %rotZ);
		GameBase::setPosition(%passenger,%posX @ " " @ %posY @ " " @ (%posZ + %height));
		%jumpDir = Vector::getFromRot(GameBase::getRotation(%passenger),%velocity,%zVec);
		Item::setVelocity(%passenger, Vector::mul(Item::getVelocity(%this), 1));
		Player::applyImpulse(%passenger,%jumpDir);
	        if (%client.lastWeapon != "") {
        	  //Player::useItem(%client, %client.lastWeapon);		 	
	        } else if (%client.lastGadget != "") {
        	  //Player::useItem(%client, %client.lastGadget);		 	
	        }
		GameBase::virtual(%this, "onPlayerDismount", %passenger);
	} else
		Client::sendMessage(Player::getClient(%passanger),0,"Can not dismount - Obstacle in the way.~wError_Message.wav");
}

function Vehicle::countRiders(%this, %includeDriver) {
  %count = 0;
  if (%includeDriver && GameBase::getControlClient(%this) != -1) %count++;

  %dname = GameBase::getDataName(%this);
  for (%i = 0; %i < $Vehicle::numSeats[%dname]+1; %i++) {
    if (%this.Seat[%i] != "") %count++;
  }

  return %count;
}

function Vehicle::giveSpecialWeapon(%player, %veh, %slot) {
  %str = $Vehicle::specialSeats[GameBase::getDataName(%veh), %slot];
  %wep = getWord(%str, 0);
  if ($WeaponAmmo[%wep] != "") {
    %ammoAmt = getWord(%str, 1);
    %ammo = $WeaponAmmo[%wep];
  } else %ammo = -1;

  Player::incItemCount(%player, %wep, 1);
  if (%ammo != "" && %ammo != -1) Player::incItemCount(%player, %ammo, %ammoAmt);
  Player::mountItem(%player, %wep, $WeaponSlot);

  %player.vehicleWep = %wep;
  %player.vehicleWepAmmo = %ammoAmt;
  %player.weaponLock = true;
}

function Vehicle::takeSpecialWeapon(%player, %vehicle) {
  %wep = %player.vehicleWep;
  if ($WeaponAmmo[%wep] && $WeaponAmmo[%wep] != "") {
    %ammo = $WeaponAmmo[%wep];
  } else %ammo = -1;

  Player::decItemCount(%player, %wep);
  if (%ammo != "" && %ammo != -1) Player::decItemCount(%player, %ammo, %player.vehicleWepAmmo);

  %player.vehicleWep = "";
  %player.vehicleWepAmmo = "";
  %player.weaponLock = false;
}

function Vehicle::canArmorPilot(%vehicle, %armor) {
  return ($Vehicle::canArmorPilot[%vehicle, %armor] && $Vehicle::canArmorPilot[%vehicle, %armor] != "");
}
function Vehicle::canArmorRide(%vehicle, %armor) {
  return ($Vehicle::canArmorRide[%vehicle, %armor] && $Vehicle::canArmorRide[%vehicle, %armor] != "");
}

function Vehicle::getCurrentWeapon(%vehicle) {
  if (%vehicle.currentWeapon == "") %vehicle.currentWeapon = 0;
  return %vehicle.currentWeapon;
}

function Vehicle::displayWeaponDescription(%this) {
  %client = GameBase::getControlClient(%this);
  %name = GameBase::getDataName(%this);
  %wep = Vehicle::getCurrentWeapon(%this);
  bottomprint(%client, $Vehicle::weaponDescription[%name, %wep]);
}

function setVehicleAllowsArmorToPilot(%vehicle, %armor) {
  $Vehicle::canArmorPilot[%vehicle, %armor] = true;
}

function setVehicleAllowsArmorToRide(%vehicle, %armor) {
  $Vehicle::canArmorRide[%vehicle, %armor] = true;
}

function addSpecialSeatToVehicle(%vehicle, %slot, %item) {
  $Vehicle::specialSeats[%vehicle, %slot] = %item;
}

setVehicleAllowsArmorToPilot(Scout, larmor);
setVehicleAllowsArmorToPilot(Scout, lfemale);
setVehicleAllowsArmorToPilot(Scout, SpyMale);
setVehicleAllowsArmorToPilot(Scout, SpyFemale);
setVehicleAllowsArmorToPilot(Scout, DMMale);
setVehicleAllowsArmorToPilot(Scout, DMFemale);
$Vehicle::numSeats[Scout] = 0;

setVehicleAllowsArmorToPilot(LAPC, larmor);
setVehicleAllowsArmorToPilot(LAPC, lfemale);
setVehicleAllowsArmorToPilot(LAPC, SpyMale);
setVehicleAllowsArmorToPilot(LAPC, SpyFemale);
setVehicleAllowsArmorToPilot(LAPC, DMMale);
setVehicleAllowsArmorToPilot(LAPC, DMFemale);
setVehicleAllowsArmorToRide(LAPC, larmor);
setVehicleAllowsArmorToRide(LAPC, lfemale);
setVehicleAllowsArmorToRide(LAPC, marmor);
setVehicleAllowsArmorToRide(LAPC, mfemale);
setVehicleAllowsArmorToRide(LAPC, harmor);
setVehicleAllowsArmorToRide(LAPC, SpyMale);
setVehicleAllowsArmorToRide(LAPC, SpyFemale);
setVehicleAllowsArmorToRide(LAPC, DMMale);
setVehicleAllowsArmorToRide(LAPC, DMFemale);
$Vehicle::numSeats[LAPC] = 2;

setVehicleAllowsArmorToPilot(HAPC, larmor);
setVehicleAllowsArmorToPilot(HAPC, lfemale);
setVehicleAllowsArmorToPilot(HAPC, SpyMale);
setVehicleAllowsArmorToPilot(HAPC, SpyFemale);
setVehicleAllowsArmorToPilot(HAPC, DMMale);
setVehicleAllowsArmorToPilot(HAPC, DMFemale);
setVehicleAllowsArmorToRide(HAPC, larmor);
setVehicleAllowsArmorToRide(HAPC, lfemale);
setVehicleAllowsArmorToRide(HAPC, marmor);
setVehicleAllowsArmorToRide(HAPC, mfemale);
setVehicleAllowsArmorToRide(HAPC, harmor);
setVehicleAllowsArmorToRide(HAPC, SpyMale);
setVehicleAllowsArmorToRide(HAPC, SpyFemale);
setVehicleAllowsArmorToRide(HAPC, DMMale);
setVehicleAllowsArmorToRide(HAPC, DMFemale);
$Vehicle::numSeats[HAPC] = 4;

//---------------------------------------------------------------------------------------------------------------------------

MarkerData VehicleRespawnMarker {
	shapeFile = "dirArrows";
};

function VehicleRespawnMarker::onAdd(%this) {
  schedule("VehicleRespawnMarker::doRespawn("@%this@");", 5);
}

function VehicleRespawnMarker::doRespawn(%this) {
  if (!isObject(%this)) return;

  if (%this.enabled) VehicleRespawnMarker::tryRespawn(%this);

  schedule("VehicleRespawnMarker::doRespawn("@%this@");", %this.respawnTime);
}

function VehicleRespawnMarker::tryRespawn(%this) {
  %v = newObject("", Flier, %this.vehicleType, true);
  if (GameBase::testPosition(%v, Vector::add(GameBase::getPosition(%this), "0 0 0.1")) &&
      (!%this.noMultipleVehicles || %this.lastVehicle.respawnPoint != %this)) {
    GameBase::setPosition(%v, GameBase::getPosition(%this));
    GameBase::setRotation(%v, GameBase::getRotation(%this));
    GameBase::startFadeIn(%v);
    GameBase::setTeam(%v, %this.team);
    addToSet(MissionCleanup, %v);
    %this.lastVehicle = %v;
    %v.respawnPoint = %this;
  } else {
    deleteObject(%v);
  }
}