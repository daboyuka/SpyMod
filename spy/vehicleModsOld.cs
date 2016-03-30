function Vehicle::onDamage(%this,%type,%value,%pos,%vec,%mom,%object) {
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

			if (%mountSlot == 1) {
				if (%object.grappling) Grappler::ungrapple(%object, false);
				if (%object.parachuting) ParachutePack::unparachute(%object);
				%weapon = Player::getMountedItem(%object,$WeaponSlot);
				if (%weapon != -1) {
					if (%weapon.className == "Weapon") %object.lastWeapon = %weapon;
					else if (%weapon.className == "Tool") %object.lastGadget = %weapon;
					Player::unMountItem(%object,$WeaponSlot);
				}
				Player::setMountObject(%object, %this, 1);
				Client::setControlObject(%client, %this);
				playSound(%name.mountSound, GameBase::getPosition(%this));
				%object.driver = 1;
				%object.vehicle = %this;
				%this.clLastMount = %client;
				GameBase::virtual(%this, "onPlayerMount", %object);
			} else if (%mountSlot > 1) {
				if (%mountSlot) {
					if (%object.grappling) Grappler::ungrapple(%object, false);
					if (%object.parachuting) ParachutePack::unparachute(%object);
					%object.vehicleSlot = %mountSlot;
					%object.vehicle = %this;
					Player::setMountObject(%object, %this, %mountSlot);
					playSound(%name.mountSound, GameBase::getPosition(%this));
					if ($Vehicle::specialSeats[%name, %mountSlot] != "") 
						Vehicle::giveSpecialWeapon(%object, %this, %mountSlot);
					GameBase::virtual(%this, "onPlayerMount", %object);
				}
			} else if (GameBase::getControlClient(%this) == -1)
				Client::sendMessage(Player::getClient(%object),0,"You cannot pilot a "@%name@" while wearing "@%armor@" armor.~wError_Message.wav");
		}
	}
}

function Vehicle::onDestroyed (%this,%mom) {
	if ($Vehicle::crashProjectile[GameBase::getDataName(%this)] != "")
          Projectile::spawnProjectile($Vehicle::crashProjectile[GameBase::getDataName(%this)],
                                      GameBase::getMuzzleTransform(%this), -1, Item::getVelocity(%this));

	$TeamItemCount[GameBase::getTeam(%this) @ $VehicleToItem[GameBase::getDataName(%this)]]--;
   %cl = GameBase::getControlClient(%this);
	%pl = Client::getOwnedObject(%cl);
	if(%pl != -1) {
	   Player::setMountObject(%pl, -1, 0);
   	Client::setControlObject(%cl, %pl);
		if(%pl.lastWeapon != "") {
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

	}
	for(%i = 0 ; %i < 4 ; %i++)
		if(%this.Seat[%i] != "") {
			%pl = Client::getOwnedObject(%this.Seat[%i]);
		if (%pl.vehicleWep != "") Vehicle::takeSpecialWeapon(%pl, %this);
		   Player::setMountObject(%pl, -1, 0);
	  	 	Client::setControlObject(%this.Seat[%i], %pl);
			%pl.vehicleSlot = "";
		   %pl.vehicle= "";
		}
	calcRadiusDamage(%this, $DebrisDamageType, 2.5, 50, 25, 13, 2, 200, 
		100, 225, 100, %this.lastDamager);
}

function Vehicle::findEmptySeatOld(%this,%client) {
	%numSlots = $Vehicle::numSeats[GameBase::getDataName(%this)];
	%count = 0;
	for (%i = 0; %i < %numSlots; %i++) {
		if (%this.Seat[%i] == "") {
			%slotPos[%count] = Vehicle::getMountPoint(%this, %i + 2);
			%slotVal[%count] = %i + 2;
			%lastEmpty = %i + 2;
			%count++;
		}
	}
	if(%count == 1) {
		%this.Seat[%lastEmpty - 2] = %client;
		return %lastEmpty;
	} else if (%count > 1) {
		%freeSlot = %slotVal[getClosestPosition(%count, GameBase::getPosition(%client), %slotPos[0], %slotPos[1], %slotPos[2], %slotPos[3])];
		%this.Seat[%freeSlot - 2] = %client;
		return %freeSlot;
	} else return "False";
}

function Vehicle::findEmptySeat(%this,%client,%includePassengers,%includeDriver) {
	%numSlots = $Vehicle::numSeats[GameBase::getDataName(%this)];
	%count = 0;
	if (%includeDriver && GameBase::getControlClient(%this) == -1) {
		%slotPos[%count] = Vehicle::getMountPoint(%this, 1);
		%slotVal[%count] = 1;
		%lastEmpty = 1;
		%count++;
	}
	for (%i = 0; %includePassengers && %i < %numSlots; %i++) {
		if (%this.Seat[%i] == "") {
			%slotPos[%count] = Vehicle::getMountPoint(%this, %i + 2);
			%slotVal[%count] = %i + 2;
			%lastEmpty = %i + 2;
			%count++;
		}
	}
	if(%count == 1) {
		if (%lastEmpty != 1) %this.Seat[%lastEmpty - 2] = %client;
		return %lastEmpty;
	} else if (%count > 1) {
		%freeSlot = %slotVal[getClosestPosition(%count, GameBase::getPosition(%client), %slotPos[0], %slotPos[1], %slotPos[2], %slotPos[3], %slotPos[4])];
		if (%slotVal[%freeSlot] != 1) %this.Seat[%freeSlot - 2] = %client;
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
  %cl = GameBase::getControlClient(%this);
  if (%cl != -1) {
    %pl = Client::getOwnedObject(%cl);
    if (getObjectType(%pl) == "Player") {
      // dismount the player
      if (GameBase::testPosition(%pl, Vehicle::getMountPoint(%this,0))) {
        %pl.lastMount = %this;
        %pl.newMountTime = getSimTime() + 3.0;
        Player::setMountObject(%pl, %this, 0);
        Player::setMountObject(%pl, -1, 0);
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
		%this.Seat[%passenger.vehicleSlot-2] = "";
		%passenger.vehicleSlot = "";
		%passenger.vehicle= "";
		Player::setMountObject(%passenger, -1, 0);
		%rotZ = getWord(GameBase::getRotation(%passenger),2);
		GameBase::setRotation(%passenger, "0 0 " @ %rotZ);
		GameBase::setPosition(%passenger,%posX @ " " @ %posY @ " " @ (%posZ + %height));
		%jumpDir = Vector::getFromRot(GameBase::getRotation(%passenger),%velocity,%zVec);
        Item::setVelocity(%passenger, Vector::mul(Item::getVelocity(%this), 1));
		Player::applyImpulse(%passenger,%jumpDir);
	        if (%client.lastWeapon != "") {
        	  Player::useItem(%client, %client.lastWeapon);		 	
	        } else if (%client.lastGadget != "") {
        	  Player::useItem(%client, %client.lastGadget);		 	
	        }
	} else
		Client::sendMessage(Player::getClient(%passanger),0,"Can not dismount - Obstacle in the way.~wError_Message.wav");
}

function Vehicle::countRiders(%this, %includeDriver) {
  %count = 0;
  if (%includeDriver && GameBase::getControlClient(%this) != -1) %count++;

  %dname = GameBase::getDataName(%this);
  for (%i = 0; %i < $Vehicle::numSeats[%dname]; %i++) {
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