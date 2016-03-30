//$SDM::numLives

// Team Stock Deathmatch - Two or more teams attempt to eliminate eachother. As soon as only one team is left standing, that
//                         team immediately wins the mission, regardless of any other remaining objectives.
//

//---------------------------------------------------------------------------------------
//
//Team Stock Deathmatch function definitions
//
//---------------------------------------------------------------------------------------
function Game::refreshClientScore(%clientId) {
  %livesRemaining = $SDM::numLives - %clientId.scoreDeaths - 1; // - 1 because you are using a life at all times (even when you are dead, you are using the next one :D)
  %team = Client::getApparentTeam(%clientId);

  if (%livesRemaining >= 0 && %team >= 0) %livesStr = %livesRemaining;
  else if (%clientId.elim) %livesStr = "ELIM";
  else %livesStr = "";

  if (%team < 0) %team = 9;

  Client::setScore(%clientId, "%n\t" @								    // Name
                              "%t\t" @							            // Team Name
                              %livesStr @ "\t" @						    // Lives
                              "%p",								    // Pingz0r
                              %livesRemaining + 10000*%team);
}

// TEAM: Player Name - Team - Score
// FFA:  Player Name - Num Lives Left - Place - Ping
function Mission::init() {
   setClientScoreHeading("Player Name\t\x55Team Name\t\xC0Lives\t\xE4Ping");
   setTeamScoreHeading("Team Name\t\xD6Score");

   $firstTeamLine = 7 + tern($Mission::extraInfo != "", 1, 0); // Changed to make room for $Mission::extraInfo
   $firstObjectiveLine = $firstTeamLine + getNumTeams() + 1;
   for (%i = -1; %i < getNumTeams(); %i++) {
      $teamFlagStand[%i] = "";
		$teamFlag[%i] = "";
      Team::setObjective(%i, $firstTeamLine - 1, " ");
      Team::setObjective(%i, $firstObjectiveLine - 1, " ");
      Team::setObjective(%i, $firstObjectiveLine, "<f5>Mission Objectives: ");
      $firstObjectiveLine++;
		$deltaTeamScore[%i] = 0;
      $teamScore[%i] = 0;
      newObject("TeamDrops" @ %i, SimSet);
      addToSet(MissionCleanup, "TeamDrops" @ %i);
      %dropSet = nameToID("MissionGroup/Teams/Team" @ %i @ "/DropPoints/Random");
      for(%j = 0; (%dropPoint = Group::getObject(%dropSet, %j)) != -1; %j++)
         addToSet("MissionCleanup/TeamDrops" @ %i, %dropPoint);
   }
   $numObjectives = 0;
   newObject(ObjectivesSet, SimSet);
   addToSet(MissionCleanup, ObjectivesSet);
   
   Group::iterateRecursive(MissionGroup, ObjectiveMission::initCheck);
   %group = nameToID("MissionCleanup/ObjectivesSet");

   ObjectiveMission::setObjectiveHeading();
   for (%i = 0; (%obj = Group::getObject(%group, %i)) != -1; %i++) {
      %obj.objectiveLine = %i + $firstObjectiveLine;
      ObjectiveMission::objectiveChanged(%obj);
   }

   News::showNews(%i + $firstObjectiveLine);

   ObjectiveMission::refreshTeamScores();
   for (%cl = Client::getFirst(); %cl != -1; %cl = Client::getNext(%cl)) {
      %cl.score = 0;
      Game::refreshClientScore(%cl);
   }
   schedule("ObjectiveMission::checkPoints();", 5);

   if ($TestMissionType == "") {
     if ($NumTowerSwitchs)
       $TestMissionType = "C&H";
     else
       $TestMissionType = "NONE";
     $NumTowerSwitchs = "";
   }
}

function Game::resetScores(%client) {
  if (%client == "") {
    for (%cl = Client::getFirst(); %cl != -1; %cl = Client::getNext(%cl)) {
      %cl.scoreKills = 0;
      %cl.scoreDeaths = 0;
      %cl.ratio = 0;
      %cl.score = 0;
      %cl.place = 0;
      %cl.elim = false;
      %cl.totalDeaths = 0;
    }
  } else {
    %client.scoreKills = 0;
    %client.scoreDeaths = 0;
    %client.ratio = 0;
    %client.score = 0;
    %client.place = 0;
    %client.elim = false;
    %client.totalDeaths = 0;
  }
}

function Game::playerSpawn(%clientId, %respawn) {
  if (!$ghosting)
    return false;

  Client::clearItemShopping(%clientId);
  %spawnMarker = Game::pickPlayerSpawn(%clientId, %respawn);
  if (!%respawn) {
    // initial drop
    bottomprint(%clientId, "<jc><f0>Mission: <f1>" @ $missionName @ "   <f0>Mission Type: <f1>" @ $Game::missionType @ "\n<f0>Press <f1>'O'<f0> for specific objectives.", 5);
  }
  if (%spawnMarker) {   
    %clientId.guiLock = "";
    %clientId.dead = "";
    if (%spawnMarker == -1) {
      %spawnPos = "0 0 300";
      %spawnRot = "0 0 0";
    } else {
      %spawnPos = GameBase::getPosition(%spawnMarker);
      %spawnRot = GameBase::getRotation(%spawnMarker);
    }

    if (!String::ICompare(Client::getGender(%clientId), "Male"))
      if ($Mission::CoOp) %armor = "SpyMale";
      else                %armor = "DMMale";
    else
      if ($Mission::CoOp) %armor = "SpyFemale";
      else                %armor = "DMFemale";

    %pl = spawnPlayer(%armor, %spawnPos, %spawnRot);
    echo("SPAWN: cl:" @ %clientId @ " pl:" @ %pl @ " marker:" @ %spawnMarker @ " armor:" @ %armor);
    if (%pl != -1) {
      GameBase::setTeam(%pl, Client::getTeam(%clientId));
      Client::setOwnedObject(%clientId, %pl);
      Game::playerSpawned(%pl, %clientId, %armor, %respawn);
	      
      if ($matchStarted) {
        Client::setControlObject(%clientId, %pl);
        %livesRemaining = $SDM::numLives - %clientId.scoreDeaths - 1;
        bottomprint(%clientId, "<JC><F1>--- " @ %livesRemaining @ " " @ tern(%livesRemaining!=1,"lives","life") @ " remaining ---", 3, true);
      } else {
        %clientId.observerMode = "pregame";
        Client::setControlObject(%clientId, Client::getObserverCamera(%clientId));
        Observer::setOrbitObject(%clientId, %pl, 3, 3, 3);
      }
    }
    return true;
  } else {
    Client::sendMessage(%clientId,0,"Sorry No Respawn Positions Are Empty - Try again later ");
    return false;
  }
}