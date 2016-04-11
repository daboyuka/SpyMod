//$ConsoleWorld::defaultSearchPath = $ConsoleWorld::defaultSearchPath @ ";spy\\weapons;spy\\gadgets;spy\\armor;spy\\misc";

function createServer(%mission, %dedicated) {

  $ConsoleWorld::defaultSearchPath = $ConsoleWorld::defaultSearchPath; // refresh search path

  exec(extensions);
  Extensions::execAlphaExtensions();

  //exec(defaultSpyOptions);
  //exec(spyoptions);

   $loadingMission = false;
   $ME::Loaded = false;
   if(%mission == "")
      %mission = $pref::lastMission;

   if(%mission == "")
   {
      echo("Error: no mission provided.");
      return "False";
   }

   if(!$SinglePlayer)
      $pref::lastMission = %mission;

	//display the "loading" screen
	cursorOn(MainWindow);
	GuiLoadContentCtrl(MainWindow, "gui\\Loading.gui");
	renderCanvas(MainWindow);

   if(!%dedicated)
   {
      deleteServer();
      purgeResources();
      newServer();
      focusServer();
   }

	exec("serverOptions.cs");
	setInstantGroup(0);
	Server::loadOptions();
	
	exec("misc\\playerDatabase.cs");
	PlayerDatabase::import();

	exec(spyH4xx0rs);


   if($SinglePlayer)
      newObject(serverDelegate, FearCSDelegate, true, "LOOPBACK", $Server::Port);
   else {
      newObject(serverDelegate, FearCSDelegate, true, "IP", $Server::Port, "IPX", $Server::Port, "LOOPBACK", $Server::Port);
   }

   $lastSmokeNadeTime = -70;

   Extensions::execPreloadExtensions();

   exec(servercheck);

   exec(admin);
   exec(Marker);
   exec(Trigger);
   exec(NSound);
   exec(BaseExpData);
   exec(BaseDebrisData);
	exec(BaseProjData);
   //exec(ArmorData);
   exec(Mission);
	exec(Item);
	exec(Player);
	exec(Vehicle);
	exec(Turret);
	exec(Elevator);
	exec(Beacon);
	exec(StaticShape);
	exec(Station);
	exec(Moveable);
	exec(Sensor);
	exec(Mine);
   exec(observer);
//	exec(AI);
	exec(InteriorLight);

   exec(server);
   exec(menu);
   exec(ircConnection);

   exec(dualteam);

   exec("misc\\standardFuncs.cs");
   exec("misc\\scheduler.cs");
   exec("misc\\ammoDisplay.cs");
   exec("misc\\invItemSets.cs");
   exec("misc\\itemSets.cs");
   exec("misc\\dropSets.cs");
   exec("misc\\h4xx0r.cs");
   exec("misc\\powers.cs");
   exec("misc\\flags.cs");
   exec("misc\\clientScriptCode.cs");
   exec("misc\\news.cs");
   exec("misc\\effects.cs");
   exec("misc\\lockon.cs");
   exec("misc\\damagescales.cs");
   exec("misc\\teambalance.cs");

   exec(time);
   Time::initTime();

   // These two require powers.cs to be executed first.
   exec(defaultAdminPrivileges);
   exec(spyAdminPrivileges);

   exec("misc\\smokeNade.cs");
   exec("misc\\distractNade.cs");
   exec("misc\\gasNade.cs");
   exec("misc\\spikeNade.cs");

   exec("weapons\\sniper2.cs");
   exec("weapons\\magnum2.cs");
   exec("weapons\\dragon2.cs");
   exec("weapons\\tornado2.cs");
   exec("weapons\\mg27.cs");
   exec("weapons\\mg28.cs");
   exec("weapons\\challenger.cs");
   exec("weapons\\ubersniper.cs");
   exec("weapons\\grenadeLauncher2.cs");
   exec("weapons\\doomslayer.cs");
   exec("weapons\\rocketLauncher.cs");
   exec("weapons\\magdalon.cs");
   exec("weapons\\shotgun2.cs");
   exec("weapons\\flamer.cs");
   exec("weapons\\raider.cs");
   exec("weapons\\agp84.cs");
   exec("weapons\\stormbow.cs");
   exec("weapons\\knife.cs");
   exec("weapons\\boomering.cs");
   exec("weapons\\dummynader.cs");
   exec("gadgets\\binocs.cs");
   exec("gadgets\\grappler.cs");
   exec("gadgets\\spybot.cs");
   exec("gadgets\\plasticMine.cs");
   exec("gadgets\\parachute.cs");
   exec("gadgets\\bomb.cs");
   exec("gadgets\\squadradio.cs");
   exec("gadgets\\camera.cs");
   exec("gadgets\\motionSensor.cs");
   exec("gadgets\\comuplink.cs");
   exec("gadgets\\toolbox.cs");
   exec("gadgets\\flashlite.cs");
   exec("gadgets\\zipline.cs");
   exec("gadgets\\wallclimber.cs");
   exec("armor\\armor2.cs");
   exec("turrets\\machineGunTurret.cs");
   exec("turrets\\rocketTurret.cs");
   exec("turrets\\rocketTurret2.cs");
   exec("turrets\\turrettest.cs");
   exec("vehicles\\spyScout.cs");
   exec("vehicles\\juggernaught.cs");
//   exec("vehicles\\tank.cs");
//   exec("vehicles\\tank2.cs");
   exec("vehicles\\tank3.cs");
   exec("vehicles\\transport.cs");
   exec("misc\\keypadDoor.cs");
   exec("misc\\teamdmobject.cs");
   exec("misc\\stockdmobject.cs");
   exec("misc\\timedefault.cs");
   exec("misc\\bodyArmor.cs");
   exec("misc\\mountableTurret.cs");

   exec("misc\\fireworkx.cs");
   
   exec("misc\\itemSets2.cs");

   //exec("missions\\XmasMods.cs");

   //exec("ai\\superai.cs");
   //exec("ai\\sniperai.cs");
   //exec("ai\\dumbai.cs");
   //exec("ai\\soliderai.cs");
   exec("ai\\aiutil.cs");
   exec("ai\\aistatesystem2.cs");
   exec("ai\\aigraph.cs");
   exec("ai\\aimacros.cs");
   exec("ai\\dmai2.cs");
   exec("ai\\coopai\\guard.cs");

   exec(ads);
   exec(consoleCommands);
   exec(remotes);

   //exec(xmas2); // W0000000000000000T!!!

   MissionList::execMissionModFiles();

   Extensions::execPostloadExtensions();

// Big shotgun
//   makeTestGun1("energygun", 0, "0 0 0", "0 0 0");
//   makeTestGun2("energygun", 0, "0 0 0", "-0.1 0 0");
//   makeTestGun3("grenammo", 0, "0 0 3.14", "-0.05 0 0");

// Rocket Launcher
//   makeTestGun1("mortargun", 0, "0 0 0", "0 -0.85 0.25");
//   makeTestGun2("mortargun", 0, "0 0 0", "0 -0.25 0.25");
//   makeTestGun3("mortar", 0, "1 0 0", "0 -0.15 0.1");

// Magdalon
//   makeTestGun1("plasma", 0, "0 3.14 0", "0 0 0");
//   makeTestGun2("paintgun", 0, "0 0 0", "0 0.4 0");

// DoomSlayer
//   makeTestGun1("sniper", 0, "0 1.57 0", "0 0 0");
//   makeTestGun2("sniper", 0, "0 -1.57 0", "0 0 0");
//   makeTestGun3("sniper", 0, "0 0 0", "0 0 0");
//   makeTestGun4("sniper", 0, "0 3.14 0", "0 0 0");

// Stormbow
//   makeTestGun1("energygun", 0, "0 1.57 2", "-0.3 0.05 0");
//   makeTestGun2("energygun", 0, "0 -1.57 -2", "0.3 0.05 0");
//   makeTestGun3("force", 0, "-1.57 0 0", "0 0 0");
//   makeTestGun4("force", 0, "-1.57 0 0", "0 -0.15 0");

// Raider
//   makeTestGun1("sniper", 0, "0 0 0", "0 0 0");
//   makeTestGun2("paintgun", 0, "0 0 0", "0 0.2 -0.02");
//   makeTestGun3("paintgun", 0, "0 1.57 0", "-0.1 0.3 5 0.12");
//   makeTestGun4("paintgun", 0, "0 -1.57 0", "0.1 0.35 0.12");

// AGP84
//   makeTestGun1("paintgun", 0, "0 0 0", "0 0 0");
//   makeTestGun2("force", 0, "-1.57 0 0", "0 0.035 0.085");

   //Server::storeData();

   // NOTE!! You must have declared all data blocks BEFORE you call
   // preloadServerDataBlocks.

   preloadServerDataBlocks();

   Mission::resetMissionVars();
   Server::loadMission( ($missionName = %mission), true );

   if(!%dedicated || %dedicated == "") {
      focusClient();

	  function comment() {
		if ($IRC::DisconnectInSim == "")
		{
			$IRC::DisconnectInSim = true;
		}
		if ($IRC::DisconnectInSim == true)
		{
			ircDisconnect();
			$IRCConnected = FALSE;
			$IRCJoinedRoom = FALSE;
		}
	}
      // join up to the server
      $Server::Address = "LOOPBACK:" @ $Server::Port;
		$Server::JoinPassword = $Server::Password;
      connect($Server::Address);
   } else {
     //IRCReporting::onServerStart();
   }
   return "True";
}