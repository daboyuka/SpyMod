function Game::playerSpawned(%pl, %clientId, %armor) {						  
  if (getNumTeams() == 1) Client::setSkin(%clientId, $Client::info[%clientId, 0]);

  if (%clientId.invis) GameBase::startFadeOut(%pl);

  if (Player::isAIControlled(%pl)) return;

  %clientId.spawn = 1;

  for(%i = 0; (%str = $spawnItems[%i]) != ""; %i++) {
    %item = getWord(%str, 1);
    %amt = getWord(%str, 0);

    if (%item.className == "Weapon") { %weps[%numweps++ - 1] = %item; continue; }

    Player::setItemCount(%pl, %item, %amt);
    if ($ClipSize[%item] != "" && $ClipSize[%item] > 0)
      %pl.loadedAmmo[%item] = min($ClipSize[%item], Player::getItemCount(%pl, $WeaponAmmo[%item]));
    if (%item.className == "Backpack" && Player::getMountedItem(%pl, $BackpackSlot) == -1) Player::useItem(%pl, %item);
  }

  Player::setItemCount(%pl, LoadedAmmoDisplay, 1);

  %pl.halfDamageTill = getSimTime() + $HalfDamageTime;
  %clientId.spawn = "";
  if (%clientId.lastWeapon != "") {
    schedule("Player::useItem("@%pl@","@%clientId.lastWeapon@");",0.1);
    %clientId.spawnWeapon = "";
  } else if (%clientId.lastGadget != "") {
    schedule("Player::useItem("@%pl@","@%clientId.lastGadget@");",0.1);
    %clientId.spawnWeapon = "";
  } else if (%clientId.spawnWeapon != "") {
    Player::useItem(%pl,%clientId.spawnWeapon);	
    %clientId.spawnWeapon = "";
  }
}