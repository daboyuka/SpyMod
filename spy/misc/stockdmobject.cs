StaticShapeData TeamStockDMObject {
  description = "";
  shapeFile = "breath";
  visibleToSensor = false;
};

function TeamStockDMObject::objectiveInit(%this) {
  for (%t = 0; %t < getNumTeams(); %t++) %this.playersLeft[%forTeam] = %this.teamPwnz0red[%forTeam] = 0;
  return true;
}

function TeamStockDMObject::getObjectiveString(%this, %forTeam) {
   if (getNumTeams() == 2) {
     %teams2kill = getTeamName(1 - %forTeam); // The other team
   } else {
     %teams2kill = "all opposing teams";
   }

   if ($missionComplete) {
     if (%this.playersLeft[%forTeam] == 0)
       return "<Bskull_big.bmp>\n" @ " Your team was eliminated!";
     else if (%this.teamPwnz0red[%forTeam])
       return "<Bskull_big.bmp>\n" @ " Your team eliminated "@%teams2kill@" and ended with " @ %this.playersLeft[%forTeam] @ " players left!";
     else
       return "<Bskull_big.bmp>\n" @ " Your team survived with " @ %this.playersLeft[%forTeam] @ " players left!";
   } else if ($matchStarted) {
     if (%this.playersLeft[%forTeam] == 0)
       return "<Bskull_big.bmp>\n" @ " Your team was eliminated!";
     else
       return "<Bskull_big.bmp>\n" @ " Eliminate "@%teams2kill@" to win the mission!";
   } else {
     return "<Bskull_big.bmp>\n" @ " Eliminate "@%teams2kill@" to win the mission!";
   }
}

function TeamStockDMObject::clientKilled(%this, %playerId, %killerId) {
  if (!isClientObject(%playerId)) { echo("IGNORING AI"); return; }

  %playerId.totalDeaths++;
  if (%playerId.totalDeaths >= $SDM::numLives) {
    %playerId.elim = true;
    %playerId.elimedFromTeam = Client::getTeam(%playerId);
    Observer::enterObserverMode(%playerId);
    if (%killerId != 0 && %killerId != -1 && Client::getName(%killerId) != "")
      messageAll(0, Client::getName(%playerId) @ " has been eliminated by " @ Client::getName(%killerId) @ "!");
    else
      messageAll(0, Client::getName(%playerId) @ " has been eliminated!");
  }
  Game::refreshClientScore(%playerId);
  if (%killerId != 0) Game::refreshClientScore(%killerId);

  SDMTEAM::checkTeams(%this);

  ObjectiveMission::ObjectiveChanged(%this);
}

function TeamStockDMObject::clientDropped(%this, %clientId) {
  echo("DROP: " @ %clientId @ ", num players: " @ getNumClients());
  %clientId.elim = true;

  SDMTEAM::checkTeams(%this);

  ObjectiveMission::ObjectiveChanged(%this);
}

function TeamStockDMObject::timeLimitReached(%this) {
  for (%c = Client::getFirst(); %c != -1; %c = Client::getNext(%c)) {
    if (%c.elim && %c.elimedFromTeam != "") {
      Client::setInitialTeam(%c, %c.elimedFromTeam);
      GameBase::setTeam(%c, %c.elimedFromTeam);
    }
  }
}

function SDMTEAM::checkTeams(%this) {
  if (!$matchStarted) return;
  if (%this.matchOver) return;

  %teams = 0;

  for (%t = 0; %t < getNumTeams(); %t++) %this.playersLeft[%t] = 0;

  for (%c = Client::getFirst(); %c != -1; %c = Client::getNext(%c)) {
    if (Client::getTeam(%c) >= 0 && !%c.elim) {
      if (%this.playersLeft[Client::getTeam(%c)] == 0) %teams++;
      %this.playersLeft[Client::getTeam(%c)]++;
      %lastTeam = Client::getTeam(%c);
    }
  }

  if (%teams == 0) { // Er, oops...we fixy now...
    messageAll(1,"Not enough teams to play team stock death match, switching missions...");
    schedule("Server::loadMission("@$nextMission[$missionName]@");",3);
    return;
  }
  if (%teams == 1) {
    %this.teamPwnz0red[%lastTeam] = true;
    messageAll(1, getTeamName(%lastTeam) @ " has eliminated all opposing teams!");
    %this.matchOver = true;
    schedule("SDMTEAM::teamWon("@%this@","@%lastTeam@");", 5);
  }
}

function SDMTEAM::teamWon(%this, %team) {
  $teamScore[%team] = $teamScoreLimit; // WINZ0R!!!
  ObjectiveMission::checkScoreLimit();
}

function TeamStockDMObject::matchStarted(%this) {
  for (%c = Client::getFirst(); %c != -1; %c = Client::getNext(%c)) {
    %livesRemaining = $SDM::numLives - %c.scoreDeaths - 1;
    bottomprint(%c, "<JC><F1>--- " @ %livesRemaining @ " " @ tern(%livesRemaining!=1,"lives","life") @ " remaining ---");
  }
}