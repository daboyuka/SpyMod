// Admin privilege table
// The value of each privilege variable indicates the minimum admin level required to execute that action. For example:
//
//   $AdminPrivileges["kick"] = $Powers::POWER_ADMIN;
//
// means that you must be at least an admin to kick someone. This means that admins and superadmins can kick, but not
// moderators. The "modifyPower" privilege is the privilege to grant a certain power, and to strip it as well (assuming the
// strippee is not a higher admin than the stripper [so rong...]). If a privilege variable is set to 0, it means that anyone
// can execute that command, even if they have no powers at all. If a privilege variable is set to -1 or not set at all, it
// means no one can execute that action, no matter what level of admin they are. Such actions must be manually executed by
// the server operator (for example, to grant uberadmin, one must go into the database file and edit it currently).
//
// Possible powers:
//   $Powers::POWER_MODERATOR  : Moderator or higher
//   $Powers::POWER_ADMIN      : Admin or higher
//   $Powers::POWER_SUPERADMIN : SuperAdmin or higher
//   $Powers::POWER_UBERADMIN  : UberAdmin or higher

$AdminPrivileges["adminMenu"] = $Powers::POWER_ADMIN;
$AdminPrivileges["serverRulesMenu"] = $Powers::POWER_ADMIN;
$AdminPrivileges["cheatMenu"] = $Powers::POWER_SUPERADMIN;
$AdminPrivileges["punishMenu"] = $Powers::POWER_MODERATOR;
$AdminPrivileges["annoyMenu"] = $Powers::POWER_SUPERADMIN;

$AdminPrivileges["forceVote"] = $Powers::POWER_ADMIN;
$AdminPrivileges["poll"] = $Powers::POWER_ADMIN;

$AdminPrivileges["setTimeLimit"] = $Powers::POWER_ADMIN;
$AdminPrivileges["setServerPassword"] = $Powers::POWER_SUPERADMIN;
$AdminPrivileges["resetServer"] = $Powers::POWER_SUPERADMIN;
$AdminPrivileges["clearN00bs"] = $Powers::POWER_UBERADMIN;
$AdminPrivileges["changeMission"] = $Powers::POWER_ADMIN;
$AdminPrivileges["forceTourneyStart"] = $Powers::POWER_ADMIN;
$AdminPrivileges["databaseAdmin"] = $Powers::POWER_SUPERADMIN;
$AdminPrivileges["databaseManage"] = $Powers::POWER_SUPERADMIN;

$AdminPrivileges["setTeamDamage"] = $Powers::POWER_ADMIN;
$AdminPrivileges["setTourneyMode"] = $Powers::POWER_ADMIN;
$AdminPrivileges["setObsGlobalChat"] = $Powers::POWER_ADMIN;
$AdminPrivileges["setAllowTeamchange"] = $Powers::POWER_ADMIN;
$AdminPrivileges["setEnforceFairTeams"] = $Powers::POWER_ADMIN;
$AdminPrivileges["setSpawnItems"] = $Powers::POWER_ADMIN;

$AdminPrivileges["forceTeamChange"] = $Powers::POWER_ADMIN;

$AdminPrivileges["kick"] = $Powers::POWER_ADMIN;
$AdminPrivileges["ban"] = $Powers::POWER_SUPERADMIN;
$AdminPrivileges["mute"] = $Powers::POWER_MODERATOR;
$AdminPrivileges["report"] = $Powers::POWER_MODERATOR;
$AdminPrivileges["redtext"] = $Powers::POWER_MODERATOR;
$AdminPrivileges["receiveAdminMsgs"] = $Powers::POWER_ADMIN;

$AdminPrivileges["modifyPowers"] = $Powers::POWER_SUPERADMIN;

$AdminPrivileges["modifyPower", $Powers::POWER_MODERATOR] = $Powers::POWER_SUPERADMIN;
$AdminPrivileges["modifyPower", $Powers::POWER_ADMIN] = $Powers::POWER_SUPERADMIN;
$AdminPrivileges["modifyPower", $Powers::POWER_SUPERADMIN] = $Powers::POWER_UBERADMIN;
$AdminPrivileges["modifyPower", $Powers::POWER_UBERADMIN] = -1; // Can't do this no matter what

$AdminPrivileges["setSADPassword"] = $AdminPrivileges["modifyPower", $Powers::POWER_SUPERADMIN]; // setting the SAD password requires the same admin level as granting SAD

$AdminPrivileges["changePassword"] = $Powers::POWER_SUPERADMIN;

$AdminPrivileges["immuneToVotekick"] = $Powers::POWER_SUPERADMIN;
$AdminPrivileges["bypassFloodProtection"] = $Powers::POWER_SUPERADMIN;