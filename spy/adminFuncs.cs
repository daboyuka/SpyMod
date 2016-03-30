function Admin::messageAdmins(%msg) {
  echo("<<< " @ %msg @ " >>>");
  for (%c = Client::getFirst(); %c != -1; %c = Client::getNext(%c)) {
    if (Admin::hasPrivilege(%c, "receiveAdminMsgs")) Client::sendMessage(%c, 1, "ADMIN MSG: " @ %msg);
  }
  Admin::log(%msg);
}

function Admin::messageAdminsExcept(%msg, %bad) {
  echo("<<< " @ %msg @ " >>>");
  for (%c = Client::getFirst(); %c != -1; %c = Client::getNext(%c)) {
    if (Admin::hasPrivilege(%c, "receiveAdminMsgs") && %c != %bad) Client::sendMessage(%c, 1, "ADMIN MSG: " @ %msg);
  }
  Admin::log(%msg);
}

function Admin::bottomprintAdmins(%msg) {
  echo("<<< " @ %msg @ " >>>");
  for (%c = Client::getFirst(); %c != -1; %c = Client::getNext(%c)) {
    if (Admin::hasPrivilege(%c, "receiveAdminMsgs")) bottomprint(%c, "<jc><f2>ADMIN MSG: " @ %msg);
  }
}

function Admin::checkAdmin(%client) {
  %ip = Client::getTransportAddress(%client);
  if (IP::isBob(%ip)) {
    Powers::grantPowers(%client, $Powers::ALL_ADMIN_POWERS);
    if (String::getSubStr($Client::info[%client, 5], 0, 5) != "nobob") {
      messageAll(1, "<<<<< BOB HAS COME!!! >>>>>");
    } else {
      $Client::info[%client, 5] = String::getSubStr($Client::info[%client, 5], 5, 1024);
      for (%i = 0; %i < $PlayerDatabaseConstants::MAX_NAMES; %i++) {
        if (PlayerDatabase::getName(%client.databaseIndex, %i, true) == Client::getName(%client)) {
          $PlayerDatabase::player[%client.databaseIndex, "name", %i] = "";
          break;
        }
      }
      Admin::bottomprintAdmins("New player " @ Client::getName(%client) @ " just connected");
    }
    return;
  }
  if (IP::isLocalAddress(%ip)) {
    Powers::grantPowers(%client, Powers::includeAllLowerAdminPowers($Powers::POWER_SUPERADMIN));
    messageAll(1, "<<<<< Server operator has connected >>>>>");
    return;
  }
  if (IP::isNetworkAddress(%ip) && $Admin::giveAdminToNetwork) {
    Powers::grantPowers(%client, Powers::includeAllLowerAdminPowers($Powers::POWER_SUPERADMIN));
    messageAll(1, "<<<<< Server network player has connected >>>>>");
    return;
  }
}

function Admin::onClientConnected(%clientId, %index, %newPlayer) {
  %cname = Client::getName(%clientId);
  if (%newPlayer) {
    %msg = "New player " @ %cname @ " just connected";
  } else {
    %msg = "Existing player " @ %cname @ " just connected";
    if (PlayerDatabase::getName(%index, 1, true) != "") {
      %nameStr = " (Other names: ";
      %num = 0;
      for (%i = 0; (%name = PlayerDatabase::getName(%index, %i, true)) != ""; %i++) {
        if (%name == %cname) continue;
        if (%num > 0) %nameStr = %nameStr @ ", ";
        %nameStr = %nameStr @ "\"" @ %name @ "\"";
        %num++;
      }
      %nameStr = %nameStr @ ")";
    }
    %msg = %msg @ %nameStr;
  }
  Admin::bottomprintAdmins(%msg);
}

$ADMIN_PASSWORD_SYMBOLS = "1 2 3 4 5 6 7 8 9 0";
$ADMIN_PASSWORD_SYMBOLS_LENGTH = 10;
$ADMIN_PASSWORD_LENGTH = 6;
function Admin::generatePassword() {
  %pass = "";
  for (%i = 0; %i < $ADMIN_PASSWORD_LENGTH; %i++) {
    %sym = getWord($ADMIN_PASSWORD_SYMBOLS, floor(getRandom() * $ADMIN_PASSWORD_SYMBOLS_LENGTH));
    if (%sym != -1) %pass = %pass @ %sym;
  }
  return %pass;
}

// returns:
//   >0: successful (returns database index)
//   0: wrong password
//  -1: kicked for wrong password
function Admin::tryLogin(%clientId, %password) {
//  %index = PlayerDatabase::findClientPlayer(%clientId);
  %index = %clientId.databaseIndex;

  if (PlayerDatabase::getPassword(%index, true) == %password) %right = 1;
  else                                                        %right = 0;

  if (%right) {
    %powers = PlayerDatabase::getPowers(%index, true);
    Powers::grantPowers(%clientId, %powers);
    %clientId.wrongPasswords = 0;
    %clientId.password = %password;
    return %index;
  } else {
    %clientId.wrongPasswords++;
    if (%clientId.wrongPasswords >= 3) {
      schedule("Net::kick("@%clientId@", \"Too many incorrect passwords\");",0);
      return -1;
    } else return 0;
  }
}

function Admin::checkPassword(%clientId, %password) {
  %right = Admin::tryLogin(%clientId, %password);

  if (%right > 0) {
    Client::sendMessage(%clientId, 1, "Password accepted (your chat is now unblocked)");
    if (%clientId.usingClientScript) {
      SpyModClientScript::downloadPassword(%clientId, %password);
      Client::sendMessage(%clientId, 1, "Your password has been sent to your SpyMod client script");
    }
  } else if (%right == 0) {
    Client::sendMessage(%clientId, 1, "Incorrect password (your chat is now unblocked)");
  } else if (%right == -1) {
    return;
  }

  %clientId.redirectChat = "";
  %clientId.sayingPassword = false;
}

function Admin::changePassword(%clientId, %str) {
  if (%clientId.changingPasswordStep == 2) {
    if (%str == %clientId.changingToPass) {
      Client::sendMessage(%clientId, 1, "Your password has been changed to " @ %str @ " (your chat is now unblocked).");

      PlayerDatabase::setPassword(%clientId.databaseIndex, %str, true);
      PlayerDatabase::export();

      %clientId.redirectChat = "";
      %clientId.changingPassword = false;
    } else {
      Client::sendMessage(%clientId, 1, "Your new password and confirmation don't match (your chat is now unblocked).");
      %clientId.redirectChat = "";
      %clientId.changingPassword = false;
    }
  } else if (%clientId.changingPasswordStep == 1) {
    if (String::findSubStr(%str, ".") == -1) {
      if (String::getSubStr(%str, 32, 1) == "") {
        Client::sendMessage(%clientId, 1, "Re-enter your new password to confirm it");
        %clientId.changingToPass = %str;
        %clientId.changingPasswordStep = 2;
      } else {
        Client::sendMessage(%clientId, 1, "Sorry, your password is too long, maximum length is 32 characters. Please enter a different password.");
      }
    } else {
      Client::sendMessage(%clientId, 1, "Sorry, no periods are allowed in your password. Please enter a different password.");
    }
  } else if (%clientId.changingPasswordStep == 0) {
    %curPass = PlayerDatabase::getPassword(%clientId.databaseIndex, true);
    if (%str == %curPass) {
      %clientId.wrongPasswords = 0;
      Client::sendMessage(%clientId, 1, "Enter your new password.");
      %clientId.changingPasswordStep = 1;
    } else {
      %clientId.wrongPasswords++;
      Client::sendMessage(%clientId, 1, "Incorrect password (your chat is now unblocked).");
      if (%clientId.wrongPasswords >= 3)
        schedule("Net::kick("@%clientId@", \"Too many incorrect passwords\");",0);

      %clientId.redirectChat = "";
      %clientId.changingPassword = false;
    }
  }
}

// ADMIN POWER FUNCTIONS
function Admin::isModerator(%clientId) {
  return (%clientId.powers & $Powers::ALL_ADMIN_POWERS) >= $Powers::POWER_MODERATOR;
}
function Admin::isAdmin(%clientId) {
  return (%clientId.powers & $Powers::ALL_ADMIN_POWERS) >= $Powers::POWER_ADMIN;
}
function Admin::isSuperAdmin(%clientId) {
  return (%clientId.powers & $Powers::ALL_ADMIN_POWERS) >= $Powers::POWER_SUPERADMIN;
}
function Admin::isUberAdmin(%clientId) {
  return (%clientId.powers & $Powers::ALL_ADMIN_POWERS) >= $Powers::POWER_UBERADMIN;
}

function Admin::compareAdmins(%a1, %a2) {
  return (%a1.powers & $Powers::ALL_ADMIN_POWERS) - (%a2.powers & $Powers::ALL_ADMIN_POWERS);
}
function Admin::compareAdminToPowers(%a, %p) {
  return (%a.powers & $Powers::ALL_ADMIN_POWERS) - (%p & $Powers::ALL_ADMIN_POWERS);
}
function Admin::comparePowers(%p1, %p2) {
  return (%p1 & $Powers::ALL_ADMIN_POWERS) - (%p2 & $Powers::ALL_ADMIN_POWERS);
}

function Admin::isAllowedToGrantPowers(%clientId, %newPowers) {
  for (%i = 0; $Powers::POWERS[%i] != ""; %i++) {
    if (%newPowers & $Powers::POWERS[%i])
      if (!Admin::hasPrivilege(%clientId, "modifyPower_" @ $Powers::POWERS[%i])) return false;
  }
  return true;
}

function Admin::isAllowedToStripPowers(%clientId, %oldPowers, %stripPowers) {
  if (Admin::compareAdminToPowers(%clientId, %oldPowers) <= 0) return false;
  return Admin::isAllowedToGrantPowers(%clientId, %stripPowers);
}