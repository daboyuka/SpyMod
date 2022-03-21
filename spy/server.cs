// putting a global variable in the argument list means:
// if an argument is passed for that parameter it gets
// assigned to the global scope, not the scope of the function

//$Server::countNetworkConnects = false;
//$Server::logPlayerMode = $Server::EXPORT_EVERY_CONNECT;
//$Server::restartOnNoPlayers = true;

function createTrainingServer() {
   $SinglePlayer = true;
   createServer($pref::lastTrainingMission, false);
}

function remoteSetCLInfo(%clientId, %skin, %name, %email, %tribe, %url, %info, %autowp, %enterInv, %msgMask) {
   $Client::info[%clientId, 0] = %skin;
   $Client::info[%clientId, 1] = %name;
   $Client::info[%clientId, 2] = %email;
   $Client::info[%clientId, 3] = %tribe;
   $Client::info[%clientId, 4] = %url;
   $Client::info[%clientId, 5] = %info;
   if(%autowp)
      %clientId.autoWaypoint = true;
   if(%enterInv)
      %clientId.noEnterInventory = true;
   if(%msgMask != "" && String::getSubStr(%msgMasg, 2, 1) == "")
      %clientId.messageFilter = %msgMask;

  if (getWord($Client::info[%clientId, 5], 0) == "password") {
    Admin::tryLogin(%clientId, getWord($Client::info[%clientId, 5], 1));
  }

  Admin::checkAdmin(%clientId);
}

function Server::storeData() {
   deprecated();
   return;

   $ServerDataFile = "serverTempData" @ $Server::Port @ ".cs";

   $Server::hostname = String::escapeGood($server::hostName);

   export("Server::*", "temp\\" @ $ServerDataFile, False);
   export("pref::lastMission", "temp\\" @ $ServerDataFile, true);

   //exec(defaultSpyOptions);
   //exec(spyOptions);

   Server::storeOptions();

   EvalSearchPath();
}

function Server::refreshData() {
   deprecated();
   return;

   exec($ServerDataFile);  // reload prefs.
   eval("$Server::HostName = \""@$Server::HostName@"\";");

   //exec(defaultSpyOptions);
   //exec(spyOptions);

   Server::loadOptions();

   checkMasterTranslation();
   Server::nextMission(true);//false);
}

function Server::shutdown() {
  PlayerDatabase::export();
  quit();
}

function Server::onClientDisconnect(%clientId) {
	// Need to kill the player off here to make everything
	// is cleaned up properly.
   %player = Client::getOwnedObject(%clientId);
   if(%player != -1 && getObjectType(%player) == "Player" && !Player::isDead(%player)) {
		playNextAnim(%player);
	   Player::kill(%player);
	}

   Client::setControlObject(%clientId, -1);
   Client::leaveGame(%clientId);
   Game::CheckTourneyMatchStart();

   if (getNumClients() == 1 && $Server::restartOnNoPlayers) // this is the last client.
     if ($Server::DefaultMission != "") { Server::loadMission($Server::DefaultMission, true); }
     else                               { Server::nextMission(); }

   BanList::add(Client::getTransportAddress(%clientId), 8);

//   if ($dedicated) IRCReporting::onClientDisconnected(%clientId);
}

function KickDaJackal(%clientId)
{
   Net::kick(%clientId, "The FBI has been notified.  You better buy a legit copy before they get to your house.");
}

function Server::isValidPlayerName(%name) {
  if (%name == "") return false;
  if (String::findSubStr(%name, "<B") != -1) return false;
  if (String::findSubStr(%name, "<F") != -1) return false;
  if (String::findSubStr(%name, "<J") != -1) return false;
  if (String::findSubStr(%name, "<L") != -1) return false;
  if (String::findSubStr(%name, "<N") != -1) return false;
  if (String::findSubStr(%name, "<S") != -1) return false;
  if (String::findSubStr(%name, "<R") != -1) return false;
  return true;
}

function Server::onClientConnect(%clientId) {
//   echo("----------------------------------------------------------------------------------------------------");
//   echo("CONNECT: " @ %clientId @
//              " \"" @ escapeString(Client::getName(%clientId)) @
//              "\" " @ Client::getTransportAddress(%clientId));
//   echo("----------------------------------------------------------------------------------------------------");

   if(Client::getName(%clientId) == "DaJackal")
      schedule("KickDaJackal(" @ %clientId @ ");", 20, %clientId);

   if (!Server::isValidPlayerName(Client::getName(%clientId))) {
     schedule("Net::kick("@%clientId@", \"Sorry, you cannot play with that name. Please remove any greater than or less than signs, and make sure your name has at least 1 character.\");", 0);
     return;
   }

   %clientId.noghost = true;
   %clientId.messageFilter = -1; // all messages
   remoteEval(%clientId, SVInfo, version(), $Server::Hostname, $realModList, $Server::Info, $ItemFavoritesKey);
   remoteEval(%clientId, MODInfo, $Server::MODInfo);
   remoteEval(%clientId, FileURL, $Server::FileURL);

   // clear out any client info:
   for(%i = 0; %i < 10; %i++)
      $Client::info[%clientId, %i] = "";

   Server::log(%clientId);

   Server::initPlayerVars(%clientId);
   Game::onPlayerConnected(%clientId);
   SpyModClientScript::checkClientUsing(%clientId);

   //if ($dedicated) IRCReporting::onClientConnected(%clientId);
}

function Server::log(%clientId) {
   if (getNumClients() > PlayerDatabase::getMostClients(true))
     PlayerDatabase::setMostClients(getNumClients(), true);

   if ($Server::countNetworkConnects ||
       (!IP::isNetworkAddress(Client::getTransportAddress(%clientId)) && !IP::isLocalAddress(Client::getTransportAddress(%clientId))))
     PlayerDatabase::incNumConnects(true);

   %num = PlayerDatabase::getNumPlayers(true);
   %index = PlayerDatabase::logPlayer(%clientId, true, true);
   %clientId.databaseIndex = %index;

   if (PlayerDatabase::getFlags(%index, true) & $Flags::FLAG_BANNED) {
     BanList::add(Client::getTransportAddress(%clientId), 900);
     schedule("Net::kick("@%clientId@", \"You are currently banned from this server.\");", 0);
     return;
   }

   %newPlayer = (PlayerDatabase::getNumPlayers(true) != %num);
   %clientId.isN00b = %newPlayer;

   PlayerDatabase::export();

   Admin::onClientConnected(%clientId, %index, %newPlayer);
}

function Server::initPlayerVars(%clientId) {
  %clientId.lastWeapon = $FirstWeapon;
  %clientId.lastGadget = $FirstGadget;
  %clientId.isAdmin = false;
  %clientId.isSuperAdmin = false;
  %clientId.powers = 0;
  %clientId.grenadeType = $FirstGrenadeType;
  %clientId.connectTime = getIntegerTime(true);

  %clientId.weaponMode[Tornado] = 0;
  %clientId.weaponMode[Challenger] = 0;
  %clientId.weaponMode[GrenadeLauncher2] = 0;
  %clientId.weaponMode[Knife] = 0;
  %clientId.weaponMode[Raider] = 0;
  %clientId.weaponMode[Grappler] = 0;
  %clientId.weaponMode[Shotgun2] = 0;
  %clientId.weaponMode[Flamer] = 0;
}

// createServer() is in spy.cs

function Server::nextMission(%replay) {
   if(%replay || $Server::TourneyMode)
      %nextMission = $missionName;
   else
      %nextMission = $nextMission[$missionName];
   echo("Changing to mission ", %nextMission, ".");
   // give the clients enough time to load up the victory screen

   %nextMission = Server::findValidMission(%nextMission);

   Server::loadMission(%nextMission);
}

function remoteCycleMission(%clientId) {
  if (Admin::isAdmin(%clientId)) {
    messageAll(0, Client::getName(%playerId) @ " cycled the mission.");
    Server::nextMission();
  }
}

function remoteDataFinished(%clientId) {
  if (%clientId.dataFinished)
    return;
  %clientId.dataFinished = true;
  Client::setDataFinished(%clientId);
  %clientId.svNoGhost = ""; // clear the data flag
  if ($ghosting) {
    %clientId.ghostDoneFlag = true; // allow a CGA done from this dude
    startGhosting(%clientId);  // let the ghosting begin!
  }
}

function remoteCGADone(%playerId) {
   if(!%playerId.ghostDoneFlag || !$ghosting)
      return;
   %playerId.ghostDoneFlag = "";

   Game::initialMissionDrop(%playerid);

	if ($cdTrack != "")
		remoteEval (%playerId, setMusic, $cdTrack, $cdPlayMode);
   remoteEval(%playerId, MInfo, $missionName);
}

function Server::isSpyModMissionType(%type) {
  return String::getSubStr($MLIST::Type[%type], 0, 6) == "SpyMod" && ($MLIST::Type[%type] != "SpyModCoOpMission");
}

function Server::isValidMission(%missionNum) {
  if ($Server::EnforceMissionMinMaxPlayers) {
    %mm = $MLIST::EMinMaxPlayers[%missionNum];
    if (%mm != "") {
      %np = 0;
      for (%c = Client::getFirst(); %c != -1; %c = Client::getNext(%c)) 
        if (Client::getTeam(%c) >= 0) %np++;

      if ((getWord(%mm, 0) > %np && getWord(%mm, 0) != -1) || (getWord(%mm, 1) < %np && getWord(%mm, 1) != -1))
        return false;
    }
  }

  return true;
}

function Server::findValidMission(%missionName) {
  if (Server::isValidMission($MLIST::missionNumber[%missionName])) return %missionName;

  for (%m = $nextMission[%missionName]; %m != %missionName; %m = $nextMission[%m]) {
    %i = $MLIST::missionNumber[%m];
    if (Server::isValidMission(%i)) return %m;
  }

  return 0;
}

$waitingForSmokers = false;
function Server::loadMission(%missionName, %immed) {
   if ($loadingMission)
      return;

   if ($lastSmokeNadeTime + 60 > getSimTime() && !$dedicated) {
     messageAll(1, "Match will start soon");
     if (%immed == "") %immed = false;
     schedule("Server::loadMission(\""@%missionName@"\","@%immed@");", $lastSmokeNadeTime + 60.5 - getSimTime());
     $waitingForSmokers = true;
     return;
   }

   $waitingForSmokers = false;
   $lastSmokeNadeTime = -70;

   %missionFile = "missions\\" $+ %missionName $+ ".mis";
   if (File::FindFirst(%missionFile) == "") {
      %missionName = $firstMission;
      %missionFile = "missions\\" $+ %missionName $+ ".mis";
      if (File::FindFirst(%missionFile) == "") {
         echo("invalid nextMission and firstMission...");
         echo("aborting mission load.");
         return;
      }
   }
   for (%cl = Client::getFirst(); %cl != -1; %cl = Client::getNext(%cl)) {
      Client::setGuiMode(%cl, $GuiModeVictory);
      %cl.guiLock = true;
      %cl.nospawn = true;
      remoteEval(%cl, missionChangeNotify, %missionName);
   }

   $loadingMission = true;
   $missionName = %missionName;
   $missionFile = %missionFile;
   $prevNumTeams = getNumTeams();

   deleteObject("MissionGroup");
   deleteObject("MissionCleanup");
//   deleteObject("ConsoleScheduler");
   resetPlayerManager();
   resetGhostManagers();
   resetScheduler();
   $matchStarted = false;
   $countdownStarted = false;
   $ghosting = false;

   resetSimTime(); // deal with time imprecision
   Time::initTime(); // Reinit time to reduce error buildup

   for (%c = Client::getFirst(); %c != -1; %c = Client::getNext(%c)) {
     %c.printLockTill = ""; // Make sure there are no lingering print locks, since they reference the old "SimTime"
   }


//   newObject(ConsoleScheduler, SimConsoleScheduler);
   if (!%immed) {
     schedule("PlayerDatabase::export();", 1);
     schedule("Server::finishMissionLoad();", $Server::victoryScreenTime);
   } else {
     PlayerDatabase::export();
     Server::finishMissionLoad();
   }
}

function Server::finishMissionLoad()
{
   $loadingMission = false;
	$TestMissionType = "";
   // instant off of the manager
   setInstantGroup(0);
   newObject(MissionCleanup, SimGroup);
   newObject(Cameras, SimGroup);
   addToSet(MissionCleanup, Cameras);

   for (%t = 0; %t < $prevNumTeams; %t++)
     %oldTR[%t] = $Mission::teamPlayerRatio[%t];

   Mission::resetMissionVars();

   Extensions::execPremissionExtensions();
   exec($missionFile);
   Extensions::execPostmissionExtensions();

   if ($Mission::TourneyMode) {
     $Mission::prevTourneyMode = $Server::TourneyMode;
     $Server::TourneyMode = true;
   }

   Mission::init();
   Mission::reinitData();
   if($prevNumTeams != getNumTeams()) {
      // loop thru clients and setTeam to -1;
      messageAll(0, "New teamcount - resetting teams.");
      for(%cl = Client::getFirst(); %cl != -1; %cl = Client::getNext(%cl))
         GameBase::setTeam(%cl, -1);
   }

   for (%t = 0; %t < $prevNumTeams; %t++) {
     if (%oldTR[%t] != $Mission::teamPlayerRatio[%t]) {
       messageAll(0, "New team ratios - resetting teams.");
       for (%cl = Client::getFirst(); %cl != -1; %cl = Client::getNext(%cl))
         GameBase::setTeam(%cl, -1);
     }
   }

   $ghosting = true;
   for(%cl = Client::getFirst(); %cl != -1; %cl = Client::getNext(%cl))
   {
      if(!%cl.svNoGhost)
      {
         %cl.ghostDoneFlag = true;
         startGhosting(%cl);
      }
      Client::cancelMenu(%cl); // Cancel all menus, just in case they have teams listed on them that are unjoinable or something similar
      Client::clearApparentTeam(%cl); // Clear all apparent team settings
   }

   if ($SinglePlayer)
     Game::startMatch();
   else if ($Server::warmupTime && !$Server::TourneyMode)
     Server::Countdown($Server::warmupTime);
   else if (!$Server::TourneyMode)
     Game::startMatch();
   else if ($Server::TourneyStartForceTime)
     schedule("Server::forceTourneyStart();", $Server::TourneyStartForceTime);

   $teamplay = (getNumTeams() != 1);
   purgeResources(true);

   for (%c = Client::getFirst(); %c != -1; %c = Client::getNext(%c))
     Game::refreshClientScore(%c);

   // make sure the match happens within 5-10 hours.
   schedule("Server::CheckMatchStarted();", 3600);
   schedule("Server::nextMission();", 18000);
   Ads::scheduleAd();
   
   return "True";
}

function Server::forceTourneyStart() {
  if (!$matchStarted && !$countdownStarted) {
    if (!Game::ForceTourneyMatchStart()) schedule("Server::forceTourneyStart();", $Server::TourneyStartForceTime);
  }
}

function Mission::resetMissionVars() {
  $Mission::coOp = false;
  $Mission::whenYouDieYoureDead = false;
  $Mission::extraInfo = "";
  $Mission::damageMultiplier = 1;

  $Mission::TourneyMode = "";

  if ($Mission::prevTourneyMode != "")
    $Server::TourneyMode = $Mission::prevTourneyMode;

  for (%t = 0; %t < 8; %t++) $Mission::teamPlayerRatio[%t] = 1;

  $Mission::n["OA\x44S"] = false;
}

function Server::CheckMatchStarted()
{
   // if the match hasn't started yet, just reset the map
   // timing issue.
   if(!$matchStarted)
      Server::nextMission(true);
}

function Server::Countdown(%time)
{
   $countdownStarted = true;
   schedule("Game::startMatch();", %time);
   Game::notifyMatchStart(%time);
   if(%time > 30)
      schedule("Game::notifyMatchStart(30);", %time - 30);
   if(%time > 15)
      schedule("Game::notifyMatchStart(15);", %time - 15);
   if(%time > 10)
      schedule("Game::notifyMatchStart(10);", %time - 10);
   if(%time > 5)
      schedule("Game::notifyMatchStart(5);", %time - 5);
}

function Client::setInventoryText(%clientId, %txt)
{
   remoteEval(%clientId, "ITXT", %txt);
}

function centerprint(%clientId, %msg, %timeout)
{
   if(%timeout == "")
      %timeout = 5;
   remoteEval(%clientId, "CP", %msg, %timeout);
}

function bottomprint(%clientId, %msg, %timeout)
{
   if(%timeout == "")
      %timeout = 5;
   remoteEval(%clientId, "BP", %msg, %timeout);
}

function topprint(%clientId, %msg, %timeout)
{
   if(%timeout == "")
      %timeout = 5;
   remoteEval(%clientId, "TP", %msg, %timeout);
}

function centerprintall(%msg, %timeout)
{
   if(%timeout == "")
      %timeout = 5;
   for(%clientId = Client::getFirst(); %clientId != -1; %clientId = Client::getNext(%clientId))
      remoteEval(%clientId, "CP", %msg, %timeout);
}

function bottomprintall(%msg, %timeout)
{
   if(%timeout == "")
      %timeout = 5;
   for(%clientId = Client::getFirst(); %clientId != -1; %clientId = Client::getNext(%clientId))
      remoteEval(%clientId, "BP", %msg, %timeout);
}

function topprintall(%msg, %timeout)
{
   if(%timeout == "")
      %timeout = 5;
   for(%clientId = Client::getFirst(); %clientId != -1; %clientId = Client::getNext(%clientId))
      remoteEval(%clientId, "TP", %msg, %timeout);
}

function onExit() {
   if(isObject(playGui))
      storeObject(playGui, "config\\play.gui");

   saveActionMap("config\\config.cs", "actionMap.sae", "playMap.sae", "pdaMap.sae");

	//update the video mode - since it can be changed with alt-enter
	$pref::VideoFullScreen = isFullScreenMode(MainWindow);

   checkMasterTranslation();
   echo("exporting pref::* to prefs.cs");
   export("pref::*", "config\\ClientPrefs.cs", False);

   $Server::HostName = String::escapeGood($Server::HostName);

   export("Server::*", "config\\ServerPrefs.cs", False);
   export("pref::lastMission", "config\\ServerPrefs.cs", True);
   BanList::export("config\\banlist.cs");
   echo("Exporting player database...");
   PlayerDatabase::export();
   echo("Done, shutting down.");
}