//$CorpseTimeoutValue = 25;

function Player::onAdd(%this) {
  GameBase::setRechargeRate(%this,0);
}

function Player::onKilled(%this) {
      if (%this.usingBinocs) Binoculars::deactivate(%this);
      if (%this.grappling) schedule("Grappler::ungrapple("@%this@");", 5);
	  if (%this.grappwning) schedule("Grappwner::ungrappwn("@%this@");", 5);

	if (Player::isAIControlled(%this) && %this.aiName != "") AI::onAIKilled(%this.aiName, %this);

	%cl = GameBase::getOwnerClient(%this);
	%cl.dead = 1;
	if($AutoRespawn > 0)
		schedule("Game::autoRespawn(" @ %cl @ ");",$AutoRespawn,%cl);
	if(%this.outArea==1)	
		leaveMissionAreaDamage(%cl);
	Player::setDamageFlash(%this,0.75);
	for (%i = 0; %i < 8; %i++) {
		%type = Player::getMountedItem(%this,%i);
		if (%type != -1) {
			Player::trigger(%this, %i, false);
			if (Weapon::usesClips(%type)) {
                          Weapon::returnClipAmmo(%this, %type);
                        }
			if (%i == $FlagSlot) Player::dropItem(%this, %type);
		}
	}

   if(%cl != -1)
   {
		if(%this.vehicle != "")	{
			if(%this.driver != "") {
				%this.driver = "";
        	 	Client::setControlObject(Player::getClient(%this), %this);
        	 	Player::setMountObject(%this, -1, 0);
			}
			else {
				%this.vehicle.Seat[%this.vehicleSlot-2] = "";
				%this.vehicleSlot = "";
			}
			%this.vehicle = "";		
		}

      addToSet(MissionCleanup, %this);
      schedule("GameBase::startFadeOut(" @ %this @ ");", $CorpseTimeoutValue, %this);
      schedule("deleteObject(" @ %this @ ");", $CorpseTimeoutValue + 2.5, %this);
      if (!$Mission::whenYouDieYoureDead) {
        Client::setOwnedObject(%cl, -1);
        Client::setControlObject(%cl, Client::getObserverCamera(%cl));
        Observer::setOrbitObject(%cl, %this, 5, 5, 5);
        %cl.observerMode = "dead";
      }
      %cl.dieTime = getSimTime();
   }
}

// %this = player object id, %object = player object id
function Player::onDamage(%this,%type,%value,%pos,%vec,%mom,%vertPos,%quadrant,%object) {
  %curTime = getSimTime();

  if (%type == $DetectDamageType) {
    Mission::onPlayerDetected(%this, %value, %object);
    return;
  }
  if (%type == $SuperAI::CheckDamageType) {
    if (%this == %object) return;
    if (GameBase::getTeam(%this) == GameBase::getTeam(%object)) return;

    %aiName = %object.aiName;
    SuperAI::callCustom(%aiName, "checkTarget", 1, %this);
    return;
  }

  if (GameBase::getOwnerClient(%this).invinc || %this.invinc) {
    %thisPos = getBoxCenter(%this);
    %offsetZ =((getWord(%pos,2))-(getWord(%thisPos,2)));
    GameBase::activateShield(%this,%vec,%offsetZ);
    return;
  }

  if (Player::isAIControlled(%this) && %this.aiName != "") {
    %aiName = %this.aiName;
    SuperAI::callCustom(%aiName, "onDamage", 5, %object, %type, %value, %pos, %vec);
  }
  if (%this.dummyMount != "") {
    GameBase::virtual(%this.dummyMount,"onDamage",%type,%value,%pos,%vec,%mom,%object);
    return;
  }
  if (%this.damageVehicle && %this.damageVehicle != "" && isObject(%this.damageVehicle)) {
    if (%type != $ExplosionDamageType && %type != $ShrapnelDamageType && %type != $PlasmaDamageType && %type != $MissileDamageType)
      GameBase::virtual(%this.damageVehicle,"onDamage",%type,%value,%pos,%vec,%mom,%object);
    return;
  }

  if (%type == $ImpactDamageType && %this.lastMount == %object && %this.lastVehicleUnmountTime + 1 > %curTime) {
    return; // Dude was jumping between seats and/or out of the vehicle; don't hurt him!
  }

  if (%type == $SmokeBlindDamageType) {
    %flash = Player::getDamageFlash(%this);
    %flash += %value * 100;
    Player::setDamageFlash(%this, %flash);
    return;
  }

  if (%type == $KnifeStabDamageType) {
    // Testing straight-out dmg*=vel approach. Used to be that the vertical (z) component would hurt alot more, to make
    // falling damage hurt alot more. But the reason it would hurt more for a fall stab is because you are moving fast,
    // which is already taken into account, so this is redundant and overpowering.
    //%vec2 = getWord(%vec, 0) @ " " @ getWord(%vec, 1) @ " 0";
    //%down = abs(getWord(%vec, 2));
    %value *= max(Vector::length(%vec) - KnifeStab.muzzleVelocity, 0);// + %down * 2.5;
  }

  if (%type == $LandingDamageType) {
    %armor = Player::getArmor(%this);
    if ($RealMinDamageSpeed[%armor] != "") {
      %speed = %value / %armor.damageScale + %armor.minDamageSpeed;
      %value = (%speed - $RealMinDamageSpeed[%armor]) * %armor.damageScale;
      %value = max(0, %value);
    }

    if (%this.grapplerShotOff && Player::getClient(%this) != %this.grapplerDislodger) {
      %object = %this.grapplerDislodger;
      %type = $GrappleFallDamageType;
    }
    if (%this.grapplerShotOff && Player::getClient(%this) != %this.grapplerDislodger) {
      %object = %this.grapplerDislodger;
      %type = $GrappleFallDamageType;
    }
    if (%this.grappwnerShotOff && Player::getClient(%this) != %this.grappwnerDislodger) {
      %object = %this.grappwnerDislodger;
      %type = $GrappleFallDamageType;
    }
  }

  // value = (speed - min) * scale
  // value / scale + min = speed

  if ($Mission::damageMultiplier != "" && $Mission::damageMultiplier != 0)
    %value *= $Mission::damageMultiplier;

  %value *= DamageScale::getDamageScale(%this, %type);

	if (Player::isExposed(%this)) {
      %damagedClient = Player::getClient(%this);
      %shooterClient = %object;

		Player::applyImpulse(%this,%mom);
		
		if($teamplay && %damagedClient != %shooterClient && Client::getTeam(%damagedClient) == Client::getTeam(%shooterClient)) {
			if (%shooterClient != -1) {
				if (%mom != "0 0 0") { %this.lastTeamKickbackTime = %curTime; %this.lastTeamKickback = %shooterClient; }
				if ((%curTime - %this.DamageTime > 3.5 || %this.LastHarm != %shooterClient) && %damagedClient != %shooterClient && $Server::TeamDamageScale > 0) {
					if(%type != $MineDamageType) {
						Client::sendMessage(%shooterClient,0,"You just harmed Teammate " @ Client::getName(%damagedClient) @ "!");
						Client::sendMessage(%damagedClient,0,"You took Friendly Fire from " @ Client::getName(%shooterClient) @ "!");
					}
					else {
						Client::sendMessage(%shooterClient,0,"You just harmed Teammate " @ Client::getName(%damagedClient) @ " with your mine!");
						Client::sendMessage(%damagedClient,0,"You just stepped on Teamate " @ Client::getName(%shooterClient) @ "'s mine!");
					}
					%this.LastHarm = %shooterClient;
					%this.DamageStamp = %curTime;
				} else if ((%curTime - %this.DamageTime > 3.5 || %this.LastHarm != %shooterClient) && %damagedClient != %shooterClient && Vector::lengthSquared(%mom) > 100) {
					Client::sendMessage(%shooterClient,0,"You just blasted away Teammate " @ Client::getName(%damagedClient) @ "!");
					Client::sendMessage(%damagedClient,0,"You just got blasted away by Teammate " @ Client::getName(%shooterClient) @ "!");
				} else if ((%curTime - %this.DamageTime > 3.5 || %this.LastHarm != %shooterClient) && %damagedClient != %shooterClient) {
					Client::sendMessage(%shooterClient,0,"Don't shoot Teammate " @ Client::getName(%damagedClient) @ "!");
                	        }
				%friendFire = $Server::TeamDamageScale;
			}
			if (!%shooterClient.ffWarning && %shooterClient.isN00b) {
				%shooterClient.ffWarning = true;
				centerprint(%shooterClient, "<JC><F0>Hey, don't shoot at your teammates!\n\n<F1>Your teammates have the same skin as you do. Go into third person view (R by default) to check your own skin.\n\n<F2>(this message will disappear in 15 seconds)", 15, true);
			}
		}
		else if(%type == $ImpactDamageType && Client::getTeam(%object.clLastMount) == Client::getTeam(%damagedClient)) 
			%friendFire = $Server::TeamDamageScale;
		else  
			%friendFire = 1.0;	

		if (!Player::isDead(%this)) {
			%armor = Player::getArmor(%this);

				if (%curTime <= %this.halfDamageTill) {
                                  %value /= 2;
				  %thisPos = getBoxCenter(%this);
				  %offsetZ =((getWord(%pos,2))-(getWord(%thisPos,2)));
				  GameBase::activateShield(%this,%vec,%offsetZ);
                                }

			// --- Damage modifiers ---
			if (%vertPos == "head" && %type != $ExplosionDamageType) %value *= 2;
			if (%vertPos == "legs" && %type != $ExplosionDamageType) %value *= 0.7;
			if (String::getSubStr(%quadrant,0,4) == "back" && (%type == $KnifeDamageType || %type == $KnifeStabDamageType)) %value *= 2;
			// --- End damage modifiers ---

  			if (%value) {
				%value = %value * %friendFire;//$DamageScale[%armor, %type] * %value * %friendFire;

			// Took out shield damage stuff here

			%energy = GameBase::getEnergy(%this) / 100;
			if (%energy > 0.05 && %type != $LandingDamageType && %type != $GrappleFallDamageType && %type != $CrushDamageType && %type != $PoisonGasDamageType && %type != $OutOfMissionAreaDamageType) {
				if (%energy > %value + 0.05) {
					%flash = min(Player::getDamageFlash(%this) + %value / 50, 0.75);
					if (%type == $KnifeDamageType || %type = $KnifeStabDamageType) {
						%flash *= min(5*%flash,1);
					}
					Player::setDamageFlash(%this, %flash);
					GameBase::setEnergy(%this, (%energy - %value) * 100);
					%value = 0;
				} else {
					%flash = min(Player::getDamageFlash(%this) + %energy / 50, 0.75);
					Player::setDamageFlash(%this, %flash);
					%value = %value - %energy;
					GameBase::setEnergy(%this, 5);
				}
			}

            %dlevel = GameBase::getDamageLevel(%this) + %value;
            %spillOver = %dlevel - %armor.maxDamage;
				GameBase::setDamageLevel(%this,%dlevel);
				%flash = Player::getDamageFlash(%this) + %value / 50;
				if (%type == $PoisonGasDamageType) %flash *= 2;
				if (%flash > 0.75) 
					%flash = 0.75;
				Player::setDamageFlash(%this,%flash);
				//If player not dead then play a random hurt sound
				if(!Player::isDead(%this)) { 
					if(%damagedClient.lastDamage < getSimTime()) {
						%sound = radnomItems(3,injure1,injure2,injure3);
						playVoice(%damagedClient,%sound);
						%damagedClient.lastdamage = getSimTime() + 1.5;
					}
				}
				else {
               if(%spillOver > 50 && (%type== $ExplosionDamageType || %type == $ShrapnelDamageType || %type== $MortarDamageType|| %type == $MissileDamageType)) {
		 				Player::trigger(%this, $WeaponSlot, false);
						%weaponType = Player::getMountedItem(%this,$WeaponSlot);
						if(%weaponType != -1)
							Player::dropItem(%this,%weaponType);
                	Player::blowUp(%this);
					}
					else
					{
						if ((%value > 0.40 && (%type== $ExplosionDamageType || %type == $ShrapnelDamageType || %type== $MortarDamageType || %type == $MissileDamageType )) || (Player::getLastContactCount(%this) > 6) ) {
					  		if(%quadrant == "front_left" || %quadrant == "front_right") 
								%curDie = $PlayerAnim::DieBlownBack;
							else
								%curDie = $PlayerAnim::DieForward;
						}
						else if( Player::isCrouching(%this) ) 
							%curDie = $PlayerAnim::Crouching;							
						else if(%vertPos=="head") {
							if(%quadrant == "front_left" ||	%quadrant == "front_right"	) 
								%curDie = radnomItems(2, $PlayerAnim::DieHead, $PlayerAnim::DieBack);
						  	else 
								%curDie = radnomItems(2, $PlayerAnim::DieHead, $PlayerAnim::DieForward);
						}
						else if (%vertPos == "torso") {
							if(%quadrant == "front_left" ) 
								%curDie = radnomItems(3, $PlayerAnim::DieLeftSide, $PlayerAnim::DieChest, $PlayerAnim::DieForwardKneel);
							else if(%quadrant == "front_right") 
								%curDie = radnomItems(3, $PlayerAnim::DieChest, $PlayerAnim::DieRightSide, $PlayerAnim::DieSpin);
							else if(%quadrant == "back_left" ) 
								%curDie = radnomItems(4, $PlayerAnim::DieLeftSide, $PlayerAnim::DieGrabBack, $PlayerAnim::DieForward, $PlayerAnim::DieForwardKneel);
							else if(%quadrant == "back_right") 
								%curDie = radnomItems(4, $PlayerAnim::DieGrabBack, $PlayerAnim::DieRightSide, $PlayerAnim::DieForward, $PlayerAnim::DieForwardKneel);
						}
						else if (%vertPos == "legs") {
							if(%quadrant == "front_left" ||	%quadrant == "back_left") 
								%curDie = $PlayerAnim::DieLegLeft;
							if(%quadrant == "front_right" ||	%quadrant == "back_right") 
								%curDie = $PlayerAnim::DieLegRight;
						}
						Player::setAnimation(%this, %curDie);
					}
					if(%type == $ImpactDamageType && %object.clLastMount != "")  
						%shooterClient = %object.clLastMount;
					Client::onKilled(%damagedClient,%shooterClient, %type);
				}
			}
		}
	}
}

function Player::onCollision(%this,%object) {
	if (Player::isDead(%this)) {
		if (getObjectType(%object) == "Player") {
			// Transfer all our items to the player
			%sound = false;
			%max = getNumItems();
			for (%i = 0; %i < %max; %i = %i + 1) {
				%count = Player::getItemCount(%this,%i);
				if (%count) {
					%item = getItemData(%i);
					%delta = Item::giveItem(%object,%item,%count);
					if (%delta > 0) {
						Player::decItemCount(%this,%i,%delta);
						%sound = true;
					}
				}
			}
			if (%sound) {
				// Play pickup if we gave him anything
				playSound(SoundPickupItem,GameBase::getPosition(%this));
			}
		}
	}
}

function Player::onNoAmmo(%player) {
  if (Player::getMountedItem(%player,0) == -1 || Player::isDead(%player)) return;
  Player::trigger(%player, 0, false);
  eval(Player::getMountedItem(%player, 0) @ "::onNoAmmo("@%player@");");
}

function remoteKill(%client) {
   if(!$matchStarted)
      return;

   %player = Client::getOwnedObject(%client);
   if (%player.dummyMount != "") {
     %spybot = %player.dummyMount;
     %spybot.controller = -1;

     %tempPlayer = %player.dummyMount.controllerObj;
     %player.dummyMount = "";
     %player = %tempPlayer;
     Client::setOwnedObject(%client, %player);
   }
   if(%player != -1 && getObjectType(%player) == "Player" && !Player::isDead(%player))
   {
		playNextAnim(%client);
	   Player::kill(%client);
	   Client::onKilled(%client,%client);
   }
}

function Mission::onPlayerDetected(%this, %value, %object) {
  return;
}


function Client::isObjectControllable(%clientId, %objectId, %contact) {
  // remote control
  if (%objectId == -1)
    return false;

  

  %pl = Client::getOwnedObject(%clientId);
  // If mounted to a vehicle then can't mount any other objects
  if (%pl.driver != "" || %pl.vehicleSlot != "")
    return false;
  

  if (GameBase::getTeam(%objectId) != Client::getApparentTeam(%clientId) && GameBase::getTeam(%objectId) != -1)
    return false;
  

  if (GameBase::getControlClient(%objectId) != -1) {
    return false;
  }
  

  %name = GameBase::getDataName(%objectId);
  if (%name != CameraTurret && %name != DeployableTurret) {
    if (!GameBase::isPowered(%objectId)) {
      return false;
    }
  }
  

  if (%name == CameraTurret && (getNumTeams() == 1 && %objectId.deployerClient != %clientId)) {
    Client::sendMessage(%clientId, 0, "Cannot connect to camera data channel; camera is owned by someone else");
    return false;
  }

  
  if (%name.className != Turret && %name.className != MountableTurret)
    return false;

  
  if (!(Client::getOwnedObject(%clientId)).CommandTag && GameBase::getDataName(%objectId) != CameraTurret && !$TestCheats && !%contact) {
    Client::SendMessage(%clientId,0,"Must be at a Command Station to control turrets");
    return false;
  }
  
  if (GameBase::getDamageState(%objectId) != "Enabled") return false;

  
  return true;
}

function Client::takeControl(%clientId, %objectId, %contact) {
  if (Client::isObjectControllable(%clientId, %objectId, %contact)) {
    Client::setControlObject(%clientId, %objectId);
    Client::setGuiMode(%clientId, $GuiModePlay);
    return true;
  }
  return false;
}

function Player::deactivateVelocityModdingItems(%player) {
  if (%player.grappling) Grappler::ungrapple(%player);
  if (%player.grappwning) Grappwner::ungrappwn(%player);
  if (%player.parachuting) ParachutePack::unparachute(%player);
  if (%player.wallClimbing) WallClimber::unWallClimb(%player);
}