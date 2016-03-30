exec("game.cs");
$MaxNumKills = 15;
$SDM::matchOver = false;

//$SDM::numLives

// Stock Deathmatch - A free for all mission type, where each player gets a fixed number of lives at the beginning of the
//                    match and then play continues until all but one player is out of lives, who is the winner. Rankings
//                    are determined by the order in which the players were eliminated.

//calc scores upon fraging
function Game::clientKilled(%playerId, %killerId) {
   if ($SDM::matchOver) return;
   if ($teamplay) {
     if (%killerId == -1 || %playerId == -1) {
       return;
     }
     %kteam = Client::getTeam(%killerId);
     %pteam = Client::getTeam(%playerId);

     if (%kteam == %pteam)
       $teamScore[%kteam] = $teamScore[%kteam] - 1;
     else
       $teamScore[%pteam] = $teamScore[%pteam] - 1;

     %playerId.livesRemaining--;
     if (%playerId.livesRemaining == -1) {
       %playerId.elim = true;
       Observer::enterObserverMode(%playerId);
       if (%killerId != 0)
         messageAll(0, Client::getName(%playerId) @ " has been eliminated by " @ Client::getName(%killerId) @ "!");
       else
         messageAll(0, Client::getName(%playerId) @ " has been eliminated!");
     }
     Game::refreshClientScore(%playerId);
     if (%killerId != 0) Game::refreshClientScore(%killerId);

     SDMTEAM::checkMissionObjectives(%killerId);
   } else {
     %playerId.livesRemaining--;
     if (%playerId.livesRemaining == -1) {
       %playerId.place = SDM::countRemainingPlayers() + 1;
       %playerId.elim = true;
       Observer::enterObserverMode(%playerId);
       if (%killerId != 0)
         messageAll(0, Client::getName(%playerId) @ " has been eliminated by " @ Client::getName(%killerId) @ "!");
       else
         messageAll(0, Client::getName(%playerId) @ " has been eliminated!");
     }
     Game::refreshClientScore(%playerId);
     if (%killerId != 0) Game::refreshClientScore(%killerId);
     SDM::checkMissionObjectives(%killerId);
   }
}

//Player has a total of 10 seconds per life allowed outside designated mission area.
//After a player expends this 10 sec, the player is remotely killed.
//-lesson to be learned= stay in the mission area!
function Player::leaveMissionArea(%player)
{
   %cl = Player::getClient(%player);
	Client::sendMessage(%cl,1,"You have left the mission area.");
	%player.outArea=1;
	alertPlayer(%player, 3);
}

//checking for timeout of dieSeqCount
function Player::checkLMATimeout(%player, %seqCount)
{
   echo("checking player timeout " @ %player @ " " @ %seqCount);
   if(%player.dieSeqCount == %seqCount)
      remoteKill(Player::getClient(%player));
}

//called if player leaves mission area
function Player::enterMissionArea(%player)
{
   %player.outArea="";
   %player.dieSeqCount = 0;
   %player.timeLeft = %player.timeLeft - (getSimTime() - %player.leaveTime);
}
  
function alertPlayer(%player, %count)
{
	if(%player.outArea == 1) {
		%clientId = Player::getClient(%player);
	  	Client::sendMessage(%clientId,1,"~wLeftMissionArea.wav");
		if(%count > 1)
		   schedule("alertPlayer(" @ %player @ ", " @ %count - 1 @ ");",1.5,%clientId);
		else 
	   	schedule("leaveMissionAreaDamage(" @ %clientId @ ");",1,%clientId);
	}
}

function leaveMissionAreaDamage(%client)
{
	%player = Client::getOwnedObject(%client);
	if(%player.outArea == 1) {
		if(!Player::isDead(%player)) {
		  	Player::setDamageFlash(%client,0.1);
			GameBase::setDamageLevel(%player,GameBase::getDamageLevel(%player) + 0.05);
	   	schedule("leaveMissionAreaDamage(" @ %client @ ");",1);
		}
		else { 
			playNextAnim(%client);	
			Client::onKilled(%client, %client);
		}
	}
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

      if ($teamplay) {
        //for (%t = 0; %t < getNumTeams(); %t++) {
        //  %teamPlayersLeft[%t] = 0;
        //  %teamLivesLeft[%t] = 0;
        //}
        //%winningTeam = 0;
        //%winningTeamLives = 0;
        //for (%c = Client::getFirst(); %c != -1; %c = Client::getNext(%c)) {
        //  if (%c.livesRemaining >= 0 && Client::getTeam(%c) >= 0) {
        //    %teamLivesLeft[Client::getTeam(%c)] += %c.livesRemaining + 1;
        //    if (%teamLivesLeft[Client::getTeam(%c)] > %winningTeamLives) {
        //      %winningTeamLives = %teamLivesLeft[Client::getTeam(%c)];
        //      %winningTeam = Client::getTeam(%c);
        //    }
        //  }
        //}
        %maxScore = $teamScore[0];
        %maxScorer = 0;
        for (%t = 1; %t < getNumTeams(); %t++) if ($teamScore[%t] > %maxScore) { %maxScore = $teamScore[%t]; %maxScorer = %t; }
        $SDM::winningTeam = %maxScorer;//%winningTeam;
      }

      SDM::missionObjectives();
      Server::nextMission();
    } else {
      schedule("Game::checkTimeLimit();", 20);
      UpdateClientTimes(%curTimeLeft);
   }
}

function Vote::changeMission()
{
	$timeLimitReached = true;
   $timeReached = 1;
	SDM::missionObjectives();
}
  
//---------------------------------------------------------------------------------------
//
//Free for all Stock Deathmatch function definitions
//
//---------------------------------------------------------------------------------------
function SDM::countRemainingPlayers() {
  %numPlayersLeft = 0;
  for (%c = Client::getFirst(); %c != -1; %c = Client::getNext(%c)) {
    if (Client::getTeam(%c) >= 0 && %c.livesRemaining >= 0) {
      %numPlayersLeft++;
    }
  }
  return %numPlayersLeft;
}

function SDMTEAM::countRemainingTeams() {
  %teams = 0;

     for (%c = Client::getFirst(); %c != -1; %c = Client::getNext(%c)) {
       if (Client::getTeam(%c) >= 0 && !%c.elim && !%teams[Client::getTeam(%c)]) {
         %teams++;
         %teams[Client::getTeam(%c)] = true;
       }
     }

  return %teams;
}

function SDM::checkMissionObjectives(%playerId) {
   if (SDM::countRemainingPlayers() == 1) {
     $timeLimitReached = true;
     $timeReached = 1;
     %playerId.place = 1;
     Game::refreshClientScore(%playerId);
     SDM::missionObjectives();
     $SDM::matchOver = true;
     schedule("Server::nextMission();", 5);
   }

   if ($SDM::matchOver) return;

   if (SDM::missionObjectives(%playerId)) {
     $SDM::matchOver = true;
     schedule("Server::nextMission();", 5);
   }
}

function SDMTEAM::checkMissionObjectives(%playerId) {
   %rt = SDMTEAM::countRemainingTeams();
   if (%rt == 0) {
     Server::nextMission();
     return;
   }
   if (%rt == 1) {
     $timeLimitReached = true;
     $timeReached = 1;

     for (%c = Client::getFirst(); %c != -1; %c = Client::getNext(%c)) {
       if (Client::getTeam(%c) >= 0 && !%c.elim && !%teams[Client::getTeam(%c)]) {
         $SDM::winningTeam = Client::getTeam(%c);
         break;
       }
     }

     SDM::missionObjectives();
     $SDM::matchOver = true;
     schedule("Server::nextMission();", 5);
   }

   if ($SDM::matchOver) return;

   if (SDM::missionObjectives(%playerId)) {
     $SDM::matchOver = true;
     schedule("Server::nextMission();", 5);
   }
}

function SDM::missionObjectives() {
  %numClients = getNumClients();
  %m = 0;
  %numPlayers = 0;

  if (!$teamplay) {

  for (%i = 0; %i < %numClients; %i++) {
    %clientList[%m] = getClientByIndex(%i);
    if ((Client::getTeam(%clientList[%m]) >= 0 || %clientList[%m].elim) && %clientList[%m].livesRemaining != "") { %m++; %numPlayers++; }
  }

  %doIt = 1;
  while (%doIt == 1) {
    %doIt = "";
    for (%i = 0; %i < %numPlayers; %i++) {
      if ((%clientList[%i]).livesRemaining < (%clientList[%i+1]).livesRemaining) {
        %hold = %clientList[%i];
        %clientList[%i] = %clientList[%i+1];
	%clientList[%i+1] = %hold;
        %doIt = 1;
      }
      if (((%clientList[%i]).livesRemaining == -1 && (%clientList[%i+1]).livesRemaining == -1) &&
          (%clientList[%i]).place < (%clientList[%i+1]).place) {
        %hold = %clientList[%i];
        %clientList[%i] = %clientList[%i+1];
	%clientList[%i+1] = %hold;
        %doIt = 1;
      }
    }
  }

  } else {

    for (%t = 0; %t < getNumTeams(); %t++) {
      %teamPlayersLeft[%t] = 0;
      %teamLivesLeft[%t] = 0;
    }
    for (%c = Client::getFirst(); %c != -1; %c = Client::getNext(%c)) {
      if (%c.livesRemaining >= 0 && Client::getTeam(%c) >= 0) {
        %teamPlayersLeft[Client::getTeam(%c)]++;
        %teamLivesLeft[Client::getTeam(%c)] += %c.livesRemaining + 1;
      }
    }

  }

  if (!$matchStarted)
    %str = "<f1>   - Waiting for match to start.";
  else if (!$Server::timeLimit)
    %str = "<f1>   - No time limit on the game.";
  else if ($timeLimitReached)
    %str = "<f1>   - Time limit reached.";
  else
    %str = "<f1>   - Time remaining: " @ floor($Server::timeLimit - (getSimTime() - $missionStartTime) / 60) @ " minutes.";

  for (%l = -1; %l < getNumTeams(); %l++) {		
    %lineNum = 0;
    if ($timeReached == "") {
      Team::setObjective(%l, %lineNum, "<jc><F8><B0,0:deathmatch1.bmp><B0,0:deathmatch2.bmp>");
      Team::setObjective(%l, %lineNum++, "<f5>Mission Information:");
      Team::setObjective(%l, %lineNum++, "<f1>   - Mission Name: " @ $missionName); 
      Team::setObjective(%l, %lineNum++, %str);
      if ($Mission::extraInfo != "") Team::setObjective(%l, %lineNum++, $Mission::extraInfo);
      Team::setObjective(%l, %lineNum++, " ");

      if (!$teamplay) {

      Team::setObjective(%l, %lineNum++, "<f5>Stock Deathmatch Rules:");
      Team::setObjective(%l, %lineNum++, "<f1>  At the beginning of the match, each player is given a fixed number of lives.");
      Team::setObjective(%l, %lineNum++, "<f1>  Play continues until only one player is left, who is the winner. Rankings");
      Team::setObjective(%l, %lineNum++, "<f1>  are determined by the order in which players are eliminated.");
      Team::setObjective(%l, %lineNum++, " ");
      Team::setObjective(%l, %lineNum++, " ");
      Team::setObjective(%l, %lineNum++, "<f5>TOP PLAYERS ARE: " );
      Team::setObjective(%l, %lineNum++, " ");
      Team::setObjective(%l, %lineNum++, "<f1>Place<L15>Player Name<L45>Num Lives Left");
      //print out top 5 scores
      %index = 0;
      while (%index < %numPlayers && %index < 5) {
        %client = %clientList[%index];//getClientByIndex(%count);
        if (%client.livesRemaining == "" || Client::getTeam(%client) < 0)
          %lives = " ";
        else
          %lives = tern(%clientList[%index].livesRemaining >= 0, %clientList[%index].livesRemaining, "ELIMINATED");
        Team::setObjective(%l, %lineNum++,"<Bskull_small.bmp>" @ (%index+1) @ getSuffix(%index+1) @ " <L16>" @ Client::getName(%clientList[%index]) @ " <L46>" @ %lives);
        %index++;
      }

      } else {

      Team::setObjective(%l, %lineNum++, "<f5>Team Stock Deathmatch Rules:");
      Team::setObjective(%l, %lineNum++, "<f1>  At the beginning of the match, each player is given a fixed number of lives.");
      Team::setObjective(%l, %lineNum++, "<f1>  Play continues until only one team has any players left, which is the winner.");
      Team::setObjective(%l, %lineNum++, " ");
      Team::setObjective(%l, %lineNum++, " ");
      Team::setObjective(%l, %lineNum++, "<f5>TEAMS: " );
      Team::setObjective(%l, %lineNum++, " ");
      Team::setObjective(%l, %lineNum++, "<f1>Team Name<L30>Num Players Left<L45>Total Num Lives Left");

      for (%t = 0; %t < getNumTeams(); %t++) {
        Team::setObjective(%l, %lineNum++, "<Bskull_small.bmp>" @ $Server::teamName[%t] @ " <L31>" @ %teamPlayersLeft[%t] @ " <L46>" @ %teamLivesLeft[%t]);
      }

      }
    } else {
      if (!$teamplay) {

      Team::setObjective(%l, %lineNum++, "<f5>Mission Summary:");
      Team::setObjective(%l, %lineNum++, " " );
      Team::setObjective(%l, %lineNum++, "<f1>     - Top Player: " );
      Team::setObjective(%l, %lineNum++, "<L14><f5><Bskull_big.bmp>" @ Client::getName(%clientList[0]) @ "<Bskull_big.bmp>");
      Team::setObjective(%l, %lineNum++, " ");

      Team::setObjective(%l, %lineNum++, " ");
      Team::setObjective(%l, %lineNum++, "<f5>TOP 5 PLAYERS ARE: " );
      Team::setObjective(%l, %lineNum++, " ");
      Team::setObjective(%l, %lineNum++, "<f1>Place<L15>Player Name<L45>Num Lives Left");
      //print out top 5 scores
      %index = 0;
      while (%index < %numPlayers && %index < 5) {
        %client = %clientList[%index];//getClientByIndex(%count);
        if (%client.livesRemaining == "" || (Client::getTeam(%client) < 0 && !%client.elim))
          %lives = " ";
        else
          %lives = tern(%clientList[%index].livesRemaining >= 0, %clientList[%index].livesRemaining, "ELIMINATED");
        Team::setObjective(%l, %lineNum++,"<Bskull_small.bmp>" @ (%index+1) @ getSuffix(%index+1) @ " <L16>" @ Client::getName(%clientList[%index]) @ " <L46>" @ %lives);
        %index++;
      }

      } else {

      Team::setObjective(%l, %lineNum++, "<f5>Mission Summary:");
      Team::setObjective(%l, %lineNum++, " " );
      Team::setObjective(%l, %lineNum++, "<f1>     - Winning Team: " );
      Team::setObjective(%l, %lineNum++, "<L14><f5><Bskull_big.bmp> " @ $Server::teamName[$SDM::winningTeam] @ " <Bskull_big.bmp>");
      Team::setObjective(%l, %lineNum++, " ");

      for (%t = 0; %t < getNumTeams(); %t++) {
        Team::setObjective(%l, %lineNum++, "<Bskull_small.bmp>" @ $Server::teamName[%t] @ " <L31>" @ %teamPlayersLeft[%t] @ " <L46>" @ %teamTotalLivesLeft[%t]);
      }

      }
    }
    for (%s = %lineNum+1; %s < 30; %s++)
      Team::setObjective(%l, %s, " ");
  }

  News::showNews(%lineNum+1);
  return false;
}

function Game::refreshClientScore(%clientId) {
  if ($teamplay) {
    Client::setScore(%clientId, "%n\t%t\t" @ %clientId.score  @ "\t%p\t%l", %clientId.score);
    if ($matchStarted) {
      if (Client::getTeam(%clientId) >= 0 && %clientId.livesRemaining != "") {
        %numLives = tern(%clientId.livesRemaining >= 0, %clientId.livesRemaining, "ELIM");
      } else if (%clientId.elim) {
        %numLives = "ELIM";
      } else %numLives = " ";
      Client::setScore(%clientId, "%n\t" @                                                                       // Name
                                  %numLives @ "\t" @                                                             // Lives
                                  %clientId.score @ "\t" @                                                       // Score
                                  "%p",                                                                          // Pingz0r
                                  tern(%clientId.place, $Server::maxPlayers - %clientId.place, $Server::maxPlayers + 1));
    } else {
      Client::setScore(%clientId, "%n\t" @                                                                       // Name
                                  " " @ "\t" @                                                                   // Lives
                                  %clientId.score @ "\t" @                                                       // Score
                                  "%p",                                                                          // Pingz0r
                                  0);
    }
  } else {
    if ($matchStarted) {
      if (Client::getTeam(%clientId) >= 0 && %clientId.livesRemaining != "") {
        %numLives = tern(%clientId.livesRemaining >= 0, %clientId.livesRemaining, "ELIM");
      } else if (%clientId.elim) {
        %numLives = "ELIM";
      } else %numLives = " ";
      Client::setScore(%clientId, "%n\t" @                                                                       // Name
                                  %numLives @ "\t" @                                                             // Lives
                                  tern(%clientId.place, %clientId.place @ getSuffix(%clientId.place), "Still alive") @ "\t" @       // Place
                                  "%p",                                                                          // Pingz0r
                                  tern(%clientId.place, $Server::maxPlayers - %clientId.place, $Server::maxPlayers + 1));
    } else {
      Client::setScore(%clientId, "%n\t" @                                                                       // Name
                                  " " @ "\t" @                                                                   // Lives
                                  " " @ "\t" @                                                                   // Place
                                  "%p",                                                                          // Pingz0r
                                  0);
    }
  }

  SDM::missionObjectives();
}

// TEAM: Player Name - Team - Score
// FFA:  Player Name - Num Lives Left - Place - Ping
function Mission::init() {
  $numTeams = getNumTeams();
  for (%i = 0; %i < $numTeams; %i++)
    $teamScore[%i] = 0;

  if ($teamplay = !($numTeams == 1)) {
    //setTeamScoreHeading("Team Name\t\xC8Score");
    setClientScoreHeading("Player Name\t\x55Num Lives\t\xA0Score\t\xE4Ping");
    $SMD::winningTeam = -1;
  } else {
    setTeamScoreHeading("");
    setClientScoreHeading("Player Name\t\x55Num Lives\t\xA0Place\t\xE4Ping");
  }
  $dieSeqCount = 0;
  $timeReached="";
  SDM::missionObjectives();
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
    }
  } else {
    %client.scoreKills = 0;
    %client.scoreDeaths = 0;
    %client.ratio = 0;
    %client.score = 0;
    %client.place = 0;
    %client.elim = false;
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
        bottomprint(%clientId, "<JC><F1>--- " @ %clientId.livesRemaining @ " live"@tern(%cl.livesRemaining!=1,"s","")@" remaining ---", 3, true);
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

function Game::startMatch() {
  $matchStarted = true;
  $missionStartTime = getSimTime();
  messageAll(0, "Match started.");
  Game::resetScores();	

  %numTeams = getNumTeams();
  for (%i = 0; %i < %numTeams; %i = %i + 1) {
    if ($TeamEnergy[%i] != "Infinite")
      schedule("replenishTeamEnergy(" @ %i @ ");", $secTeamEnergy);
  }

  for(%cl = Client::getFirst(); %cl != -1; %cl = Client::getNext(%cl)) {
    if (%cl.observerMode == "pregame") {
      %cl.livesRemaining = $SDM::numLives - 1;
      %cl.observerMode = "";
      %cl.elim = false;
      Client::setControlObject(%cl, Client::getOwnedObject(%cl));
      bottomprint(%cl, "<JC><F1>--- " @ %cl.livesRemaining @ " live"@tern(%cl.livesRemaining!=1,"s","")@" remaining ---", 3, true);
    } else {
      %cl.livesRemaining = -1;
      %cl.elim = false;
    }
    Game::refreshClientScore(%cl);
  }
  Game::checkTimeLimit();
}