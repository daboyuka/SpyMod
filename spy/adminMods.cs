exec(trainer);

//$MAX
//$Admin::giveAdminToNetwork = true;
//$Admin::allowTeamchange = true;

//$Voting::allowVoteToChangeMission = true;
//$Voting::allowVoteToAdmin = false;

// ------ GENERAL ADMIN STUFF ------

function Admin::forceVote(%admin, %passOrFail) {
  if ($curVoteTopic == "") return;

  if (Admin::compareAdmins(%admin, $curVoteForcer) <= 0) {
    Client::sendMessage(%admin, 1, "The vote has already been forced by an equal or greater admin.");
    return;
  }

  $curVoteForcedTo = %passOrFail;
  $curVoteForceCount = $curVoteCount;
  $curVoteForcer = %admin;

  Admin::messageAdmins(Client::getName(%admin) @ " just forced the current vote to " @ $curVoteTopic @ " to " @ %passOrFail @ "!");
}

function Admin::voteSucceded() {
  if ($curVoteAction == "poll") return;

  $curVoteInitiator.numVotesFailed = "";
  if ($curVoteAction == "kick") {
    if ($curVoteOption.voteTarget)
      Admin::kick(-1, $curVoteOption);
  } else if ($curVoteAction == "admin" && $Voting::allowVoteToAdmin) {
    if ($curVoteOption.voteTarget) {
      Powers::grantPowers($curVoteOption, $Powers::POWER_ADMIN);
      messageAll(0, Client::getName($curVoteOption) @ " has become an administrator.");
      if ($curVoteOption.menuMode == "options")
        Game::menuRequest($curVoteOption);
    }
    $curVoteOption.voteTarget = false;
  } else if ($curVoteAction == "cmission") {
    messageAll(0, "Changing to mission " @ $curVoteOption @ ".");
    Vote::changeMission();
    Server::loadMission($curVoteOption);
  } else if ($curVoteAction == "tourney")
    Admin::setModeTourney(-1);
  else if ($curVoteAction == "ffa")
    Admin::setModeFFA(-1);
  else if ($curVoteAction == "etd")
    Admin::setTeamDamageEnable(-1, true);
  else if ($curVoteAction == "dtd")
    Admin::setTeamDamageEnable(-1, false);
  else if ($curVoteOption == "smatch")
    Admin::startMatch(-1);
}

function Admin::countVotes(%curVote) {
  if (%curVote != $curVoteCount)
    return;

  %votesFor = 0;
  %votesAgainst = 0;
  %votesAbstain = 0;
  %totalClients = 0;
  %totalVotes = 0;
  for (%cl = Client::getFirst(); %cl != -1; %cl = Client::getNext(%cl)) {
    %totalClients++;
    if (%cl.vote == "yes") {
      %votesFor++;
      %totalVotes++;
    } else if (%cl.vote == "no") {
      %votesAgainst++;
      %totalVotes++;
    } else
      %votesAbstain++;
  }

  if ($curVoteAction == "poll") {
    messageAll(0, "Poll: " @ $curVoteTopic @ " Results: " @ %votesFor @ " to " @ %votesAgainst @ " with " @ %totalClients - (%votesFor + %votesAgainst) @ " abstentions");
    $curVoteTopic = "";
    return;
  }

  %minVotes = floor($Server::MinVotesPct * %totalClients);
  if (%minVotes < $Server::MinVotes)
    %minVotes = $Server::MinVotes;

  if (%totalVotes < %minVotes) {
    %votesAgainst += %minVotes - %totalVotes;
    %totalVotes = %minVotes;
  }

  %margin = $Server::VoteWinMargin;
  if ($curVoteAction == "admin") {
    %margin = $Server::VoteAdminWinMargin;
    %totalVotes = %votesFor + %votesAgainst + %votesAbstain;
    if (%totalVotes < %minVotes)
      %totalVotes = %minVotes;
  }

  if ($curVoteForcedTo != "") {
    if ($curVoteForceCount == $curVoteCount) {
      messageAll(0, Client::getName($curVoteForcer) @ " forced the vote to " @ $curVoteForcedTo @ ".");
      if ($curVoteForcedTo == "pass") {
        Admin::voteSucceded();
      } else {
        Admin::voteFailed();
      }
    }
    $curVoteForcedTo = "";
    $curVoteForceCount = -1;
    $curVoteForcer = -1;
    $curVoteTopic = "";
    return;
  }

  if (%votesFor / %totalVotes >= %margin) {
    messageAll(0, "Vote to " @ $curVoteTopic @ " passed: " @ %votesFor @ " to " @ %votesAgainst @ " with " @ %totalClients - (%votesFor + %votesAgainst) @ " abstentions.");
    Admin::voteSucceded();
  } else {  // special team kick option:
    if ($curVoteAction == "kick") {  // check if the team did a majority number on him:
      %votesFor = 0;
      %totalVotes = 0;
      for (%cl = Client::getFirst(); %cl != -1; %cl = Client::getNext(%cl)) {
        if (GameBase::getTeam(%cl) == $curVoteOption.kickTeam) {
          %totalVotes++;
          if (%cl.vote == "yes")
            %votesFor++;
        }
      }
      if (%totalVotes >= $Server::MinVotes && %votesFor / %totalVotes >= $Server::VoteWinMargin) {
        messageAll(0, "Vote to " @ $curVoteTopic @ " passed: " @ %votesFor @ " to " @ %totalVotes - %votesFor @ ".");
        Admin::voteSucceded();
        $curVoteTopic = "";
        return;
      }
    }

    messageAll(0, "Vote to " @ $curVoteTopic @ " did not pass: " @ %votesFor @ " to " @ %votesAgainst @ " with " @ %totalClients - (%votesFor + %votesAgainst) @ " abstentions.");
    Admin::voteFailed();
  }
  $curVoteTopic = "";
}

function Admin::startVote(%clientId, %topic, %action, %option) {
  if (%action == "poll" && !Admin::hasPrivilege(%clientId, "poll")) {
    H4XX0R::hackerDetected(%clientId, "Attempting to use an admin-only feature: poll", $H4XX0RPunishment::adminOnlyAccess);
  }

  if (%clientId.lastVoteTime == "")
    %clientId.lastVoteTime = -$Server::MinVoteTime;

  // we want an absolute time here.
  %time = getIntegerTime(true) >> 5;
  %diff = %clientId.lastVoteTime + $Server::MinVoteTime - %time;

  if(%diff > 0) {
     Client::sendMessage(%clientId, 0, "You can't start another vote for " @ floor(%diff) @ " seconds.");
     return;
  }
  if ($curVoteTopic == "") {
    if (%clientId.numVotesFailed && %action != "poll")
      %time += %clientId.numVotesFailed * $Server::VoteFailTime;

    %clientId.lastVoteTime = %time;
    $curVoteInitiator = %clientId;
    $curVoteTopic = %topic;
    $curVoteAction = %action;
    $curVoteOption = %option;
    if (%action == "kick")
      $curVoteOption.kickTeam = Client::getTeam($curVoteOption);
    $curVoteCount++;

    if (%action != "poll") {
      bottomprintall("<jc><f1>" @ Client::getName(%clientId) @ " <f0>initiated a vote to <f1>" @ $curVoteTopic, 10, true);
      messageAll(1, Client::getName(%clientId) @ " initiated a vote to " @ $curVoteTopic);
      echo(Client::getName(%clientId) @ " initiated a vote to " @ $curVoteTopic);
    } else {
      bottomprintall("<jc><f1>" @ Client::getName(%clientId) @ " <f0>initiated a poll: <f1>" @ $curVoteTopic, 10, true);
      messageAll(1, Client::getName(%clientId) @ " initiated a poll: " @ $curVoteTopic);
      echo(Client::getName(%clientId) @ " initiated a poll: " @ $curVoteTopic);
    }

    for (%cl = Client::getFirst(); %cl != -1; %cl = Client::getNext(%cl))
      %cl.vote = "";

    if (%action != "poll") %clientId.vote = "yes";

    for (%cl = Client::getFirst(); %cl != -1; %cl = Client::getNext(%cl))
      if (%cl.menuMode == "options")
        Game::menuRequest(%clientId);

    schedule("Admin::countVotes(" @ $curVoteCount @ ", true);", $Server::VotingTime, 35);
  } else {
    Client::sendMessage(%clientId, 0, "Voting already in progress.");
  }
}

function Admin::startPoll(%clientId, %topic) {
  clearPrintLock(%clientId);
  Admin::startVote(%clientId, %topic, "poll", "");
  %clientId.redirectChat = "";
}

// ------ Remote functions ------
function remoteSelectClient(%clientId, %selId) {
   if (%clientId.nextClientSel > getSimTime()) return;
   %client.nextClientSel = getSimTime() + 0.125;

   if (String::getSubStr(%selId, 4, 1) != "" || %selId < 2049 || %selId > 2048 + $Server::maxPlayers || %selId != floor(%selId)) {
     H4XX0R::hackerDetected(%clientId, "Attempting to use a menu hack with invalid selected client", $H4XX0RPunishment::menuHack);
     return;
   }

   if (%clientId.selClient != %selId) {
      %clientId.selClient = %selId;
      if (%clientId.menuMode == "options")
        Game::menuRequest(%clientId);
      if (%clientId.menuMode == "votingOptions")
        Menu::votingMenu(%clientId);
      if (%clientId.menuMode == "manageOptions")
        Menu::manageMenu(%clientId);
      if (%clientId.menuMode == "annoyOptions")
        Menu::annoyMenu(%clientId);
      if (%clientId.menuMode == "punishOptions")
        Menu::punishMenu(%clientId);
      if (%clientId.menuMode == "cheatOptions")
        Menu::cheatMenu(%clientId);
      if (%clientId.menuMode == "clientScript")
        Menu::clientScriptMenu(%clientId);

      remoteEval(%clientId, "setInfoLine", 1, "Player Info for " @ Client::getName(%selId) @ ":");
      remoteEval(%clientId, "setInfoLine", 2, "Real Name: " @ $Client::info[%selId, 1]);
      remoteEval(%clientId, "setInfoLine", 3, "Email Addr: " @ $Client::info[%selId, 2]);
      remoteEval(%clientId, "setInfoLine", 4, "Tribe: " @ $Client::info[%selId, 3]);
      remoteEval(%clientId, "setInfoLine", 5, "URL: " @ $Client::info[%selId, 4]);

      if (%selId.powers&$Powers::ALL_ADMIN_POWERS > 0) {
        remoteEval(%clientId, "setInfoLine", 6, "Admin Level: " @
                   Powers::getNames(Powers::simplifyAdminPowers(%selId.powers&$Powers::ALL_ADMIN_POWERS)));
      } else if (getWord($Client::info[%selId, 5], 0) != "password") {
        remoteEval(%clientId, "setInfoLine", 6, "Other: " @ $Client::info[%selId, 5]);
      } else {
        remoteEval(%clientId, "setInfoLine", 6, "");
      }
   }
}

function remoteSetTimeLimit(%client, %time) {
  if (!Admin::hasPrivilege(%client, "setTimeLimit")) {
    H4XX0R::hackerDetected(%clientId, "Attempting to use admin-only feature: set time limit", $H4XX0RPunishment::adminOnlyAccess);
    return;
  }

  %time = floor(%time);
  if (%time == $Server::timeLimit || (%time != 0 && %time < 1))
    return;
  $Server::timeLimit = %time;

  if (%time)
    messageAll(0, Client::getName(%client) @ " changed the time limit to " @ %time @ " minute(s).");
  else
    messageAll(0, Client::getName(%client) @ " disabled the time limit.");
}


function remoteSetTeamInfo(%client, %team, %teamName, %skinBase) {
  if (!Admin::hasPrivilege(%client, "setTeamInfo")) {
    //H4XX0R::hackerDetected(%clientId, "Attempting to use admin-only feature: set team info", $H4XX0RPunishment::adminOnlyAccess);
    Client::sendMessage(%client, 1, "You are not a high enough admin to set team info.");
    return;
  }
  if (%team < 0 || %team > 7) {
    Client::sendMessage(%client, 1, "Invalid team number");
    return;
  }
  if (String::getSubStr(%teamName, 64, 1) != "") {
    Client::sendMessage(%client, 1, "Team name too long; max characters 64");
    return;
  }
  if (String::getSubStr(%skinBase, 32, 1) != "") {
    Client::sendMessage(%client, 1, "Skin name too long; max characters 32");
    return;
  }

  $Server::teamName[%team] = %teamName;
  $Server::teamSkin[%team] = %skinBase;
  messageAll(0, "Team " @ %team @ " is now \"" @ %teamName @ "\" with skin: "
              @ %skinBase @ " courtesy of " @ Client::getName(%client) @ ".  Changes will take effect next mission.");
}

function remoteSetPassword(%client, %password) {
  if (!Admin::hasPrivilege(%client, "setServerPassword")) {
    //H4XX0R::hackerDetected(%clientId, "Attempting to use admin-only feature: set password", $H4XX0RPunishment::adminOnlyAccess);
    Client::sendMessage(%client, 1, "You are not a high enough admin to set the server password");
    return;
  }
  $Server::Password = %password;
}

function remoteSetSADPassword(%client, %password) {
  // If you are setting the SAD password from the server console, use:
  //   setSADPassword("your pass here");
  // You can also set the UberAdmin password with this:
  //   setUADPassword("your pass here");
  if (!Admin::hasPrivilege(%client, "setSADPassword")) {
    Client::sendMessage(%client, 1, "You are not a high enough admin to set the SAD password");
    return;
  }
  setSADPassword(%password);
}

function remoteAdminPassword(%client, %password) {
   if ($AdminPassword != "" && %password == $AdminPassword) {
      Powers::grantPowers(%client, $Powers::POWER_SUPERADMIN);
   }
}

function remoteUberAdminPassword(%client, %password) {
   if ($UberAdminPassword != "" && %password == $UberAdminPassword) {
      Powers::grantPowers(%client, $Powers::POWER_UBERADMIN);
   }
}

function remoteVoteNo(%clientId) {
  if (%clientId.training) {
    %clientId.trainingStep++;
    if (%clientId.trainingType == "quick")        Help::quikN00bTeacher(%clientId);
    if (%clientId.trainingType == "full")         Help::n00bTeacher(%clientId);
    if (%clientId.trainingType == "grappler")     Help::grapplerTeacher(%clientId);
    if (%clientId.trainingType == "passwordhelp") Help::passwordHelp(%clientId);
    if (%clientId.trainingType == "modinfo")      Help::modInfo(%clientId);
    if (%clientId.trainingType == "bindHelp")     Help::bindHelp(%clientId);
  } else {
    %clientId.vote = "no";
    clearPrintLock(%clientId);
    centerprint(%clientId, "", 0);
  }
}

function remoteVoteYes(%clientId) {
  if (%clientId.training) {
    if (%clientId.trainingType == "quick")        Help::quikN00bTeacher(%clientId);
    if (%clientId.trainingType == "full")         Help::n00bTeacher(%clientId);
    if (%clientId.trainingType == "grappler")     Help::grapplerTeacher(%clientId);
    if (%clientId.trainingType == "passwordhelp") Help::passwordHelp(%clientId);
    if (%clientId.trainingType == "modinfo")      Help::modInfo(%clientId);
    if (%clientId.trainingType == "bindHelp")     Help::bindHelp(%clientId);
  } else {
    %clientId.vote = "yes";
    clearPrintLock(%clientId);
    centerprint(%clientId, "", 0);
  }
}

exec(adminMenu);
exec(adminFuncs);
exec(adminPowers);
exec(adminDatabase);
exec(adminLog);
exec(adminPrivileges);

