
function ChristmasPartay::spawnAllGates(%ingroup, %outgroup) {
	focusserver();

	if (%ingroup == "" || %outgroup == "") return;
	
	function spawngates(%m, %outgroup) {
		echo("STUFF:" @ %m @ ", " @ getObjectType(%m));
		if (getObjectType(%m) != "Marker") return;
		%pos = GameBase::getPosition(%m);
		%rotZ = getWord(GameBase::getRotation(%m), 2);
		
		%g = newObject("Gate", SimGroup);
		addToSet(%outgroup, %g);
		
		ChristmasPartay::spawnGate(%g, %pos, %rotZ, $ChristmasPartay::gateWidth, 0);

		//removeFromSet(getGroup(%m), %m);
		//deleteObject(%m);
	}
	Group::iterateRecursive(%ingroup, spawngates, %outgroup);
}

$ChristmasPartay::gateHeight = 16;
$ChristmasPartay::gateWidth = 6;
function ChristmasPartay::spawnGate(%group, %position, %rotZ, %gateWidth, %gateID) {
	%rot = "0 0 " @ %rotZ;
	%rotMat = Matrix::rotZ(%rotZ);
	%vecX = Matrix::mul(%rotMat, 3, 3, "1 0 0", 3, 1);

	%leftPos = Vector::add(%position, Vector::mul(%vecX, -%gateWidth/2));
	%rightPos = Vector::add(%position, Vector::mul(%vecX, %gateWidth/2));

	echo(%leftPos);
	if (!getLOSInfo(Vector::add(%leftPos, "0 0 10000"), Vector::add(%leftPos, "0 0 -10000"), 1)) {echo("ERROR: No terrain found for gate at " @ %position); return; }
	%leftPos = Vector::add($los::position, "0 0 -1");

	if (!getLOSInfo(Vector::add(%rightPos, "0 0 10000"), Vector::add(%rightPos, "0 0 -10000"), 1)) {echo("ERROR: No terrain found for gate at " @ %position); return; }
	%rightPos = Vector::add($los::position, "0 0 -1");

	%gatePos = Vector::mul(Vector::add(%leftPos, %rightPos), 0.5);
	%zOffset = -abs(getWord(%leftPos, 2) - getWord(%rightPos, 2))/2;
	%gatePos = Vector::add(%gatepos, "0 0 " @ %zOffset);
	%gateBox = -%gateWidth/2 @ " -1 0 " @ %gateWidth/2 @ " 1 " @ ($ChristmasPartay::gateHeight) + %zOffset*2;
	
	%leftGatePost = newObject("GatePost", StaticShape, LargeAntenna, false);
	%leftGatePost.invinc = true;
	GameBase::setPosition(%leftGatePost, %leftPos);
	GameBase::setRotation(%leftGatePost, %rot);
	addToSet(%group, %leftGatePost);

	%rightGatePost = newObject("GatePost", StaticShape, LargeAntenna, false);
	%rightGatePost.invinc = true;
	GameBase::setPosition(%rightGatePost, %rightPos);
	GameBase::setRotation(%rightGatePost, %rot);
	addToSet(%group, %rightGatePost);

	setInstantGroup(0);
	instant Trigger "TMP" {
		dataBlock = "GroupTrigger";
		name = "";
		position = %gatePos;
		rotation = %rot;
		boundingBox = %gateBox;
		isSphere = "False";
		gateID = %gateID;
	};
	%id = nameToID("TMP");
	renameObject("TMP", "GateTrigger");
	
	addToSet(%group, %id);
}




























$Ski::callRate = 0.07;
$Ski::drag = 0.01;
function Ski::checkSki(%this) {
	if (Player::getArmor(%this) != IceMale && Player::getArmor(%this) != IceFemale) return;

	if (Player::getLastContactCount(%this) != 0) {
		schedule("Ski::checkSki("@%this@");", $Ski::callRate);
		return;
	}

	%vel = Item::getVelocity(%this);
	%velP = getWord(%vel, 0)@" "@getWord(%vel, 1)@" 0";
	%speed = Vector::length(%velP);

	%rot = GameBase::getRotation(%this);
	%rotVec = Vector::getFromRot(%rot, 1);

	%skiCos = Vector::dot(%velP, %rotVec)/%speed;
	%skiSin = sqrt(1-%skiCos*%skiCos);
	
	if (%skiCos < 0) {
		%skiCos *= -1;
		//%rotVec = Vector::neg(%rotVec);
	}

	%normalVec = -getWord(%rotVec, 1)@" "@getWord(%rotVec,0)@" 0";
	if (Vector::dot(%normalVec, %velP) > 0) %normalVec = Vector::neg(%normalVec);

	//echo("Q: " @ %velP @ ", " @ %normalVec @ ", " @ %skiCos @ ", " @ %skiSin);
	
	%normalForce = Vector::mul(%normalVec, %skiSin*%speed*5);
	%forwardForce = "0 0 0";//Vector::mul(%rotVec, %skiCos*%speed*0.1);
	
	%forceVec = Vector::add(%normalForce, %forwardForce);
	%newVel = Vector::add(%vel, %forceVec*$Ski::callRate);
	//%newVel2 = Vector::mul(%newVel2, 1 - $Ski::drag*$Ski::callRate);

	if (Player::getLastContactCount(%this) == 0 && Player::isJetting(%this) && %speed<165) {
		if (Vector::dot(%velP, %rotVec) > 0) %newVel = Vector::add(%newVel, Vector::mul(%rotVec, 10*$Ski::callRate));
		else                               %newVel = Vector::add(%newVel, Vector::mul(%rotVec, 10*$Ski::callRate));
	}

	Item::setVelocity(%this, %newVel);
	schedule("Ski::checkSki("@%this@");", $Ski::callRate);
}

PlayerData IceMale {
   className = "Armor";
   shapeFile = "larmor";
   damageSkinData = "armorDamageSkins";
	debrisId = playerDebris;
   flameShapeName = "breath";
   shieldShapeName = "shield";
   shadowDetailMask = 1;

   visibleToSensor = True;
	mapFilter = 1;
	mapIcon = "M_player";
   canCrouch = true;

   maxJetSideForceFactor = 1;
   maxJetForwardVelocity = 20;
   minJetEnergy = 1;
   jetForce = 1;
   jetEnergyDrain = 0;

	maxDamage = 65;
   maxForwardSpeed = 15;
   maxBackwardSpeed = 1;
   maxSideSpeed = 0;
   groundForce = 4*0.2;//40 * 9.0;
   mass = 9.0;
   groundTraction = 0.3;//30.0;
	maxEnergy = 4000;
   drag = 1.0;
   density = 1.2;

	minDamageSpeed = 36;
	damageScale = 4;

   jumpImpulse = 100;
   jumpSurfaceMinDot = 0.2;

   // animation data:
   // animation name, one shot, direction
	// firstPerson, chaseCam, thirdPerson, signalThread
   // movement animations:
   animData[0]  = { "root", none, 1, true, true, true, false, 0 };
   animData[1]  = { "run", none, 1, true, false, true, false, 3 };
   animData[2]  = { "runback", none, 1, true, false, true, false, 3 };
   animData[3]  = { "side left", none, 1, true, false, true, false, 3 };
   animData[4]  = { "side left", none, -1, true, false, true, false, 3 };
   animData[5] = { "jump stand", SoundLandOnGround, 1, true, false, true, false, 3 };
   animData[6] = { "jump run", SoundLandOnGround, 1, true, false, true, false, 3 };
   animData[7] = { "crouch root", none, 1, true, true, true, false, 3 };
   animData[8] = { "crouch root", none, 1, true, true, true, false, 3 };
   animData[9] = { "crouch root", none, -1, true, true, true, false, 3 };
   animData[10] = { "crouch forward", none, 1, true, false, true, false, 3 };
   animData[11] = { "crouch forward", none, -1, true, false, true, false, 3 };
   animData[12] = { "crouch side left", none, 1, true, false, true, false, 3 };
   animData[13] = { "crouch side left", none, -1, true, false, true, false, 3 };
   animData[14]  = { "fall", none, 1, true, true, true, false, 3 };
   animData[15]  = { "landing", SoundLandOnGround, 1, true, false, false, false, 3 };
   animData[16]  = { "landing", SoundLandOnGround, 1, true, false, false, false, 3 };
   animData[17]  = { "tumble loop", none, 1, true, false, false, false, 3 };
   animData[18]  = { "tumble end", none, 1, true, false, false, false, 3 };
   animData[19] = { "celebration 1", none, -1, true, false, true, false, 0 };

   // misc. animations:
   animData[20] = { "PDA access", none, 1, true, false, false, false, 3 };
   animData[21] = { "throw", none, 1, true, false, false, false, 3 };
   animData[22] = { "flyer root", none, 1, false, false, false, false, 3 };
   animData[23] = { "apc root", none, 1, true, true, true, false, 3 };
   animData[24] = { "apc pilot", none, 1, false, false, false, false, 3 };
   
   // death animations:
   animData[25] = { "crouch die", SoundPlayerDeath, 1, false, false, false, false, 4 };
   animData[26] = { "die chest", SoundPlayerDeath, 1, false, false, false, false, 4 };
   animData[27] = { "die head", SoundPlayerDeath, 1, false, false, false, false, 4 };
   animData[28] = { "die grab back", SoundPlayerDeath, 1, false, false, false, false, 4 };
   animData[29] = { "die right side", SoundPlayerDeath, 1, false, false, false, false, 4 };
   animData[30] = { "die left side", SoundPlayerDeath, 1, false, false, false, false, 4 };
   animData[31] = { "die leg left", SoundPlayerDeath, 1, false, false, false, false, 4 };
   animData[32] = { "die leg right", SoundPlayerDeath, 1, false, false, false, false, 4 };
   animData[33] = { "die blown back", SoundPlayerDeath, 1, false, false, false, false, 4 };
   animData[34] = { "die spin", SoundPlayerDeath, 1, false, false, false, false, 4 };
   animData[35] = { "die forward", SoundPlayerDeath, 1, false, false, false, false, 4 };
   animData[36] = { "die forward kneel", SoundPlayerDeath, 1, false, false, false, false, 4 };
   animData[37] = { "die back", SoundPlayerDeath, 1, false, false, false, false, 4 };

   // signal moves:
	animData[38] = { "sign over here",  none, 1, true, false, false, false, 2 };
   animData[39] = { "sign point", none, 1, true, false, false, true, 1 };
   animData[40] = { "sign retreat",none, 1, true, false, false, false, 2 };
   animData[41] = { "sign stop", none, 1, true, false, false, true, 1 };
   animData[42] = { "sign salut", none, 1, true, false, false, true, 1 }; 


    // celebration animations:
   animData[43] = { "celebration 1",none, 1, true, false, false, false, 2 };
   animData[44] = { "celebration 2", none, 1, true, false, false, false, 2 };
   animData[45] = { "celebration 3", none, 1, true, false, false, false, 2 };
 
    // taunt animations:
	animData[46] = { "taunt 1", none, 1, true, false, false, false, 2 };
	animData[47] = { "taunt 2", none, 1, true, false, false, false, 2 };
 
    // poses:
	animData[48] = { "pose kneel", none, 1, true, false, false, true, 1 };
	animData[49] = { "pose stand", none, 1, true, false, false, true, 1 };

	// Bonus wave
   animData[50] = { "wave", none, 1, true, false, false, true, 1 };

   jetSound = SoundJetLight;
   rFootSounds = 
   {
     SoundLFootRSoft,
     SoundLFootRHard,
     SoundLFootRSoft,
     SoundLFootRHard,
     SoundLFootRSoft,
     SoundLFootRSoft,
     SoundLFootRSoft,
     SoundLFootRHard,
     SoundLFootRSnow,
     SoundLFootRSoft,
     SoundLFootRSoft,
     SoundLFootRSoft,
     SoundLFootRSoft,
     SoundLFootRSoft,
     SoundLFootRSoft
  }; 
   lFootSounds =
   {
      SoundLFootLSoft,
      SoundLFootLHard,
      SoundLFootLSoft,
      SoundLFootLHard,
      SoundLFootLSoft,
      SoundLFootLSoft,
      SoundLFootLSoft,
      SoundLFootLHard,
      SoundLFootLSnow,
      SoundLFootLSoft,
      SoundLFootLSoft,
      SoundLFootLSoft,
      SoundLFootLSoft,
      SoundLFootLSoft,
      SoundLFootLSoft
   };

   footPrints = { 0, 1 };

   boxWidth = 0.5;
   boxDepth = 0.5;
   boxNormalHeight = 2.3;
   boxCrouchHeight = 1.8;

   boxNormalHeadPercentage  = 0.83;
   boxNormalTorsoPercentage = 0.53;
   boxCrouchHeadPercentage  = 0.6666;
   boxCrouchTorsoPercentage = 0.3333;

   boxHeadLeftPercentage  = 0;
   boxHeadRightPercentage = 1;
   boxHeadBackPercentage  = 0;
   boxHeadFrontPercentage = 1;
};

PlayerData IceFemale {
   className = "Armor";
   shapeFile = "lfemale";
   damageSkinData = "armorDamageSkins";
	debrisId = playerDebris;
   flameShapeName = "";
   shieldShapeName = "shield";
   shadowDetailMask = 1;

   visibleToSensor = True;
	mapFilter = 1;
	mapIcon = "M_player";
   canCrouch = true;

   maxJetSideForceFactor = 1;
   maxJetForwardVelocity = 20;
   minJetEnergy = 1;
   jetForce = 1;
   jetEnergyDrain = 0;

	maxDamage = 65;
   maxForwardSpeed = 15;
   maxBackwardSpeed = 1;
   maxSideSpeed = 0;
   groundForce = 4*0.2;//40 * 9.0;
   mass = 9.0;
   groundTraction = 0.3;//30.0;
	maxEnergy = 4000;
   drag = 1.0;
   density = 1.2;

	minDamageSpeed = 36;
	damageScale = 4;

   jumpImpulse = 100;
   jumpSurfaceMinDot = 0.2;

   // animation data:
   // animation name, one shot, direction
	// firstPerson, chaseCam, thirdPerson, signalThread
   // movement animations:
   animData[0]  = { "root", none, 1, true, true, true, false, 0 };
   animData[1]  = { "run", none, 1, true, false, true, false, 3 };
   animData[2]  = { "runback", none, 1, true, false, true, false, 3 };
   animData[3]  = { "side left", none, 1, true, false, true, false, 3 };
   animData[4]  = { "side left", none, -1, true, false, true, false, 3 };
   animData[5] = { "jump stand", none, 1, true, false, true, false, 3 };
   animData[6] = { "jump run", none, 1, true, false, true, false, 3 };
   animData[7] = { "crouch root", none, 1, true, true, true, false, 3 };
   animData[8] = { "crouch root", none, 1, true, true, true, false, 3 };
   animData[9] = { "crouch root", none, -1, true, true, true, false, 3 };
   animData[10] = { "crouch forward", none, 1, true, false, true, false, 3 };
   animData[11] = { "crouch forward", none, -1, true, false, true, false, 3 };
   animData[12] = { "crouch side left", none, 1, true, false, true, false, 3 };
   animData[13] = { "crouch side left", none, -1, true, false, true, false, 3 };
   animData[14]  = { "fall", none, 1, true, true, true, false, 3 };
   animData[15]  = { "landing", SoundLandOnGround, 1, true, false, false, false, 3 };
   animData[16]  = { "landing", SoundLandOnGround, 1, true, false, false, false, 3 };
   animData[17]  = { "tumble loop", none, 1, true, false, false, false, 3 };
   animData[18]  = { "tumble end", none, 1, true, false, false, false, 3 };
   animData[19] = { "celebration 1", none, -1, true, false, true, false, 0 };

   // misc. animations:
   animData[20] = { "PDA access", none, 1, true, false, false, false, 3 };
   animData[21] = { "throw", none, 1, true, false, false, false, 3 };
   animData[22] = { "flyer root", none, 1, false, false, false, false, 3 };
   animData[23] = { "apc root", none, 1, true, true, true, false, 3 };
   animData[24] = { "apc pilot", none, 1, false, false, false, false, 3 };
   
   // death animations:
   animData[25] = { "crouch die", SoundPlayerDeath, 1, false, false, false, false, 4 };
   animData[26] = { "die chest", SoundPlayerDeath, 1, false, false, false, false, 4 };
   animData[27] = { "die head", SoundPlayerDeath, 1, false, false, false, false, 4 };
   animData[28] = { "die grab back", SoundPlayerDeath, 1, false, false, false, false, 4 };
   animData[29] = { "die right side", SoundPlayerDeath, 1, false, false, false, false, 4 };
   animData[30] = { "die left side", SoundPlayerDeath, 1, false, false, false, false, 4 };
   animData[31] = { "die leg left", SoundPlayerDeath, 1, false, false, false, false, 4 };
   animData[32] = { "die leg right", SoundPlayerDeath, 1, false, false, false, false, 4 };
   animData[33] = { "die blown back", SoundPlayerDeath, 1, false, false, false, false, 4 };
   animData[34] = { "die spin", SoundPlayerDeath, 1, false, false, false, false, 4 };
   animData[35] = { "die forward", SoundPlayerDeath, 1, false, false, false, false, 4 };
   animData[36] = { "die forward kneel", SoundPlayerDeath, 1, false, false, false, false, 4 };
   animData[37] = { "die back", SoundPlayerDeath, 1, false, false, false, false, 4 };

   // signal moves:
	animData[38] = { "sign over here",  none, 1, true, false, false, false, 2 };
   animData[39] = { "sign point", none, 1, true, false, false, true, 1 };
   animData[40] = { "sign retreat",none, 1, true, false, false, false, 2 };
   animData[41] = { "sign stop", none, 1, true, false, false, true, 1 };
   animData[42] = { "sign salut", none, 1, true, false, false, true, 1 }; 


    // celebration animations:
   animData[43] = { "celebration 1",none, 1, true, false, false, false, 2 };
   animData[44] = { "celebration 2", none, 1, true, false, false, false, 2 };
   animData[45] = { "celebration 3", none, 1, true, false, false, false, 2 };
 
    // taunt animations:
	animData[46] = { "taunt 1", none, 1, true, false, false, false, 2 };
	animData[47] = { "taunt 2", none, 1, true, false, false, false, 2 };
 
    // poses:
	animData[48] = { "pose kneel", none, 1, true, false, false, true, 1 };
	animData[49] = { "pose stand", none, 1, true, false, false, true, 1 };

	// Bonus wave
   animData[50] = { "wave", none, 1, true, false, false, true, 1 };

   jetSound = SoundJetLight;
   rFootSounds = 
   {
     SoundLFootRSoft,
     SoundLFootRHard,
     SoundLFootRSoft,
     SoundLFootRHard,
     SoundLFootRSoft,
     SoundLFootRSoft,
     SoundLFootRSoft,
     SoundLFootRHard,
     SoundLFootRSnow,
     SoundLFootRSoft,
     SoundLFootRSoft,
     SoundLFootRSoft,
     SoundLFootRSoft,
     SoundLFootRSoft,
     SoundLFootRSoft
  }; 
   lFootSounds =
   {
      SoundLFootLSoft,
      SoundLFootLHard,
      SoundLFootLSoft,
      SoundLFootLHard,
      SoundLFootLSoft,
      SoundLFootLSoft,
      SoundLFootLSoft,
      SoundLFootLHard,
      SoundLFootLSnow,
      SoundLFootLSoft,
      SoundLFootLSoft,
      SoundLFootLSoft,
      SoundLFootLSoft,
      SoundLFootLSoft,
      SoundLFootLSoft
   };

   footPrints = { 0, 1 };

   boxWidth = 0.5;
   boxDepth = 0.5;
   boxNormalHeight = 2.3;
   boxCrouchHeight = 1.8;

   boxNormalHeadPercentage  = 0.83;
   boxNormalTorsoPercentage = 0.53;
   boxCrouchHeadPercentage  = 0.6666;
   boxCrouchTorsoPercentage = 0.3333;

   boxHeadLeftPercentage  = 0;
   boxHeadRightPercentage = 1;
   boxHeadBackPercentage  = 0;
   boxHeadFrontPercentage = 1;
};