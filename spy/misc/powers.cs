$Powers::POWER_MODERATOR = 1;
$Powers::POWER_ADMIN = 2;
$Powers::POWER_SUPERADMIN = 4;
$Powers::POWER_UBERADMIN = 8;

//$Powers::POWER_PRIZE1 = 256;

$Powers::POWERS[0] = $Powers::POWER_MODERATOR;
$Powers::POWERS[1] = $Powers::POWER_ADMIN;
$Powers::POWERS[2] = $Powers::POWER_SUPERADMIN;
$Powers::POWERS[3] = $Powers::POWER_UBERADMIN;
//$Powers::POWERS[4] = $Powers::POWER_PRIZE1;

$Powers::POWER_NAMES[$Powers::POWER_MODERATOR]  = "Moderator";
$Powers::POWER_NAMES[$Powers::POWER_ADMIN]      = "Admin";
$Powers::POWER_NAMES[$Powers::POWER_SUPERADMIN] = "SuperAdmin";
$Powers::POWER_NAMES[$Powers::POWER_UBERADMIN] = "UberAdmin";
//$Powers::POWER_NAMES[$Powers::POWER_PRIZE1] = "FlamingMunkee";

$Powers::ALL_ADMIN_POWERS = $Powers::POWER_MODERATOR | $Powers::POWER_ADMIN | $Powers::POWER_SUPERADMIN | $Powers::POWER_UBERADMIN;

$Powers::ALL_POWERS = $Powers::ALL_ADMIN_POWERS;// | $Powers::POWER_PRIZE1;

$Powers::NUM_POWERS = 4;

function Powers::grantPowers(%clientId, %power) {
  %clientId.powers |= %power;
}

function Powers::stripPowers(%clientId, %power) {
  %clientId.powers &= ~%power;
}

function Powers::getNames(%powers) {
  %str = "";
  for (%i = 0; $Powers::POWERS[%i] != ""; %i++) {
    %p = $Powers::POWERS[%i];
    if (%powers & %p) %str = %str @ tern(%str == "", $Powers::POWER_NAMES[%p], ", " @ $Powers::POWER_NAMES[%p]);
  }
  if (%str == "") %str = "None";
  return %str;
}

function Powers::areValidPowers(%power) {
  return (%power | $Powers::ALL_POWERS) == $Powers::ALL_POWERS;
}

function Powers::simplifyAdminPowers(%adminPowers) {
  %topAPower = 0;
  for (%i = 0; $Powers::POWERS[%i] != ""; %i++) {
    if (($Powers::POWERS[%i] & $Powers::ALL_ADMIN_POWERS) && (%adminPowers & $Powers::POWERS[%i])) {
      if ($Powers::POWERS[%i] > %topAPower) {
        %adminPowers &= ~ %topAPower;
        %topAPower = $Powers::POWERS[%i];
      } else {
        %adminPowers = ~$Powers::POWERS[%i];
      }
    }
  }
  return %adminPowers;
}

function Powers::includeAllLowerAdminPowers(%adminPowers) {
  for (%i = 0; $Powers::POWERS[%i] != ""; %i++) {
    if (($Powers::POWERS[%i] & $Powers::ALL_ADMIN_POWERS) && $Powers::POWERS[%i] < (%adminPowers & $Powers::ALL_ADMIN_POWERS)) {
      %adminPowers |= $Powers::POWERS[%i];
    }
  }
  return %adminPowers;
}






function Powers::addNewPower(%bit, %name, %isAdminPower) {
  %i = $Powers::NUM_POWERS;

  $Powers::POWERS[%i] = %bit;
  $Powers::POWER_NAMES[%bit] = %name;

  if (%isAdminPower) $Powers::ALL_ADMIN_POWERS |= %bit;
  $Powers::ALL_POWERS |= %bit;

  $Powers::NUM_POWERS++;

  return %bit;
}