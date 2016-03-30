//
// - Server Options -
//
// There are different levels of options in SpyMod, listed below in increasing priority (eg later entries override earlier):
//
//   * Default: these options are used as a fallback for any options not defined in Standard. They are never cleared.
//   * Standard: these options are also loaded when the server starts. They are never cleared.
//   * Current: these options set by vote or admin. They are cleared when the server restarts or all players drop.
//   * Map: these options are set by the current map. They are cleared after each match.
//   * Map current: these options are set by an admin for the current map only. They are cleared when a different map is loaded.
//
//

$ServerOptions::LEVEL_MAP_CURRENT = 0;
$ServerOptions::LEVEL_MAP = 1;
$ServerOptions::LEVEL_CURRENT = 2;
$ServerOptions::LEVEL_STANDARD = 3;
$ServerOptions::LEVEL_DEFAULT = 4;
$ServerOptions::numLevels = 5;

//for (%i = 0; %i < $ServerOptions::numLevels; %i++) {
//  deleteObject("ServerOptions" @ %i);
//  $ServerOptions[%i] = newObject("ServerOptions" @ %i, SimSet);
//}

function Server::getOption(%optName) {
  for (%i = 0; %i < $ServerOptions::numLevels; %i++) {
    Server::ensureOptionLevelExists(%i);

    %opt = ("ServerOptions"@%i).option[%optName];
    if (%opt != "") return %opt;
  }
  return "";
}

function Server::setOption(%level, %optName, %optValue) {
  if (%level < 0 || %level >= $ServerOptions::numLevels) return;

  Server::ensureOptionLevelExists(%level);
  ("ServerOptions"@%level).option[%optName] = %optValue;
}

function Server::clearOptions(%level) {
  if (%level < 0 || %level >= $ServerOptions::numLevels) return;

  if (isObject("ServerOptions"@%level)) deleteObject("ServerOptions"@%level);
  newObject("ServerOptions"@%level, SimSet);
}

function Server::ensureOptionLevelExists(%level) {
	if (!isObject("ServerOptions"@%level)) createObject("ServerOptions"@%level, SimSet);
}

function Server::exportOptions(%level, %filename) {
  if (%level < 0 || %level >= $ServerOptions::numLevels) return;

  if (isObject("ServerOptions"@%level)) {
    if (isObject("ServerOptions")) deleteObject("ServerOptions");
    if (isObject("ServerOptions")) renameObject("ServerOptions"@%level, "ServerOptions");
    exportObjectToScript("ServerOptions", %filename, true);
  }
}

function Server::importOptions(%level, %filename) {
  if (%level < 0 || %level >= $ServerOptions::numLevels) return;

  if (File::findFirst(%filename) != "") {
	echo("0000000");
	if (isObject("ServerOptions"@%level)) deleteObject("ServerOptions"@%level);
	echo("1111111");
    exec(%filename);
	echo("2222222");
    if (isObject("ServerOptions")) renameObject("ServerOptions", "ServerOptions"@%level);
	echo("3333333");
  }
}



function Server::loadOptions() {
  exec(defaultSpyOptions);
  exec(spyOptions);

  Server::importOptions($ServerOptions::LEVEL_DEFAULT, "defaultSpyOptions2.cs");
  echo("IMPORT DONE");
  Server::importOptions($ServerOptions::LEVEL_STANDARD, "spyOptions2.cs");
}

function Server::storeOptions() {
  Server::exportOptions($ServerOptions::LEVEL_STANDARD, "config\\spyOptions2.cs");
}