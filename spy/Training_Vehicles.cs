//Training_Vehicle.cs
//////////////////////////
exec("game.cs");
exec("training_AI.cs");

//Globals
//////////////////////////
$Train::missionType = "VEHICLE";
$currentLeg = 0;
//vehicle buy limits
$TeamItemMax[ScoutVehicle] = 50;
$TeamItemMax[HAPCVehicle] = 50;
$TeamItemMax[LAPCVehicle] = 50;
//crash respawn warps
$spawn[1] = "-38.9365 43.7014 114.682";
$spawn[2] = "-522.794 768.53 135.786";
$spawn[3] = "-1487.25 1201.25 115.386";


//---------------------------
//vehicle::leg1()
//---------------------------
function vehicle::leg1(%clientId)
{
	schedule("bottomprint(" @ %clientId @ ", \"<f1><jc>In front of you is a vehicle station that is used for buying vehicles. When you step up to it, you get the Vehicle Inventory screen.\", 10);", 0);
	schedule("messageAll(0, \"~wshell_click.wav\");", 0);
	schedule("bottomprint(" @ %clientId @ ", \"<f1><jc>Step up to the panel and the Inventory screen will apear in front of you.\", 10);", 10);
	schedule("messageAll(0, \"~wshell_click.wav\");", 10);
	schedule("bottomprint(" @ %clientId @ ", \"<f1><jc>The right side of the menu is the available inventory you can purchase. Select the Scout from the right side menu.\", 10);", 20);
	schedule("messageAll(0, \"~wshell_click.wav\");", 20);
	schedule("bottomprint(" @ %clientId @ ", \"<f1><jc>Click on the BUY button to purchase the Scout.  To return to the play screen, click on the X button in the upper right hand corner of the screen or hit the inventory(I) key.\", 10);", 30);
	schedule("messageAll(0, \"~wshell_click.wav\");", 30);
	schedule("bottomprint(" @ %clientId @ ", \"<f1><jc>Once the scout is purchased, a scout vehicle will appear on the vehicle pad that is at the front of the vehicle bay. Once you buy the scout, a waypoint to the next station will be assigned to you.\", 10);", 40);
	schedule("messageAll(0, \"~wshell_click.wav\");", 40);
	schedule("bottomprint(" @ %clientId @ ", \"<f1><jc>Jump into the Scout to pilot it.  Press the forward key to move forward.  Press the jetpack key to hover upwards. You can also use the move backward key as a brake to help stop the vehicle\", 10);", 50); 
	schedule("messageAll(0, \"~wshell_click.wav\");", 50);
	schedule("bottomprint(" @ %clientId @ ", \"<f1><jc>Once you have arrived at the next station, walk up to the vehicle buying station there for further instructions.\", 10);", 60);
	schedule("messageAll(0, \"~wshell_click.wav\");", 60);
	schedule("bottomprint(" @ %clientId @ ", \"<f1><jc>For an added challenge, try and maneuver through the pylons between here and the waypoint.\", 10);", 70);
	schedule("messageAll(0, \"~wshell_click.wav\");", 70);

	//limit vehicle inventory
	$VehicleInvList["ScoutVehicle"] = 1;
	$VehicleInvList["LAPCVehicle"] = 0;
	$VehicleInvList["HAPCVehicle"] = 0;
}

//-----------------------------
//vehicle::leg2()
//-----------------------------
function vehicle::leg2(%clientId)
{
	schedule("bottomprint(" @ %clientId @ ", \"<f1><jc>How was the flight? Did you enjoy the view?\", 5);", 0);
	schedule("messageAll(0, \"~wshell_click.wav\");", 0);
	schedule("bottomprint(" @ %clientId @ ", \"<f1><jc>Next on the list is the Light Personnel Carrier, or LPC.\", 5);", 5);
	schedule("messageAll(0, \"~wshell_click.wav\");", 5);
	schedule("bottomprint(" @ %clientId @ ", \"<f1><jc>The LPC is a small two-man transport. It's not as fast as the Scout but a little tougher.\", 10);", 10);
	schedule("messageAll(0, \"~wshell_click.wav\");", 10);
	schedule("bottomprint(" @ %clientId @ ", \"<f1><jc>Go ahead and purchase one and take it for a spin. As before, you must pilot the LPC to a waypoint on the map and land there.\", 10);", 20);
	schedule("messageAll(0, \"~wshell_click.wav\");", 20);

   //limit vehicle inventory	
	$VehicleInvList["ScoutVehicle"] = 0;
	$VehicleInvList["LAPCVehicle"] = 1;
	$VehicleInvList["HAPCVehicle"] = 0;
}

//--------------------------------
//vehicle::leg3()
//--------------------------------
function vehicle::leg3(%clientId)
{
  schedule("bottomprint(" @ %clientId @ ", \"<f1><jc>On to the last vehicle, the Heavy Personnel Carrier, or HPC.\", 5);", 0);
  schedule("messageAll(0, \"~wshell_click.wav\");", 0);
  schedule("bottomprint(" @ %clientId @ ", \"<f1><jc>Besides the driver, the HPC holds up to four tribesmen.\", 5);", 5);
  schedule("messageAll(0, \"~wshell_click.wav\");", 5);
  schedule("bottomprint(" @ %clientId @ ", \"<f1><jc>The HPC is far slower than the LPC, but can take much more punishment.\", 10);", 10);
  schedule("messageAll(0, \"~wshell_click.wav\");", 10);
  schedule("bottomprint(" @ %clientId @ ", \"<f1><jc>This vehicle is great for ambushes deep into enemy territory, with four Heavy Armors lobbing mortar shells from on high, making it a gunship of sorts.\", 10);", 20);
  schedule("messageAll(0, \"~wshell_click.wav\");", 20);
  schedule("bottomprint(" @ %clientId @ ", \"<f1><jc>Just like before, pilot the HPC to the assigned waypoint. You must land on directly on the last building and exit your vehicle to end the mission.\", 10);", 30);
  schedule("messageAll(0, \"~wshell_click.wav\");", 30);
  
  //limit vehicle inventory
  $VehicleInvList["ScoutVehicle"] = 0;
  $VehicleInvList["LAPCVehicle"] = 0;
  $VehicleInvList["HAPCVehicle"] = 1;
}

//-------------------------------------
//vehicle::setWayPoint()
//-------------------------------------
function vehicle::setWayPoint(%clientid)
{
	dbecho(2,"setting up waypoints...");
	%group = nameToId("MissionGroup\\destinations");

	//flow controlled by triggers
	if($currentLeg == 0)
	{
		%obj = Group::getObject(%group, 0);
		$currentLeg++;
		ObjectiveScreen();
	}
	else if($currentLeg == 1)
	{
		%obj = Group::getObject(%group, 1);
		$currentLeg++;
		objectiveScreen();
	}
	else if($currentLeg == 2)
	{
		%obj = Group::getObject(%group, 2);
		$currentLeg++;
		objectiveScreen();
	}
	
	//find the position of the next station
	%position = GameBase::getPosition(%obj);
	%x = getWord(%position, 0);
	%y = getWord(%position, 1);
	//setway point
	issueCommand(%clientId, %clientId, 0, "Waypoint set to next vehicle station.", %x, %y);
}       


//-----------------------------------------
//GroupTrigger::onEnter()
//-----------------------------------------
function GroupTrigger::onEnter(%this, %object)
{
	if(!%this.secret) 
	{  
		objectiveScreen();
		if(getObjectType(%object) != "Player")
			return;

		%cl = Player::getClient(%object);

		if(%this.num == "Trigger1" && $currentLeg == 1)
		{
			%group = nameToId("MissionCleanup");
			for(%i = 0; (%obj = Group::getObject(%group, %i)) != -1; %i++)
			{
				dbecho(2,%obj);
				if(getObjectType(%obj) == "Flier")
					deleteObject(%obj);
			}
			Vehicle::setWayPoint(%cl);
			vehicle::leg2(%cl);
		}
		else if(%this.num == "Trigger2" && $currentLeg == 2)
		{
			%group = nameToId("MissionCleanup");
			for(%i = 0; (%obj = Group::getObject(%group, %i)) != -1; %i++)
			{
				if(getObjectType(%obj) == "Flier")
				{
					deleteObject(%obj);
				}
			}
			Vehicle::setWayPoint(%cl);
			vehicle::leg3(%cl);
		}
		else if(%this.num == "Trigger3" && $currentLeg == 3)
		{  
			bottomprint(%cl, "<jc><f2>This concludes your introduction to vehicles.", 8);
			schedule("Training::missionComplete(" @ %cl @ ");", 8);
		}
	}
	else
		griffonTown(%this, 400);
}	

//------------------------------
//Game::initialMissionDrop()
//------------------------------
function Game::initialMissionDrop(%clientId)
{
	GameBase::setTeam(%clientId, 0);
	Client::setGuiMode(%clientId, $GuiModePlay);
	Game::playerSpawn(%clientId, false);

	schedule("bottomprint(" @ %clientId @ ", \"<f1><jc>Training Mission 3 - Introduction to vehicles\", 5);", 0);
	schedule("bottomprint(" @ %clientId @ ", \"<f1><jc>In this training excercise, you will fly the 3 different vehicles to particular destinations assigned to you by waypoints.\", 10);", 5);
	schedule("messageAll(0, \"~wshell_click.wav\");", 0);
	schedule("messageAll(0, \"~wshell_click.wav\");", 5);
	schedule("vehicle::leg1(" @ %clientId @ ");", 15);
	$VehicleInvList["ScoutVehicle"] = 1;
	$VehicleInvList["LAPCVehicle"] = 0;
	$VehicleInvList["HAPCVehicle"] = 0;
	schedule("vehicle::setWayPoint(" @ %clientId @ ");", 0);
}

//--------------------------------
//Vehicle::onDestroyed()
//--------------------------------
function Vehicle::onDestroyed (%this,%mom)
{
	%clientId = "2049";
	$TeamItemCount[GameBase::getTeam(%this) @ $VehicleToItem[GameBase::getDataName(%this)]]--;
   %cl = GameBase::getControlClient(%this);
	%pl = Client::getOwnedObject(%cl);
	if(%pl != -1) {
	   Player::setMountObject(%pl, -1, 0);
   	Client::setControlObject(%cl, %pl);
	}
	calcRadiusDamage(%this, $DebrisDamageType, 2.5, 0.05, 25, 13, 2, 0.55, 
		0.1, 225, 100);
		
   bottomprint(%clientId, "<f1><jc>Lets try that again.", 5); 
	%position = $spawn[$currentLeg];
	GameBase::setPosition( 2049, %position);
}

//----------------------------------
//ObjectiveScreen()
//----------------------------------
function ObjectiveScreen()
{
   %time = getSimTime() - $MatchStartTime;
   
   Training::displayBitmap(0);
   Team::setObjective(0, 1, "<f5><jl>Mission Completion:");
   Team::setObjective(0, 2, "<f1>   -Travel to all vehicle stations that are assigned");
   Team::setObjective(0, 3, "\n");
   Team::setObjective(0, 4, "<f5><jl>Mission Information:");
   Team::setObjective(0, 5, "<f1>   -Mission Name: Introduction to vehicles");
   Team::setObjective(0, 6, "\n");
   
   Team::setObjective(0, 7, "<f5><j1>Mission Objectives:");
   Team::setObjective(0, 8, "<f1>   -Current station waypoint: " @ $currentLeg);
   
   Team::setObjective(0, 9, "\n");
   Team::setObjective(0, 10, "\n");
   Team::setObjective(0, 11, "\n");
   Team::setObjective(0, 12, "\n");
   Team::setObjective(0, 13, "\n");
}

//------------------------------
//MissionSummary()
//------------------------------
function missionSummary()
{
	%time = getSimTime() - $MatchStartTime;

	Training::displayBitmap(0);
	Team::setObjective(0, 1, "<f5><jl>Mission Completion:");
	Team::setObjective(0, 2, "<f1>   -Completed:");
	Team::setObjective(0, 3, "\n");
	Team::setObjective(0, 4, "<f5><jl>Mission Information:");
	Team::setObjective(0, 5, "<f1>   -Mission Name: Introduction to vehicles");
	Team::setObjective(0, 6, "\n");

	Team::setObjective(0, 7, "<f5><j1>Mission Summary:");

	Team::setObjective(0, 8, "<f1>   -Total Mission Time: " @ "<f1>" @ Time::getMinutes(%time) @ " Minutes " @ Time::getSeconds(%time) @ " Seconds");
	Team::setObjective(0, 9, "\n");
	Team::setObjective(0, 10, "\n");
	Team::setObjective(0, 11, "\n");
	Team::setObjective(0, 12, "\n");
	Team::setObjective(0, 13, "\n");
	Team::setObjective(0, 14, "\n");
}


//---------------------------------------
//griffonTown() -can you find this place?
//---------------------------------------
function griffonTown(%this, %time)
{
   dbecho(2,"time = " @  getSimTime() - $MatchStartTime);
   if(getSimTime() - $MatchStartTime > %time)
   {   
      %positionIn = "-2847.21 -2254.32 -16.137";
      %positionOut = "-38.9356 43.7005 114.673";
      if(%this.in)
      {
         GameBase::setPosition( 2049, %positionIn);
      	messageAll(0, "~wshieldhit.wav");
         centerprint(2049, "<f1><jc>Welcome to GriffonTown.  Jett, Skeet, and Sne/\\ker's home away from home!", 10);
      	schedule("centerprint(2049, \"<f1><jc>Make yourself at home, and to get back to your primary goal, jump into the tunnel that is in front of you.\", 10);", 10);
      }
      else
      {
         GameBase::setPosition( 2049, %positionOut);
      	messageAll(0, "~wshieldhit.wav");
      }
   }
   else
      //please leave me alone until I am done!
      messageAll(0, "I'm busy......");

}

//-----------------------------
//StaticShape::onDamage()
//-----------------------------
function StaticShape::onDamage(%this,%type,%value,%pos,%vec,%mom,%object)
{
	%shape = GameBase::getDataName(%this);
	if(%shape.className != Vehicle)
	   return;
	
	%damageLevel = GameBase::getDamageLevel(%this);
	%TDS= 1;
	if(GameBase::getTeam(%this) == GameBase::getTeam(%object))
		%TDS = $Server::TeamDamageScale;
	GameBase::setDamageLevel(%this,%damageLevel + %value * %TDS);
   %this.lastDamageObject = %object;
   %this.lastDamageTeam = GameBase::getTeam(%object);
}

//----------------------------
//Training::MissionComplete
//----------------------------
function Training::missionComplete(%cl)
{
  Client::setGuiMode(%cl, $GuiModeObjectives);
  missionSummary();
  remoteEval(2049, TrainingEndMission);
}

//---------------------------
//TrainEndMission()
//---------------------------
function remoteTrainingEndMission()
{
   schedule("EndGame();", 8);
}

//do nothing functions
function Player::onDamage(%this,%type,%value,%pos,%vec,%mom,%vertPos,%quadrant,%object)
{
}

function remoteScoresOn(%clientId)
{
}

function remoteScoresOff(%clientId)
{
}

aaa..M,(IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII\aaaaaaaaadd.pmaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaadpI,,,,,,,,P(.......maaaaaaaaaaaaaaaaaaaaaaaaaaaypmmmmmmmdaaaaaaaaaaaaaaaaaaaaa..6$





























































>99999999999999999999999999999999999999999999WB]!kkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkLccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccOI	PM\\\\\\\\\\\\\\\\\\\\\\\\y,,,,,,,pmmmmmmmaaaaaaaaaaaaaaaaaaaaadddddddaa.......pccccccccccccccd(((((((maaaaaaaaaaaaaaaaaaaaayp	IIIIIIId.mmmmmmmcaaaaaaaaaaaaaaaaaaaaa(((((((((((((((((((((((((((((((((((((((,pddddddd.mmmmmmmaaaaaaaaaaaaaaaaaaaaacccccccaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa)888888888888888888886$2\kLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL“™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™P(g3bpy							.,IIIIIIIIIIIIIIcdmmmmmmmmmmmmmaaaaaaaaaaaaaaaaaaaaa.pcummmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmMMMMMMMMMMMMMMMMMMMMMMMMMMMaaaaaaaaaddddddddddddddddddddddddddddddddd("y2g3..........c,bmpaaaaaaaaaaaaaaaaaaaaaaaaaaaaduuuuuuuuuuuuuuuuuuuuuuuu							aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaacmmmmmmmmmmd.Ipppppppppppppppppppppppppppaaaaaaaaauuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuf0AHHHHHHHHHHHHHHHHHHHHHHHHHHHHH{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{VVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVC7777777777777777777777777777777777777777777777YF[&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&-NJ=================L
+++++++++++++++++++++++++++++++E99999999999999999999999999999999W>]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]OBBBBBBBBBBBBBBBBBBB(68\\\\\\\\\\\\\\\\\\\\)$ggggggggggggggggggggggggggggggggggkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkk®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®!!!!!!!!!!!!!!!!!!!!!!!!Py2"m3,dcIbbbbbbbbbbaaaaaaaaaaaaaaaaaaaaaaaaaaaau.pppppppaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaad	mucppppppppppppppppppppppppppppppppppppaaaaaaaaa.................................2(fIMyg	3,"udpmaaaaaaaaaaaaaaaaaaaaaaaaaaaa.ccccccccccccccccaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa upb.ddddddddddmmmmmmmmmmmmmmmmmmmmmmmmmmmaa        ccccccccccccccccccccccccccccccccc60------------------------------LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%C777777777777777777777777777777777777v()\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\8$yyyyyyyyyyyyyyyyyyyyyyyykkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkk2,IMfpg	.uuuuuuuuuub3a                            cdmeeeeeeea                                   ..........pcum"eeeeeeeaaaaaaaaaaaaaaaaaaaaa         deeeeeeeeeeeeeeeeeeeeeeeeeeeaal1(yPPPPPPPPPPPPP2,bIfggggggggggsc.mp        dueeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeaaaaaa        "																					eeeeeeeeeeeclmsd.......aaaaaa                            3pueeeeeeeaaaaaa                                   nnnnnnnnnnnnnnnjAHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD[64wwwwwwwwwwwwwwww
GGGGGGGGGGGGGGGGGGGGGGGGGGGGGGG=9+WEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO>]____________________________________________0---------------------------------------------------------------------------------------------------------------------------%LC777777777777777777777777777777777777M)\vvvvvvvvvvvvvvvvvvvv8$$$$$$$$$$$$$$$$$$$$$$$$PPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPkhhhhhhhhhhhb(y1"2,,,,,,,,,,,,,cIfmmmmmmmmmmdl3gsu.eeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaa         peeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaamcdn										ul        pseeeeeeeeeeeeeeeeeeeeeeeeeeeaaaaaaa        .eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeebBh"""""""""""(yI12,mmmmmmmmmd3333333333333	fcunppppppppppaaaaaa        .llllllllllllllllllllleeeeeeeeaaaaaa                            ssssssssssdmgcunpppppppppp.eeeeeeeaaaaaa                                   leeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaa               T5/66666666666666666666666666666666666666666666666666666666666wwwwwwwwwwwwwwww0-44444444444444444444444444444444444444444444444444$%LC7777777777777777777777777777777777777777777777777777777777777777777777777777777M!\vvvvvvvvvvvvvvvvvvvv8))))))))))))))))))))))))P((((((((((((bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb2"""""""""""h	I1yyyyyyyyy,3mscdngggggggggggggggggggggguuuuuuuulpeeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa        .eeeeeeeeeeeeeeeeeeeeeeeeeeeaammmmmmmmmcinssssssssssdlffffff        .ueeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeaaaaaa        pppppppppppppppppppppee2((((((((((((b	k""""""""""",hI1mtcgynnnnnnnnnnnnnnnnnnils.dddddddaaaaaa                            pf3ueeeeeeeaaaaaa                                   cmnttttttttttolllllllll.ipsudeeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaa                     eeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa¨ß¶•§£¢°†üûùúõöôòóñïîìíëêıéçåãäâàáÜÖÑÉÇÅÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄ„–|Ìzªµ˛Û’≥≥≥≥≥≥≥≥≥≥≥≥≥≥≥≥≥≥≥≥≥≥≥≥≥≥≥≥≥≥≥≥≥≥≥≥≥≥≥≥≥≥≥≥≥≥≥Ã´»∞ÆΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩ‚Â€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€€ ÒƒÓ∫ﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂ`›^≈Ë‡Zÿ◊‰¬Ÿ”Í±Qø÷ÀÙ‹K‘‘‘‘‘‘‘‘‘‘‘‘‘‘‘‘‘‘‘‘‘‘‘‘‘‘‘‘‘‘‘‘‘ˇ∑©æÕ—⁄ÚÎŒÊ∂Á≤¡˘˙ˆ¯ﬁ˜˚˝œØ¸Èº≠«·…∏Ô¿#¥π∆X
}è Ï√~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~U;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx@RRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRR<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<'?“*V{YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY:::::::::::::::NNNNNNNNNNNNNNNNNNNNNNAF&&&&&&&&&&&&&&&&&&&&&&&&&jDHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH®qqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqq9[GGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGO=+W_JEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE>]L5/66666666666666666666666666666666666666666666666666666666666wTv-44444444444444444444444444444444444444444444444444$%022222222222222222227!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!CM																								\\\\\\\\\\\\\\\\\\\\88888888888888888888888888),,,,,,,,,,,,b(g"""""""""""kPchInf1111111111mlt.opppppppppui                    yseeeeeeeeeeeeeeeeeeeeeeeeeeeaaaaaaa        deeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeennnnnnnnnnrlc.mptuo333333333aaaaaa        diiiiiiiiiiiiiiiiiiiiieeeeeeeeaaaaaa                            ss,	b2gggggggggggggggggggggg(h"fffffffffffffffffffffffffffffffffffffffffffffffffffInnnnnnnnnnrlc.mptuo3111111111deeeeeeeaaaaaa                                   ieeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaa  nrsccccccccccmlt.opppppppppuuuuuuuuiyeeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa        deeeeeeeeeeeeeeeeeeeeeeeeeeeaa4444444444444444SSSSSSSSSSSSSS:L5/BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBwTv-66666666666666666666666666666666666666666666666666666666666$%000000000000000000077777777777777777bbbbbbbbbbbbbbbbbbbbbbbbCCCCCCCCCCCCCCCCCCCCMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM\\\\\\\\\\\,	k8fgggggggggggg23h"(rrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr)))))))))))))cnmsttttttttttolllllllll.ipppppp        dueeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeaaaaaa        yIIIIIIIIIIIIIIIIIIIIIeecmrtnossssssssssssssssssild.......aaaaaa                            1pueeeeeeeaaaaaa                                                        ,	"fgby3h2m(Ptcorrrrrrrrrnisdddddddddd1111111111111lu.eeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaa         peeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaatommmmmmmmmcirdnIsuuuuuuuuuu        pleeeeeeeeeeeeeeeeeeeeeeeeeeeaaaaaaa        .eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeexA
























jDHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH599999999999999999999999GGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGO[+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++================================WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWE-4444444444444444SSSSSSSSSSSSSS:L!>7B]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]wTv/kkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkk$%00000000000000000006,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,CCCCCCCCCCCCCCCCCMgggggggggggggggggggggggggggggggggggggggggggggggggggggggggggg\h"f	1y3bo2(((((((((timdcIP8runpsaaaaaa        ..............................eeeeeeeeaaaaaa                            lloooooooootimdcccccccccccccrunps.eeeeeeeaaaaaa                                            eeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaa  fgggggggggggggggggggggg3h",I1y	ob2tlmmmmmmmmmcirdnnnnnnnnnnnnn(suuuuuuuuuuuuuuuuupeeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa        .eeeeeeeeeeeeeeeeeeeeeeeeeeeaatmoclrrrrrrrrrnisdddddddddd))))))        .ueeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeaaaaaa        pppppppppppppppppppppeev-4444444444444444SSSSSSSSSSSSSS:LLLLLLLLLLLLLLLLLLL777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777wT555555555555555555555555555555555555555kkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkk$%0//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////6C"fgPMy3hhhhhhhhhhhhhhhhhhhhhhhI1,m	bctronlssssssssssssssssssi.dddddddaaaaaa                            p)\2ueeeeeeeaaaaaa                                   crmntsooooooooool.........piudeeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaa         (eeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaah"fg1y3333333333338888888888888IIIIIIIIIIIr,	ncsmmmmmmmmmmt.opluuuuuuuuu        (bieeeeeeeeeeeeeeeeeeeeeeeeeeeaaaaaaa        deeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeensrrrrrrrrrrc.mptuo2laaaaaa        dddddddddddddddddddddddddddddeeeeeeeeaaaaaa                            iiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiii}}}}}}}}}}}}}}}}}}}}}}}}}}}}}<????????????????????????????V'HN;{Y™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™F_&+xA
























jDDDDDDDDDDDDDDD!99999999999999999999999GGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGO[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[4444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444===============================W:v-BEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE>SSSSSSSSSSSSSSSSSSSSSSSSSSSSSTTTTTTTTTTTTTTTTTTT7LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww000000000000000000000000000000000000000k55555555555555555$%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%PPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPP/hhhhhhhhhhhhhhhhhhhhhhhh6f)C1"3g8MyIIIIIIIIIIII,,,,,,,,,,,,,(((((((((((nsrrrrrrrrrrc.mptuo2	ldeeeeeeeaaaaaa                                           eeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaa  nricsmmmmmmmmmmt.opluuuuuuuuuuuuuuuubeeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa        deeeeeeeeeeeeeeeeeeeeeeeeeeeaa\fh1,3g"2IIIIIIIIIIIIyrrrrrrrrrrrrr(cnmitsooooooooool.........pppppp        dueeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeaaaaaa        bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbeecmrtnoilssssssssssssssssssd.......aaaaaa                            	pueeeeeeeaaaaaa                                   w:v-4]SSSSSSSSSSSSSS%TTTTTTTTTTTTTTTTTTT7LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL)000000000000000000000000000000000000000k55555555555555555$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$hhhhhhhhhhhhhhhhhhhhPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPP/g\6C8ffffffffffff,31b2I"myyyyyyyyyyyyytcorlnnnnnnnnnids	((((((((((u.eeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaa         peeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaatomlcccccccccrdnnnnnnnnnnnius        ppppppppppeeeeeeeeeeeeeeeeeeeeeeeeeeeaaaaaaa        .eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee3ghMIIIIIIIIIIII,f	b21o"yltttttttttmdcccccccccccccccccccccccrunpiaaaaaa        .ssssssssssssssssssssseeeeeeeeaaaaaa                                              oltttttttttmdc(runpi.eeeeeeeaaaaaa                                   seeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaa  BH+xA
























jDSSSSSSSSSSSSSSSSSSSSSSS!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!9[GGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOJJJJJJJJJJJJJJJ==================w:v-4]WEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE$%TTTTTTTTTTTTTTTTTTT7LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL\)000000000000000000000000000000000000000k55555555555555555555555555555555hhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPP,3gC/2IIIIIIIIIIIIM66666666666	bfo1"ttttttttttmlcccccccccrdn(yiuuuuuuuuspeeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa        .eeeeeeeeeeeeeeeeeeeeeeeeeeeaatmoccccccccccrlnnnnnnnnnidssssssssssssssssss        .ueeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeaaaaaa        pppppppppppppppppppppeeeeeeeeeeeee,3gb2Ih(((((((((((	8mf1ctronnnnnnnnnnilsssssssss.dddddddaaaaaa                            ppppppppppppp"ueeeeeeeaaaaaa                                   crmntiossssssssss.lpppppppppudeeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaa         yeeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaw:v-4SSSSSSSSSSSSSSSSS$%TTTTTTTTTTTTTTTTTTT7L>C\)000000000000000000000000000000000000000k55555555555555333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333PIIIIIIIIIIII,MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM	b2ggggggggggggg(((((((((((hr8/fncimst.oppppppppppul        y111111111eeeeeeeeeeeeeeeeeeeeeeeeeeeaaaaaaa        deeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeenirsc.mptuo""""""""""aaaaaa        dllllllllllllllllllllleeeeeeeeaaaaaa                                            ,I												23333333333333bbbbbbbbbbbg6(yhnirsc.mptuo"ffffffffffdeeeeeeeaaaaaa                                   leeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaa  nrrrrrrrrrcimst.oppppppppppuuuuuuuul1eeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa        deeeeeeeeeeeeeeeeeeeeeeeeeeeaajjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXU~®RRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRR@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““?}V<NNNNNNNNNNNNNNNNNNNNNNNNNNNN]™*';{_YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYBH+xA




























































F44444444444444444444444444444444444444444444444444444444444444444444[!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!999999999999999999999999999999999999999999999999999999999GGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOODJ&LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLw:v-E=55555555555555555$%TTTTTTTTTTTTTTTTTTT7SMC\)000000000000000000000000000000000000000k>W																																																																																																																																																													,I8P66666666666666666666666666666666666666666666666666666666666662333333333333"""""""""""gbr(ycnmmmmmmmmmtiossssssssss.lpppppp        dueeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeaaaaaa        1hhhhhhhhhhhhhhhhhhhhheecmrtnooooooooooooooooooilsd.......aaaaaa                            fpueeeeeeeaaaaaa                                   3333333333333,Ig/2	1""""""""""""""""""""""mb(tcorrrrrrrrrrnllllllllldifysu.eeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaa         peeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaatommmmmmmmmmclrdnhhhhhhhhhui        pseeeeeeeeeeeeeeeeeeeeeeeeeeeaaaaaaa        .eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee7LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLw:v-k55555555555555555$%TTTTTTTTTTTTTTTTTTT48MC\)000000000000000000000000000000000000000S,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,23333333333333666666666666666666666666666666g/PIf1"	oooooooooooobbbbbbbbbbtlmdch(runpppppppppaaaaaa        .iiiiiiiiiiiiiiiiiiiiieeeeeeeeaaaaaa                            ssooooooooootlmdcyrunppppppppp.eeeeeeeaaaaaa                                   ieeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaa                                                              23333333333333"""""""""""g,hf1Io												tsmmmmmmmmmmclrdnybbbbbbbbbuuuuuuuuipeeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa        .eeeeeeeeeeeeeeeeeeeeeeeeeeeaatmocsrrrrrrrrrrnllllllllldi((((((        .ueeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeaaaaaa        pppppppppppppppppppppeeE]BH+xA
























v[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!999999999999999GDOjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjj7LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLw:>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>k55555555555555555$%T-68MC\)04333333333333333333333333333333333333333333333333333333333333333333333333==========================================SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggg/21"""""""""""""""""""""""yhf,mI	ctronssssssssssssssssssil.dddddddaaaaaa                            p((((((((((((ueeeeeeeaaaaaa                                   crmntttttttttois..........pludeeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaa         beeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaag3Pf1"2(yhhhhhhhhhhhhhr,Incccccccccmit.opsuuuuuuuuuu        b	leeeeeeeeeeeeeeeeeeeeeeeeeeeaaaaaaa        deeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeennnnnnnnnric.mptuoooooooooooosaaaaaa        ddddddddddddddddddddddddddddddeeeeeeeeaaaaaa                            ll:::::::::::::::::::7L555555555555555555wwwwwwwwwwwwwwwwwwwwwwwwwwTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTkvM$%%%%%%%%%%%%%%%%%068-W\)CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC4444444444444444444444444444444444444444444444444444S33333333333333333333333333333333333333333333333333333fg"PPPPPPPPPPPPPPPPPPPPPPPP(1h2,ybbbbbbbbbbbbbnnnnnnnnnric.mptuooooooooooooIsdeeeeeeeaaaaaa                                            eeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaa  nrlcccccccccmit.opsuuuuuuuuuuuuuuuuu	eeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa        deeeeeeeeeeeeeeeeeeeeeeeeeeeaa(33333333333f,"/ggggggggggggh21rybcnmltttttttttois..........pppppp        dueeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeaaaaaa        																																	eecmrtnolssssssssssssssssssid.......aaaaaa                            Ipueeeeeeeaaaaaa                                   


































































































































































































?????????????????????????????????????????VVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVN}q<>____________________________';;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;{YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYE]BH+xAJJJJJJJJJJJJJJJJJJJJJJwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[!D9jGGGGGGGGGGGGGGGGGGGGGGGGGO%:::::::::::::::::::7L55555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555)TTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTkvM$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$068-WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWF\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\C4/SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSP3(2,"f												hgm1ytcorsnnnnnnnnnnldddddddddIbiu.eeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaa         peeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaatomsccccccccccrdnnnnnnnnnnnnnluuuuuuuuu        pieeeeeeeeeeeeeeeeeeeeeeeeeeeaaaaaaa        .eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee""""""""""""""""""""""""""""""""""3h2,(I												fog1sttttttttttmdcccccccccccccyrunplaaaaaa        .............................eeeeeeeeaaaaaa                            iiosttttttttttmdcbrunpl.eeeeeeeaaaaaa                                           eeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaa  $%:::::::::::::::::::7L555555555555555555\)TTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTkvMw////////////////////068-==================================================================================================================================================================================C44444444444444444,"""""""""""SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSh23333333333333I	(ofgtimsccccccccccrdnb1luuuuuuuuuuuuuuuupeeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa        .eeeeeeeeeeeeeeeeeeeeeeeeeeeaatmocirsnnnnnnnnnnldddddddddyyyyyy        .ueeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeaaaaaa        pppppppppppppppppppppee2,"""""""""""												hPbbbbbbbbbbbbbI3m(fctronilssssssssssssssssss.dddddddaaaaaa                            pygueeeeeeeaaaaaa                                   crmntloooooooooi.sppppppppppudeeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaa         1eeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa>E]BH+xA5555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[DDDDDDDDDDDDDDDDDDDDDDDj!!!!!!!!!!!!!!!!!!!!!!!!!9
GM$%:::::::::::::::::::7LWO===================================\)TTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTkvvvvvvvvvvvvvvvvvv4////////////////////068-w""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""h2,SCI																						ybbbbbbbbbbbbbPPPPPPPPPPPPPPPPPr3(nclmmmmmmmmmt.opius        1ffffffffffeeeeeeeeeeeeeeeeeeeeeeeeeeeaaaaaaa        deeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeenlrrrrrrrrrc.mptuogiaaaaaa        dssssssssssssssssssssseeeeeeeeaaaaaa                                              ,hI222222222222"y																							3b1111111111111111111111111111111111nlrrrrrrrrrc.mptuog(ideeeeeeeaaaaaa                                   seeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaa  nrrrrrrrrrrclmmmmmmmmmt.opiuuuuuuuusfeeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa        deeeeeeeeeeeeeeeeeeeeeeeeeeeaavM$%:::::::::::::::::::7L-&\)TTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTk5S4////////////////////06888888888888888888IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIwwwwwwwwwwwwwwwwy,hPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPP333333333333"2ggggggggggggggggggggggg	rb1cnmmmmmmmmmmtloooooooooi.spppppp        dueeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeaaaaaa        ffffffffffffffffffffffffffffffffffCCCCCCCCCCCCCCCCCCCCCeecmrtnooooooooooilsssssssssd.......aaaaaa                            (pueeeeeeeaaaaaa                                   "y,hhhhhhhhhhh333333333333Ifggggggggggggg2m	btcorinssssssssssdl(111111111u.eeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaa         peeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaatomicsrdnnnnnnnnnnnnnnnnnnnnnnnnnnul        pppppppppeeeeeeeeeeeeeeeeeeeeeeeeeeeaaaaaaa        .eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee«_°ß¶•ù£¢§ôüû†ïõöúëóñòçìíîâıéêÖãäåÅáÜà–ÉÇÑª„Ä’Ìz|´˛ÛµΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩΩÃ≥€∞Æ»ƒ‚ÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂÂ ÒÒÒÒÒÒÒÒÒÒÒÒÒÒÒÒÒÒÒÒÒÒÒÒÒÒÒÒÒÒÒÒÒÒÒÒÒÒÒÒÒÒÒÒÒÒÒÒÒÒÒ≈∫ﬂÓÿ›^`Ÿ‡ZËQ‰¬◊ÙÍ±”””””””””””””””””””””””””””””””””÷ÀøæK‘‹Ú∑©ˇ∂—⁄Õ˘ŒÊÎﬁ≤¡Á˝ˆ¯˙¸˚˜¨œØxº≠È·…∏Ô#¥π∆ ¿èÏUXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX√™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™™~®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®RRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRR@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@???????????????????????????????????????????????????????????VVVVVVVVVVVVVVVVVVVVVVVVVVVVVVNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNq““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““““}WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW<<<<<<<<<<<<<<<<<<<<<<<<<<<<'J;{YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY>E]BH+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777D[jjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjj
!A9kvM$%:::::::::::::::::::=G8-&&&&&&&&&&&&&&&&&&&&&&O\)TTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTLPS4////////////////////065,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww"yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy3h(fgIo2	itsmdccccccccccccccccccccccccccccccccccccccccccccccccccccccccccbrunppppppppppaaaaaa        .llllllllllllllllllllleeeeeeeeaaaaaa                                            oitsmdc1runpppppppppp.eeeeeeeaaaaaa                                   leeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaa  333333333333"yggggggggggggggggggggggg,C(fhoI2tttttttttmicsrdn1										uuuuuuuulpeeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa        .eeeeeeeeeeeeeeeeeeeeeeeeeeeaatmocccccccccrinssssssssssdlbbbbbb        .ueeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeaaaaaa        pppppppppppppppppppppeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeekvM$%:::::::::::::::::::68-----------------------------------\)T7777777777777777777777777777777777PS4////////////////////0L"wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww55555555555555555555555555555555555555555555555555555555555555555555555333333333333333333333333333333333333333333333333333fgggggggggggggy1CCCCCCCCCCCCCCCC(,mhIctronnnnnnnnnnnnnnnnnnils.dddddddaaaaaa                            pb2ueeeeeeeaaaaaa                                   crmnttttttttttolllllllll.ipsudeeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaa         	eeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa333333333333(fg"b111111111111111111111111111111111111111111yr,hnccccccccccmlt.opppppppppui        	Iseeeeeeeeeeeeeeeeeeeeeeeeeeeaaaaaaa        deeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeennnnnnnnnnrlc.mptuo222222222aaaaaa        diiiiiiiiiiiiiiiiiiiiieeeeeeeeaaaaaa                            ss+WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW>j]BHE====================================================================================================================================D[xv
























AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA!:::::::::::::::::::::::::::::::::::::::kF9-$%MT68888888888888888888S\)))))))))))))))))))))))))))))))))))G0000000000000000000000000000000000P777777777777777777////////////////////44444444444444444wwwwwwwwwwwwwwwwwwwwwwwwwwLLLLLLLLLLLLLLLLLLLLLLLLLL53CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC(((((((((((ggggggggggggbfffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff",1	ynnnnnnnnnnrlc.mptuo2hhhhhhhhhdeeeeeeeaaaaaa                                   ieeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaa  nrsccccccccccmlt.opppppppppuuuuuuuuiIeeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa        deeeeeeeeeeeeeeeeeeeeeeeeeeeaab3333333333333(,gggggggggggggggggggggg2222222222222222"fr1	cnmsttttttttttolllllllll.ipppppp        dueeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeaaaaaa        Iyyyyyyyyyyyyyyyyyyyyyeecmrtnossssssssssssssssssild.......aaaaaa                            hpueeeeeeeaaaaaa                                   ):::::::::::::::::::::::::::::::::::::::kv-$%%%%%%%%%%%%%%%%%%%%T68888888888888888888S\MC0000000000000000000000000000000000P777777777777777777/OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOw4LLLLLLLLLLLLb3333333333333333333333333333333333333333335",g(I22222222222222222222222222222222222222222222222222222222222222222222222222222222222222mf1tcorrrrrrrrrnisddddddddddh	lu.eeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaa         peeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaatommmmmmmmmcirdnysuuuuuuuuuu        pleeeeeeeeeeeeeeeeeeeeeeeeeeeaaaaaaa        .eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeggggggggggggb333333333333333333333333",,,,,,,,,,,,,hI2(oooooooooooffffffffftimdcy1runpsaaaaaa        ..............................eeeeeeeeaaaaaa                            lloooooooootimdc	runps.eeeeeeeaaaaaa                                            eeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaa                                                  ?_VVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNN*************************************************************************************************************************************************************************F}<<<<<<<<<<<<<<<<<<<<<<<<<<<<'J;{YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY&WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW>j]BH+$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$==============================================================[[[[[[[[[[[[[[[
DAxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxEEEEEEEEEEEEEEEEEEEEEEE\):::::::::::::::::::::::::::::::::::::::kv-----------------------------------!////////////////////T68888888888888888888S%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%C0000000000000000000000000000000000P777777777777777777Mbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb44444444444444444O9w,gggggggggggggggggggggggggggL2222222222222222222222225"3yhIIIIIIIIIIIIIo(((((((((((tlmmmmmmmmmcirdn	fsuuuuuuuuuuuuuuuuupeeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa        .eeeeeeeeeeeeeeeeeeeeeeeeeeeaatmoclrrrrrrrrrnisdddddddddd111111        .ueeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeaaaaaa        pppppppppppppppppppppee",ggggggggggggI2222222222222222222222222222222222222222222222222222222222222b	yh3mmmmmmmmmmmmm(ctronlssssssssssssssssssi.dddddddaaaaaa                            p11111111111ueeeeeeeaaaaaa                                   crmntsooooooooool.........piudeeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaa         feeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaS\):::::::::::::::::::::::::::::::::::::::kv------------------////////////////////T68888888888888888888$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$C0000000000000000000000000000000000P7%g44444444444444GGGGGGGGGGGGGGGGGGGGGGGGGGMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMwLLLLLLLLLLLLLLLLLLLLLLLL,"hI2222222222221	ybr3333333333333ncsmmmmmmmmmmt.opluuuuuuuuu        f(ieeeeeeeeeeeeeeeeeeeeeeeeeeeaaaaaaa        deeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeensrrrrrrrrrrc.mptuooooooooooolaaaaaa        dddddddddddddddddddddddddddddeeeeeeeeaaaaaa                            ii,gh52"1Iyyyyyyyyyyyy3	fbnsrrrrrrrrrrc.mptuoooooooooooooooooooooooldeeeeeeeaaaaaa                                           eeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaa  nricsmmmmmmmmmmt.opluuuuuuuuuuuuuuuu(eeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa        deeeeeeeeeeeeeeeeeeeeeeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW>j]BHvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv[=






























AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADEx+++++++++++++++++++++++++++++++++++++++++++S\):::::::::::::::::::::::::::::::::::::::kOOOOOOOOOOOOOOOOOOOOOOO777777777777777777////////////////////T68---------------------------------------------------------------------------------------------------------------------C0000000000000000000000000000000000P$hG!4MMMMMMMMMMMMMM%%%%%%%%%%%%%%%%%%%%%%%%%%1,gLLLLLLLLLLLLLLLLL32"5wwwwwwwwwwwyyyyyyyyyyyyIr	fcnmitsooooooooool.........pppppp        dueeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeaaaaaa        (bbbbbbbbbbbbbbbbbbbbbeecmrtnoilssssssssssssssssssd.......aaaaaa                                        pueeeeeeeaaaaaa                                   "1,gggggggggggg32h(((((((((((yyyyyyyyyyyyyyyyyyyyyyyymI	tcorlnnnnnnnnnidsssssssssssssffffffffffu.eeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaa         peeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaatomlcccccccccrdnbius        ppppppppppeeeeeeeeeeeeeeeeeeeeeeeeeeeaaaaaaa        .eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee8888888888888888888S\):::::::::::::::::::::::::::::::::::::::kP777777777777777777////////////////////T6vLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLC0000000000000000000000000000000000-,M9%4$$$$$$$$$$$$$$2"155555555555555555555555555yyyyyyyyyyyy3ggggggggggggg(((((((((((hooooooooooooooooooooooooooooooooooooooooIltttttttttmdcb	runpiaaaaaa        .ssssssssssssssssssssseeeeeeeeaaaaaa                                              oltttttttttmdcfrunpi.eeeeeeeaaaaaa                                   seeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaa  32"11111111111yyyyyyyyyyyy,bbbbbbbbbbbbb(gohwttttttttttmlcccccccccrdnfIiuuuuuuuuspeeeeeeeaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa        .eeeeeeeeeeeeeeeeeeeeeeeeeeeaatmoccccccccccrlnnnnnnnnnids						        .ueeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeaaaaaa        pppppppppppppppppppppeeU™XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRq~®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®®???????????????????????????????????????????@VVVVVVVVVVVVVVVVVVVVVVVVVVVVVN_**********************************************************************************************************************************************************************************************************************************************************FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFO''''''''''''''''''''''''''''''''''''''}<YYYYYYYYYYYYYYYYYYYYYYYYYYYYJ;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;{{