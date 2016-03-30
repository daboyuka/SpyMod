// Keypad doors

MoveableData KeypadDoor1 {
	shapeFile = "newdoor5";
	className = "KeypadDoor";
	maxDamage = 300;
	debrisId = defaultDebrisLarge;
	speed = 10;
	sfxStart = SoundDoorOpen;
	sfxStop = SoundDoorClose;
	side = "single";
  //	triggerRadius = 3;
	explosionId = debrisExpLarge;
   isPerspective = true;
	displace = false;
	blockBackward = false;
};

MoveableData KeypadDoorForceField4x8 {
	shapeFile = "forcefield_4x8";
	className = "KeypadDoor";//"Door";
	maxDamage = 10000;
	debrisId = defaultDebrisLarge;
	speed = 100;
	sfxStop = ForceFieldClose;
	side = "single";
	triggerRadius = 4;
	explosionId = debrisExpLarge;
	isTranslucent = true;
	displace = false;
	blockBackward = false;
};

$ForceFields[KeypadDoorForceField4x8] = 1;

function KeypadDoor::onAdd(%this)
{
	if(!GameBase::isPowered(%this))
		Door::onPower(%this,"False");   

	%this.closeTime = getSimTime();
 	%this.fadeTime	 = getSimTime();
 	%this.faded = "";
}

function KeypadDoor::onNewPath(%this) {
	%numPoints = Moveable::getWaypointCount(%this);
	%name = GameBase::getDataName(%this);
	if(%numPoints <= 2 && (%name.side=="left" || %name.side == "right"))
		%name.side = "single";

	if(%name.side == "single") {
		%center = %numPoints-1;
		Moveable::setWaypoint(%this,%center);
	} else {
		%center = floor(%numPoints/2);
		Moveable::setWaypoint(%this,%center);
	}
	%this.center = %center;
}

function KeypadDoor::onEnabled(%this) {
	GameBase::setActive(%this,true);
	if (%this.center != "" && GameBase::isPowered(%this)) { 
		%this.closeTime = getSimTime() + 1;
		KeypadDoor::closeCheck(%this);
	} else KeypadDoor::onPower(%this,"False");   
}

function KeypadDoor::trigger(%this) {
	%waypoint = Moveable::getPosition(%this);
	if((%waypoint != 0 && %waypoint != Moveable::getWaypointCount(%this)-1) || ((GameBase::getDataName(%this)).side == "single" && %waypoint != 0)) {
		%this.status = "open";
		if(!%this.triggerOpen == "")
			%this.playerTrigger = 1; 
		KeypadDoor::onMove(%this);	
	}
}

function KeypadDoor::onPower(%this, %state, %generator) {
	if(%state) { 
		if(Moveable::getPosition(%this) != %this.center && %this.center != "") {
			%this.status = "close";
			%this.closeTime = getSimTime() + 1;
			KeypadDoor::closeCheck(%this);
		}
	} else { 
		%this.status = "open";
		KeypadDoor::onMove(%this, 1);
	}
}

function KeypadDoor::onFirst(%this) {
	if (%this.triggerOpen == "") {
		%this.status = "close";
		%this.closeTime = getSimTime() + 3;
		KeypadDoor::closeCheck(%this);
	} else if(%this.playerTrigger) {
		%this.status = "close";
		%this.closeTime = getSimTime() + 1;
		KeypadDoor::closeCheck(%this);
		%this.playerTrigger = "";
	}	
}

function KeypadDoor::onLast(%this) {
	if((GameBase::getDataName(%this)).side != "single") {
		if(%this.triggerOpen == "") {
			%this.status = "close";
			%this.closeTime = getSimTime() + 3;
			KeypadDoor::closeCheck(%this);
		} else if(%this.playerTrigger) {
			%this.status = "close";					
			%this.closeTime = getSimTime() + 1;
			KeypadDoor::closeCheck(%this);
			%this.playerTrigger = "";
		}	
	}
}

function KeypadDoor::onWaypoint(%this) {
	if(Moveable::getPosition(%this) == %this.center && %this.status == "close") 
		%this.triggerTrigger = "";
}

function KeypadDoor::onMove(%this, %forceClose) {
	if($ForceFields[GameBase::getDataName(%this)] != "" && %this.faded == "" && %this.status == "open") {
		GameBase::startFadeOut(%this);
		%time = getSimTime() - %this.fadeTime;
		if(%time > 2.5)
			%time = 2.5;
		%this.faded = 1;
		if(%forceClose != "")
			schedule("KeypadDoor::onMove(" @ %this @ "," @ %forceClose @ ");",%time,%this);
		else
			schedule("KeypadDoor::onMove(" @ %this @ ");",%time,%this);
		schedule("GameBase::playSound(" @ %this @ ",ForceFieldOpen,0);",(%time/2),%this);
		return;		
	} if((GameBase::isActive(%this) && GameBase::isPowered(%this)) || %forceClose != "") {
		if(%this.status == "open") {
			if((GameBase::getDataName(%this)).side == "left")
				Moveable::moveToWaypoint(%this,(Moveable::getWaypointCount(%this)-1));				
			else
				Moveable::moveToWaypoint(%this,0);				
	 	}
		else { 
			Moveable::moveToWaypoint(%this,%this.center);				
		}
	}
	if($ForceFields[GameBase::getDataName(%this)] != "" && %this.status == "close" && !%this.faded == "" && GameBase::isPowered(%this)) {
		GameBase::startFadeIn(%this);
	 	%this.faded="";
		%this.fadeTime = getSimTime();
	}
}

function KeypadDoor::onCollision(%this, %object) {
	return;

	if(!Player::isDead(%object) && getObjectType(%object) == "Player") 
		if (GameBase::isActive(%this) && GameBase::isPowered(%this) && %this.faded == "")  
			if(GameBase::getTeam(%this) == Client::getApparentTeam(Player::getClient(%object)) || GameBase::getTeam(%this) == -1 || %this.noTeam != "" ) 
				if((%this.triggerOpen == "" || %this.triggerTrigger) ) 
					KeypadDoor::trigger(%this);
}

function KeypadDoor::onBlocker(%this,%obj) {
	GameBase::applyDamage(%obj,$CrushDamageType,1,
		GameBase::getPosition(%this),"0 0 0","0 0 0",%this);
}

function KeypadDoor::onDamage(%this,%type,%value,%pos,%vec,%mom,%object) {
	%damageLevel = GameBase::getDamageLevel(%this);
	%TDS= 1;
	if(GameBase::getTeam(%this) == Client::getApparentTeam(Player::getClient(%object)))
		%TDS = $Server::TeamDamageScale;
	GameBase::setDamageLevel(%this,%damageLevel + %value * DamageScale::getDamageScale(%this, %type));
}

function KeypadDoor::onDisabled(%this) {
	GameBase::setActive(%this,false);
   Moveable::stop(%this);
}

function KeypadDoor::onDestroyed(%this) {
	GameBase::setActive(%this,false); 		
   Moveable::stop(%this);
}	

function KeypadDoor::closeCheck(%this) {
	if (getSimTime() >= %this.closeTime)
		KeypadDoor::onMove(%this);
	else
		schedule("KeypadDoor::closeCheck(" @ %this @ ");",1,%this);
}

function KeypadDoor::onTrigEnter(%this,%object,%trigger) {
	return;

	if(GameBase::getTeam(%this) == Client::getApparentTeam(Player::getClient(%object)) || GameBase::getTeam(%this) == -1 || %this.noTeam != "" ) {
		%this.status = "open";
		KeypadDoor::onMove(%this);
	}
}

function KeypadDoor::onTrigLeave(%this,%object,%trigger) {
	return;

	if(GameBase::getTeam(%this) == Client::getApparentTeam(Player::getClient(%object)) || GameBase::getTeam(%this) == -1 || %this.noTeam != "" ) {
		%this.status = "close";
		%this.triggerTrigger = 1;
		KeypadDoor::onMove(%this);
	}
}

function KeypadDoor::onTrigger(%this,%object,%trigger) {

}

// Keypads

function Keypad::onAdd(%this) {
  %this.numDoors = 0;
  schedule("Keypad::checkForDoors("@%this@", getGroup("@%this@"));",5);
  GameBase::setActive(%this, !%this.disabled);
}

function Keypad::onCollision(%this, %object) {
  if (getObjectType(%object) == "Player" && GameBase::isActive(%this)) {
    if (!Player::isDead(%object)) {
      for (%i = 0; %i < %this.numDoors; %i++) {
        if (%this.door[%i].playerTrigger == "") {
          GameBase::virtual(%this.door[%i], "trigger", %this.door[%i]);//KeypadDoor::trigger(%this.door[%i]);
          GameBase::playSequence(%this, 0, "Sequence01");
          schedule("GameBase::stopSequence("@%this@", 0);",1);
        }
      }
    }
  }
}

function Keypad::checkForDoors(%this, %group) {
  %numObj = Group::objectCount(%group);
  for (%i = 0; %i < %numObj; %i++) {
    %obj = Group::getObject(%group, %i);
    if (getObjectType(%obj) == "SimGroup") { Keypad::checkForDoors(%this, %obj); continue; }
    %name = GameBase::getDataName(%obj);
    if (getObjectType(%obj) == "Moveable") {
      %this.door[%this.numDoors] = %obj;
      %this.numDoors++;
    }
  }
}

StaticShapeData Keypad1 {
   description = "Keypad";
   shapeFile = "display_two";
	className = "Keypad";
	debrisId = flashDebrisSmall;
	explosionId = flashExpSmall;
   maxDamage = 20000.0;
	damageSkinData = "objectDamageSkins";
	sequenceSound[0] = { "Sequence01", SoundActivateMotionSensor };
};

function Keypad1::onCollision(%this, %object) { Keypad::onCollision(%this, %object); }