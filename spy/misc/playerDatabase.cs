$PlayerDatabaseConstants::DATABASE_PATH = "config\\";
$PlayerDatabaseConstants::DATABASE_FILE = "database.cs";
$PlayerDatabaseConstants::DATABASE_LOOKUP_PATH = "config\\";
$PlayerDatabaseConstants::DATABASE_LOOKUP_FILE = "databaseLookup.cs";
$PlayerDatabaseConstants::DOWNLOADED_DATABASE_PATH = "config\\";
$PlayerDatabaseConstants::DOWNLOADED_DATABASE_FILE = "dledDatabase.cs";
$PlayerDatabaseConstants::DOWNLOADED_DATABASE_LOOKUP_PATH = "config\\";
$PlayerDatabaseConstants::DOWNLOADED_DATABASE_LOOKUP_FILE = "dledDatabaseLookup.cs";

//$PlayerDatabaseConstants::MAX_NAMES = 5;
//$PlayerDatabaseConstants::MAX_IPS = 3;

//$PlayerDatabaseOptions::compressOnExit = false;
//$PlayerDatabaseOptions::displayStatsPause = 8;
//$PlayerDatabaseOptions::displayStatsPageSize = 50;
//$PlayerDatabaseOptions::acceleratedLookups = true;

// $PlayerDatabase::numPlayers
// $PlayerDatabase::numConnects
// $PlayerDatabase::mostClients
// $PlayerDatabase::player[%index, "name", %nameIndex]
// $PlayerDatabase::player[%index, "ip", %ipIndex]
// $PlayerDatabase::player[%index, "password"]
// $PlayerDatabase::player[%index, "powers"]
// $PlayerDatabase::player[%index, "flags"]

// $PlayerDatabaseLookup::name[%name, %nameIndex, "realName"]
// $PlayerDatabaseLookup::name[%name, %nameIndex]
// $PlayerDatabaseLookup::ip[%ip]

// General database functions

function PlayerDatabase::initDatabase() {
  $PlayerDatabase::numPlayers = 0;
  $PlayerDatabase::numConnects = 0;
  $PlayerDatabase::mostClients = 0;
}

function PlayerDatabase::initPlayer(%index) {
  $PlayerDatabase::player[%index, "name", 0] = "";
  $PlayerDatabase::player[%index, "ip", 0] = "";

  //$PlayerDatabase::player[%index, "password"] = "";
  //$PlayerDatabase::player[%index, "powers"] = 0;
}

function PlayerDatabase::import() {
  exec($PlayerDatabaseConstants::DATABASE_FILE);
  if ($PlayerDatabaseOptions::acceleratedLookups) exec($PlayerDatabaseConstants::DATABASE_LOOKUP_FILE);
}

function PlayerDatabase::importDownloaded() {
  exec($PlayerDatabaseConstants::DOWNLOADED_DATABASE_FILE);
  if ($PlayerDatabaseOptions::acceleratedLookups) exec($PlayerDatabaseConstants::DOWNLOADED_DATABASE_LOOKUP_FILE);
}

function PlayerDatabase::export() {
  export("$PlayerDatabase::*", $PlayerDatabaseConstants::DATABASE_PATH @ $PlayerDatabaseConstants::DATABASE_FILE, false);
  if ($PlayerDatabaseOptions::acceleratedLookups) export("$PlayerDatabaseLookup::*", $PlayerDatabaseConstants::DATABASE_LOOKUP_PATH @ $PlayerDatabaseConstants::DATABASE_LOOKUP_FILE, false);
}

function PlayerDatabase::exportDownloaded() {
  export("$PlayerDatabase::*", $PlayerDatabaseConstants::DOWNLOADED_DATABASE_PATH @ $PlayerDatabaseConstants::DOWNLOADED_DATABASE_FILE, false);
  if ($PlayerDatabaseOptions::acceleratedLookups) export("$PlayerDatabaseLookup::*", $PlayerDatabaseConstants::DOWNLOADED_DATABASE_LOOKUP_PATH @ $PlayerDatabaseConstants::DOWNLOADED_DATABASE_LOOKUP_FILE, false);
}

// Log functions

function PlayerDatabase::logPlayer(%client, %dontLoadDatabase, %dontLogImmed) {
  if (File::findFirst($PlayerDatabaseConstants::DATABASE_FILE) == "") {
    PlayerDatabase::initDatabase();
  } else if (!%dontLoadDatabase) {
    //PlayerDatabase::import();
  }

  %name = Client::getName(%client);
  %ip = Client::getTransportAddress(%client);
  %ipStr = IP::getAddress(%ip);

  %index = PlayerDatabase::findPlayer(%name, %ipStr, true);

  if (%index != -1) { // he's not new, just add the new name or ip if there is one
    PlayerDatabase::updatePlayer(%index, %name, %ipStr, true);
  } else { // he's new, init and log him
    %index = $PlayerDatabase::numPlayers;
    PlayerDatabase::initPlayer($PlayerDatabase::numPlayers);
    PlayerDatabase::updatePlayer($PlayerDatabase::numPlayers, %name, %ipStr, true);
    $PlayerDatabase::numPlayers++;
  }

  //if (!%dontLogImmed)
    //PlayerDatabase::export();

  return %index;
}

function PlayerDatabase::updatePlayer(%index, %name, %ip, %dontLoadDatabase) {
  //if (!%dontLoadDatabase) PlayerDatabase::import();

  %ipKnown = !($PlayerDatabase::player[%index, "name", $PlayerDatabaseConstants::MAX_NAMES - 1] == "");
  %nameKnown = !($PlayerDatabase::player[%index, "ip", $PlayerDatabaseConstants::MAX_IPS - 1] == "");

  if (!%nameKnown)
    for (%i = 0; $PlayerDatabase::player[%index, "name", %i] != ""; %i++)
      if (%name == $PlayerDatabase::player[%index, "name", %i]) { %nameKnown = true; break; }

  if (!%ipKnown)
    for (%i = 0; $PlayerDatabase::player[%index, "ip", %i] != ""; %i++)
      if (IP::areEqual(%ip, $PlayerDatabase::player[%index, "ip", %i])) { %ipKnown = true; break; }

  if (!%ipKnown)
    PlayerDatabase::addIP(%index, %ip, true);
  if (!%nameKnown)
    PlayerDatabase::addName(%index, %name, true);
}

// Accessor and mutator functions

function PlayerDatabase::addName(%index, %name, %dontLoadDatabase) {
  //if (!%dontLoadDatabase) PlayerDatabase::import();
  for (%i = 0; %i < $PlayerDatabaseConstants::MAX_NAMES; %i++)
    if ($PlayerDatabase::player[%index, "name", %i] == "") break;

  if (%i == $PlayerDatabaseConstants::MAX_NAMES) return;

  $PlayerDatabase::player[%index, "name", %i] = %name;

  if ($PlayerDatabaseOptions::acceleratedLookups) PlayerDatabaseLookup::registerName(%name, %index, true);
}

function PlayerDatabase::getName(%index, %index2, %dontLoadDatabase) {
  //if (!%dontLoadDatabase) PlayerDatabase::import();
  return $PlayerDatabase::player[%index, "name", %index2];
}

function PlayerDatabase::addIP(%index, %ip, %dontLoadDatabase) {
  //if (!%dontLoadDatabase) PlayerDatabase::import();
  for (%i = 0; %i < $PlayerDatabaseConstants::MAX_IPS; %i++)
    if ($PlayerDatabase::player[%index, "ip", %i] == "") break;

  if (%i == $PlayerDatabaseConstants::MAX_IPS) return;

  $PlayerDatabase::player[%index, "ip", %i] = %ip;

  if ($PlayerDatabaseOptions::acceleratedLookups) PlayerDatabaseLookup::registerIP(%ip, %index, true);
}

function PlayerDatabase::getIP(%index, %index2, %dontLoadDatabase) {
  //if (!%dontLoadDatabase) PlayerDatabase::import();
  return $PlayerDatabase::player[%index, "ip", %index2];
}

function PlayerDatabase::addPowers(%index, %power, %dontLoadDatabase) {
  //if (!%dontLoadDatabase) PlayerDatabase::import();
  $PlayerDatabase::player[%index, "powers"] |= %power;
}

function PlayerDatabase::removePowers(%index, %power, %dontLoadDatabase) {
  //if (!%dontLoadDatabase) PlayerDatabase::import();
  $PlayerDatabase::player[%index, "powers"] &= ~%power;
}

function PlayerDatabase::getPowers(%index, %dontLoadDatabase) {
  //if (!%dontLoadDatabase) PlayerDatabase::import();
  if ($PlayerDatabase::player[%index, "powers"] == "") return 0;
  return $PlayerDatabase::player[%index, "powers"];
}

function PlayerDatabase::setPowers(%index, %powers, %dontLoadDatabase) {
  //if (!%dontLoadDatabase) PlayerDatabase::import();
  $PlayerDatabase::player[%index, "powers"] = %powers;
}

function PlayerDatabase::getPassword(%index, %dontLoadDatabase) {
  //if (!%dontLoadDatabase) PlayerDatabase::import();
  return $PlayerDatabase::player[%index, "password"];
}

function PlayerDatabase::setPassword(%index, %password, %dontLoadDatabase) {
  //if (!%dontLoadDatabase) PlayerDatabase::import();
  $PlayerDatabase::player[%index, "password"] = %password;
}

function PlayerDatabase::addFlags(%index, %flags, %dontLoadDatabase) {
  //if (!%dontLoadDatabase) PlayerDatabase::import();
  $PlayerDatabase::player[%index, "flags"] |= %flags;
}

function PlayerDatabase::removeFlags(%index, %flags, %dontLoadDatabase) {
  //if (!%dontLoadDatabase) PlayerDatabase::import();
  $PlayerDatabase::player[%index, "flags"] &= ~%flags;
}

function PlayerDatabase::getFlags(%index, %dontLoadDatabase) {
  //if (!%dontLoadDatabase) PlayerDatabase::import();
  if ($PlayerDatabase::player[%index, "flags"] == "") return 0;
  return $PlayerDatabase::player[%index, "flags"];
}

function PlayerDatabase::setFlags(%index, %flags, %dontLoadDatabase) {
  //if (!%dontLoadDatabase) PlayerDatabase::import();
  $PlayerDatabase::player[%index, "flags"] = %flags;
}

function PlayerDatabase::getMostClients(%dontLoadDatabase) {
  //if (!%dontLoadDatabase) PlayerDatabase::import();
  return $PlayerDatabase::mostClients;
}

function PlayerDatabase::setMostClients(%num, %dontLoadDatabase) {
  //if (!%dontLoadDatabase) PlayerDatabase::import();
  $PlayerDatabase::mostClients = %num;
}

function PlayerDatabase::getNumConnects(%dontLoadDatabase) {
  //if (!%dontLoadDatabase) PlayerDatabase::import();
  return $PlayerDatabase::numConnects;
}

function PlayerDatabase::incNumConnects(%dontLoadDatabase) {
  //if (!%dontLoadDatabase) PlayerDatabase::import();
  $PlayerDatabase::numConnects++;
}

function PlayerDatabase::getNumPlayers(%dontLoadDatabase) {
  //if (!%dontLoadDatabase) PlayerDatabase::import();
  return $PlayerDatabase::numPlayers;
}

// ---- Database lookup acceleration functions ----

function PLayerDatabaseLookup::convertName(%name) {
  %newName = "";
  for (%i = 0; (%c = String::getSubStr(%name, %i, 1)) != ""; %i++) {
    %x = char(%c);
    //  |     digits and :   |    | upper case letters |    |  lower case letters |    |   _   |
    if ((%x >= 48 && %x <= 58) || (%x >= 65 && %x <= 90) || (%x >= 97 && %x <= 122) || %x == 95) %newName = %newName @ %c;
    else %newName = %newName @ ":";
  }
  return %newName;
}

function PlayerDatabaseLookup::registerName(%name, %index, %dontLoadDatabase) {
  //if (!%dontLoadDatabase) PlayerDatabase::import();

  %name2 = PlayerDatabaseLookup::convertName(%name);//String::convertSpaces(%name);
  for (%i = 0; %i < (1 << 16); %i++) {
    if ($PlayerDatabaseLookup::name[%name2, %i, "realName"] == %name) break;
    if ($PlayerDatabaseLookup::name[%name2, %i, "realName"] == "") break;
  }
  echo("Registering ", %name, "->", %index);
  $PlayerDatabaseLookup::name[%name2, %i, "realName"] = %name;
  $PlayerDatabaseLookup::name[%name2, %i] = %index;
}

function PlayerDatabaseLookup::registerIP(%ip, %index, %dontLoadDatabase) {
  //if (!%dontLoadDatabase) PlayerDatabase::import();

  echo("Registering ", %ip, "->", %index);

  %ip2 = String::replace(%ip, ".", ":");
  $PlayerDatabaseLookup::ip[%ip2] = %index;
}

function PlayerDatabaseLookup::findPlayer(%name, %ip, %dontLoadDatabase) {
  //if (!%dontLoadDatabase) PlayerDatabase::import();

  %ip2 = String::replace(%ip, ".", ":");

  %name2 = PlayerDatabaseLookup::convertName(%name);

  %index = -1;

  for (%i = 0; %i < (1 << 16); %i++) {
    if ($PlayerDatabaseLookup::name[%name2, %i, "realName"] == %name) {
      %index = $PlayerDatabaseLookup::name[%name2, %i];
      break;
    }

    if ($PlayerDatabaseLookup::name[%name2, %i, "realName"] == "") break;
  }

  if ($PlayerDatabaseLookup::ip[%ip2] != "") {
    if (%index == -1) %index = $PlayerDatabaseLookup::ip[%ip2];
    else %index = min(%index, $PlayerDatabaseLookup::ip[%ip2]);
  }

  return %index;
}

// ---- Database search functions ----

function PlayerDatabase::findPlayer(%name, %ip, %dontLoadDatabase) {
  //if (!%dontLoadDatabase) PlayerDatabase::import();

  if ($PlayerDatabaseOptions::acceleratedLookups) {
    return PlayerDatabaseLookup::findPlayer(%name, %ip, true);
  } else {
    for (%i = 0; %i < $PlayerDatabase::numPlayers; %i++) {
      if (PlayerDatabase::isSamePlayer(%i, %name, %ip, true)) {
        return %i;
      }
    }
    return -1;
  }
}

function PlayerDatabase::findClientPlayer(%clientId, %dontLoadDatabase) {
  return PlayerDatabase::findPlayer(Client::getName(%clientId), IP::getAddress(Client::getTransportAddress(%clientId)), %dontLoadDatabase);
}

function PlayerDatabase::isSamePlayer(%index, %name, %ip, %dontLoadDatabase) {
  //if (!%dontLoadDatabase) PlayerDatabase::import();

  if (%name != "")
    for (%i = 0; $PlayerDatabase::player[%index, "name", %i] != ""; %i++)
      if (%name == $PlayerDatabase::player[%index, "name", %i]) return true;

  if (%ip != "")
    for (%i = 0; $PlayerDatabase::player[%index, "ip", %i] != ""; %i++)
      if (IP::areEqual(%ip, $PlayerDatabase::player[%index, "ip", %i])) return true;

  return false;
}

function PlayerDatabase::findPlayerLike(%start, %name, %ip, %dontLoadDatabase) {
  //if (!%dontLoadDatabase) PlayerDatabase::import();

  for (%i = %start; %i < $PlayerDatabase::numPlayers; %i++) {
    if (PlayerDatabase::isLikePlayer(%i, %name, %ip, true)) {
      return %i;
    }
  }

  return -1;
}

function PlayerDatabase::findPlayersLike(%start, %name, %ip, %dontLoadDatabase) {
  //if (!%dontLoadDatabase) PlayerDatabase::import();

  %results = -1;
  %numResults = 0;

  for (%i = %start; %i < $PlayerDatabase::numPlayers; %i++) {
    if (PlayerDatabase::isLikePlayer(%i, %name, %ip, true)) {
      %results = %results @ " " @ %i;
      %numResults++;
    }
  }

  return %numResults @ %results;
}

function PlayerDatabase::isLikePlayer(%index, %name, %ip, %dontLoadDatabase) {
  //if (!%dontLoadDatabase) PlayerDatabase::import();

  if (%ip != "")
    for (%i = 0; $PlayerDatabase::player[%index, "ip", %i] != ""; %i++)
      if (String::findSubStr($PlayerDatabase::player[%index, "ip", %i], %ip) != -1) return true;

  if (%name != "")
    for (%i = 0; $PlayerDatabase::player[%index, "name", %i] != ""; %i++)
      if (String::findSubStr($PlayerDatabase::player[%index, "name", %i], %name) != -1) return true;

  return false;
}

// ---- displayStats() ----

function displayStats(%showNames, %showMultiNames, %showIPs, %showMultiIPs, %downloaded) {
  if (%showNames == "") %showNames = true;
  if (%showMultiNames == "") %showMultiNames = true;
  if (%showIPs == "") %showIPs = true;
  if (%showMultiIPs == "") %showMultiIPs = true;

  PlayerDatabase::export();

  if (%downloaded) PlayerDatabase::importDownloaded();

  %numPlayers = PlayerDatabase::getNumPlayers(true);
  echo("*---------------------------------------------");
  echo("|   SpyMod server statistics   ");
  echo("*---------------------------------------------");
  echo("|   General statistics");
  echo("|     > Most clients ever:    \t" @ PlayerDatabase::getMostClients(true));
  echo("|     > Total players logged: \t" @ %numPlayers);
  echo("|     > Total connects:       \t" @ PlayerDatabase::getNumConnects(true));
  echo("*---------------------------------------------");

  if (!%showNames && !%showIPs) return;
  for (%i = 0; %i < %numPlayers; %i += $PlayerDatabaseOptions::displayStatsPageSize) {
    schedule("displayPlayers("@%i@", "@min(%i+$PlayerDatabaseOptions::displayStatsPageSize-1, %numPlayers - 1)@"," @
             %showNames@","@%showMultiNames@","@%showIPs@","@%showMultiIPs@");",
             %i / $PlayerDatabaseOptions::displayStatsPageSize * $PlayerDatabaseOptions::displayStatsPause);
  }
  schedule("echo(\"*---------------------------------------------\");", floor((%numPlayers - 1) / $PlayerDatabaseOptions::displayStatsPageSize) * $PlayerDatabaseOptions::displayStatsPause);
}

function displayPlayers(%a, %b, %showNames, %showMultiNames, %showIPs, %showMultiIPs) {
  for (%i = %a; %i <= %b; %i++) displayPlayer(%i, %showNames, %showMultiNames, %showIPs, %showMultiIPs);
}

function displayPlayer(%index, %showNames, %showMultiNames, %showIPs, %showMultiIPs) {
  if (%showNames) {
    %nameStr = "|   Name: " @ PlayerDatabase::getName(%index, 0, true);
    if (%showMultiNames && PlayerDatabase::getName(%index, 1, true) != "") {
      %nameStr = %nameStr @ " (";
      for (%i = 1; (%name = PlayerDatabase::getName(%index, %i, true)) != ""; %i++) {
        if (%i > 1) %nameStr = %nameStr @ ", ";
        %nameStr = %nameStr @ %name;
      }
      %nameStr = %nameStr @ ")";
    }
    echo(%nameStr);
  }

  if (%showIPs) {
    %ipStr   = "|   IP:   " @ PlayerDatabase::getIP(%index, 0, true);
    if (%showMultiIPs && PlayerDatabase::getIP(%index, 1, true) != "") {
      %ipStr = %ipStr @ " (";
      for (%i = 1; (%ip = PlayerDatabase::getIP(%index, %i, true)) != ""; %i++) {
        if (%i > 1) %ipStr = %ipStr @ ", ";
        %ipStr = %ipStr @ %ip;
      }
      %ipStr = %ipStr @ ")";
    }
    echo(%ipStr);
  }
}

// remoteGetStats

function comment() {
function remoteGetStats(%clientId, %numNames, %numIPs) {
  PlayerDatabase::import();
  %numPlayers = PlayerDatabase::getNumPlayers(true);
  remoteEval(%clientId, "stat", "mostClients", PlayerDatabase::getMostClients(true));
  remoteEval(%clientId, "stat", "numPlayers", %numPlayers);
  remoteEval(%clientId, "stat", "numConnects", PlayerDatabase::getNumConnects(true));

  for (%i = 0; %i < %numPlayers; %i++) {
    for (%j = 0; (%name = PlayerDatabase::getName(%i, %j, true)) != "" && %j < %numNames; %j++)
      remoteEval(%clientId, "stat", "player" @ %i @ "_name_" @ %j, %name);

    for (%j = 0; (%ip = PlayerDatabase::getIP(%i, %j, true)) != "" && %j < %numIPs; %j++)
      remoteEval(%clientId, "stat", "player" @ %i @ "_ip_" @ %j, %ip);
  }
  remoteEval(%clientId, "stat", "done");
}
}






function PlayerDatabase::removeName(%index, %index2) {
  %name = PlayerDatabase::getName(%index, %index2, true);
  %cname = PlayerDatabaseLookup::convertName(%name);

  %m = 0;
  for (%i = 0; %i < $PlayerDatabaseConstants::MAX_NAMES; %i++) {
    if ($PlayerDatabase::player[%index, "name", %i] == "") break;
    if (%i == %index2) $PlayerDatabase::player[%index, "name", %i] = "";
    if (%i > %index2) {
      $PlayerDatabase::player[%index, "name", %i-1] = $PlayerDatabase::player[%index, "name", %i];
      $PlayerDatabase::player[%index, "name", %i] = "";
    }
  }

  for (%i = 0; %i < (1 << 16); %i++) {
    if ($PlayerDatabaseLookup::name[%cname, %i, "realName"] == "") break;
    if (String::ICompare($PlayerDatabaseLookup::name[%cname, %i, "realName"], %name) == 0) {
      %i2 = %i;
      $PlayerDatabaseLookup::name[%cname, %i2, "realName"] = "";
      $PlayerDatabaseLookup::name[%cname, %i2] = "";
    }
    if (%i2 != "") {
      $PlayerDatabaseLookup::name[%cname, %i-1, "realName"] = $PlayerDatabaseLookup::name[%cname, %i, "realName"];
      $PlayerDatabaseLookup::name[%cname, %i, "realName"] = "";
      $PlayerDatabaseLookup::name[%cname, %i-1] = $PlayerDatabaseLookup::name[%cname, %i];
      $PlayerDatabaseLookup::name[%cname, %i] = "";
    }
  }

  %tmp = $PlayerDatabaseOptions::acceleratedLookups;
  $PlayerDatabaseOptions::acceleratedLookups = "";
  if ((%newIndex = PlayerDatabase::findPlayer(%name, "", true)) != -1) {
    echo("REGISTERING " @ %name @ " TO INDEX " @ %newIndex);
    PlayerDatabaseLookup::registerName(%name, %newIndex, true);
  }
  $PlayerDatabaseOptions::acceleratedLookups = %tmp;
}

function PlayerDatabase::removeIP(%index, %index2) {
  %ip = PlayerDatabase::getIP(%index, %index2, true);
  %cip = String::replace(%ip, ".", ":");

  %m = 0;
  for (%i = 0; %i < $PlayerDatabaseConstants::MAX_IPS; %i++) {
    if ($PlayerDatabase::player[%index, "ip", %i] == "") break;
    if (%i == %index2) $PlayerDatabase::player[%index, "ip", %i] = "";
    if (%i > %index2) {
      $PlayerDatabase::player[%index, "ip", %i-1] = $PlayerDatabase::player[%index, "ip", %i];
      $PlayerDatabase::player[%index, "ip", %i] = "";
    }
  }

  $PlayerDatabaseLookup::ip[%cip] = "";

  %tmp = $PlayerDatabaseOptions::acceleratedLookups;
  $PlayerDatabaseOptions::acceleratedLookups = "";
  if ((%newIndex = PlayerDatabase::findPlayer("", %ip, true)) != -1) {
    echo("REGISTERING " @ %ip @ " TO INDEX " @ %newIndex);
    PlayerDatabaseLookup::registerIP(%ip, %newIndex, true);
  }
  $PlayerDatabaseOptions::acceleratedLookups = %tmp;
}