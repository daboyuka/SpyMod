StaticShapeData WinByDefaultObject {
  description = "";
  shapeFile = "breath";
  visibleToSensor = false;
};

function WinByDefaultObject::objectiveInit(%this) {
  return true;
}
//WinByDefault
function WinByDefaultObject::getObjectiveString(%this, %forTeam) {
   %team = GameBase::getTeam(%this);
   %teamName = getTeamName(%team);
   if ($missionComplete) {
     if (%team == %forTeam) {
       if (%this.wonByDefault)
         return "<Bskull_big.bmp>\n" @ " Your team survived until time ran out!";
       else
         return "<Bskull_big.bmp>\n" @ " The mission ended before time ran out.";
     } else {
       if (%this.wonByDefault)
         return "<Bskull_big.bmp>\n" @ " " @ %teamName @ " survived until time ran out!";
       else
         return "<Bskull_big.bmp>\n" @ " The mission ended before time ran out.";
     }
   } else {
     if (%forTeam == -1)
       return "<Bskull_big.bmp>\n" @ " Team " @ %teamName @ " will win if time runs out!";
     else if (%team == %forTeam)
       return "<Bskull_big.bmp>\n" @ " Survive until time runs out to win the mission!";
     else
       return "<Bskull_big.bmp>\n" @ " Win the mission before time runs out or " @ %teamName @ " will win!";
   }
}

function WinByDefaultObject::timeLimitReached(%this) {
  if ($timeLimitReached) {
    %this.wonByDefault = true;
    $teamScore[GameBase::getTeam(%this)] = $teamScoreLimit; // WINZ0R!!!
  }
//  ObjectiveMission::checkScoreLimit();
}