exec("game.cs");

$RabbitHunt::HUNTER_KILL_POINTS = 2;
$RabbitHunt::RABBIT_KILL_POINTS = 5;

// START MISSION AREA STUFF

//Player has a total of 10 seconds per life allowed outside designated mission area.
//After a player expends this 10 sec, the player is remotely killed.
//-lesson to be learned= stay in the mission area!
function Player::leaveMissionArea(%player) {
  %cl = Player::getClient(%player);
  Client::sendMessage(%cl, 1, "You have left the mission area.");
  %player.outArea = 1;
  alertPlayer(%player, 3);
}

//checking for timeout of dieSeqCount
function Player::checkLMATimeout(%player, %seqCount) {
  echo("checking player timeout " @ %player @ " " @ %seqCount);
  if (%player.dieSeqCount == %seqCount)
    remoteKill(Player::getClient(%player));
}

//called if player leaves mission area
function Player::enterMissionArea(%player) {
  %player.outArea = "";
  %player.dieSeqCount = 0;
  %player.timeLeft = %player.timeLeft - (getSimTime() - %player.leaveTime);
}
  
function alertPlayer(%player, %count) {
  if (%player.outArea == 1) {
    %clientId = Player::getClient(%player);
    Client::sendMessage(%clientId, 1, "~wLeftMissionArea.wav");
    if (%count > 1)
      schedule("alertPlayer(" @ %player @ ", " @ %count - 1 @ ");", 1.5, %clientId);
    else 
      schedule("leaveMissionAreaDamage(" @ %clientId @ ");", 1, %clientId);
  }
}

function leaveMissionAreaDamage(%client) {
	%player = Client::getOwnedObject(%client);
	if(%player.outArea == 1) {
		if(!Player::isDead(%player)) {
		  	Player::setDamageFlash(%client,0.1);
			GameBase::setDamageLevel(%player,GameBase::getDamageLevel(%player) + 5);
	   	schedule("leaveMissionAreaDamage(" @ %client @ ");",1);
		}
		else { 
			playNextAnim(%client);	
			Client::onKilled(%client, %client);
		}
	}
}

// END MISSION AREA STUFF

function Client::onKilled(%playerId, %killerId, %damageType) {
   echo("GAME: kill " @ %killerId @ " " @ %playerId @ " " @ %damageType);
   %playerId.guiLock = true;
   Client::setGuiMode(%playerId, $GuiModePlay);

   if (!String::ICompare(Client::getGender(%playerId), "Male"))
      %playerGender = "his";
   else
     %playerGender = "her";

   %victimName = tern(Client::getTeam(%playerId) == 0, "Rabbit ", "Hunter ") @ Client::getName(%playerId);

   %playerObject = Client::getOwnedObject(%playerId);

   if(!%killerId || !isClientObject(%killerId) || ($numDeathMsgs[%damageType] <= 0 && %killerId != %playerId)) {
     messageAll(0, strcat(%victimName, " dies."), $DeathMessageMask);
     %playerId.scoreDeaths++;
   } else if(%killerId == %playerId && !(%playerObject.lastTeamKickbackTime + 1 > getSimTime() && %damageType == $LandingDamageType)) {
      if (%damageType == $LandingDamageType) %dt = %damageType;
      else                                   %dt = -2;

      %ridx = floor(getRandom() * ($numDeathMsgs[%dt] - 0.01));
      %oopsMsg = sprintf($deathMsg[%dt, %ridx], %victimName, %playerGender);
      messageAll(0, %oopsMsg, $DeathMessageMask);
      %playerId.scoreDeaths++;
      %playerId.score--;
      Game::refreshClientScore(%playerId);
   } else {
     %killerName = tern(Client::getTeam(%killerId) == 0, "Rabbit ", "Hunter ") @ Client::getName(%killerId);

     if (%playerObject.lastTeamKickbackTime + 1 > getSimTime() && %damageType == $LandingDamageType) %killerId = %playerObject.lastTeamKickback;

     if (!String::ICompare(Client::getGender(%killerId), "Male"))
       %killerGender = "his";
     else
       %killerGender = "her";

     if ($teamplay && (Client::getTeam(%killerId) == Client::getTeam(%playerId))) {
       if (%damageType != $MineDamageType) 
         messageAll(0, strcat(%killerName, 
   	 " mows down ", %killerGender, " teammate, ", %victimName), $DeathMessageMask);
       else
         messageAll(0, strcat(%killerName, 
   	 " killed ", %killerGender, " teammate, ", %victimName ," with a mine."), $DeathMessageMask);

       %killerId.scoreDeaths++;
       %killerId.score--;
       Game::refreshClientScore(%killerId);
     } else {
       %ridx = floor(getRandom() * ($numDeathMsgs[%damageType] - 0.01));
       %obitMsg = sprintf($deathMsg[%damageType, %ridx], %killerName,
       %victimName, %killerGender, %playerGender);
       messageAll(0, %obitMsg, $DeathMessageMask);
       %killerId.scoreKills++;
       %playerId.scoreDeaths++;  // test play mode
       %killerId.score++;
       Game::refreshClientScore(%killerId);
       Game::refreshClientScore(%playerId);
    }
  }
  Game::clientKilled(%playerId, %killerId);
  if ($Game::missionType == "DMT") DMT::clientKilled(%playerId, %killerId);

  %tk = $teamplay && (Client::getTeam(%killerId) == Client::getTeam(%playerId));
  if (%playerId && %playerId != -1) remoteEval(%playerId,"onKilled", %playerId, %killerId, %damageType, %tk);
  if (%killerId && %killerId != -1) remoteEval(%killerId,"onKill", %killerId, %playerId, %damageType, %tk);
}

function Game::clientKilled(%clientId, %killerId) {

  %clientTeam = Client::getTeam(%clientId);
  %killerTeam = Client::getTeam(%killerId);

  if (!String::ICompare(Client::getGender(%clientId), "Male")) { %clientGenderP = "his"; %clientGenderO = "him"; }
  else                                                         { %clientGenderP = "her"; %clientGenderO = "her"; }

  if (!String::ICompare(Client::getGender(%clientId), "Male")) { %killerGenderP = "his"; %killerGenderO = "him"; }
  else                                                         { %killerGenderP = "her"; %killerGenderO = "her"; }

  if (%clientTeam == 0) { // If the dead guy is Rabbit...

    // Someone is going to be a new Rabbit.
    //   If a Hunter killed this Rabbit, give Rabbit to him
    //   If it was suicide, pick a Hunter to randomly Rabbitize
    //   If it was a TK, turn the TKer into a Hunter and give Rabbit to a random Hunter


    if (%clientId == %killerId) {                // Suicide
      // Take care of scoring...
      %clientId.rabbitScore--;

      %newRabbit = RabbitHunt::pickRandomHunter(); 
      echo("SUICIDE, NEW RABBIT = " @ %newRabbit);
      if (%newRabbit != -1) {
        RabbitHunt::rabbitize(%newRabbit);
        RabbitHunt::hunterize(%clientId);

        Game::refreshClientScore(%newRabbit);
        Game::refreshClientScore(%clientId);

        //messageAll(0, "Rabbit " @ Client::getName(%clientId) @ " killed " @ %killerGenderO @ "self; " @ Client::getName(%newRabbit) @ " has been chosen to be the new Rabbit");
        messageAll(0, Client::getName(%newRabbit) @ " has been chosen to succeed " @ Client::getName(%clientId) @ " as a Rabbit");
      }
    } else if (%clientTeam == %killerTeam) {     // TK
      // Take care of scoring...
      %killerId.rabbitScore--;

      %newRabbit = RabbitHunt::pickRandomHunter(); 
      echo("TK, NEW RABBIT = " @ %newRabbit);
      if (%newRabbit != -1) {
        RabbitHunt::rabbitize(%newRabbit);
        RabbitHunt::hunterize(%killerId);

        Game::refreshClientScore(%newRabbit);
        Game::refreshClientScore(%killerId);

        //messageAll(0, "Rabbit " @ Client::getName(%killerId) @ " teamkilled " @ Client::getName(%clientId) @ "; " @ Client::getName(%newRabbit) @ " has been chosen to be the new Rabbit");
        messageAll(0, Client::getName(%newRabbit) @ " has been chosen to succeed " @ Client::getName(%killerId) @ " as a Rabbit");
      }
    } else {                                     // Hunter
      %killerId.rabbitScore += $RabbitHunt::RABBIT_KILL_POINTS;

      RabbitHunt::rabbitize(%killerId);
      RabbitHunt::hunterize(%clientId);

      Game::refreshClientScore(%clientId);
      Game::refreshClientScore(%killerId);
    }

  } else { // Else the dead guy is a Hunter...

    // If it was suicide, subtract points
    // If it was a TK, subtract points from the killer
    // If a Rabbit killed this Hunter, give points to the Rabbit


    if (%clientId == %killerId) {                // Suicide
      %clientId.rabbitScore--;
      Game::refreshClientScore(%clientId);
    } else if (%clientTeam == %killerTeam) {     // TK
      %killerId.rabbitScore--;
      Game::refreshClientScore(%killerId);
    } else {                                     // Rabbit
      %killerId.rabbitScore += $RabbitHunt::HUNTER_KILL_POINTS;
      Game::refreshClientScore(%killerId);
    }

  }

}

function Client::setTeamSilent(%clientId, %team) {
  Client::setInitialTeam(%clientId, %team);
  GameBase::setTeam(%clientId, %team);
  Client::setSkin(%clientId, $Server::teamSkin[%team]);
}

function RabbitHunt::pickRandomHunter() {
  %numHunters = 0;
  for (%c = Client::getFirst(); %c != -1; %c = Client::getNext(%c)) {
    if (Client::getTeam(%c) == 1) {
      %hunters[%numHunters] = %c;
      %numHunters++;
    }
  }

  return tern(%numHunters > 0, %hunters[floor(getRandom() * %numHunters)], -1);
}

function RabbitHunt::rabbitize(%clientId) {
  %o = Client::getOwnedObject(%clientId);
  if (%o != -1) {
    playNextAnim(%clientId);
    Player::kill(%clientId);
  }
  Client::setTeamSilent(%clientId, 0);
  Game::playerSpawn(%clientId, true);
  Client::sendMessage(%clientId, 1, "You are now a Rabbit.");
}

function RabbitHunt::hunterize(%clientId) {
  %o = Client::getOwnedObject(%clientId);
  if (%o != -1) {
    playNextAnim(%clientId);
    Player::kill(%clientId);
  }
  Client::setTeamSilent(%clientId, 1);
  Game::playerSpawn(%clientId, true);
  Client::sendMessage(%clientId, 1, "You are now a Hunter.");
}


function Game::getSpawnArmor(%clientId) {
  %t = Client::getTeam(%clientId);

  if (!String::ICompare(Client::getGender(%clientId), "Male"))
    if (%t == 0) %armor = "DMMale";
    else         %armor = "DMMale";
  else
    if (%t == 0) %armor = "DMFemale";
    else         %armor = "DMFemale";

  return %armor;
}

function Game::checkTimeLimit() {
   // if no timeLimit set or timeLimit set to 0,
   // just reschedule the check for a minute hence
   $timeLimitReached = false;

   if(!$Server::timeLimit)
   {
      schedule("Game::checkTimeLimit();", 60);
      return;
   }
   %curTimeLeft = ($Server::timeLimit * 60) + $missionStartTime - getSimTime();
   if(%curTimeLeft <= 0)
   {
      $timeLimitReached = true;
      $timeReached = 1;
		RabbitHunt::missionObjectives();
		Server::nextMission();
	}
   else
   {
      schedule("Game::checkTimeLimit();", 20);
      UpdateClientTimes(%curTimeLeft);
   }
}

function Vote::changeMission() {
	$timeLimitReached = true;
	$timeReached = 1;
	RabbitHunt::missionObjectives();
}

function RabbitHunt::checkMissionObjectives(%playerId)  {
   if (RabbitHunt::missionObjectives(%playerId))
     schedule("Server::nextMission();", 0);

   if ($RabbitHuntScoreLimit > 0) {
     if (%playerId.rabbitScore >= $RabbitHuntScoreLimit) {
       $timeLimitReached = true;
       $timeReached = 1;
       RabbitHunt::missionObjectives();
       schedule("Server::nextMission();", 1);
     }
   }
}

function RabbitHunt::missionObjectives() {
  %numClients = getNumClients();
  for (%i = 0 ; %i < %numClients ; %i++) 
    %clientList[%i] = getClientByIndex(%i);

  %doIt = 1;
  while (%doIt == 1) {
    %doIt = "";
    for (%i = 0; %i < %numClients; %i++) {
      if ((%clientList[%i]).rabbitScore < (%clientList[%i+1]).rabbitScore) {
        %hold = %clientList[%i];
        %clientList[%i] = %clientList[%i+1];
        %clientList[%i+1] = %hold;
        %doIt = 1;
      }
    }
  }

  if (!$Server::timeLimit)
    %str = "<f1>   - No time limit on the game.";
  else if ($timeLimitReached)
    %str = "<f1>   - Time limit reached.";
  else
    %str = "<f1>   - Time remaining: " @ floor($Server::timeLimit - (getSimTime() - $missionStartTime) / 60) @ " minutes.";

  // %l = team number
  for (%l = -1; %l <= 1; %l++) {		
    %lineNum = 0;
    if ($timeReached == "") {
      Team::setObjective(%l, %lineNum, "<jc><B0,0:deathmatch1.bmp><B0,0:deathmatch2.bmp>");
      Team::setObjective(%l, %lineNum++, "<f5>Mission Information:");
      Team::setObjective(%l, %lineNum++, "<f1>   - Mission Name: " @ $missionName); 
      Team::setObjective(%l, %lineNum++, %str);
      Team::setObjective(%l, %lineNum++, " ");
      Team::setObjective(%l, %lineNum++, "<f5>Mission Objectives:");
      Team::setObjective(%l, %lineNum++, "<f1>   -I'll write better rules later. Hunters: kill Rabbits. Rabbits: kill Hunters, but try to stay alive.");
      Team::setObjective(%l, %lineNum++, "<f1>Remember to stay within the mission area, which is defined by the extents of your commander screen map."	@ 
                                         " If you go outside of the mission area you will have 3 seconds to get back into the mission area, or you'll start taking damage!");
      Team::setObjective(%l, %lineNum++, " ");
      Team::setObjective(%l, %lineNum++, " ");
      Team::setObjective(%l, %lineNum++, "<f5>TOP PLAYERS ARE: " );
      Team::setObjective(%l, %lineNum++, " ");
      Team::setObjective(%l, %lineNum++, "<f1>Player Name<L30>Score");
    } else {
      Team::setObjective(%l, %lineNum++, "<f5>Mission Summary:");
      Team::setObjective(%l, %lineNum++, " " );
      Team::setObjective(%l, %lineNum++, "<f1>     - The Best Player(s): " );
      %i = 0;
      %TopScore = "";
      while (%i < %numClients && %clientList[%i].rabbitScore > 0 && (%TopScore == "" || (%TopScore ==  (%clientList[%i+1]).rabbitScore && %TopScore > 0))) {
        Team::setObjective(%l, %lineNum++, "<L14><f5><Bskull_big.bmp>\n" @ Client::getName(%clientList[%i]) @ "<f1> with a score of <f5>" @ (%clientList[%i]).rabbitScore);
        %TopScore = (%clientList[%i]).rabbitScore;
        %i++;
      }
      if (%i == 0) {
        Team::setObjective(%l, %lineNum++, "<L14><f1>NONE with a ratio greater than 0.0%");
      }
      Team::setObjective(%l, %lineNum++, " ");
      Team::setObjective(%l, %lineNum++, " ");
      Team::setObjective(%l, %lineNum++, "<f5>TOP PLAYERS ARE: " );
      Team::setObjective(%l, %lineNum++, " ");
      Team::setObjective(%l, %lineNum++, "<f1>Player Name<L30>Score");
    }
    //print out top 5 scores
    %index = 0;
    // WHILE (THERE ARE MORE CLIENTS AND THE PLAYER HAS A SCORE > 0 AND (EITHER WE HAVEN'T REACHED 5 PLAYERS YET OR THIS PLAYER IS TIED WITH THE LAST ONE)
    while (%index < %numClients && %clientList[%index].rabbitScore > 0 &&
           (%index < 5 || (%clientList[%index].rabbitScore == %lastScore && %lastScore != 0))) {
      %client = getClientByIndex(%index);
      Team::setObjective(%l, %lineNum++, "<Bskull_small.bmp>" @ Client::getName(%clientList[%index]) @ " <L31>" @ (%clientList[%index]).rabbitScore);
      %lastRatio = (%clientList[%index]).rabbitScore;
      %index++;
    }
    for (%s = %lineNum + 1; %s < 30; %s++)
      Team::setObjective(%l, %s, " ");
  }
  $timeReached = "";

  News::showNews(%lineNum+1);
  return false;
}

function Game::refreshClientScore(%clientId) {
  %t = Client::getTeam(%clientId);
  if (%t == 0) %status = "Rabbit";
  else if (%t == 1) %status = "Hunter";
  else              %status = " ";

  if (%clientId.rabbitScore == "") %clientId.rabbitScore = 0;
  Client::setScore(%clientId, "%n\t" @ %status @ "\t" @ %clientId.rabbitScore @ "\t%p\t%l", %clientId.rabbitScore);
  RabbitHunt::missionObjectives();
}

function Game::resetScores(%client) {
  if (%client == "") {
    for (%cl = Client::getFirst(); %cl != -1; %cl = Client::getNext(%cl)) {
      %cl.scoreKills = 0;
      %cl.scoreDeaths = 0;
      %cl.ratio = 0;
      %cl.score = 0;
      %cl.rabbitScore = 0;
    }
  } else {
    %client.scoreKills = 0;
    %client.scoreDeaths = 0;
    %client.ratio = 0;
    %client.score = 0;
    %client.rabbitScore = 0;
  }
}

function Mission::init() {
   //setClientScoreHeading("Player Name\t\x78Team\t\xC8Score");

   $numTeams = getNumTeams();
   for (%i = 0; %i < $numTeams; %i++)
     $teamScore[%i] = 0;

   setTeamScoreHeading("");
   setClientScoreHeading("Player Name\t\x65Status\t\x95Score\t\xC5Ping\t\xE2PL");

   $dieSeqCount = 0;
   //setup ai if any
   // -- or not :D
   //AI::setupAI();
   RabbitHunt::missionObjectives();
}