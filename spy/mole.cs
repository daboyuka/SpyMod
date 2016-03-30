exec(teamstockdm);

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