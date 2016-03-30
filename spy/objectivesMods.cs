function objective::displayBitmap(%team, %line) {
	if($TestMissionType == "CTF") {
		%bitmap1 = "capturetheflag1.bmp";
		%bitmap2 = "capturetheflag2.bmp";
	}
	else if($TestMissionType == "C&H") {
		%bitmap1 = "captureandhold1.bmp";
		%bitmap2 = "captureandhold2.bmp";
	}
	else if($TestMissionType == "D&D") {
		%bitmap1 = "defendanddest1.bmp";
		%bitmap2 = "defendanddest2.bmp";
	}	   
	else if($TestMissionType == "F&R") {
		%bitmap1 = "findandret1.bmp";
		%bitmap2 = "findandret2.bmp";
	}
	else if($TestMissionType == "DMT") {
		%bitmap1 = "deathmatch1.bmp";
		%bitmap2 = "deathmatch2.bmp";
	}
	if(%bitmap1 == "" || %bitmap2 == "")
 		Team::setObjective(%team, %line, " ");
	else
 		Team::setObjective(%team, %line, "<jc><B0,0:" @ %bitmap1 @ "><B0,0:" @ %bitmap2 @ ">");
}

function ObjectiveMission::initCheck(%object) {
	if($TestMissionType == "") {
		%name = gamebase::getdataname(%object); 
	   if(%name == Flag) { 
			if(gamebase::getteam(%object) != -1)
				$TestMissionType = "CTF";
			else
				$TestMissionType = "F&R";
		}
		else if(%object.objectiveName != "" && %object.scoreValue)
			$TestMissionType = "D&D";
		else if(%name == TowerSwitch)
			$NumTowerSwitchs++;
		else if(%name == TeamDMObject)
			$TestMissionType = "DMT";
	}

   %object.trainingObjectiveComplete = "";
   %object.objectiveLine = "";
   if(GameBase::virtual(%object, objectiveInit))
      addToSet("MissionCleanup/ObjectivesSet", %object);
}

function Mission::init() {
   setClientScoreHeading("Player Name\t\x6FTeam\t\xA6Score\t\xCFPing\t\xEFPL");
//   setClientScoreHeading("Player Name\t\x6FTeam\t\xD6Score");//\t\xFFPing\t\xFFPL");
   setTeamScoreHeading("Team Name\t\xD6Score");

   $firstTeamLine = 7 + tern($Mission::extraInfo != "", 1, 0); // Changed to make room for $Mission::extraInfo
   $firstObjectiveLine = $firstTeamLine + getNumTeams() + 1;
   for(%i = -1; %i < getNumTeams(); %i++)
   {
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
   for(%i = 0; (%obj = Group::getObject(%group, %i)) != -1; %i++)
   {
      %obj.objectiveLine = %i + $firstObjectiveLine;
      ObjectiveMission::objectiveChanged(%obj);
   }

   News::showNews(%i + $firstObjectiveLine);

   ObjectiveMission::refreshTeamScores();
   for(%cl = Client::getFirst(); %cl != -1; %cl = Client::getNext(%cl))
   {
      %cl.score = 0;
      Game::refreshClientScore(%cl);
   }
   schedule("ObjectiveMission::checkPoints();", 5);

	if($TestMissionType == "") {
		if($NumTowerSwitchs) 
			$TestMissionType = "C&H";
		else 
			$TestMissionType = "NONE";		
		$NumTowerSwitchs = "";
	}
//   AI::setupAI();
}

function ObjectiveMission::setObjectiveHeading() {
  if ($missionComplete) {
    %curLeader = 0;
    %tieGame = false;
    %tie = 0;
    %tieTeams[%tie] = %curLeader; 
    for (%i = 0; %i < getNumTeams() ; %i++) 
      echo("GAME: teamfinalscore " @ %i @ " " @ $teamScore[%i]);

    for (%i = 1; %i < getNumTeams(); %i++) {
      if ($teamScore[%i] == $teamScore[%curLeader]) { 
        %tieGame = true;
        %tieTeams[%tie++] = %i;
      } else if ($teamScore[%i] > $teamScore[%curLeader]) {
        %curLeader = %i;
        %tieGame = false;
        %tie = 0;
        %tieTeams[%tie] = %curLeader; 
      }
    }
    if (%tieGame) {
      for (%g = 0; %g <= %tie; %g++) { 
        %names = %names @ getTeamName(%tieTeams[%g]);
        if (%g == %tie-1)
          %names = %names @ " and "; 
        else if (%g != %tie)
          %names = %names @ ", "; 
      }
      if (%tie > 1) 
        %names = %names @ " all"; 
    }
    for (%i = -1; %i < getNumTeams(); %i++) {
      Objective::displayBitmap(%i,0);
      if (!%tieGame) {
        if (%i == %curLeader) { 
          if ($teamScore[%curLeader] == 1)
            Team::setObjective(%i, 1, "<F5>           Your team won the mission with " @ $teamScore[%curLeader] @ " point!");
          else
            Team::setObjective(%i, 1, "<F5>           Your team won the mission with " @ $teamScore[%curLeader] @ " points!");
        } else {
          if ($teamScore[%curLeader] == 1)
            Team::setObjective(%i, 1, "<F5>     The " @ getTeamName(%curLeader) @ " team won the mission with " @ $teamScore[%curLeader] @ " point!");
          else
            Team::setObjective(%i, 1, "<F5>     The " @ getTeamName(%curLeader) @ " team won the mission with " @ $teamScore[%curLeader] @ " points!");
        }
      } else {
        if (getNumTeams() > 2) {
          Team::setObjective(%i, 1, "<F5>     The " @ %names @ " tied with a score of " @ $teamScore[%curLeader]);
        } else
          Team::setObjective(%i, 1, "<F5>     The mission ended in a tie where each team had a score of " @ $teamScore[%curLeader]);
      }
      Team::setObjective(%i, 2, " ");
    }
  } else {
    for (%i = -1; %i < getNumTeams(); %i++) {
      Objective::displayBitmap(%i,0);
      Team::setObjective(%i,1, "<f5>Mission Completion:");
      Team::setObjective(%i, 2,"<f1>   - " @ $teamScoreLimit @ " points needed to win the mission.");
    }
  }
  if (!$Server::timeLimit) {
    %str = "<f1>   - No time limit on the game.";
  } else if ($timeLimitReached) {
    %str = "<f1>   - Time limit reached.";
  } else if ($missionComplete) {
    %time = getSimTime() - $missionStartTime;
    %minutes = Time::getMinutes(%time);
    %seconds = Time::getSeconds(%time);
    if (%minutes < 10)
      %minutes = "0" @ %minutes;
    if (%seconds < 10)
      %seconds = "0" @ %seconds;
    %str = "<f1>   - Total match time: " @ %minutes @ ":" @ %seconds;
  } else {
    %str = "<f1>   - Time remaining: " @ floor($Server::timeLimit - (getSimTime() - $missionStartTime) / 60) @ " minutes.";
  }
  for (%i = -1; %i < getNumTeams(); %i++) {
    Team::setObjective(%i, 3, " ");
    Team::setObjective(%i, 4, "<f5>Mission Information:");
    Team::setObjective(%i, 5, "<f1>   - Mission Name: " @ $missionName); 
    if ($Mission::extraInfo != "") {
      Team::setObjective(%i, 6, $Mission::extraInfo);
      Team::setObjective(%i, 7, %str);
    } else {
      Team::setObjective(%i, 6, %str);
    }
  }
}

function Mission::endMission() {
  schedule("Server::nextMission();", 0);
}