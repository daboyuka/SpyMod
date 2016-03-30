// A client's real team is the native DarkStar one, obtained with Client::get/setTeam
// A client's apparent team is store in a field variable, obtained with Client::get/setApparentTeam

function Client::getApparentTeam(%client) {
  if (%client.apparentTeam != "") return %client.apparentTeam;
  else return Client::getTeam(%client);
}

function Client::setApparentTeam(%client, %team) {
  %client.apparentTeam = %team;
}

function Client::clearApparentTeam(%client) {
  %client.apparentTeam = "";
}

function Client::setTeamSilent(%client, %team) {
  Client::setInitialTeam(%client, %team);
  GameBase::setTeam(%client, %team);
  Client::setSkin(%client, $Server::teamSkin[%team]);
}

function GameBase::getApparentTeam(%object) {
  if (getObjectType(%object) == "Player") return Client::getApparentTeam(Player::getClient(%object));
  else if (%object.apparentTeam != "") return %object.apparentTeam;
  else return GameBase::getTeam(%object);
}