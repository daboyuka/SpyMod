function Observer::triggerUp(%client) {
  if (%client.observerMode == "dead") {
    if (%client.dieTime + $Server::respawnTime < getSimTime()) {
      if (Game::playerSpawn(%client, true)) {
        %client.observerMode = "";
        Observer::checkObserved(%client);
      }
    }
  } else if (%client.observerMode == "observerOrbit") {
    Observer::nextObservable(%client);
  } else if (%client.observerMode == "observerFly") {
    %camSpawn = Game::pickObserverSpawn(%client);
    Observer::setFlyMode(%client, GameBase::getPosition(%camSpawn), 
                         GameBase::getRotation(%camSpawn), true, true);
  } else if (%client.observerMode == "justJoined") {
    if (%client.isN00b) {
      Help::quikN00bTeacher(%client);
      %client.observerMode = "n00b";
    } else {
      %client.observerMode = "";
      Game::playerSpawn(%client, false);

      if (%client.doClientScriptFS) {
        if (%client.usingClientScript)
          SpyModClientScript::firstSpawn(%client);
        if (%client.incompatibleClientScript)
          schedule("bottomprint("@%client@", \"<jc><f1>Sorry, but your version of SpyMod Client Script is incompatible with this server's version.\",5,true);",1);
        %client.doClientScriptFS = "";
      }
    }
  } else if (%client.observerMode == "pregame" && $Server::TourneyMode) {
    if ($CountdownStarted) return;
    if (%client.notready) {
      %client.notready = "";
      MessageAll(0, Client::getName(%client) @ " is READY.");
      if (%client.notreadyCount < 3)
        bottomprint(%client, "<f1><jc>Waiting for match start (FIRE if not ready).", 0);
      else 
        bottomprint(%client, "<f1><jc>Waiting for match start.", 0);
    } else {
      %client.notreadyCount++;
      if (%client.notreadyCount < 4) {
        %client.notready = true;
        MessageAll(0, Client::getName(%client) @ " is NOT READY.");
        bottomprint(%client, "<f1><jc>Press FIRE when ready.", 0);
      }
      return;
    }
    Game::CheckTourneyMatchStart();
  } else if (%client.observerMode == "n00b") {
    if (!%client.training) {
      //%client.isN00b = false;
      %client.observerMode = "";
      Game::playerSpawn(%client, false);

      if (%client.doClientScriptFS) {
        if (%client.usingClientScript)
          SpyModClientScript::firstSpawn(%client);
        if (%client.incompatibleClientScript)
          schedule("bottomprint("@%client@", \"<jc><f1>Sorry, but your version of SpyMod Client Script is incompatible with this server's version.\",5,true);",1);
        %client.doClientScriptFS = "";
      }
    } else { %client.trainingStep++; Help::quikN00bTeacher(%client); }
  }
}

function Observer::printMOTD(%clientId) {
  if ($Server::JoinMOTDArgs[0] != "") %v0 = eval("%x=" @ $Server::JoinMOTDArgs[0] @ ";");
  if ($Server::JoinMOTDArgs[1] != "") %v1 = eval("%x=" @ $Server::JoinMOTDArgs[1] @ ";");
  if ($Server::JoinMOTDArgs[2] != "") %v2 = eval("%x=" @ $Server::JoinMOTDArgs[2] @ ";");
  centerprint(%clientId, sprintf($Server::JoinMOTD, %v0, %v1, %v2), 0);
}