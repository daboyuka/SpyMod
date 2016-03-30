function MountableTurret::onAdd(%this) {
	if (GameBase::getMapName(%this) == "") {
		GameBase::setMapName (%this, "Mountable Turret");
	}
}

function MountableTurret::onActivate(%this) {
	GameBase::playSequence(%this,0,power);
}

function MountableTurret::onDeactivate(%this) {
	GameBase::stopSequence(%this,0);
	MountableTurret::checkOperator(%this);
}

function MountableTurret::onSetTeam(%this,%oldTeam) {
	if(GameBase::getTeam(%this) != Client::getApparentTeam(GameBase::getControlClient(%this))) 
		MountableTurret::checkOperator(%this);
}

function MountableTurret::checkOperator(%this) {
   %cl = GameBase::getControlClient(%this);
   if(%cl != -1) {
   	%pl = Client::getOwnedObject(%cl);
		Player::setMountObject(%pl, -1,0);
	   Client::setControlObject(%cl, %pl);
     if (%pl.damageVehicle == %this) %pl.damageVehicle = "";
   }
   //Client::setGuiMode(%cl, 2);
}

function MountableTurret::onPower(%this,%power,%generator) {
	if (%power) {
		//%this.shieldStrength = 0.03;
		//GameBase::setRechargeRate(%this,10);
	} else {
		//%this.shieldStrength = 0;
		//GameBase::setRechargeRate(%this,0);
		MountableTurret::checkOperator(%this);
	}
	GameBase::setActive(%this,%power);
}

function MountableTurret::onEnabled(%this) {
	if (GameBase::isPowered(%this)) {
		//%this.shieldStrength = 0.03;
		//GameBase::setRechargeRate(%this,10);
		GameBase::setActive(%this,true);
	}
}

function MountableTurret::onDisabled(%this) {
	//%this.shieldStrength = 0;
	//GameBase::setRechargeRate(%this,0);
	MountableTurret::onDeactivate(%this);
}

function MountableTurret::onDestroyed(%this) {
	StaticShape::objectiveDestroyed(%this);
	//%this.shieldStrength = 0;
	//GameBase::setRechargeRate(%this,0);
	MountableTurret::onDeactivate(%this);
	MountableTurret::objectiveDestroyed(%this);
	calcRadiusDamage(%this, $DebrisDamageType, 2.5, 5, 25, 9, 3, 0.40, 
		0.1, 200, 100); 
}

function MountableTurret::onDamage(%this,%type,%value,%pos,%vec,%mom,%object) {
   if(%this.objectiveLine)
		%this.lastDamageTeam = Client::getApparentTeam(Player::getClient(%object));
	%TDS= 1;
	if(GameBase::getTeam(%this) == Client::getApparentTeam(Player::getClient(%object))) {
		%name = GameBase::getDataName(%this);
		if(%name != DeployableTurret && %name != CameraTurret )	
			%TDS = $Server::TeamDamageScale;
	}
	StaticShape::shieldDamage(%this,%type,%value * %TDS,%pos,%vec,%mom,%object);
}

function MountableTurret::onControl (%this, %object) {
	%client = Player::getClient(%object);
	Client::sendMessage(%client,0,"Controlling " @ GameBase::getMapName(%this));
}

function MountableTurret::onDismount (%this, %object) {
	%client = Player::getClient(%object);
	Client::sendMessage(%client,0,"Leaving " @ GameBase::getMapName(%this));
}

function MountableTurret::onCollision (%this, %object) {
  if (getObjectType(%object) == "Player" && GameBase::isActive(%this) && (%object.lastControlTurret != %this || %object.lastControlTurretTime + 4 < getSimTime())) {
    if (%object.grappling) Grappler::ungrapple(%object, false);
    if (%object.parachuting) Parachute::unparachute(%object);

    if (Client::takeControl(Player::getClient(%object), %this, true)) {
      //Player::setMountObject(%object, %this, 0);
      //%object.damageVehicle = %this;
      %object.lastControlTurret = %this;
      %object.lastControlTurretTime = getSimTime();
    }
  }
}

function MountableTurret::jump(%this) {
  MountableTurret::checkOperator(%this);
}