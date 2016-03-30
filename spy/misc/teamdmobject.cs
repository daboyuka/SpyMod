StaticShapeData TeamDMObject {
  description = "";
  shapeFile = "breath";
  visibleToSensor = false;
};

function TeamDMObject::objectiveInit(%this) {
  if (GameBase::getTeam(%this) == -1) {
    for (%i = 0; %i < getNumTeams(); %i++) %this.suicides[%i] = 0;
  } else {
    %this.totalKills = 0;
    %this.totalDeaths = 0;
  }
  return true;
}

function TeamDMObject::getObjectiveString(%this, %forTeam) {
   %thisTeam = GameBase::getTeam(%this);

   if ($missionComplete) {
     if (%thisTeam == -1)
       return "Your team had " @ %this.suicides[%forTeam] @ " suicides!";
     else if (%thisTeam == %forTeam)
       return "Your team had " @ %this.totalKills @ " kills and " @ %this.totalDeaths @ " deaths!";
     else
       return getTeamName(%thisTeam) @ " had " @ %this.totalKills @ " kills and " @ %this.totalDeaths @ " deaths!";
   } else {
     if (%thisTeam == -1)
       return "Your team has had " @ %this.suicides[%forTeam] @ " suicides so far!";
     else if (%thisTeam == %forTeam)
       return "Your team has " @ %this.totalKills @ " kills and " @ %this.totalDeaths @ " deaths so far!";
     else
       return getTeamName(%thisTeam) @ " has " @ %this.totalKills @ " kills and " @ %this.totalDeaths @ " deaths so far!";
   }
}