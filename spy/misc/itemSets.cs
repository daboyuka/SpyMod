$ITEM_SET_UNSET = -2;

$ITEM_SET_CUSTOM = -1;

$ITEM_SET_ALL = 0;
$ITEM_SET_DM = 1;
$ITEM_SET_NO_XPLO = 2;
$ITEM_SET_HIGH_CALIBER = 3;
$ITEM_SET_RAPID_FIRE = 4;
$ITEM_SET_XPLO = 5;
$ITEM_SET_MG_ONLY = 6;
$ITEM_SET_DM_MG_AND_KNIFE = 7;
$ITEM_SET_DM_KNIFE_ONLY = 8;
$ITEM_SET_MG_AND_KNIFE = 9;
$ITEM_SET_DM_NOGRAP = 10;

$itemSetType = $ITEM_SET_UNSET; // the item set has not yet been picked at this point

%i = -1;
$ITEM_SET[$ITEM_SET_ALL, %i++] = "30 MagnumAmmo";
$ITEM_SET[$ITEM_SET_ALL, %i++] = "150 DragonAmmo";
$ITEM_SET[$ITEM_SET_ALL, %i++] = "320 TornadoAmmo";
$ITEM_SET[$ITEM_SET_ALL, %i++] = "50 MG27Ammo";
$ITEM_SET[$ITEM_SET_ALL, %i++] = "60 ChallengerAmmo";
$ITEM_SET[$ITEM_SET_ALL, %i++] = "50 MagdalonAmmo";
$ITEM_SET[$ITEM_SET_ALL, %i++] = "4 GrenadeLauncherAmmo";
$ITEM_SET[$ITEM_SET_ALL, %i++] = "2 DoomSlayerAmmo";
$ITEM_SET[$ITEM_SET_ALL, %i++] = "3 RocketLauncherAmmo";
$ITEM_SET[$ITEM_SET_ALL, %i++] = "16 Shotgun2Ammo";
$ITEM_SET[$ITEM_SET_ALL, %i++] = "80 FlamerAmmo";
$ITEM_SET[$ITEM_SET_ALL, %i++] = "80 RaiderAmmo";
$ITEM_SET[$ITEM_SET_ALL, %i++] = "6 AGP84Ammo";
$ITEM_SET[$ITEM_SET_ALL, %i++] = "1 Binoculars";
$ITEM_SET[$ITEM_SET_ALL, %i++] = "1 Grappler";
$ITEM_SET[$ITEM_SET_ALL, %i++] = "5 PlasticMineAmmo";
$ITEM_SET[$ITEM_SET_ALL, %i++] = "1 Magnum";
$ITEM_SET[$ITEM_SET_ALL, %i++] = "1 Dragon";
$ITEM_SET[$ITEM_SET_ALL, %i++] = "1 Tornado";
$ITEM_SET[$ITEM_SET_ALL, %i++] = "1 MG27";
$ITEM_SET[$ITEM_SET_ALL, %i++] = "1 Challenger";
$ITEM_SET[$ITEM_SET_ALL, %i++] = "1 Magdalon";
$ITEM_SET[$ITEM_SET_ALL, %i++] = "1 Shotgun2";
$ITEM_SET[$ITEM_SET_ALL, %i++] = "1 GrenadeLauncher2";
$ITEM_SET[$ITEM_SET_ALL, %i++] = "1 RocketLauncher";
$ITEM_SET[$ITEM_SET_ALL, %i++] = "1 DoomSlayer";
$ITEM_SET[$ITEM_SET_ALL, %i++] = "1 Flamer";
$ITEM_SET[$ITEM_SET_ALL, %i++] = "1 Raider";
$ITEM_SET[$ITEM_SET_ALL, %i++] = "1 Knife";
$ITEM_SET[$ITEM_SET_ALL, %i++] = "1 AGP84";
$ITEM_SET[$ITEM_SET_ALL, %i++] = "2 SmokeGrenadeItem";
$ITEM_SET[$ITEM_SET_ALL, %i++] = "1 DistractionGrenadeItem";
$ITEM_SET[$ITEM_SET_ALL, %i++] = "1 GasGrenadeItem";
$ITEM_SET[$ITEM_SET_ALL, %i++] = "1 SpikeGrenadeItem";
$ITEM_SET[$ITEM_SET_ALL] = %i++;

%i = -1;
$ITEM_SET[$ITEM_SET_DM, %i++] = "30 MagnumAmmo";
$ITEM_SET[$ITEM_SET_DM, %i++] = "150 DragonAmmo";
$ITEM_SET[$ITEM_SET_DM, %i++] = "320 TornadoAmmo";
$ITEM_SET[$ITEM_SET_DM, %i++] = "50 MG27Ammo";
$ITEM_SET[$ITEM_SET_DM, %i++] = "60 ChallengerAmmo";
$ITEM_SET[$ITEM_SET_DM, %i++] = "50 MagdalonAmmo";
$ITEM_SET[$ITEM_SET_DM, %i++] = "16 Shotgun2Ammo";
$ITEM_SET[$ITEM_SET_DM, %i++] = "80 RaiderAmmo";
$ITEM_SET[$ITEM_SET_DM, %i++] = "6 AGP84Ammo";
$ITEM_SET[$ITEM_SET_DM, %i++] = "1 Grappler";
$ITEM_SET[$ITEM_SET_DM, %i++] = "5 PlasticMineAmmo";
$ITEM_SET[$ITEM_SET_DM, %i++] = "1 Magnum";
$ITEM_SET[$ITEM_SET_DM, %i++] = "1 Dragon";
$ITEM_SET[$ITEM_SET_DM, %i++] = "1 Tornado";
$ITEM_SET[$ITEM_SET_DM, %i++] = "1 MG27";
$ITEM_SET[$ITEM_SET_DM, %i++] = "1 Challenger";
$ITEM_SET[$ITEM_SET_DM, %i++] = "1 GrenadeLauncher2";
$ITEM_SET[$ITEM_SET_DM, %i++] = "1 Magdalon";
$ITEM_SET[$ITEM_SET_DM, %i++] = "1 Shotgun2";
$ITEM_SET[$ITEM_SET_DM, %i++] = "1 Raider";
$ITEM_SET[$ITEM_SET_DM, %i++] = "1 Knife";
$ITEM_SET[$ITEM_SET_DM, %i++] = "1 AGP84";
$ITEM_SET[$ITEM_SET_DM, %i++] = "2 SmokeGrenadeItem";
$ITEM_SET[$ITEM_SET_DM, %i++] = "1 GasGrenadeItem";
$ITEM_SET[$ITEM_SET_DM, %i++] = "1 SpikeGrenadeItem";
$ITEM_SET[$ITEM_SET_DM] = %i++;

%i = -1;
$ITEM_SET[$ITEM_SET_NO_XPLO, %i++] = "30 MagnumAmmo";
$ITEM_SET[$ITEM_SET_NO_XPLO, %i++] = "150 DragonAmmo";
$ITEM_SET[$ITEM_SET_NO_XPLO, %i++] = "320 TornadoAmmo";
$ITEM_SET[$ITEM_SET_NO_XPLO, %i++] = "60 ChallengerAmmo";
$ITEM_SET[$ITEM_SET_NO_XPLO, %i++] = "50 MG27Ammo";
$ITEM_SET[$ITEM_SET_NO_XPLO, %i++] = "50 MagdalonAmmo";
$ITEM_SET[$ITEM_SET_NO_XPLO, %i++] = "16 Shotgun2Ammo";
$ITEM_SET[$ITEM_SET_NO_XPLO, %i++] = "80 RaiderAmmo";
$ITEM_SET[$ITEM_SET_NO_XPLO, %i++] = "1 Magnum";
$ITEM_SET[$ITEM_SET_NO_XPLO, %i++] = "1 Dragon";
$ITEM_SET[$ITEM_SET_NO_XPLO, %i++] = "1 Tornado";
$ITEM_SET[$ITEM_SET_NO_XPLO, %i++] = "1 MG27";
$ITEM_SET[$ITEM_SET_NO_XPLO, %i++] = "1 Magdalon";
$ITEM_SET[$ITEM_SET_NO_XPLO, %i++] = "1 Challenger";
$ITEM_SET[$ITEM_SET_NO_XPLO, %i++] = "1 GrenadeLauncher2";
$ITEM_SET[$ITEM_SET_NO_XPLO, %i++] = "1 Shotgun2";
$ITEM_SET[$ITEM_SET_NO_XPLO, %i++] = "1 Raider";
$ITEM_SET[$ITEM_SET_NO_XPLO, %i++] = "1 Knife";
$ITEM_SET[$ITEM_SET_NO_XPLO, %i++] = "1 Binoculars";
$ITEM_SET[$ITEM_SET_NO_XPLO, %i++] = "1 Grappler";
$ITEM_SET[$ITEM_SET_NO_XPLO, %i++] = "2 SmokeGrenadeItem";
$ITEM_SET[$ITEM_SET_NO_XPLO, %i++] = "1 DistractionGrenadeItem";
$ITEM_SET[$ITEM_SET_NO_XPLO, %i++] = "1 GasGrenadeItem";
$ITEM_SET[$ITEM_SET_NO_XPLO] = %i++;

%i = -1;
$ITEM_SET[$ITEM_SET_HIGH_CALIBER, %i++] = "30 MagnumAmmo";
$ITEM_SET[$ITEM_SET_HIGH_CALIBER, %i++] = "60 ChallengerAmmo";
$ITEM_SET[$ITEM_SET_HIGH_CALIBER, %i++] = "50 MG27Ammo";
$ITEM_SET[$ITEM_SET_HIGH_CALIBER, %i++] = "50 MagdalonAmmo";
$ITEM_SET[$ITEM_SET_HIGH_CALIBER, %i++] = "16 Shotgun2Ammo";
$ITEM_SET[$ITEM_SET_HIGH_CALIBER, %i++] = "80 RaiderAmmo";
$ITEM_SET[$ITEM_SET_HIGH_CALIBER, %i++] = "6 AGP84Ammo";
$ITEM_SET[$ITEM_SET_HIGH_CALIBER, %i++] = "1 Magnum";
$ITEM_SET[$ITEM_SET_HIGH_CALIBER, %i++] = "1 MG27";
$ITEM_SET[$ITEM_SET_HIGH_CALIBER, %i++] = "1 Challenger";
$ITEM_SET[$ITEM_SET_HIGH_CALIBER, %i++] = "1 Magdalon";
$ITEM_SET[$ITEM_SET_HIGH_CALIBER, %i++] = "1 Shotgun2";
$ITEM_SET[$ITEM_SET_HIGH_CALIBER, %i++] = "1 Raider";
$ITEM_SET[$ITEM_SET_HIGH_CALIBER, %i++] = "1 AGP84";
$ITEM_SET[$ITEM_SET_HIGH_CALIBER, %i++] = "1 Binoculars";
$ITEM_SET[$ITEM_SET_HIGH_CALIBER, %i++] = "1 Grappler";
$ITEM_SET[$ITEM_SET_HIGH_CALIBER, %i++] = "2 SmokeGrenadeItem";
$ITEM_SET[$ITEM_SET_HIGH_CALIBER, %i++] = "1 DistractionGrenadeItem";
$ITEM_SET[$ITEM_SET_HIGH_CALIBER, %i++] = "1 GasGrenadeItem";
$ITEM_SET[$ITEM_SET_HIGH_CALIBER, %i++] = "1 SpikeGrenadeItem";
$ITEM_SET[$ITEM_SET_HIGH_CALIBER] = %i++;

%i = -1;
$ITEM_SET[$ITEM_SET_RAPID_FIRE, %i++] = "150 DragonAmmo";
$ITEM_SET[$ITEM_SET_RAPID_FIRE, %i++] = "320 TornadoAmmo";
$ITEM_SET[$ITEM_SET_RAPID_FIRE, %i++] = "80 RaiderAmmo";
$ITEM_SET[$ITEM_SET_RAPID_FIRE, %i++] = "1 Dragon";
$ITEM_SET[$ITEM_SET_RAPID_FIRE, %i++] = "1 Tornado";
$ITEM_SET[$ITEM_SET_RAPID_FIRE, %i++] = "1 Raider";
$ITEM_SET[$ITEM_SET_RAPID_FIRE, %i++] = "1 Binoculars";
$ITEM_SET[$ITEM_SET_RAPID_FIRE, %i++] = "1 Grappler";
$ITEM_SET[$ITEM_SET_RAPID_FIRE, %i++] = "2 SmokeGrenadeItem";
$ITEM_SET[$ITEM_SET_RAPID_FIRE, %i++] = "1 DistractionGrenadeItem";
$ITEM_SET[$ITEM_SET_RAPID_FIRE, %i++] = "1 GasGrenadeItem";
$ITEM_SET[$ITEM_SET_RAPID_FIRE, %i++] = "1 SpikeGrenadeItem";
$ITEM_SET[$ITEM_SET_RAPID_FIRE] = %i++;

%i = -1;
$ITEM_SET[$ITEM_SET_XPLO, %i++] = "4 GrenadeLauncherAmmo";
$ITEM_SET[$ITEM_SET_XPLO, %i++] = "2 DoomSlayerAmmo";
$ITEM_SET[$ITEM_SET_XPLO, %i++] = "3 RocketLauncherAmmo";
$ITEM_SET[$ITEM_SET_XPLO, %i++] = "6 AGP84Ammo";
$ITEM_SET[$ITEM_SET_XPLO, %i++] = "1 GrenadeLauncher2";
$ITEM_SET[$ITEM_SET_XPLO, %i++] = "1 DoomSlayer";
$ITEM_SET[$ITEM_SET_XPLO, %i++] = "1 RocketLauncher";
$ITEM_SET[$ITEM_SET_XPLO, %i++] = "1 AGP84";
$ITEM_SET[$ITEM_SET_XPLO, %i++] = "1 Grappler";
$ITEM_SET[$ITEM_SET_XPLO, %i++] = "5 PlasticMineAmmo";
$ITEM_SET[$ITEM_SET_XPLO, %i++] = "1 SpikeGrenadeItem";
$ITEM_SET[$ITEM_SET_XPLO] = %i++;

%i = -1;
$ITEM_SET[$ITEM_SET_MG_ONLY, %i++] = "50 MG27Ammo";
$ITEM_SET[$ITEM_SET_MG_ONLY, %i++] = "1 MG27";
$ITEM_SET[$ITEM_SET_MG_ONLY, %i++] = "1 Grappler";
$ITEM_SET[$ITEM_SET_MG_ONLY, %i++] = "2 SmokeGrenadeItem";
$ITEM_SET[$ITEM_SET_MG_ONLY] = %i++;

%i = -1;
$ITEM_SET[$ITEM_SET_DM_MG_AND_KNIFE, %i++] = "24 MG27Ammo";
$ITEM_SET[$ITEM_SET_DM_MG_AND_KNIFE, %i++] = "1 MG27";
$ITEM_SET[$ITEM_SET_DM_MG_AND_KNIFE, %i++] = "1 Knife";
$ITEM_SET[$ITEM_SET_DM_MG_AND_KNIFE] = %i++;

%i = -1;
$ITEM_SET[$ITEM_SET_KNIFE_ONLY, %i++] = "1 Knife";
$ITEM_SET[$ITEM_SET_KNIFE_ONLY, %i++] = "1 SmokeGrenadeItem";
$ITEM_SET[$ITEM_SET_KNIFE_ONLY] = %i++;

%i = -1;
$ITEM_SET[$ITEM_SET_MG_AND_KNIFE, %i++] = "50 MG27Ammo";
$ITEM_SET[$ITEM_SET_MG_AND_KNIFE, %i++] = "1 MG27";
$ITEM_SET[$ITEM_SET_MG_AND_KNIFE, %i++] = "1 Knife";
$ITEM_SET[$ITEM_SET_MG_AND_KNIFE, %i++] = "1 Grappler";
$ITEM_SET[$ITEM_SET_MG_AND_KNIFE] = %i++;

%i = -1;
$ITEM_SET[$ITEM_SET_DM_NOGRAP, %i++] = "30 MagnumAmmo";
$ITEM_SET[$ITEM_SET_DM_NOGRAP, %i++] = "150 DragonAmmo";
$ITEM_SET[$ITEM_SET_DM_NOGRAP, %i++] = "320 TornadoAmmo";
$ITEM_SET[$ITEM_SET_DM_NOGRAP, %i++] = "50 MG27Ammo";
$ITEM_SET[$ITEM_SET_DM_NOGRAP, %i++] = "60 ChallengerAmmo";
$ITEM_SET[$ITEM_SET_DM_NOGRAP, %i++] = "50 MagdalonAmmo";
$ITEM_SET[$ITEM_SET_DM_NOGRAP, %i++] = "16 Shotgun2Ammo";
$ITEM_SET[$ITEM_SET_DM_NOGRAP, %i++] = "80 FlamerAmmo";
$ITEM_SET[$ITEM_SET_DM_NOGRAP, %i++] = "80 RaiderAmmo";
$ITEM_SET[$ITEM_SET_DM_NOGRAP, %i++] = "6 AGP84Ammo";
$ITEM_SET[$ITEM_SET_DM_NOGRAP, %i++] = "1 Magnum";
$ITEM_SET[$ITEM_SET_DM_NOGRAP, %i++] = "1 Dragon";
$ITEM_SET[$ITEM_SET_DM_NOGRAP, %i++] = "1 Tornado";
$ITEM_SET[$ITEM_SET_DM_NOGRAP, %i++] = "1 MG27";
$ITEM_SET[$ITEM_SET_DM_NOGRAP, %i++] = "1 Challenger";
$ITEM_SET[$ITEM_SET_DM_NOGRAP, %i++] = "1 Magdalon";
$ITEM_SET[$ITEM_SET_DM_NOGRAP, %i++] = "1 Shotgun2";
$ITEM_SET[$ITEM_SET_DM_NOGRAP, %i++] = "1 GrenadeLauncher2";
$ITEM_SET[$ITEM_SET_DM_NOGRAP, %i++] = "1 Flamer";
$ITEM_SET[$ITEM_SET_DM_NOGRAP, %i++] = "1 Raider";
$ITEM_SET[$ITEM_SET_DM_NOGRAP, %i++] = "1 Knife";
$ITEM_SET[$ITEM_SET_DM_NOGRAP, %i++] = "1 AGP84";
$ITEM_SET[$ITEM_SET_DM_NOGRAP, %i++] = "1 SmokeGrenadeItem";
$ITEM_SET[$ITEM_SET_DM_NOGRAP, %i++] = "1 DistractionGrenadeItem";
$ITEM_SET[$ITEM_SET_DM_NOGRAP, %i++] = "1 GasGrenadeItem";
$ITEM_SET[$ITEM_SET_DM_NOGRAP, %i++] = "1 SpikeGrenadeItem";
$ITEM_SET[$ITEM_SET_DM_NOGRAP] = %i++;

function clearSpawnBuyList() {
  for(%i = 0; (%str = $spawnAmmo[%i]) != ""; %i++) $spawnAmmo[%i] = "";
  for(%i = 0; (%str = $spawnItems[%i]) != ""; %i++) $spawnItems[%i] = "";
  for(%i = 0; (%str = $spawnWeapons[%i]) != ""; %i++) $spawnWeapons[%i] = "";
  for(%i = 0; (%str = $spawnGadgets[%i]) != ""; %i++) $spawnGadgets[%i] = "";
  $itemSetType = $ITEM_SET_UNSET;
  $ItemGroup::currentSpawnList = "";
}

function clearTeamSpawnBuyList(%t) {
  for(%i = 0; (%str = $spawnAmmo[%t, %i]) != ""; %i++) $spawnAmmo[%t, %i] = "";
  for(%i = 0; (%str = $spawnItems[%t, %i]) != ""; %i++) $spawnItems[%t, %i] = "";
  for(%i = 0; (%str = $spawnWeapons[%t, %i]) != ""; %i++) $spawnWeapons[%t, %i] = "";
  for(%i = 0; (%str = $spawnGadgets[%t, %i]) != ""; %i++) $spawnGadgets[%t, %i] = "";
  $itemSetType[%t] = $ITEM_SET_UNSET;
}

function setSpawnBuyList(%type) {
  clearSpawnBuyList();
  for (%t = 0; %t < getNumTeams(); %t++) clearTeamSpawnBuyList(%t);

  for(%i = 0; (%str = $ITEM_SET[%type, %i]) != ""; %i++) $spawnItems[%i] = %str;
  $itemSetType = %type;
}

function setTeamSpawnBuyList(%t, %type) {
  clearTeamSpawnBuyList(%t);
  for(%i = 0; (%str = $ITEM_SET[%type, %i]) != ""; %i++) $spawnItems[%t, %i] = %str;
  $itemSetType[%t] = %type;
}