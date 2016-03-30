// $Mission::teamPlayerRatio[team] = ratio

// fills variables $teamPlayerCount[team] where team is between 0 and getNumTeams() - 1
function TeamBalance::getTeamPlayerCounts() {
  for (%t = 0; %t < getNumTeams(); %t++) $teamPlayerCount[%t] = 0;

  for (%c = Client::getFirst(); %c != -1; %c = Client::getNext(%c)) {
    %t = Client::getTeam(%c);
    if (%t >= 0 && %t < getNumTeams()) {
      $teamPlayerCount[%t]++;
    }
  }
}

function TeamBalance::canPlayerSwitchToTeam(%oldTeam, %newTeam) {
  if ($Mission::teamPlayerRatio[%newTeam] == 0) return false;

  TeamBalance::getTeamPlayerCounts();
  if (%oldTeam >= 0) $teamPlayerCount[%oldTeam]--;

  %nt = getNumTeams();
  %p = 1;
  for (%t = 0; %t < %nt; %t++) %p *= $Mission::teamPlayerRatio[%t];

  %min = -1;
  for (%t = 0; %t < %nt; %t++) {
    %teamPlayerBalance[%t] = $teamPlayerCount[%t] * (%p / $Mission::teamPlayerRatio[%t]);
    if (%teamPlayerBalance[%t] < %min || %min == -1) %min = %teamPlayerBalance[%t];
  }

  return (%teamPlayerBalance[%newTeam] == %min);
}

function TeamBalance::getJoinableTeams(%oldTeam) {
  TeamBalance::getTeamPlayerCounts();
  if (%oldTeam >= 0) $teamPlayerCount[%oldTeam]--;

  %nt = getNumTeams();
  %p = 1;
  for (%t = 0; %t < %nt; %t++) {
    if ($Mission::teamPlayerRatio[%t] == 0) continue;
    %p *= $Mission::teamPlayerRatio[%t];
  }

  %min = -1;
  %joinables = "";
  for (%t = 0; %t < %nt; %t++) {
    if ($Mission::teamPlayerRatio[%t] == 0) continue;
    %teamPlayerBalance[%t] = $teamPlayerCount[%t] * (%p / $Mission::teamPlayerRatio[%t]);

    if (%teamPlayerBalance[%t] < %min || %min == -1) {
      %min = %teamPlayerBalance[%t];
      %joinables = %t;
    } else if (%teamPlayerBalance[%t] == %min) {
      %joinables = %joinables @ " " @ %t;
    }
  }

  return %joinables;
}

function TeamBalance::getAutomaticTeam(%oldTeam) {
  %joinables = TeamBalance::getJoinableTeams(%oldTeam);

  %leastScore = $teamScore[getWord(%joinables, 0)];
  %leastScorer = 0;
  for (%i = 1; (%t = getWord(%joinables, %i)) != -1; %i++) {
    if (%leastScore > $teamScore[%t]) {
      %leastScorer = %i;
      %leastScore = $teamScore[%t];
    }
  }

  return getWord(%joinables, %leastScorer);
}