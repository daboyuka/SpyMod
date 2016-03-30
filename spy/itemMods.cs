$InvHeading[Weapon] = "aWeapons";
$InvHeading[Gadget] = "bGadgets";
$InvHeading[LoadedAmmo] = "xAmmunition";
$InvHeading[Ammo] = "xAmmunition";

$FirstWeapon = "";
$FirstGadget = "";
$FirstGrenadeType = "";
function registerWeapon(%wep, %ammo, %sellAmmo) {
  $InvList[%wep] = 1;
  if (%ammo != "") {
    if ($InvList[%ammo] == "")
      $InvList[%ammo] = 1;

    $WeaponAmmo[%wep] = %ammo;
    if (%sellAmmo != "")
      $SellAmmo[%ammo] = %sellAmmo;
  }
}
function registerGadget(%gad, %ammo, %sellAmmo) {
  registerWeapon(%gad, %ammo, %sellAmmo);
}

function registerGrenade(%g, %sellAmmo) {
  registerWeapon(%g, %g, %sellAmmo);
}

function linkWeapon(%wep) {
  if ($FirstWeapon == "") {
    $FirstWeapon = %wep;
    $NextWeapon[%wep] = $PrevWeapon[%wep] = %wep;
  } else {
    %lastWep = $PrevWeapon[$FirstWeapon];

    $NextWeapon[%lastWep] = %wep;
    $NextWeapon[%wep] = $FirstWeapon;
    $PrevWeapon[$FirstWeapon] = %wep;
    $PrevWeapon[%wep] = %lastWep;
  }
}
function linkGadget(%gad) {
  if ($FirstGadget == "") {
    $FirstGadget = %gad;
    $NextWeapon[%gad] = $PrevWeapon[%gad] = %gad;
  } else {
    %lastGad = $PrevWeapon[$FirstGadget];

    $NextWeapon[%lastGad] = %gad;
    $NextWeapon[%gad] = $FirstGadget;
    $PrevWeapon[$FirstGadget] = %gad;
    $PrevWeapon[%gad] = %lastGad;
  }
}
function linkGrenade(%nade) {
  if ($FirstGrenadeType == "") {
    $FirstGrenadeType = %nade;
    $NextGrenadeType[%nade] = %nade;
  } else {
    for (%type = $FirstGrenadeType; true; %type = $NextGrenadeType[%type]) {
      if ($NextGrenadeType[%type] == $FirstGrenadeType) break;
    }
    $NextGrenadeType[%type] = %nade;
    $NextGrenadeType[%nade] = $FirstGrenadeType;
  }
}

function setArmorAllowsItem(%armor, %item, %maxAmmo) {
  $ItemMax[%armor, %item] = 1;
  %ammoType = $WeaponAmmo[%item];
  if (%ammoType != "" && %maxAmmo > 0) {
    $ItemMax[%armor, %ammoType] = %maxAmmo;
    if ($WeaponAmmo[%item] == "")
      $WeaponAmmo[%item] = %ammoType;
    if ($SellAmmo[%ammoType] == "")
      $SellAmmo[%ammoType] = ceil(%maxAmmo / 5);
  }
}


// WARNING!!!! THIS FUNCTION IS NOT CALLED DIRECTLY, AND %client IS ACTUALLY THE PLAYER OBJECT ID
function remoteUseItem(%client,%type) {

	if (getObjectType(%client) != "Player") return;

	%item = getItemData(%type);

	if (%item == "Blaster")         if (Weapon::onUseSpecial(%client, 1)) return;
	if (%item == "PlasmaGun")       if (Weapon::onUseSpecial(%client, 2)) return;
	if (%item == "Chaingun")        if (Weapon::onUseSpecial(%client, 3)) return;
	if (%item == "Disclauncher")    if (Weapon::onUseSpecial(%client, 4)) return;
	if (%item == "GrenadeLauncher") if (Weapon::onUseSpecial(%client, 5)) return;
	if (%item == "LaserRifle")      if (Weapon::onUseSpecial(%client, 6)) return;
	if (%item == "EnergyRifle")     if (Weapon::onUseSpecial(%client, 7)) return;
	if (%item == "Mortar")          if (Weapon::onUseSpecial(%client, 8)) return;
	if (%item == "TargetingLaser")  if (Weapon::onUseSpecial(%client, 9)) return;

	%client.throwStrength = 1;

	if (%item == Backpack) 
		%item = Player::getMountedItem(%client,$BackpackSlot);
	else {
		if (%item == Weapon) 
			%item = Player::getMountedItem(%client,$WeaponSlot);
	}
	if (%item == "Beacon") {
		ComUplink::onUse(%client);
	}

  if (%client.weaponLock) return;
	Player::useItem(%client,%item);
}

function remoteThrowItem(%client,%type,%strength) {
	%player = Client::getOwnedObject(%client);
	if(%player.Station == "" && %player.waitThrowTime + $WaitThrowTime <= getSimTime()) {
		if(GameBase::getControlClient(%player) != -1 || %player.vehicle != "") {
			%item = getItemData(%type);

			if (%item == Grenade) %item = %client.grenadeType;

			if (%item == "SmokeGrenadeItem" && $waitingForSmokers) {
				Client::sendMessage(%client,1,"Sorry, no one can use smoke grenades until the next mission starts");
				return;
			}

			if (%item == SmokeGrenadeItem || %item == DistractionGrenadeItem || %item == GasGrenadeItem || %item == SpikeGrenadeItem) {
				if (%strength < 0)
					%strength = 0;
				else
					if (%strength > 60)
						%strength = 60;
				%client.throwStrength = 0.3 + 0.7 * (%strength / 100);
				Player::useItem(%client,%item);
			}
		}
	}
}

function remoteDropItem(%client,%type) {
	if (!Client::getOwnedObject(%client).driver && !Client::getOwnedObject(%client).weaponLock &&
            !Client::getOwnedObject(%client).vehicleWep) {
		//echo("Drop item: ",%type);
		%client.throwStrength = 1;

		%item = getItemData(%type);
		if (%item == Backpack) {
			%item = Player::getMountedItem(%client,$BackpackSlot);
			Player::dropItem(%client,%item);
		} else if (%item == Weapon) {
			%item = Player::getMountedItem(%client,$WeaponSlot);
			Player::dropItem(%client,%item);
		} else if (%item == Ammo) {
			%item = Player::getMountedItem(%client,$WeaponSlot);
			if(%item.className == Weapon || %item.className == Tool) {
				%item = $WeaponAmmo[%item];
				if (%item != "") Player::dropItem(%client,%item);
			}
		} else if (%item == "flag") {
			Player::dropItem(%client, Player::getMountedItem(%client, $FlagSlot));
		} else 
			Player::dropItem(%client,%item);
	}
}

function remoteNextWeapon(%client) {
  if (Client::getControlObject(%client).weaponLock) return;

  if (getObjectType(Client::getControlObject(%client)) == "Flier") {
    %veh = Client::getControlObject(%client);
    %name = GameBase::getDataName(%veh);
    if ($Vehicle::numWeps[%name]) {
      %veh.currentWeapon++;
      %veh.currentWeapon %= $Vehicle::numWeps[%name];
      GameBase::virtual(%veh, "onNextWeapon", %veh.currentWeapon);
    }
    return;
  }

  if (getObjectType(Client::getControlObject(%client)) == "Turret") {
    %turret = Client::getControlObject(%client);
    %name = GameBase::getDataName(%turret);
    if ($Turret::catchWepChanges[%name] != "") {
      eval(%name @ "::onNextWeapon("@%turret@");");//GameBase::virtual(%turret, "onNextWeapon");
    }
    return;
  }

	if (Client::getOwnedObject(%client).vehicleWep != "") return;
	%item = Player::getMountedItem(%client, $WeaponSlot);
	if (%item == -1 || $NextWeapon[%item] == "")
		selectValidWeapon(%client);
	else {
		for (%weapon = $NextWeapon[%item]; %weapon != %item && %weapon != ""; %weapon = $NextWeapon[%weapon]) {
			if (isSelectableWeapon(%client, %weapon)) {
				Player::useItem(%client, %weapon);
				// Make sure it mounted (laser may not), or at least
				// next in line to be mounted.
				if (Player::getMountedItem(%client, $WeaponSlot) == %weapon ||
						Player::getNextMountedItem(%client, $WeaponSlot) == %weapon)
					break;
			}
		}
		if (%weapon.className == "Weapon") %client.lastWeapon = %weapon;
		else if (%weapon.className == "Tool") %client.lastGadget = %weapon;
	}
}

function remotePrevWeapon(%client) {
  if (Client::getControlObject(%client).weaponLock) return;

  if (getObjectType(Client::getControlObject(%client)) == "Flier") {
    %veh = Client::getControlObject(%client);
    %name = GameBase::getDataName(%veh);
    if ($Vehicle::numWeps[%name]) {
      %veh.currentWeapon += $Vehicle::numWeps[%name] - 1;
      %veh.currentWeapon %= $Vehicle::numWeps[%name];
      GameBase::virtual(%veh, "onPrevWeapon", %veh.currentWeapon);
    }
    return;
  }

  if (getObjectType(Client::getControlObject(%client)) == "Turret") {
    %turret = Client::getControlObject(%client);
    %name = GameBase::getDataName(%turret);
    if ($Turret::catchWepChanges[%name]) {
      GameBase::virtual(%turret, "onPrevWeapon");
    }
    return;
  }


	if (Client::getOwnedObject(%client).vehicleWep != "") return;
	%item = Player::getMountedItem(%client, $WeaponSlot);
	if (%item == -1 || $PrevWeapon[%item] == "")
		selectValidWeapon(%client);
	else {
		for (%weapon = $PrevWeapon[%item]; %weapon != %item && %weapon != "";
				%weapon = $PrevWeapon[%weapon]) {
			if (isSelectableWeapon(%client, %weapon)) {
				Player::useItem(%client, %weapon);
				// Make sure it mounted (laser may not), or at least
				// next in line to be mounted.
				if (Player::getMountedItem(%client, $WeaponSlot) == %weapon ||
						Player::getNextMountedItem(%client, $WeaponSlot) == %weapon)
					break;
			}
		}
	}
}



function selectValidWeapon(%client, %start) {
  if (%client == "" || %client == -1) return;

  if (Client::getControlObject(%client).weaponLock) return;
	if (%start != "") %item = %start;
        else %item = $FirstWeapon;

	%foundOne = false;
	if (isSelectableWeapon(%client,%item)) {
		Player::useItem(%client,%item);
		%foundOne = true;
        } else {
		if ($NextWeapon[%item] == "") %item = $FirstWeapon;
		for (%weapon = $NextWeapon[%item]; %weapon != %item; %weapon = $NextWeapon[%weapon]) {
			if (isSelectableWeapon(%client,%weapon)) {
				Player::useItem(%client,%weapon);
				%foundOne = true;
				break;
			}
		}
	}
	if (!%foundOne && isSelectableWeapon(%client, $FirstWeapon)) Player::useItem(%client,$FirstWeapon);
}
function selectValidGadget(%client, %start) {
  if (%client == "" || %client == -1) return;

  if (Client::getControlObject(%client).weaponLock) return;
	if (%start != "") %item = %start;
        else %item = $FirstGadget;

	%foundOne = false;

	if (isSelectableWeapon(%client,%item)) {
		Player::useItem(%client,%item);
		%foundOne = true;
        } else {
		for (%weapon = $NextWeapon[%item]; %weapon != %item; %weapon = $NextWeapon[%weapon]) {
			if (isSelectableWeapon(%client,%weapon)) {
				Player::useItem(%client,%weapon);
				%foundOne = true;
				break;
			}
		}
	}
	if (!%foundOne && isSelectableWeapon(%client, $FirstGadget)) Player::useItem(%client,$FirstGadget);
}

function isSelectableWeapon(%client,%weapon) {
	if (Player::getItemCount(%client,%weapon)) {
		%ammo = $WeaponAmmo[%weapon];
                %usesClips = Weapon::usesClips(%weapon);
		%player = Client::getControlObject(%client);
		if (%ammo == "") return true;
		if (Player::getItemCount(%client,%ammo) > 0 ||
                    (%usesClips && %player.loadedAmmo[%weapon] != "" && %player.loadedAmmo[%weapon] > 0) ||
                    (%weapon == GrenadeLauncher2 && GrenadeLauncher::isSelectable(%player))) return true;
	}
	return false;
}

function Weapon::onUseSpecial(%player, %num) {
  if (Player::isDead(%player)) return;

  %wep = Player::getMountedItem(%player, $WeaponSlot);
  %client = Player::getClient(%player);
  if (%num == 1) {
    if (%wep != -1)
      if (%wep.className == "Tool") %client.lastGadget = %wep;

    selectValidWeapon(Player::getClient(%player), %client.lastWeapon);
  } else if (%num == 2) {
    if (%wep != -1)
      if (%wep.className == "Weapon") %client.lastWeapon = %wep;
    selectValidGadget(Player::getClient(%player), %client.lastGadget);
  } else if (%num == 3) {
    Weapon::nextWeaponMode(%player);
  } else if (%num == 4) {
    if (%wep != -1)
      if (Weapon::usesClips(%wep)) eval(%wep @ "::reload("@%player@");");
  } else if (%num == 5) {
    Grenade::nextGrenadeType(%player);
  } else if (%num == 6) {
    PrizeFlamingMunkee::activate(%player);
  } else return false;

  return true;
}

function Item::pop(%item) {
   if (getSimTime() - %item.lastPickupTime < $ItemPopTime) return;
 	GameBase::startFadeOut(%item);
   schedule("deleteObject(" @ %item @ ");",2.5, %item);
}

function Weapon::onUse(%player,%item) {
//	if (Player::getItemState(%player, 0) == "Fire") return;
	if(%player.Station=="") {
		%ammo = $WeaponAmmo[%item];//%item.imageType.ammoType;
		if (%ammo == "") {
			// Energy weapons dont have ammo types
			Player::mountItem(%player,%item,$WeaponSlot);
		} else {
			if (Player::getItemCount(%player,%ammo) > 0) {
				Player::mountItem(%player,%item,$WeaponSlot);
			} else if (%item == "GrenadeLauncher2" && GrenadeLauncher::isSelectable(%player)) {
				Player::mountItem(%player,%item,$WeaponSlot);
			} else if (Weapon::usesClips(%item) && %player.loadedAmmo[%item] != "" && %player.loadedAmmo[%item] > 0) {
				Player::mountItem(%player,%item,$WeaponSlot);
                        } else {
				Client::sendMessage(Player::getClient(%player),0,
				strcat(%item.description," has no ammo"));
			}
		}
	}
}

function Weapon::onDrop(%player,%item) {
	%state = Player::getItemState(%player,$WeaponSlot);
	if (%state != "Fire" && %state != "Reload") {
		%obj = Item::onDrop(%player,%item);
		if (%obj && Weapon::usesClips(%item)) {
                  %obj.loadedAmmo = %player.loadedAmmo[%item];
		  Player::decItemCount(%player, $WeaponAmmo[%item], %player.loadedAmmo[%item]);
                  %player.loadedAmmo[%item] = 0;
                }
	}
}	

function Weapon::onCollision(%this, %object) {
  if (getObjectType(%object) == "Player") {
    %item = Item::getItemData(%this);
    %count = Player::getItemCount(%object,%item);
    if (Item::giveItem(%object,%item,Item::getCount(%this))) {
      Item::playPickupSound(%this);
      if (Weapon::usesClips(%item)) {
        %object.loadedAmmo[%item] = %this.loadedAmmo;
        Player::incItemCount(%object, $WeaponAmmo[%item], %this.loadedAmmo);
      }
      Item::respawn(%this);
    }
  }
  if (GameBase::getDataName(%object) == "Transport") {
    Transport::addDropItem(%object, %this);
  }
}



function Weapon::nextWeaponMode(%player) {
  %wep = Player::getMountedItem(%player, 0);
  if (%wep == -1) return false;

  %oldMode = Weapon::getMode(%player, %wep);
  %mode = (%oldMode + 1) % $NumModes[%wep];
  return Weapon::trySetMode(%player, %wep, %mode);
}

function Weapon::trySetMode(%player, %wep, %mode) {
  if (%mode >= $NumModes[%wep]) return false;
  %mwep = Player::getMountedItem(%player, 0);

  %state = Player::getItemState(%player, 0);
  if (%mwep == %wep && (%state == "Fire" || %state == "Reload")) return false;

  %client = GameBase::getControlClient(%player);
  %oldMode = Weapon::getMode(%player, %wep);
  Weapon::setMode(%player, %wep, %mode);
  eval(%wep @ "::onModeChanged("@%player@","@%mode@","@%oldMode@");");

  return true;
}

function Client::trySetItemMode(%clientId, %wep, %mode) {
  if (Client::getOwnedObject(%clientId) == -1) {
    %clientId.weaponMode[%wep] = %mode;
  } else {
    Weapon::trySetMode(Client::getOwnedObject(%clientId), %wep, %mode);
  }
}

function Weapon::getMode(%player, %item) {
  if (!Player::isAIControlled(%player)) {
    %client = GameBase::getOwnerClient(%player);
    if (%client.weaponMode[%item] == "") %client.weaponMode[%item] = 0;
    return %client.weaponMode[%item];
  } else {
    if (%player.weaponMode[%item] == "") %player.weaponMode[%item] = 0;
    return %player.weaponMode[%item];
  }
}

function Weapon::setMode(%player, %item, %mode) {
  %client = GameBase::getControlClient(%player);
  %client.weaponMode[%item] = %mode;
}

function Weapon::fireProjectile(%player, %proj, %noAimCorrect) {
  %trans = GameBase::getMuzzleTransform(%player);
  if ($Server::AimCorrectionEnabled && !%noAimCorrect) %trans = Matrix::aimCorrect(%player, %trans);
  Projectile::spawnProjectile(%proj, %trans, %player, Item::getVelocity(%player));
}

function Weapon::fireProjectileNoVel(%player, %proj, %noAimCorrect) {
  %trans = GameBase::getMuzzleTransform(%player);
  if ($Server::AimCorrectionEnabled && !%noAimCorrect) %trans = Matrix::aimCorrect(%player, %trans);
  Projectile::spawnProjectile(%proj, %trans, %player, "0 0 0");
}

function Weapon::fireFixedOffsetProjectile(%player, %proj, %offset, %noAimCorrect) {
  %oldTrans = GameBase::getMuzzleTransform(%player);
  %oldMatrix = Matrix::subMatrix(%oldTrans, 3, 4, 3, 3);
  %oldPos = Matrix::subMatrix(%oldTrans, 3, 4, 3, 1, 0, 3);
  %newTrans = %oldMatrix @ " " @ Vector::add(%oldPos, %offset);

  if ($Server::AimCorrectionEnabled && !%noAimCorrect) %newTrans = Matrix::aimCorrect(%player, %newTrans);

  Projectile::spawnProjectile(%proj, %newTrans, %player, Item::getVelocity(%player));
}

function Weapon::fireOffsetProjectile(%player, %proj, %offset, %target, %noAimCorrect) {
  %oldTrans = GameBase::getMuzzleTransform(%player);
  %oldMatrix = Matrix::subMatrix(%oldTrans, 3, 4, 3, 3);
  %oldPos = Matrix::subMatrix(%oldTrans, 3, 4, 3, 1, 0, 3);

  %newOffset = Matrix::mul(%oldMatrix, 3, 3, %offset, 3, 1);

  %newTrans = %oldMatrix @ " " @ Vector::add(%oldPos, %newOffset);

  if ($Server::AimCorrectionEnabled && !%noAimCorrect) {%newTrans = Matrix::aimCorrect(%player, %newTrans);echo("ACH");}

  Projectile::spawnProjectile(%proj, %newTrans, %player, Item::getVelocity(%player), %target);
}

function Weapon::fireAutoAim(%player, %proj, %target, %offset) {
  %trans = GameBase::getMuzzleTransform(%player);
  %pos = Matrix::subMatrix(GameBase::getMuzzleTransform(%player), 3, 4, 3, 1, 0, 3);

  %vec = Vector::sub(Vector::add(GameBase::getPosition(%target), %offset), %pos);
  %rot = Vector::getRotation(%vec);

  %matrix = Matrix::rotX(-getWord(%rot, 0) - $PI / 2);
  %matrix = Matrix::mul(%matrix, 3, 3, Matrix::rotZ(-getWord(%rot, 2)), 3, 3);

  return Projectile::spawnProjectile(%proj, %matrix @ " " @ %pos, %player, 0);
}

function Weapon::fireAutoAimVel(%player, %proj, %target, %offset, %speed) {
  %trans = GameBase::getMuzzleTransform(%player);
  %pos = Matrix::subMatrix(GameBase::getMuzzleTransform(%player), 3, 4, 3, 1, 0, 3);

  %vec = Vector::sub(Vector::add(GameBase::getPosition(%target), %offset), %pos);
  %len = Vector::getDistance(%vec, 0);
  %vec = Vector::mul(%vec, %speed / %len);

  %proj = Projectile::spawnProjectile(%proj, %trans, %player, %vec);
}

function Weapon::fireAutoAimTarget(%player, %proj, %target, %offset) {
  %trans = GameBase::getMuzzleTransform(%player);
  %pos = Matrix::subMatrix(GameBase::getMuzzleTransform(%player), 3, 4, 3, 1, 0, 3);

  %vec = Vector::sub(Vector::add(GameBase::getPosition(%target), %offset), %pos);
  %rot = Vector::getRotation(%vec);

  %matrix = Matrix::rotX(-getWord(%rot, 0) - $PI / 2);
  %matrix = Matrix::mul(%matrix, 3, 3, Matrix::rotZ(-getWord(%rot, 2)), 3, 3);

  return Projectile::spawnProjectile(%proj, %matrix @ " " @ %pos, %player, 0, %target);
}

function Weapon::linkSlots(%player, %delay, %slot0, %slot1, %slot2, %slot3, %slot4, %slot5, %slot6) {
  for (%i = 0; %i < 7 && %slot[%i]; %i++) Player::trigger(%player, %slot[%i], true);

  schedule("Weapon::checkSlots(" @ %player @ "," @ %delay @ "," @ %slot0 @ ",\"" @ %slot1 @ "\",\"" @ %slot2 @ "\",\"" @
                                   %slot3 @ "\",\"" @ %slot4 @ "\",\"" @ %slot5 @ "\",\"" @ %slot6 @ "\");", %delay);
}

function Weapon::checkSlots(%player, %delay, %slot0, %slot1, %slot2, %slot3, %slot4, %slot5, %slot6) {
  if (!Player::isTriggered(%player, 0))
    for (%i = 0; %i < 7 && %slot[%i]; %i++) Player::trigger(%player, %slot[%i], false);
  else
    schedule("Weapon::checkSlots(" @ %player @ "," @ %delay @ "," @ %slot0 @ ",\"" @ %slot1 @ "\",\"" @ %slot2 @ "\",\"" @
                                     %slot3 @ "\",\"" @ %slot4 @ "\",\"" @ %slot5 @ "\",\"" @ %slot6 @ "\");", %delay);
}

function Weapon::loadClip(%player, %wep) {
  %ammoType = $WeaponAmmo[%wep];
  %num = min(Player::getItemCount(%player, %ammoType), $ClipSize[%wep]);
  %num = min($ClipSize[%wep] - Player::getItemCount(%player, LoadedAmmo), %num);
  Player::decItemCount(%player, %ammoType, %num);
  Player::incItemCount(%player, LoadedAmmo, %num);
  %player.loadedAmmo[%wep] = %num;
  %player.loadedAmmoForGun = %wep;
  %player.ammoLoaded[%ammoType] += %num;
}

function Weapon::loadPartialClip(%player, %wep, %num) {
  %ammoType = $WeaponAmmo[%wep];
  %num = min(Player::getItemCount(%player, %ammoType), %num);
  %num = min($ClipSize[%wep] - Player::getItemCount(%player, LoadedAmmo), %num);
  Player::decItemCount(%player, %ammoType, %num);
  Player::incItemCount(%player, LoadedAmmo, %num);
  %player.loadedAmmo[%wep] = %num;
  %player.loadedAmmoForGun = %wep;
  %player.ammoLoaded[%ammoType] += %num;
}

function Weapon::returnClipAmmo(%player, %wep) {
  if (%player.loadedAmmoForGun != %wep) return;
  %ammoType = $WeaponAmmo[%wep];

  %player.ammoLoaded[%ammoType] -= Player::getItemCount(%player, LoadedAmmo);

  Player::incItemCount(%player, %ammoType, Player::getItemCount(%player, LoadedAmmo));
  Player::setItemCount(%player, LoadedAmmo, 0);
  %player.loadedAmmoForGun = "";
}

function Weapon::standardMount(%player, %wep) {
  if (%player.mountedItem == %wep) return;
  if ($AmmoDisplay[%wep] != "") {
    Player::setItemCount(%player, $AmmoDisplay[%wep], 1);
  }
  if (Weapon::usesClips(%wep)) Weapon::loadPartialClip(%player, %wep, %player.loadedAmmo[%wep]);
  %player.mountedItem = %wep;
}

function Weapon::standardUnmount(%player, %wep) {
  if (%player.mountedItem != %wep) return;
  if ($AmmoDisplay[%wep] != "") {
    Player::setItemCount(%player, $AmmoDisplay[%wep], 0);
  }
  if (Weapon::usesClips(%wep)) {
    %player.loadedAmmo[%wep] = Player::getItemCount(%player, LoadedAmmo);
    Weapon::returnClipAmmo(%player, %wep);
    Player::setItemCount(%player, LoadedAmmo, 0);
    %player.lastUnmount[%wep] = getSimTime();
    %player.reloading[%wep] = "";
  }
  %player.mountedItem = "";

//  Player::unmountItem(%player, 0);
}

function Weapon::standardReload(%player, %wep, %sound, %seconds) {
  if (%player.reloading[%wep]) return;
  %player.reloading[%wep] = true;

  Weapon::returnClipAmmo(%player, %wep);

  %command = %player @ ".reloading["@%wep@"] = false;" @
             "if (getSimTime() - "@%player@".lastUnmount["@%wep@"] > "@%seconds@" && !Player::isDead("@%player@") && Player::getItemCount("@%player@","@$WeaponAmmo[%wep]@") > 0)" @
             "{";

  if (%sound != "") %command = %command @ "GameBase::playSound("@%player@","@%sound@",0);";
  %command = %command @ "Weapon::loadClip("@%player@", "@%wep@"); }";

  schedule(%command, %seconds, %player);
}

function Weapon::standardReload2(%player, %wep, %onReload, %seconds) {
  if (%player.reloading[%wep]) return;
  %player.reloading[%wep] = true;

  Weapon::returnClipAmmo(%player, %wep);

  %command = %player @ ".reloading["@%wep@"] = false;" @
             "if (getSimTime() - "@%player@".lastUnmount["@%wep@"] > "@%seconds@" && !Player::isDead("@%player@") && Player::getItemCount("@%player@","@$WeaponAmmo[%wep]@") > 0)" @
             "{";

  if (%onReload != "") %command = %command @ %onReload;
  %command = %command @ "Weapon::loadClip("@%player@", "@%wep@"); }";

  schedule(%command, %seconds, %player);
}

function Weapon::standardPartialReload(%player, %wep, %sound, %seconds, %num) {
  if (%player.reloading[%wep]) return;
  %player.reloading[%wep] = true;

  %loaded = Player::getItemCount(%player, LoadedAmmo);

  Weapon::returnClipAmmo(%player, %wep);

  %command = %player @ ".reloading["@%wep@"] = false;" @
             "if (getSimTime() - "@%player@".lastUnmount["@%wep@"] > "@%seconds@" && !Player::isDead("@%player@") && Player::getItemCount("@%player@","@$WeaponAmmo[%wep]@") > 0)" @
             "{";

  if (%sound != "") %command = %command @ "GameBase::playSound("@%player@","@%sound@",0);";
  %command = %command @ "Weapon::loadPartialClip("@%player@", "@%wep@", "@(%loaded + %num)@"); }";

  schedule(%command, %seconds, %player);
}

function Weapon::usesClips(%wep) { return ($ClipSize[%wep] != "" && $ClipSize[%wep] > 0); }

function Weapon::displayDescription(%player, %wep) {
  if ($NumModes[%wep] > 0) %desc = $ItemDescription[%wep, Weapon::getMode(%player, %wep)];
  else                     %desc = $ItemDescription[%wep];
  bottomprint(GameBase::getControlClient(%player), %desc, 5);
}

function Weapon::clearDescription(%player, %wep) {
  bottomprint(Player::getClient(%player), "");
}



function Item::respawn(%this) {
	// If the item is rotating we respawn it,
	if (Item::isRotating(%this)) {
		if (%this.randRespawn) {
			%newItem = Item::respawnNow(%this);
                        Item::hide(%newItem, true);

			schedule("Item::hide("@%newItem@", false);GameBase::startFadeIn("@%newItem@");",$ItemRespawnTime,%newItem);
		} else {
			Item::hide(%this,True);
			schedule("Item::hide(" @ %this @ ",false); GameBase::startFadeIn(" @ %this @ ");",$ItemRespawnTime,%this);
		}
	} else { 
		deleteObject(%this);
	}
}

function Item::randomFromSet(%set) {
  %index = floor($DROP_SET[%set] * getRandom());
  return $DROP_SET[%set, %index];
}

function Item::onAdd(%this) {
  if (%this.randRespawn) {
    %this.originalPos = GameBase::getPosition(%this);
    if (%this.respawnNow) schedule("Item::respawnNow("@%this@");",3,%this);
  }
}

function Item::respawnNow(%this) {
	%pos = %this.originalPos;
	%set = %this.itemSet;
	%loadWeps = %this.loadWeapons;
	%item = Item::randomFromSet(%set);
	if (%item.className == Ammo) %count = $SellAmmo[%item];
        else                         %count = 1;

	deleteObject(%this);

	%newItem = newObject("",Item,%item,%count,true,true);
	%newItem.loadWeapons = %loadWeps;

	GameBase::setPosition(%newItem, %pos);
	%newItem.randRespawn = true;
	%newItem.itemSet = %set;
        %newItem.originalPos = %pos;
	if (Weapon::usesClips(%item)) {
          %newItem.loadedAmmo = $SellAmmo[$WeaponAmmo[%item]];
	}

	addToSet(MissionCleanup, %newItem);

	return %newItem;
}



function Grenade::nextGrenadeType(%player) {
  %client = GameBase::getControlClient(%player);
  %client.grenadeType = $NextGrenadeType[%client.grenadeType];
  Weapon::displayDescription(%player, %client.grenadeType);
}


// Limit on number of special Items you can buy
$TeamItemMax[DeployableAmmoPack] = 7;
$TeamItemMax[DeployableInvPack] = 5;
$TeamItemMax[TurretPack] = 10;
$TeamItemMax[CameraPack] = 15;
$TeamItemMax[DeployableSensorJammerPack] = 8;
$TeamItemMax[PulseSensorPack] = 15;
$TeamItemMax[MotionSensorPack] = 15;
$TeamItemMax[ScoutVehicle] = 3;
$TeamItemMax[HAPCVehicle] = 1;
$TeamItemMax[LAPCVehicle] = 2;
$TeamItemMax[Beacon] = 40;
$TeamItemMax[mineammo] = 35;

function Mission::reinitData() {
	for (%i = 0; %i < 8; %i++) {
		$TeamItemCount[%i @ DeployableAmmoPack] = 0;
		$TeamItemCount[%i @ DeployableInvPack] = 0;
		$TeamItemCount[%i @ TurretPack] = 0;
		$TeamItemCount[%i @ CameraPack] = 0;
		$TeamItemCount[%i @ DeployableSensorJammerPack] = 0;
		$TeamItemCount[%i @ PulseSensorPack] = 0;
		$TeamItemCount[%i @ MotionSensorPack] = 0;
		$TeamItemCount[%i @ ScoutVehicle] = 0;
		$TeamItemCount[%i @ LAPCVehicle] = 0;
		$TeamItemCount[%i @ HAPCVehicle] = 0;
		$TeamItemCount[%i @ Beacon] = 0;
		$TeamItemCount[%i @ mineammo] = 0;

		$TeamItemCount[%i @ Camera] = 0;

		$SmokeGrenade::numSmokers[%i] = 0;
	}

	$totalNumCameras = 0;
	$totalNumTurrets = 0;

	for(%i = -1; %i < 8 ; %i++)
		$TeamEnergy[%i] = $DefaultTeamEnergy; 
}

function Item::countPlayerWeapons(%player) {
  return Player::getItemClassCount(%player, "Weapon")
       - Player::getItemCount(%player, Knife)
       + 2*Player::getItemCount(%player, RocketLauncher);
}

function Item::getWeaponCountValue(%item) {
  if (%item == Knife) return 0;
  if (%item == RocketLauncher) return 3;
  return 1;
}

function buyItem(%client,%item) {
	%player = Client::getOwnedObject(%client);
	%armor = Player::getArmor(%client);
	if (($ServerCheats || Client::isItemShoppingOn(%client,%item) || $TestCheats || %client.spawn) && 
			($ItemMax[%armor, %item] || %item.className == Armor || %item.className == Vehicle || $TestCheats)) {
		if (%item.className == Armor) {
			// Assign armor by requested type & gender 
			%buyarmor = $ArmorType[Client::getGender(%client), %item];
			if(%armor != %buyarmor || Player::getItemCount(%client,%item) == 0)	{
				teamEnergyBuySell(%player,$ArmorName[%armor].price);
				if(checkResources(%player,%item,1)) {
					teamEnergyBuySell(%player,$ArmorName[%buyarmor].price * -1);
					Player::setArmor(%client,%buyarmor);
					checkMax(%client,%buyarmor);
					armorChange(%client);
     				Player::setItemCount(%client, $ArmorName[%armor], 0);  
     				Player::setItemCount(%client, %item, 1);  
					if (Player::getMountedItem(%client,$BackpackSlot) == ammopack) 
						fillAmmoPack(%client);	
					return 1;
				}

				teamEnergyBuySell(%player,$ArmorName[%armor].price * -1);
			}
		}
		else if (%item.className == Backpack) {
			if($TeamItemMax[%item] != "") {						
				if($TeamItemCount[Client::getApparentTeam(%client) @ %item] >= $TeamItemMax[%item])
			 	  return 0;
			 }

			// Only one backpack per armor.
			%pack = Player::getMountedItem(%client,$BackpackSlot);
			if (%pack != -1) {
				if(%pack == ammopack) 
					checkMax(%client,%armor);
				else if(%pack == EnergyPack) {
					if(Player::getItemCount(%client,"LaserRifle") > 0) {
						Client::sendMessage(%client,0,"Sold Energy Pack - Auto Selling Laser Rifle");
						remoteSellItem(%client,22);						
					}
				}	
				teamEnergyBuySell(%player,%pack.price);
				Player::decItemCount(%client,%pack);
			}			   
			if (checkResources(%player,%item,1) || $testCheats) {
				teamEnergyBuySell(%player,%item.price * -1);
				Player::incItemCount(%client,%item);
				Player::useItem(%client,%item);									 
				if(%item == ammopack) 
					fillAmmoPack(%client);
				return 1;
			}
			else if(%pack != -1) {
				teamEnergyBuySell(%player,%pack.price * -1);
				Player::incItemCount(%client,%pack);
				Player::useItem(%client,%pack);									 
				if(%pack == ammopack) 
					fillAmmoPack(%client);
			}				 
		}
		else if(%item.className == Weapon) {
			if(checkResources(%player,%item,1)) {
				Player::incItemCount(%client,%item);
				teamEnergyBuySell(%player,(%item.price * -1));
				%ammoItem =  $WeaponAmmo[%item];
				if (%ammoItem != "" && isItemOnInvBuyList(%player, %ammoItem)) {
					if (Weapon::usesClips(%item)) {
						%loadAmmo = tern(%item != GrenadeLauncher2, $ClipSize[%item], 0);
						%player.loadedAmmo[%item] = %loadAmmo;
						%player.ammoLoaded[%ammoItem] += %loadAmmo;
					} else {
						%loadAmmo = 0;
					}
					%delta = checkResources(%player,%ammoItem,$ItemMax[%armor, %ammoItem] + %loadAmmo);
					if(%delta || $testCheats) {
						teamEnergyBuySell(%player,(%ammoItem.price * -1 * %delta));
						Player::incItemCount(%client,%ammoitem,%delta);
					}
				}
				return 1;
			}
		}
	 	else if(%item.className == Vehicle) {
		   if($TeamItemCount[Client::getApparentTeam(%client) @ %item] < $TeamItemMax[%item]) {
				%shouldBuy = VehicleStation::checkBuying(%client,%item);
				if(%shouldBuy == 1) {
					teamEnergyBuySell(%player,(%item.price * -1));
					return 1;
				}			
 				else if(%shouldBuy == 2)
					return 1;
			}
		}
		else {
			if($TeamItemMax[%item] != "") {						
				if($TeamItemCount[Client::getApparentTeam(%client) @ %item] >= $TeamItemMax[%item])
			 	  return 0;
			 }
		    %delta = checkResources(%player,%item,$ItemMax[%armor, %item]);
			 if(%delta || $testCheats) {
				teamEnergyBuySell(%player,(%item.price * -1 * %delta));
				Player::incItemCount(%client,%item,%delta);
				return 1;
			}
		}
		
 	}
	return 0;
}

function Ammo::getLoadedAmmoCount(%player, %ammoType) {
  %loaded = tern(%ammoType.className == Ammo, %player.ammoLoaded[%ammoType], 0);
  %mountedWep = Player::getMountedItem(%player, $WeaponSlot);

  if ($WeaponAmmo[%mountedWep] == %ammoType && %mountedWep != GrenadeLauncher2)
    %loaded -= Player::getItemCount(%player, LoadedAmmo);

  if (%mountedWep == GrenadeLauncher2 &&
      GrenadeLauncher::getAmmoType(Weapon::getMode(%player, GrenadeLauncher2)) == %ammoType)
    %loaded -= Player::getItemCount(%player, LoadedAmmo);

  return %loaded;
}

function checkResources(%player,%item,%delta,%noMessage) {
  %client = Player::getClient(%player);
  %team = Client::getApparentTeam(%client);
  %extraAmmo = 0 ;
  if (Player::getMountedItem(%client,$BackpackSlot) == ammopack && $AmmoPackMax[%item] != "") {
    %extraAmmo = $AmmoPackMax[%item];
    if (%delta == $ItemMax[Player::getArmor(%client), %item]) 
      %delta = %delta + %extraAmmo;
  }
  if ($TestCheats == 0 && %client.spawn == "") {
    %energy = $TeamEnergy[%team];
    %station = %player.Station;
    %sName = GameBase::getDataName(%station);
    if (%sName == DeployableInvStation || %sName == DeployableAmmoStation) {
      %energy = %station.Energy;
    }
    if (%energy != "Infinite") {
      if (%item.price * %delta > %energy)	
        %delta = %energy / %item.price; 
      if (%delta < 1 ) {
        if (%noMessage == "")
          Client::sendMessage(%client,0,"Couldn't buy " @ %item.description @ " - "@ %energy @ " Energy points left");
          return 0;
      }
    }
  }
  if (%item.className == Weapon) {
    %armor = Player::getArmor(%client);
    %wcount = Item::countPlayerWeapons(%player);
    if (%wcount + Item::getWeaponCountValue(%item) > $MaxWeapons[%armor]) {
      Client::sendMessage(%client,0,"Too many weapons to carry");
      return 0;
    }
  } else if (%item.className == Tool) {
    %armor = Player::getArmor(%client);
    %tcount = Player::getItemClassCount(%client,"Tool");
    if (%tcount >= $MaxTools[%armor] && Player::getItemCount(%client, %item) == 0) {
      Client::sendMessage(%client,0,"Too many gadgets to carry");
      return 0;
    }
  } else if(%item == RepairPatch) {
    %pDamage = GameBase::getDamageLevel(%player);
    if (GameBase::getDamageLevel(%player) > 0) 
      return 1;
    return 0;
  } else if($TeamItemMax[%item] != "" && !$TestCheats) {
    if ($TeamItemMax[%item] <= $TeamItemCount[%team, %item]) {
      Client::sendMessage(%client,0,"Deployable Item limit reached for " @ %item.description @ "s");
      return 0;
    }
  }
  if (%item.className != Armor && %item.className != Vehicle) {
    %count = Player::getItemCount(%client,%item);
    %max = $ItemMax[(Player::getArmor(%client)), %item] + %extraAmmo;
    if (%delta + %count >= %max) 
      %delta = %max - %count;
  }

  return max(%delta, 0);
}