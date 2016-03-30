$curVoteTopic = "";
$curVoteAction = "";
$curVoteOption = "";
$curVoteCount = 0;

// Admin::changeMissionMenu, processMenuCMission, processMenuCMType in adminMods.cs

// remoteAdminPassword is in adminMods.cs

// remoteSetTimeLimit, remoteSetPassword, remoteSetTeamInfo are in adminMods.cs

// remoteVoteYes and remoteVoteNo are in adminMods.cs

function Admin::startMatch(%admin) {
  if (%admin == -1 || Admin::isAdmin(%admin)) {
    if (!$CountdownStarted && !$matchStarted) {
      if (%admin == -1)
        messageAll(0, "Match start countdown forced by vote.");
      else
        messageAll(0, "Match start countdown forced by " @ Client::getName(%admin));

      Game::ForceTourneyMatchStart();
    }
  }
}

function Admin::setTeamDamageEnable(%admin, %enabled) {
  if (%admin == -1 || Admin::isAdmin(%admin)) {
    if (%enabled) {
      $Server::TeamDamageScale = 1;
      if (%admin == -1)
        messageAll(0, "Team damage set to ENABLED by consensus.");
      else
        messageAll(0, Client::getName(%admin) @ " ENABLED team damage.");
    } else {
      $Server::TeamDamageScale = 0;
      if (%admin == -1)
        messageAll(0, "Team damage set to DISABLED by consensus.");
      else
        messageAll(0, Client::getName(%admin) @ " DISABLED team damage.");
    }
  }
}

// Admin::kick is in adminPowers.cs

function Admin::setModeFFA(%clientId)
{
   if($Server::TourneyMode && (%clientId == -1 || Admin::isAdmin(%clientId)))
   {
      $Server::TeamDamageScale = 0;
      if(%clientId == -1)
         messageAll(0, "Server switched to Free-For-All Mode.");
      else
         messageAll(0, "Server switched to Free-For-All Mode by " @ Client::getName(%clientId) @ ".");

      $Server::TourneyMode = false;

      if ($Mission::prevTourneyMode != false)
        $Mission::prevTourneyMode = false;

      centerprintall(); // clear the messages
      if(!$matchStarted && !$countdownStarted)
      {
         if($Server::warmupTime)
            Server::Countdown($Server::warmupTime);
         else   
            Game::startMatch();
      }
   }
}

function Admin::setModeTourney(%clientId)
{
   if(!$Server::TourneyMode && (%clientId == -1 || Admin::isAdmin(%clientId)))
   {
      $Server::TeamDamageScale = 1;
      if(%clientId == -1)
         messageAll(0, "Server switched to Tournament Mode.");
      else
         messageAll(0, "Server switched to Tournament Mode by " @ Client::getName(%clientId) @ ".");

      $Server::TourneyMode = true;

      if ($Mission::prevTourneyMode != true)
        $Mission::prevTourneyMode = true;

      Server::nextMission();
   }
}

function Admin::voteFailed()
{
   $curVoteInitiator.numVotesFailed++;

   if($curVoteAction == "kick" || $curVoteAction == "admin")
      $curVoteOption.voteTarget = "";
}


// Admin::startVote, Admin::countVotes, Admin::voteSucceeded in adminMods.cs

// Game::menuRequest, processMenuOptions, and remoteSelectClient are in adminMods.cs

// processMenuPickTeam, processMenuFPickTeam, and processMenuOptions are in adminMods.cs

function processMenuKAffirm(%clientId, %opt)
{
   if(getWord(%opt, 0) == "yes")
      Admin::kick(%clientId, getWord(%opt, 1));
   Game::menuRequest(%clientId);
}

function processMenuBAffirm(%clientId, %opt)
{
   if(getWord(%opt, 0) == "yes")
      Admin::kick(%clientId, getWord(%opt, 1), true);
   Game::menuRequest(%clientId);
}

// processMenuAAffirm is in adminMods.cs

function processMenuRAffirm(%clientId, %opt)
{
   if(%opt == "yes" && Admin::isAdmin(%clientId))
   {
      messageAll(0, Client::getName(%clientId) @ " reset the server to default settings.");
      Server::clearOptions($ServerOptions::LEVEL_CURRENT);
      Server::clearOptions($ServerOptions::LEVEL_MAP_CURRENT);
      //Server::refreshData();
   }
   Game::menuRequest(%clientId);
}

// processMenuCTLimit is in adminMenu.cs

exec(adminMods);