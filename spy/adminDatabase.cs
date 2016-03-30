function Menu::databaseAdminMenu(%clientId) {
  if (!Admin::hasPrivilege(%clientId, "databaseAdmin")) {
    //H4XX0R::hackerDetected(%clientId, "Attempting to access superadmin-only feature: database administration", $H4XX0RPunishment::adminOnlyAccess);
    return;
  }
  if (%clientId.curDatabaseIndex != "") {
    if (%clientId.curDatabaseIndex >= PlayerDatabase::getNumPlayers(true) ||
        %clientId.curDatabaseIndex < 0 || %clientId.curDatabaseIndex != floor(%clientId.curDatabaseIndex) ||
        (%clientId.curDatabaseIndex == 0 && %clientId.curDatabaseIndex != "0")) {
      H4XX0R::hackerDetected(%clientId, "Querying invalid database index", "kick");
      return;
    }
  }

  if (!Client::isReadyForNewMenu(%clientId)) return;
  Client::usedMenu(%clientId);

  %curItem = 0;

  if (%clientId.curDatabaseIndex == "")
    Client::buildMenu(%clientId, "Database administration", "databaseAdminOptions", true);
  else
    Client::buildMenu(%clientId, "Database administration: " @ %clientId.curDatabaseIndex, "databaseAdminOptions", true);

  Client::addMenuItem(%clientId, %curItem++ @ "New search", "newindex");

  if (%clientId.curDatabaseIndex != "")
     Client::addMenuItem(%clientId, %curItem++ @ "Next record", "nextindex");

  if (%clientId.curDatabaseIndex != "") {
    Client::addMenuItem(%clientId, %curItem++ @ "Info", "info " @ %clientId.curDatabaseIndex);

    if (Admin::hasPrivilege(%clientId, "databaseManage"))
      Client::addMenuItem(%clientId, %curItem++ @ "Manage", "manage " @ %clientId.curDatabaseIndex);

    schedule("DatabaseAdmin::listIndexInfo("@%clientId@");", 0.2);
  }
}

function DatabaseAdmin::listIndexInfo(%clientId) {
  %index = %clientId.curDatabaseIndex;

  %nNames = 0;
  %nIPs = 0;

  %line[1] = "Database index " @ %index @ " info:";

  for (%i = 1; %i < $PlayerDatabaseConstants::MAX_NAMES; %i++) {
    %name = PlayerDatabase::getName(%index, %i, true);
    if (%name == "") break;
    %nNames++;
  }
  %line[2] = "Names: " @ PlayerDatabase::getName(%index, 0, true) @ tern(%nNames > 0, " (" @ %nNames @ " more)", "");

  for (%i = 1; %i < $PlayerDatabaseConstants::MAX_IPS; %i++) {
    %ip = PlayerDatabase::getIP(%index, %i, true);
    if (%ip == "") break;
    %nIPs++;
  }
  %line[3] = "IPs: " @ PlayerDatabase::getIP(%index, 0, true) @ tern(%nIPs > 0, " (" @ %nIPs @ " more)", "");

  %line[4] = "Powers: " @ Powers::getNames(Powers::simplifyAdminPowers(PlayerDatabase::getPowers(%index, true)));

  %line[5] = "Other info: " @ Flags::getNames(PlayerDatabase::getFlags(%index, true));

  for (%i = 1; %i <= 5; %i++)
    remoteEval(%clientId, "setInfoLine", %i, %line[%i]);
}

function processMenuDatabaseAdminOptions(%clientId, %option) {
  if (!Admin::hasPrivilege(%clientId, "databaseAdmin")) {
    //H4XX0R::hackerDetected(%clientId, "Attempting to access superadmin-only feature: database administration", $H4XX0RPunishment::adminOnlyAccess);
    return;
  }

  %opt = getWord(%option, 0);
  %index = getWord(%option, 1);
  %arg = getWord(%option, 2);

  if (%index == "") %index = -1;

  if (%arg != "db" && %arg != "ip" && %arg != -1 && %arg != floor(%arg)) {
    H4XX0R::hackerDetected(%clientId, "Invalid menu item value", $H4XX0RPunishment::menuHack);
    return;
  }

  if (%index != -1) {
    // %x == floor(%x) also catches code appended to the index
    if (%index >= PlayerDatabase::getNumPlayers(true) || %index < 0 || %index != floor(%index) ||
        (%index == 0 && %index != "0")) {
      H4XX0R::hackerDetected(%clientId, "Querying invalid database index", $H4XX0RPunishment::menuHack);
      return;
    }
  }

  if (%opt == "refresh") {
    Menu::databaseAdminMenu(%clientId);
    return;
  }

  // INDEX SEARCH

  if (%opt == "newindex") {
    if (!Client::isReadyForNewMenu(%clientId)) return;
    Client::usedMenu(%clientId);
    %curItem = 0;
    Client::buildMenu(%clientId, "New search", "databaseAdminOptions", true);
    Client::addMenuItem(%clientId, %curItem++ @ "Search by name", "searchName");
    Client::addMenuItem(%clientId, %curItem++ @ "Search by IP", "searchIP");
    Client::addMenuItem(%clientId, %curItem++ @ "Search all", "searchAll");
  }
  if (%opt == "searchName") {
    clearPrintLock(%clientId);
    bottomprint(%clientId, "<jc><f1>Say something in the name of the player you are seaching for (your chat is blocked)");
    %clientId.redirectChat = "DatabaseAdmin::searchForNameChatInput";
    return;
  }
  if (%opt == "searchIP") {
    clearPrintLock(%clientId);
    bottomprint(%clientId, "<jc><f1>Say something in the IP of the player you are seaching for (your chat is blocked)");
    %clientId.redirectChat = "DatabaseAdmin::searchForIPChatInput";
    return;
  }
  if (%opt == "searchAll") {
    DatabaseAdmin::searchAll(%clientId);
  }
  if (%opt == "nextindex") {
    DatabaseAdmin::searchNext(%clientId);
  }

  // LIST INFO

  if (%opt == "info" && %index != -1) {
    if (!Client::isReadyForNewMenu(%clientId)) return;
    Client::usedMenu(%clientId);
    %curItem = 0;
    Client::buildMenu(%clientId, "Info on index " @ %index, "databaseAdminOptions", true);
    Client::addMenuItem(%clientId, %curItem++ @ "List names", "listnames " @ %index @ " 0");
    Client::addMenuItem(%clientId, %curItem++ @ "List IPs", "listips " @ %index @ " 0");
    Client::addMenuItem(%clientId, %curItem++ @ "Back", "refresh");
    return;
  }

  if (%opt == "listnames" && %index != -1) {
    if (!Client::isReadyForNewMenu(%clientId)) return;
    Client::usedMenu(%clientId);
    %curItem = 0;

    if (%arg == -1) %arg = 0;

    %theEnd = false;

    Client::buildMenu(%clientId, "List of names for index " @ %index, "databaseAdminOptions", true);
    for (%i = %arg; %i < $PlayerDatabaseConstants::MAX_NAMES && %i-%arg<6; %i++) {
      %iname = PlayerDatabase::getName(%index, %i, true);
      if (%iname == "") { %theEnd = true; break; }
      Client::addMenuItem(%clientId, %curItem++ @ %iname, "listnames " @ %index @ " " @ %arg);
    }

    if (%i == $PlayerDatabaseConstants::MAX_NAMES || %theEnd) {
      if (%arg != 0) Client::addMenuItem(%clientId, %curItem++ @ "<<< Back to start",  "listnames " @ %index @ " 0");
    } else {
      Client::addMenuItem(%clientId, %curItem++ @ "More >>>", "listnames " @ %index @ " " @ %i);
    }

    Client::addMenuItem(%clientId, %curItem++ @ "<<< Back", "info " @ %index);
    return;
  }
  if (%opt == "listips" && %index != -1) {
    if (!Client::isReadyForNewMenu(%clientId)) return;
    Client::usedMenu(%clientId);
    %curItem = 0;

    if (%arg == -1) %arg = 0;

    %theEnd = false;

    Client::buildMenu(%clientId, "List of names for index " @ %index, "databaseAdminOptions", true);
    for (%i = %arg; %i < $PlayerDatabaseConstants::MAX_IPS && %i-%arg<6; %i++) {
      %iip = PlayerDatabase::getIP(%index, %i, true);
      if (%iip == "") { %theEnd = true; break; }
      Client::addMenuItem(%clientId, %curItem++ @ %iip, "listips " @ %index @ " " @ %arg);
    }


    if (%i == $PlayerDatabaseConstants::MAX_IPS || %theEnd) {
      if (%arg != 0) Client::addMenuItem(%clientId, %curItem++ @ "<<< Back to start",  "listips " @ %index @ " 0");
    } else {
      Client::addMenuItem(%clientId, %curItem++ @ "More >>>", "listips " @ %index @ " " @ %i);
    }

    Client::addMenuItem(%clientId, %curItem++ @ "<<< Back", "info " @ %index);
    return;
  }

  if (%opt == "manage" && %index != -1) {
    if (!Admin::hasPrivilege(%clientId, "databaseManage")) return;
    Menu::databaseAdminManageMenu(%clientId);
  }

  Menu::databaseAdminMenu(%clientId);
}

function Menu::databaseAdminManageMenu(%clientId) {
  if (!Client::isReadyForNewMenu(%clientId)) return;
  Client::usedMenu(%clientId);

  if (!Admin::hasPrivilege(%clientId, "databaseManage")) return;

  %index = %clientId.curDatabaseIndex;

  if (%index == "") %index = -1;
  if (%index != -1) {
    // %x == floor(%x) also catches code appended to the index
    if (%index >= PlayerDatabase::getNumPlayers(true) || %index < 0 || %index != floor(%index) ||
        (%index == 0 && %index != "0")) {
      H4XX0R::hackerDetected(%clientId, "Querying invalid database index", $H4XX0RPunishment::menuHack);
      return;
    }
  } else {
    Menu::databaseAdminMenu(%clientId);
    return;
  }

  %curItem = 0;
  Client::buildMenu(%clientId, "Manage index " @ %index, "databaseAdminManageOptions", true);

  if (Admin::hasPrivilege(%clientId, "modifyPowers"))
    Client::addMenuItem(%clientId, %curItem++ @ "Manage powers", "managepowers " @ %clientId.curDatabaseIndex);

  if (Admin::hasPrivilege(%clientId, "changePassword") && PlayerDatabase::getPassword(%index, true) != "" &&
      (Admin::compareAdminToPowers(%clientId, PlayerDatabase::getPowers(%index, true)) > 0 || %clientId.databaseIndex == %index)) {
    Client::addMenuItem(%clientId, %curItem++ @ "Change password", "setpassword " @ %clientId.curDatabaseIndex);
  }

  if (Admin::hasPrivilege(%clientId, "ban")) {
    Client::addMenuItem(%clientId, %curItem++ @ "Ban", "ban " @ %clientId.curDatabaseIndex);
    Client::addMenuItem(%clientId, %curItem++ @ "Unban", "unban " @ %clientId.curDatabaseIndex);
  }

  Client::addMenuItem(%clientId, %curItem++ @ "Back", "refresh");
  schedule("DatabaseAdmin::listIndexInfo("@%clientId@");", 0.05);
}







function processMenuDatabaseAdminManageOptions(%clientId, %option) {
  if (%option == "refresh") {
    Menu::databaseAdminMenu(%clientId);
    return;
  }

  %opt = getWord(%option, 0);
  %index = getWord(%option, 1);
  %arg = getWord(%option, 2);

  %dbBanned = PlayerDatabase::getFlags(%index, true) & $Flags::FLAG_BANNED;

  if (%opt == "ban" && %index != -1) {
    if (%arg == -1) {
      if (!Client::isReadyForNewMenu(%clientId)) return;
      Client::usedMenu(%clientId);
      %curItem = 0;
      Client::buildMenu(%clientId, "Ban player at index " @ %index, "databaseAdminManageOptions", true);
      if (!%dbBanned) Client::addMenuItem(%clientId, %curItem++ @ "Database perma ban", "ban " @ %index @ " db");
      Client::addMenuItem(%clientId, %curItem++ @ "IP-only time ban", "ban " @ %index @ " ip");
      Client::addMenuItem(%clientId, %curItem++ @ "Cancel", "refresh");
      return;
    } else if (%arg == "db" && !%dbBanned) {
      if (!%dbBanned) DatabaseAdmin::databaseBan(%index, Client::getName(%clientId));
    } else if (%arg == "ip") {
      if (!Client::isReadyForNewMenu(%clientId)) return;
      Client::usedMenu(%clientId);
      %curItem = 0;
      Client::buildMenu(%clientId, "Select an IP to ban", "ipBanIpSelect", true);
      for (%i = 0; %i < $PlayerDatabaseConstants::MAX_IPS; %i++) {
        %iip = PlayerDatabase::getIP(%index, %i, true);
        if (%iip == "") break;
        Client::addMenuItem(%clientId, %curItem++ @ %iip, %index @ " " @ %i);
      }
      Client::addMenuItem(%clientId, %curItem++ @ "All", %index @ " all");
      Client::addMenuItem(%clientId, %curItem++ @ "Cancel", "refresh");
      return;
    }
  }
  if (%opt == "unban" && %index != -1) {
    if (%arg == -1) {
      if (!Client::isReadyForNewMenu(%clientId)) return;
      Client::usedMenu(%clientId);
      %curItem = 0;
      Client::buildMenu(%clientId, "Unban player at index " @ %index, "databaseAdminManageOptions", true);
      if (%dbBanned)
        Client::addMenuItem(%clientId, %curItem++ @ "Database unban", "unban " @ %index @ " db");
      Client::addMenuItem(%clientId, %curItem++ @ "IP-only unban", "unban " @ %index @ " ip");
      Client::addMenuItem(%clientId, %curItem++ @ "Cancel", "refresh");
      return;
    } else if (%arg == "db") {
      if (%dbBanned) DatabaseAdmin::databaseUnban(%index, Client::getName(%clientId));
    } else if (%arg == "ip") {
      Client::buildMenu(%clientId, "Select an IP to unban", "ipUnbanIpSelect", true);
      for (%i = 0; %i < $PlayerDatabaseConstants::MAX_IPS; %i++) {
        %iip = PlayerDatabase::getIP(%index, %i, true);
        if (%iip == "") break;
        Client::addMenuItem(%clientId, %curItem++ @ %iip, %index @ " " @ %i);
      }
      Client::addMenuItem(%clientId, %curItem++ @ "All", %index @ " all");
      Client::addMenuItem(%clientId, %curItem++ @ "Cancel", "refresh");
      return;
    }
  }
  if (%opt == "managepowers" && %index != -1) {
    if (!Client::isReadyForNewMenu(%clientId)) return;
    Client::usedMenu(%clientId);
    %curItem = 0;
    Client::buildMenu(%clientId, "Manage powers for index " @ %index, "manageDatabasePowers", true);
    %powers = PlayerDatabase::getPowers(%index, true);
    for (%i = 0; $Powers::POWERS[%i] != ""; %i++) {
      %p = $Powers::POWERS[%i];
      if (%powers & %p) {
        if (Admin::isAllowedToStripPowers(%clientId, %powers, %p)) {
          %strip = tern(%p & $Powers::ALL_ADMIN_POWERS, (((1<<10)-%p)&$Powers::ALL_ADMIN_POWERS&%powers), %p);
          Client::addMenuItem(%clientId, %curItem++ @ "Strip " @ $Powers::POWER_NAMES[%p] @ " and higher", %index @ " " @ -%strip);
        }
      } else {
        if (Admin::isAllowedToGrantPowers(%clientId, %p)) {
          Client::addMenuItem(%clientId, %curItem++ @ "Grant " @ $Powers::POWER_NAMES[%p], %index @ " " @ %p);
        }
      }
    }
    Client::addMenuItem(%clientId, %curItem++ @ "Cancel", "refresh");
    return;
  }
  if (%opt == "setpassword" && %index != -1) {
    clearPrintLock(%clientId);
    bottomprint(%clientId, "<jc><f1>Enter the new password for database index " @ %index @ ", or say \"cancel\" to quit (your chat is blocked)");
    %clientId.redirectChat = "DatabaseAdmin::setPasswordChatInput";
  }
  Menu::databaseAdminManageMenu(%clientId);
}

function processMenuIpBanIpSelect(%clientId, %option) {
  if (!Client::isReadyForNewMenu(%clientId)) return;
  Client::usedMenu(%clientId);

  if (%option == "refresh") {
    Menu::databaseAdminMenu(%clientId);
    return;
  }

  %index = getWord(%option, 0);
  %opt = getWord(%option, 1);

  if (%index >= PlayerDatabase::getNumPlayers(true) || %index < 0 || %index != floor(%index) ||
      (%index == 0 && %index != "0")) {
    H4XX0R::hackerDetected(%clientId, "Querying invalid database index", $H4XX0RPunishment::menuHack);
    return;
  }

  if (%opt != "all" && %opt != floor(%opt)) {
    H4XX0R::hackerDetected(%clientId, "Invalid menu item value", $H4XX0RPunishment::menuHack);
    return;
  }

  %curItem = 0;

  Client::buildMenu(%clientId, "Select ban time", "ipBanTimeSelect", true);
  Client::addMenuItem(%clientId, %curItem++ @ "30 Minutes", %index @ " " @ %opt @ " " @ (30*60));
  Client::addMenuItem(%clientId, %curItem++ @ "1 Hour"    , %index @ " " @ %opt @ " " @ (60*60));
  Client::addMenuItem(%clientId, %curItem++ @ "1 Day"     , %index @ " " @ %opt @ " " @ (24*60*60));
  Client::addMenuItem(%clientId, %curItem++ @ "1 Week"    , %index @ " " @ %opt @ " " @ (7*24*60*60));
  Client::addMenuItem(%clientId, %curItem++ @ "200 years" , %index @ " " @ %opt @ " 200years");
  Client::addMenuItem(%clientId, %curItem++ @ "Cancel", "refresh");
}

function processMenuIpBanTimeSelect(%clientId, %option) {
  if (%option == "refresh") {
    Menu::databaseAdminMenu(%clientId);
    return;
  }

  %index = getWord(%option, 0);
  %opt = getWord(%option, 1);
  %arg = getWord(%option, 2);

  if (%index >= PlayerDatabase::getNumPlayers(true) || %index < 0 || %index != floor(%index) ||
      (%index == 0 && %index != "0")) {
    H4XX0R::hackerDetected(%clientId, "Querying invalid database index", $H4XX0RPunishment::menuHack);
    return;
  }

  if (%opt != "all" && (%opt != floor(%opt) || (%opt < 0 || PlayerDatabase::getIP(%index, %opt) == ""))) {
    H4XX0R::hackerDetected(%clientId, "Invalid menu item value", $H4XX0RPunishment::menuHack);
    return;
  }

  if (%arg != "200years" && %arg != floor(%arg)) {
    H4XX0R::hackerDetected(%clientId, "Invalid menu item value", $H4XX0RPunishment::menuHack);
    return;
  }
  if (%arg == "200years") %arg = 200*365*24*60*60;

  DatabaseAdmin::ipBan(%index, %opt, %arg, Client::getName(%clientId));
  Menu::databaseAdminManageMenu(%clientId);
}

function processMenuIpUnbanIpSelect(%clientId, %option) {
  if (%option == "refresh") {
    Menu::databaseAdminMenu(%clientId);
    return;
  }

  %index = getWord(%option, 0);
  %opt = getWord(%option, 1);

  if (%index >= PlayerDatabase::getNumPlayers(true) || %index < 0 || %index != floor(%index) ||
      (%index == 0 && %index != "0")) {
    H4XX0R::hackerDetected(%clientId, "Querying invalid database index", $H4XX0RPunishment::menuHack);
    return;
  }

  if (%opt != "all" && (%opt != floor(%opt) || (%opt < 0 || PlayerDatabase::getIP(%index, %opt, true) == ""))) {
    H4XX0R::hackerDetected(%clientId, "Invalid menu item value", $H4XX0RPunishment::menuHack);
    return;
  }

  DatabaseAdmin::ipUnban(%index, %opt, Client::getName(%clientId));
  Menu::databaseAdminManageMenu(%clientId);
}

function processMenuManageDatabasePowers(%clientId, %option) {
  if (%option == "refresh") {
    Menu::databaseAdminManageMenu(%clientId);
    return;
  }

  %index = getWord(%option, 0);
  %powers = getWord(%option, 1);

  if (!Powers::areValidPowers(abs(%powers))) {
    H4XX0R::hackerDetected(%clientId, "Attempting to grant\\strip non-existant powers", "kick");
    return;
  }

  if (%powers > 0) {
    DatabaseAdmin::permaGrantPowers(%index, %powers, %clientId);
  } else if (%powers < 0) {
    DatabaseAdmin::permaStripPowers(%index, -%powers, %clientId);
  }

  Menu::databaseAdminManageMenu(%clientId);
}





function DatabaseAdmin::setPasswordChatInput(%clientId, %newPass) {
  %clientId.redirectChat = "";
  if (%newPass == "cancel") return;

  if (String::getSubStr(%str, 32, 1) != "") {
    Client::sendMessage(%clientId, 1, "Sorry, the password is too long, maximum length is 32 characters. Please enter a different password.");
  }
  if (String::findSubStr(%str, ".") != -1) {
    Client::sendMessage(%clientId, 1, "Sorry, no periods are allowed in the password. Please enter a different password.");
  }

  PlayerDatabase::setPassword(%clientId.curDatabaseIndex, %newPass, true);
  Client::sendMessage(%clientId, 1, "Password for database index " @ %clientId.curDatabaseIndex @ " has been set to " @ %newPass);
}

function DatabaseAdmin::searchForNameChatInput(%clientId, %name) {
  %clientId.redirectChat = "";
  DatabaseAdmin::searchForName(%clientId, %name, 0);
}

function DatabaseAdmin::searchForIPChatInput(%clientId, %ip) {
  %clientId.redirectChat = "";
  DatabaseAdmin::searchForIP(%clientId, %ip, 0);
}

function DatabaseAdmin::searchForName(%clientId, %name, %start) {
  if (%start == "") %start = 0;
  %clientId.lastDatabaseSearchName = %name;
  %clientId.lastDatabaseSearchIP = "";

  %index = PlayerDatabase::findPlayerLike(%start, %name, "", true);
  if (%index == -1) {
    clearPrintLock(%clientId);
    bottomprint(%clientId, "<jc><f1>No records were found with \"" @ %name @ "\" in the name field");
  } else {
    %clientId.curDatabaseIndex = %index;
    clearPrintLock(%clientId);
    bottomprint(%clientId, "<jc><f1>Search result for \"" @ %name @ "\" in the name field: index " @ %index);
  }
  Menu::databaseAdminMenu(%clientId);
}

function DatabaseAdmin::searchForIP(%clientId, %ip, %start) {
  if (%start == "") %start = 0;
  %clientId.lastDatabaseSearchName = "";
  %clientId.lastDatabaseSearchIP = %ip;

  %index = PlayerDatabase::findPlayerLike(%start, "", %ip, true);
  if (%index == -1) {
    clearPrintLock(%clientId);
    bottomprint(%clientId, "<jc><f1>No records were found with \"" @ %ip @ "\" in the IP field");
  } else {
    %clientId.curDatabaseIndex = %index;
    clearPrintLock(%clientId);
    bottomprint(%clientId, "<jc><f1>Search result for \"" @ %ip @ "\" in the IP field: index " @ %index);
  }
  Menu::databaseAdminMenu(%clientId);
}

function DatabaseAdmin::searchAll(%clientId) {
  %clientId.curDatabaseIndex = 0;
  %clientId.lastDatabaseSearchName = "";
  %clientId.lastDatabaseSearchIP = "";

  clearPrintLock(%clientId);
  bottomprint(%clientId, "<jc><f1>Starting at index 0");

  Menu::databaseAdminMenu(%clientId);
}

function DatabaseAdmin::nextDatabaseIndex(%clientId) {
  if (%clientId.curDatabaseIndex + 1 >= PlayerDatabase::getNumPlayers(true)) {
    clearPrintLock(%clientId);
    bottomprint(%clientId, "<jc><f1>End of database");
  } else {
    %clientId.curDatabaseIndex++;
    clearPrintLock(%clientId);
    bottomprint(%clientId, "<jc><f1>Next record: index " @ %clientId.curDatabaseIndex);
  }
  Menu::databaseAdminMenu(%clientId);
}

function DatabaseAdmin::searchNext(%clientId) {
  %start = %clientId.curDatabaseIndex + 1;
  if (%clientId.lastDatabaseSearchName != "") {
    if (%clientId.lastDatabaseSearchIP != "") {
    } else {
      DatabaseAdmin::searchForName(%clientId, %clientId.lastDatabaseSearchName, %start);
    }
  } else if (%clientId.lastDatabaseSearchIP != "") {
  } else {
    DatabaseAdmin::nextDatabaseIndex(%clientId);
  }
}







function DatabaseAdmin::findClientFromIndex(%index) {
  for (%cl = Client::getFirst(); %cl != -1; %cl = Client::getNext(%cl)) {
    if (%cl.databaseIndex == %index) return %cl;
  }
  return -1;
}

function DatabaseAdmin::databaseBan(%index, %adminName) {
  PlayerDatabase::addFlags(%index, $Flags::FLAG_BANNED, true);

  %client = DatabaseAdmin::findClientFromIndex(%index);
  if (%client != -1) {
    BanList::add(Client::getTransportAddress(%client), 30);
    schedule("Net::kick("@%client@", \"You were permanently banned by " @ %adminName @ "\");", 0);
  }
  Admin::messageAdmins(%adminName @ " database-banned database index " @ %index @ ".");

  //PlayerDatabase::export();
}

function DatabaseAdmin::databaseUnban(%index, %adminName) {
  PlayerDatabase::removeFlags(%index, $Flags::FLAG_BANNED, true);
  Admin::messageAdmins(%adminName @ " database-unbanned database index " @ %index @ ".");

  //PlayerDatabase::export();
}

function DatabaseAdmin::ipBan(%index, %ipIndex, %time, %adminName) {
  if (%time == -1) %time = 7*24*60*60; // Ban for a week

  if (%ipIndex == "all") {
    for (%i = 0; %i < $PlayerDatabaseConstants::MAX_IPS; %i++) {
      %ip = PlayerDatabase::getIP(%index, %i, true);
      if (%ip == "") break;
      BanList::add("IP:" @ %ip @ ":*", %time);
    }
  } else {
    BanList::add("IP:" @ PlayerDatabase::getIP(%index, %ipIndex, true) @ ":*", %time);
  }
  BanList::export("config\\banlist.cs");

  %client = DatabaseAdmin::findClientFromIndex(%index);
  if (%client != -1) {
    Net::kick(%client, "You were banned by " @ %adminName);
  }

  Admin::messageAdmins(%adminName @ " ip-banned database index " @ %index @ ", ip index " @ %ipIndex @ " for " @ %time @ " seconds.");
}

function DatabaseAdmin::ipUnban(%index, %ipIndex, %adminName) {
  if (%ipIndex == "all") {
    for (%i = 0; %i < $PlayerDatabaseConstants::MAX_IPS; %i++) {
      %ip = PlayerDatabase::getIP(%index, %i, true);
      if (%ip == "") break;
      BanList::remove("IP:" @ %ip @ ":*");
    }
  } else {
    BanList::remove("IP:" @ PlayerDatabase::getIP(%index, %ipIndex, true) @ ":*");
  }
  BanList::export("config\\banlist.cs");

  Admin::messageAdmins(%adminName @ " ip-unbanned database index " @ %index @ ", ip index " @ %ipIndex @ ".");
}

function DatabaseAdmin::permaGrantPowers(%index, %newPowers, %admin, %indexName) {
  if (%newPowers == 0) {
    return;
  }
  if (!Powers::areValidPowers(%newPowers)) {
    H4XX0R::hackerDetected(%admin, "Attempting to grant non-existant powers", "kick");
    return;
  }

  %oldPowers = PlayerDatabase::getPowers(%index, true);
  %newPowers = Powers::includeAllLowerAdminPowers(%newPowers) & ~%oldPowers;

  %nameOfPowers = Powers::getNames(%newPowers);

  if (!Admin::isAllowedToGrantPowers(%admin, %newPowers)) {
    Client::sendMessage(%admin, 1, "You are not a high enough level admin to grant " @ %nameOfPowers @ ".");
    return;
  }

  if (%indexName == "") {
    if (PlayerDatabase::getName(%index, 1, true) == "") %indexName = PlayerDatabase::getName(%index, 0, true);
    else %indexName = "database index " @ %index;
  }

  Admin::messageAdminsExcept(Client::getName(%admin) @ " granted " @ %indexName @ " " @ %nameOfPowers @ ".", %admin);
  Client::sendMessage(%admin, 1, "You granted " @ %indexName @ " " @ %nameOfPowers @ ".");

  %clientId = DatabaseAdmin::findClientFromIndex(%index);

  if (PlayerDatabase::getPassword(%index, true) == "") {
    %password = Admin::generatePassword();
    PlayerDatabase::setPassword(%index, %password, true);
    if (%clientId != -1) {
      if (%clientId.usingClientScript) {
        SpyModClientScript::downloadPassword(%clientId, %password);
        clearPrintLock(%clientId);
        centerprint(%clientId, "<jc>You have been granted <f1>" @ %nameOfPowers @ "<f0>.\n\n" @
                               "Your password is <f1>" @ %password @ "<f0>.\n\n" @
                               "Your password has been downloaded to your SpyMod Client Script, and will\n" @
                               "automatically be used every time you connect.", 45, true);
        Client::sendMessage(%clientId, 1, "Your password is " @ %password);
      } else {
        clearPrintLock(%clientId);
        centerprint(%clientId, "<jc>You have been granted <f1>" @ %nameOfPowers @ "<f0>.\n\n" @
                               "Your password is <f1>" @ %password @ "<f0>.\n\n" @
                               "Write this password down. Press Tab then Personal Options then Help then\n" @
                               "Password Help for info on how to log in.", 45, true);// @
        Client::sendMessage(%clientId, 1, "Your password is " @ %password);
      }
    } else {
      Client::sendMessage(%admin, 1, %indexName @ " is not connected, so give his password to him. His password is " @ %password);
    }
  } else {
    if (%clientId != -1) {
      clearPrintLock(%clientId);
      centerprint(%clientId, "<jc>You have been granted <f1>" @ %nameOfPowers @ "<f0>.\n" @
                             "You should continue to use the same password.", 45, true);// @
    }
  }

  PlayerDatabase::addPowers(%index, %newPowers, true);
  if (%clientId != -1) Powers::grantPowers(%clientId, %newPowers);

//  PlayerDatabase::export();
}

function DatabaseAdmin::permaStripPowers(%index, %stripPowers, %admin, %indexName) {
  if (%stripPowers == 0) return;
  if (!Powers::areValidPowers(%stripPowers)) {
    H4XX0R::hackerDetected(%admin, "Attempting to strip non-existant powers", "kick");
    return;
  }

  %oldPowers = PlayerDatabase::getPowers(%index, true);
  %nameOfPowers = Powers::getNames(%stripPowers);

  %stripPowers &= %oldPowers;

  if (%indexName == "") {
    if (PlayerDatabase::getName(%index, 1, true) == "") %indexName = PlayerDatabase::getName(%index, 0, true);
    else %indexName = "database index " @ %index;
  }

  if (!Admin::isAllowedToStripPowers(%admin, %oldPowers, %stripPowers)) {
    Client::sendMessage(%admin, 1, "You are not a high enough level admin to strip " @ %nameOfPowers @ ", or the target is a higher level admin than you.");
    return;
  }

  %targetPowers = (%oldPowers & $Powers::ALL_ADMIN_POWERS) & ~(%stripPowers & $Powers::ALL_ADMIN_POWERS);
  if (Powers::includeAllLowerAdminPowers(%targetPowers) != %targetPowers) {
    Client::sendMessage(%admin, 1, %indexName @ " has a higher admin level than you are attempting to strip. You cannot strip intermediate levels of admin.");
    return;
  }

  %clientId = DatabaseAdmin::findClientFromIndex(%index);

  if (%clientId != -1) Powers::stripPowers(%clientId, %stripPowers);
  PlayerDatabase::removePowers(%index, %stripPowers, true);

  Admin::messageAdminsExcept(Client::getName(%admin) @ " stripped " @ %indexName @ " of " @ %nameOfPowers @ ".", %admin);
  Client::sendMessage(%admin, 1, "You stripped " @ %indexName @ " of " @ %nameOfPowers @ ".");

  if (PlayerDatabase::getPowers(%index, true) == 0) {
    PlayerDatabase::setPassword(%index, "", true);
    if (%clientId != -1) {
      clearPrintLock(%clientId);
      centerprint(%clientId, "<jc>You have been stripped of <f1>" @ %nameOfPowers @ "<f0> by " @ Client::getName(%admin) @ ".\n" @
                             "As you have no powers left, your password has also been erased.", 45, true);
    }
  } else {
    if (%clientId != -1) {
      clearPrintLock(%clientId);
      centerprint(%clientId, "<jc>You have been stripped of <f1>" @ %nameOfPowers @ "<f0> by " @ Client::getName(%admin) @ ".", 45, true);
    }
  }

//  PlayerDatabase::export();
}