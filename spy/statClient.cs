exec("misc\\playerDatabase.cs");
exec("misc\\standardFuncs.cs");

function remoteStat(%server, %statName, %value) {
  if (%statName == "done") {
    PlayerDatabase::exportDownloaded();
    if ($StatClient::displayOnDone) {
      displayStats($StatClient::displayOptions[0], $StatClient::displayOptions[1],
                   $StatClient::displayOptions[2], $StatClient::displayOptions[3], true);

      $StatClient::displayOnDone = false;
      $StatClient::displayOptions[0] = "";
      $StatClient::displayOptions[1] = "";
      $StatClient::displayOptions[2] = "";
      $StatClient::displayOptions[3] = "";
    }
  } else {
    eval("$PlayerDatabase::" @ %statName @ " = \"" @ %value @ "\";");
  }
}

$StatClient::displayOnDone = false;
$StatClient::displayOptions[0] = "";
$StatClient::displayOptions[1] = "";
$StatClient::displayOptions[2] = "";
$StatClient::displayOptions[3] = "";
function getStats(%a, %b, %c, %d) {
  deleteVariables("$PlayerDatabase::*");
  $StatClient::displayOnDone = true;
  $StatClient::displayOptions[0] = %a;
  $StatClient::displayOptions[1] = %b;
  $StatClient::displayOptions[2] = %c;
  $StatClient::displayOptions[3] = %d;
  %numNames = tern(%a, tern(%b, $PlayerDatabase::MAX_NAMES, 1), 0);
  %numIPs   = tern(%c, tern(%d, $PlayerDatabase::MAX_NAMES, 1), 0);
  remoteEval(2048, "getStats", %numNames, %numIPs);
}