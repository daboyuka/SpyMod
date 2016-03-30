//if ($itemSetType == $ITEM_SET_UNSET) {
//  if ($Game::missionType == "DM") %set = $ITEM_SET_DM;
//  else                            %set = $ITEM_SET_ALL;
//  setSpawnBuyList(%set);
//}

$SensorNetworkEnabled = false;
//$HalfDamageTime = 2;

function Game::getSpawnArmor(%clientId) {
           if(!String::ICompare(Client::getGender(%clientId), "Male"))
	      if ($Mission::CoOp) %armor = "SpyMale";
              else                %armor = "DMMale";
	   else
	      if ($Mission::CoOp) %armor = "SpyFemale";
              else                %armor = "DMFemale";
  return %armor;
}


function Game::playerSpawn(%clientId, %respawn) {
   if(!$ghosting)
      return false;

	Client::clearItemShopping(%clientId);
   %spawnMarker = Game::pickPlayerSpawn(%clientId, %respawn);
   if(!%respawn)
   {
      // initial drop
      bottomprint(%clientId, "<jc><f0>Mission: <f1>" @ $missionName @ "   <f0>Mission Type: <f1>" @ $Game::missionType @ "\n<f0>Press <f1>'O'<f0> for specific objectives.", 5);
   }
	if(%spawnMarker) {   
		%clientId.guiLock = "";
	 	%clientId.dead = "";
	   if(%spawnMarker == -1)
	   {
	      %spawnPos = "0 0 300";
	      %spawnRot = "0 0 0";
	   }
	   else
	   {
	      %spawnPos = GameBase::getPosition(%spawnMarker);
	      %spawnRot = GameBase::getRotation(%spawnMarker);
	   }

           %armor = Game::getSpawnArmor(%clientId);

	   %pl = spawnPlayer(%armor, %spawnPos, %spawnRot);
	   echo("SPAWN: cl:" @ %clientId @ " pl:" @ %pl @ " marker:" @ %spawnMarker @ " armor:" @ %armor);
	   if(%pl != -1)
	   {
	      GameBase::setTeam(%pl, Client::getTeam(%clientId));
	      Client::setOwnedObject(%clientId, %pl);
	      Game::playerSpawned(%pl, %clientId, %armor, %respawn);
	      
	      if($matchStarted)
	         Client::setControlObject(%clientId, %pl);
	      else
	      {
	         %clientId.observerMode = "pregame";
	         Client::setControlObject(%clientId, Client::getObserverCamera(%clientId));
	         Observer::setOrbitObject(%clientId, %pl, 3, 3, 3);
	      }
	   }
      return true;
	}
	else {
		Client::sendMessage(%clientId,0,"Sorry No Respawn Positions Are Empty - Try again later ");
      return false;
	}
}

function Game::playerSpawned(%pl, %clientId, %armor) {						  
  if (getNumTeams() == 1) Client::setSkin(%clientId, $Client::info[%clientId, 0]);

  if (%clientId.invis) GameBase::startFadeOut(%pl);

  if (Player::isAIControlled(%pl)) return;

  %clientId.spawn = 1;

  for(%i = 0; (%str = $spawnAmmo[%i]) != ""; %i++) {
    %item = getWord(%str, 1);
    %amt = getWord(%str, 0);
    Player::setItemCount(%pl, %item, %amt);	
  }
  for(%i = 0; (%str = $spawnItems[%i]) != ""; %i++) {
    %item = getWord(%str, 1);
    %amt = getWord(%str, 0);
    Player::setItemCount(%pl, %item, %amt);
    if ($ClipSize[%item] != "" && $ClipSize[%item] > 0)
      %pl.loadedAmmo[%item] = min($ClipSize[%item], Player::getItemCount(%pl, $WeaponAmmo[%item]));
    if (%item.className == "Weapon") %clientId.spawnWeapon = %item;
    if (%item.className == "Backpack" && Player::getMountedItem(%pl, $BackpackSlot) == -1) Player::useItem(%pl, %item);
  }
  for(%i = 0; (%item = $spawnWeapons[%i]) != ""; %i++) {
    Player::setItemCount(%pl, %item, 1);
    if ($ClipSize[%item] != "" && $ClipSize[%item] > 0)
      %pl.loadedAmmo[%item] = min($ClipSize[%item], Player::getItemCount(%pl, $WeaponAmmo[%item]));
    %clientId.spawnWeapon = %item;
  }
  for(%i = 0; (%item = $spawnGadgets[%i]) != ""; %i++) {
    Player::setItemCount(%pl, %item, 1);	
    if ($ClipSize[%item] != "" && $ClipSize[%item] > 0)
      %pl.loadedAmmo[%item] = min($ClipSize[%item], Player::getItemCount(%pl, $WeaponAmmo[%item]));
  }

  // Team specific item sets
  %t = GameBase::getApparentTeam(%pl);
  for(%i = 0; (%str = $spawnAmmo[%t, %i]) != ""; %i++) {
    %item = getWord(%str, 1);
    %amt = getWord(%str, 0);
    Player::setItemCount(%pl, %item, %amt);	
  }
  for(%i = 0; (%str = $spawnItems[%t, %i]) != ""; %i++) {
    %item = getWord(%str, 1);
    %amt = getWord(%str, 0);
    Player::setItemCount(%pl, %item, %amt);
    if ($ClipSize[%item] != "" && $ClipSize[%item] > 0)
      %pl.loadedAmmo[%item] = min($ClipSize[%item], Player::getItemCount(%pl, $WeaponAmmo[%item]));
    if (%item.className == "Weapon") %clientId.spawnWeapon = %item;
    if (%item.className == "Backpack" && Player::getMountedItem(%pl, $BackpackSlot) == -1) Player::useItem(%pl, %item);
  }
  for(%i = 0; (%item = $spawnWeapons[%t, %i]) != ""; %i++) {
    Player::setItemCount(%pl, %item, 1);
    if ($ClipSize[%item] != "" && $ClipSize[%item] > 0)
      %pl.loadedAmmo[%item] = min($ClipSize[%item], Player::getItemCount(%pl, $WeaponAmmo[%item]));
    %clientId.spawnWeapon = %item;
  }
  for(%i = 0; (%item = $spawnGadgets[%t, %i]) != ""; %i++) {
    Player::setItemCount(%pl, %item, 1);	
    if ($ClipSize[%item] != "" && $ClipSize[%item] > 0)
      %pl.loadedAmmo[%item] = min($ClipSize[%item], Player::getItemCount(%pl, $WeaponAmmo[%item]));
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



function Client::onKilled(%playerId, %killerId, %damageType) {
   echo("GAME: kill " @ %killerId @ " " @ %playerId @ " " @ %damageType);
   %playerId.guiLock = true;
   Client::setGuiMode(%playerId, $GuiModePlay);

   if (!String::ICompare(Client::getGender(%playerId), "Male"))
      %playerGender = "his";
   else
     %playerGender = "her";

   %victimName = Client::getName(%playerId);

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
     if (%playerObject.lastTeamKickbackTime + 1 > getSimTime() && %damageType == $LandingDamageType) %killerId = %playerObject.lastTeamKickback;

     if (!String::ICompare(Client::getGender(%killerId), "Male"))
       %killerGender = "his";
     else
       %killerGender = "her";

     if ($teamplay && (Client::getTeam(%killerId) == Client::getTeam(%playerId))) {
       if (%damageType != $MineDamageType) 
         messageAll(0, strcat(Client::getName(%killerId), 
   	 " mows down ", %killerGender, " teammate, ", %victimName), $DeathMessageMask);
       else
         messageAll(0, strcat(Client::getName(%killerId), 
   	 " killed ", %killerGender, " teammate, ", %victimName ," with a mine."), $DeathMessageMask);

       %killerId.scoreDeaths++;
       %killerId.score--;
       Game::refreshClientScore(%killerId);
     } else {
       %ridx = floor(getRandom() * ($numDeathMsgs[%damageType] - 0.01));
       %obitMsg = sprintf($deathMsg[%damageType, %ridx], Client::getName(%killerId),
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

function DMT::clientKilled(%playerId, %killerId) {
  %playerTeam = GameBase::getTeam(%playerId);
  %killerTeam = GameBase::getTeam(%killerId);

  %group = nameToID("MissionCleanup/ObjectivesSet");
  %numObjectives = Group::objectCount(%group);
  if (%playerId == %killerId) {
    for (%i = 0; %i < %numObjectives; %i++) {
      %obj = Group::getObject(%group, %i);
      if (GameBase::getDataName(%obj) == TeamDMObject && GameBase::getTeam(%obj) == -1) {
        %obj.suicides[%playerTeam]++;
        $teamScore[%playerTeam] += %obj.deathScore;
        ObjectiveMission::objectiveChanged(%obj);
      }
    }
  } else {
    for (%i = 0; %i < %numObjectives; %i++) {
      %obj = Group::getObject(%group, %i);
      %name = GameBase::getDataName(%obj);
      %t = GameBase::getTeam(%obj);
      if (%name == TeamDMObject && %t == %playerTeam) {
        %obj.totalDeaths++;
        $teamScore[%playerTeam] += %obj.deathScore;
        ObjectiveMission::objectiveChanged(%obj);
      }
      if (%name == TeamDMObject && %t == %killerTeam && %killerTeam != %playerTeam) {
        %obj.totalKills++;
        $teamScore[%killerTeam] += %obj.killScore;
        ObjectiveMission::objectiveChanged(%obj);
      }
    }
  }
  ObjectiveMission::checkScoreLimit();
}


function processMenuInitialPickTeam(%clientId, %team) {
   if ($Server::TourneyMode && $matchStarted)
      %team = -2;

   if(%team == -2)
   {
      Observer::enterObserverMode(%clientId);
   }
   if(%team == -1)
   {
      Game::assignClientTeam(%clientId);
      %team = Client::getTeam(%clientId);
   } else if ($Admin::enforceFairTeams && %team >= 0) {
     if (!TeamBalance::canPlayerSwitchToTeam(-1, %team)) {
       Game::assignClientTeam(%clientId);
       %team = Client::getTeam(%clientId);
     }
   }
   if(%team != -2)
   {
      GameBase::setTeam(%clientId, %team);
		if($TeamEnergy[%team] != "Infinite")
			$TeamEnergy[%team] += $InitialPlayerEnergy;
      %clientId.teamEnergy = 0;
      Client::setControlObject(%clientId, -1);
      Game::playerSpawn(%clientId, false);
   }
   if($Server::TourneyMode && !$CountdownStarted)
   {
      if(%team != -2)
      {
         bottomprint(%clientId, "<f1><jc>Press FIRE when ready.", $Server::TourneyCheckTime);
         %clientId.notready = true;
         %clientId.notreadyCount = "";
      }
      else
      {
         bottomprint(%clientId, "", 0);
         %clientId.notready = "";
         %clientId.notreadyCount = "";
      }
   }
}

function Game::assignClientTeam(%playerId) {
   if($teamplay)
   {
      %name = Client::getName(%playerId);
      %numTeams = getNumTeams();
      if($teamPreset[%name] != "")
      {
         if($teamPreset[%name] < %numTeams)
         {
            GameBase::setTeam(%playerId, $teamPreset[%name]);
            echo(Client::getName(%playerId), " was preset to team ", $teamPreset[%name]);
            return;
         }            
      }
      %leastTeam = TeamBalance::getAutomaticTeam(GameBase::getTeam(%playerId));

      if (%leastTeam != GameBase::getTeam(%playerId)) {
        GameBase::setTeam(%playerId, %leastTeam);
        echo(Client::getName(%playerId), " was automatically assigned to team ", %leastTeam);
      }
   }
   else
   {
      GameBase::setTeam(%playerId, 0);
   }
}


function Game::initialMissionDrop(%clientId) {
  Client::setGuiMode(%clientId, $GuiModePlay);

  if ($Server::TourneyMode)
    GameBase::setTeam(%clientId, -1);
  else {
    if (%clientId.observerMode == "observerFly" || %clientId.observerMode == "observerOrbit") {
      %clientId.observerMode = "observerOrbit";
      %clientId.guiLock = "";
      Observer::jump(%clientId);
      return;
    }
    %numTeams = getNumTeams();
    %curTeam = Client::getTeam(%clientId);

    if (%curTeam >= %numTeams || (%curTeam == -1 && (%numTeams < 2 || $Server::AutoAssignTeams)) )
      Game::assignClientTeam(%clientId);
  }    
  Client::setControlObject(%clientId, Client::getObserverCamera(%clientId));
  %camSpawn = Game::pickObserverSpawn(%clientId);
  Observer::setFlyMode(%clientId, GameBase::getPosition(%camSpawn), 
  GameBase::getRotation(%camSpawn), true, true);

  if (Client::getTeam(%clientId) == -1) {
    if (!$Admin::allowTeamchange) {
      Observer::enterObserverMode(%clientId);
    } else {
      %clientId.observerMode = "pickingTeam";

      if ($Server::TourneyMode && ($matchStarted || $matchStarting)) {
        %clientId.observerMode = "observerFly";

        if (%clientId.doClientScriptFS) {
          if (%clientId.usingClientScript)
            SpyModClientScript::firstSpawn(%clientId);
          if (%clientId.incompatibleClientScript)
            schedule("bottomprint("@%clientId@", \"<jc><f1>Sorry, but your version of SpyMod Client Script is incompatible with this server's version.\",5,true);",1);
          %clientId.doClientScriptFS = "";
        }

        return;
      } else if($Server::TourneyMode) {
        if ($Server::TeamDamageScale)
          %td = "ENABLED";
        else
          %td = "DISABLED";
        bottomprint(%clientId, "<jc><f1>Server is running in Competition Mode\nPick a team.\nTeam damage is " @ %td, 0);
      }
      Client::buildMenu(%clientId, "Pick a team:", "InitialPickTeam");
      if (getNumTeams() > 1) {
        Client::addMenuItem(%clientId, "0Observer", -2);
        Client::addMenuItem(%clientId, "1Automatic", -1);

        if ($Admin::enforceFairTeams) {
          %joinables = TeamBalance::getJoinableTeams(Client::getTeam(%clientId));
          for (%i = 0; (%t = getWord(%joinables, %i)) != -1; %i++)
            Client::addMenuItem(%clientId, (%i+2) @ getTeamName(%t), %t);
        } else {
          for(%i = 0; %i < getNumTeams(); %i = %i + 1)
            Client::addMenuItem(%clientId, (%i+2) @ getTeamName(%i), %i);
        }
      } else {
        Client::addMenuItem(%clientId, "0Observer", -2);
        Client::addMenuItem(%clientId, "1DM Team", 0);
      }
      %clientId.justConnected = "";
    }
  } else {

    Client::setSkin(%clientId, $Server::teamSkin[Client::getApparentTeam(%clientId)]);

    if (%clientId.justConnected) {
      Observer::printMOTD(%clientId);
      %clientId.observerMode = "justJoined";
      %clientId.justConnected = "";
    } else if(%clientId.observerMode == "justJoined") {
      centerprint(%clientId, "");
      %clientId.observerMode = "";
      Game::playerSpawn(%clientId, false);
    } else {
      Game::playerSpawn(%clientId, false);
    }
  }
  if ($TeamEnergy[Client::getTeam(%clientId)] != "Infinite")
    $TeamEnergy[Client::getTeam(%clientId)] += $InitialPlayerEnergy;

  %clientId.teamEnergy = 0;
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

  for (%cl = Client::getFirst(); %cl != -1; %cl = Client::getNext(%cl)) {
    if (%cl.observerMode == "pregame") {
      %cl.observerMode = "";
      Client::setControlObject(%cl, Client::getOwnedObject(%cl));
    }
    Game::refreshClientScore(%cl);
  }
  Game::checkTimeLimit();

  %group = nameToID("MissionCleanup/ObjectivesSet");
  for (%i = 0; (%obj = Group::getObject(%group, %i)) != -1; %i++)
    GameBase::virtual(%obj, "matchStarted");
}

exec("misc\\deathMessages.cs");