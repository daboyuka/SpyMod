function ComUplink::onUse(%player) {
  %client = GameBase::getControlClient(%player);
  %g = nameToID("MissionCleanup\\Cameras");
  %n = Group::objectCount(%g);
  for (%i = 0; %i < %n; %i++) {
    %o = Group::getObject(%g, %i);
    echo(%client, ",", %o.deployerClient, ",", GameBase::getTeam(%o));
//    if (getNumTeams() == 1 && %o.deployerClient == %client) {
//      Client::takeControl(%client, %o);
//      break;
//    }
//    if (getNumTeams() > 1 && GameBase::getTeam(%o) == Client::getTeam(%client)) {
//      Client::takeControl(%client, %o);
//      break;
//    }
    if (Client::takeControl(%client, %o)) return;
  }
}