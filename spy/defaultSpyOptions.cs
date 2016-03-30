// --- Hacker Options -------------------------------------------------------------------------------------------------------

// Punishments for hacking
//
// Possible values are:
//   "ban X" ban for X seconds
//   "kick"  kick but don't ban
$H4XX0RPunishment::adminOnlyAccess = "bankick 10800";  // Unauthorized access to admin features
$H4XX0RPunishment::menuHack        = "bankick 86400";  // Invalid menu option selection
$H4XX0RPunishment::hexCrash        = "ban 604800 hexcrash"; // Trying to crash the server
$H4XX0RPunishment::tabsOrNewlines  = "kick";           // Newlines or tabs

$Server::showWallOfN00bs = true; // Adds the "Wall of Noob Hackers" option to the tab menu.  Highly recommended :)

// --- Player Database Options ----------------------------------------------------------------------------------------------

$PlayerDatabaseConstants::MAX_NAMES = 15;          // Maximum names that will be recorded for one player
$PlayerDatabaseConstants::MAX_IPS = 3;             // Maximum IP addresses that will be recorded for one player
$PlayerDatabaseOptions::displayStatsPause = 8;     // Seconds between pages displayed by displayStats()
$PlayerDatabaseOptions::displayStatsPageSize = 50; // Number of players displayed per page by displayStats()
$PlayerDatabaseOptions::acceleratedLookups = true; // Activates the database lookup table (creating another file in the config folder, databaseLookup.cs), which increases database lookup speed

$Server::countNetworkConnects = false;             // if true, increment the connection count for network connections

// --- Admin Options --------------------------------------------------------------------------------------------------------

$Admin::giveAdminToNetwork = true; // Whether to give superadmin to network connections

// --- Voting Options -------------------------------------------------------------------------------------------------------

$Voting::allowVoteToChangeMission = true; // Allow votes to change mission
$Voting::allowVoteToAdmin = false;        // Allow votes to admin players (strongly recommended to leave off)
$Server::AdminMinVotes = 4;               // Minimum number of votes required to admin a player
$Server::MinVotes = 1;                    // Minimum votes that are required; if there are less, the rest are considered no's
$Server::MinVotesPct = 0.5;               // Minimum percent of the players that must vote; if less vote, the rest are no's
$Server::MinVoteTime = 45;                // Time a player must wait to start another vote
$Server::VoteAdminWinMargin = 0.66;       // The percentage of people that must vote "yes" for the vote admin someone to pass
$Server::VoteFailTime = 30;               // Additional time a player must wait between votes for each failed vote
$Server::VoteWinMargin = 0.55;            // The percentage of people that must vote "yes" for the vote to pass
$Server::VotingTime = 20;                 // The time to allow for a vote

// --- Game Options ---------------------------------------------------------------------------------------------------------

$Server::warmupTime = 5;                // Seconds to wait ("warm up") at the beginning of each match
$Server::timeLimit = 25;                // Minutes a match lasts for
$Server::respawnTime = 2;               // Minimum number of seconds to wait after you die to respawn
$Server::TeamDamageScale = 1;           // Damage is multiplied by this if it is friendly fire (0 is team damage off, 1 is on)
$Server::AutoAssignTeams = "true";      // Automatically pick a team for new connections
$HalfDamageTime = 2;                    // Number of seconds of half damage after a player spawns
$CorpseTimeoutValue = 15;               // Number of seconds to wait before removing dead bodies
$Server::TourneyCheckTime = 120;        // This many seconds after a tourney starts, SpyMod will force the match to start if anyone is ready
$Admin::allowObserverGlobalChat = true; // Allow observers to use global chat and whisper to players
$Admin::allowTeamchange = true;         // Allow players to change teams
$Server::TourneyStartForceTime = 30;    // Forces tourneys to start after this many seconds, unless it's 0, in which case tourneys are never forced.

$Server::aimCorrectionEnabled = true;   // Determines whether or not the guns with fire at the crosshairs (as opposed to directly ahead)

$Game::maxSmokersPerTeam = 4; // The maximum number of smoke grenades each team is allowed to have on the field at once (team matches only)
$Game::maxSmokersForDM = 6;   // The maximum number of smoke grenades allowed on the field at once during a free-for-all (FFA matches only)

$Server::DefaultMission = "Spy3"; // The mission that is loaded when the last player leaves. "" means that the mission just cycles when the last player leaves.

// --- Server Options -------------------------------------------------------------------------------------------------------

$Server::HostName = "\xF8\xF8\xF8Bob is testing stuff...\xF8\xF8\xF8"; // Server name
$Server::HostPublicGame = "false";                                  // Whether the server be listed on server list

$Server::Info = "<jc><f1>Bob's SpyMod server<f0>\nAdmin:I AM BOB\nContact us: AIM screenname MattJCasey";

$Server::JoinMOTD = "<jc><f1>Welcome to I AM BOB's SpyMod Server\n\n<f0>Current server date and time: <f2>%1";
$Server::JoinMOTDArgs[0] = "Time::formatDate()";
$Server::JoinMOTDArgs[1] = "";
$Server::JoinMOTDArgs[2] = "";

$Server::MODInfo = "Welcome to SpyMod!\nGrapplers, magnums, machine guns, plastique explosives...\nJust have fun and behave!";

$Server::TourneyMode = false;             // Enable tournament mode
$Server::Port = 28003;                    // Socket port to run the server on
$Server::MaxPlayers = 32;                 // Max players that can be on at one time
$Server::FloodProtectionEnabled = "true"; // Enable chat flood protection
$pref::LastMission = "Spy3";              // First mission to use when server starts
$Console::logMode = 1;                    // if 1, Tribes will log to console.log in the Tribes directory
$Console::printLevel = 1;                 // 1 is normal, 2 is some debug, 3 is lots of debug, 4 is all debug (get ready)
$Server::password = "";

$Server::restartOnNoPlayers = true;       // Whether the server should reload the mission after the last client drops

$Server::CurrentMaster = 0;
$Server::Master0 = "t1m1.masters.dynamix.com:28000 t1m2.masters.dynamix.com:28000 t1m3.masters.dynamix.com:28000";
$Server::MasterAddressN0 = "t1m1.masters.dynamix.com:28000 t1m2.masters.dynamix.com:28000 t1m3.masters.dynamix.com:28000";
$Server::MasterName0 = "US Tribes Master";

$Server::teamName0 = "Blood Eagle";             // Team 1 name
$Server::teamName1 = "Diamond Sword";           // Team 2 name 
$Server::teamName2 = "Children of the Phoenix"; // Team 3 name
$Server::teamName3 = "Starwolf";                // Team 4 name
$Server::teamName4 = "Generic 1";               // Team 5 name
$Server::teamName5 = "Generic 2";               // Team 6 name
$Server::teamName6 = "Generic 3";               // Team 7 name
$Server::teamName7 = "Generic 4";               // Team 8 name
$Server::teamSkin0 = "beagle";                  // Team 1 skin
$Server::teamSkin1 = "dsword";                  // Team 2 skin
$Server::teamSkin2 = "cphoenix";                // Team 3 skin
$Server::teamSkin3 = "swolf";                   // Team 4 skin 
$Server::teamSkin4 = "base";                    // Team 5 skin
$Server::teamSkin5 = "base";                    // Team 6 skin
$Server::teamSkin6 = "base";                    // Team 7 skin
$Server::teamSkin7 = "base";                    // Team 8 skin

// --- Telnet Options -------------------------------------------------------------------------------------------------------

// If the port is non-zero, the server will accept telnet connections.  NEVER set this to a non-zero value without setting
// the telnet password, and be very careful with the port and password; if someone gets both, they can execute commands on
// your server
$TelnetPort = 0;      // Telnet port
$TelnetPassword = ""; // Telnet password

// --- IRC Reporting Options ------------------------------------------------------------------------------------------------
$Server::IRCReporting::server = "jasper.heavenlyplace.net";
$Server::IRCReporting::serverPort = 6667;
$Server::IRCReporting::room = "#BobsSpyModServer";
$Server::IRCReporting::nickname = "TehBob";
