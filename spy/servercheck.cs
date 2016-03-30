function serverCheck() {
  deleteVariables("$ServerStatus::*");
  exec(spyServerStatus);

  exec(spyModeratorReports);
  exec(spyH4xx0rs);

  echo("--------------------------------");
  echo("Begin status report");
  echo("--------------------------------");

  echo("Since last status update:");
  echo("  " @ ($PlayerDatabase::numPlayers - $ServerStatus::numPlayers) @ " new players");
  echo("  " @ (($PlayerDatabase::numConnects - $ServerStatus::numConnects) - ($PlayerDatabase::numPlayers - $ServerStatus::numPlayers)) @ " existing players");
  echo("  " @ ($PlayerDatabase::numConnects - $ServerStatus::numConnects) @ " total connections");

  if ($PlayerDatabase::mostClients != $ServerStatus::mostClients)
    echo("  New simultaneous client record: " @ $PlayerDatabase::mostClients @ "!!!");



  if (%q = ($H4XX0R::numHackers - $ServerStatus::numHackers))
    echo("  " @ %q @ " hackers:");
  else
    echo("  0 hackers");

  for (%i = $ServerStatus::numHackers; %i < $H4XX0R::numHackers; %i++) {
    echo("   -" @ $H4XX0R::hackers[%i, "name"]);
    echo("    " @ $H4XX0R::hackers[%i, "ip"]);
    echo("    " @ $H4XX0R::hackers[%i, "reason"]);
  }



  if (%q = ($Moderator::numReports - $ServerStatus::numReports))
    echo("  " @ %q @ " moderator reports:");
  else
    echo("  0 moderator reports");

  for (%i = $ServerStatus::numReports; %i < $Moderator::numReports; %i++) {
    echo("   -" @ $Moderator::report[%i]);
  }

  echo("--------------------------------");
  echo("End status report");
  echo("--------------------------------");
}

function updateServerStatus() {
  exec(spyModeratorReports);
  exec(spyH4xx0rs);

  $ServerStatus::numPlayers  = $PlayerDatabase::numPlayers;
  $ServerStatus::numConnects = $PlayerDatabase::numConnects;
  $ServerStatus::mostClients = $PlayerDatabase::mostClients;
  $ServerStatus::numHackers  = $H4XX0R::numHackers;
  $ServerStatus::numReports  = $Moderator::numReports;

  export("$ServerStatus::*", "config\\spyServerStatus.cs", false);
}