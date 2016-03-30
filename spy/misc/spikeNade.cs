ItemData SpikeGrenadeItem {
   heading = $InvHeading[Ammo];
	description = "Spike Grenade";
	className = "Ammo";
   shapeFile  = "grenade";
	hudIcon = "blaster";
	shadowDetailMask = 4;
	price = 85;
	showWeaponBar = false;
};

function SpikeGrenadeItem::onUse(%player,%item) {
	if($matchStarted) {
		if(%player.throwTime < getSimTime()) {
			%client = Player::getClient(%player);
			%team = Client::getTeam(%client);

			Player::decItemCount(%player,%item);
			%obj = newObject("", Mine, SpikeGrenade);
			%obj.thrower = %player;
			%obj.throwerTeam = GameBase::getTeam(%player);
 	 	 	addToSet("MissionCleanup", %obj);
			GameBase::throw(%obj,%player, 9 * %client.throwStrength, false);
			%player.throwTime = getSimTime() + 0.5;
		}
	}
}

$ItemDescription[SpikeGrenadeItem] = "<jc><f1>Spike Grenade:<f0> Explodes in a shower of deadly spikes";

registerGrenade(SpikeGrenadeItem, 1);
linkGrenade(SpikeGrenadeItem);
setArmorAllowsItem(larmor, SpikeGrenadeItem, 1);
setArmorAllowsItem(lfemale, SpikeGrenadeItem, 1);
//setArmorAllowsItem(SpyMale, SpikeGrenadeItem, SpikeGrenadeItem, 0);
//setArmorAllowsItem(SpyFemale, SpikeGrenadeItem, SpikeGrenadeItem, 0);
setArmorAllowsItem(DMMale, SpikeGrenadeItem, 1);
setArmorAllowsItem(DMFemale, SpikeGrenadeItem, 1);

MineData SpikeGrenade {
   mass = 0.3;
   drag = 1.0;
   density = 2.0;
	elasticity = 0.3;//0.15;
	friction = 1.0;
	className = "Grenade";
   description = "Spike Grenade";
   shapeFile = "grenade";
   shadowDetailMask = 4;
	explosionId = flashExpSmall;
	explosionRadius = 0;
	damageValue = 0;
	damageType = 0;
	kickBackStrength = 0;
	triggerRadius = 0.5;
	maxDamage = 200;
	//disableCollision = true;
};

BulletData Spike {
   bulletShapeName    = "bullet.dts";
   explosionTag       = bulletExp0;
   expRandCycle       = 3;
   mass               = 0.05;
   bulletHoleIndex    = 0;

   damageClass        = 0;       // 0 impact, 1, radius
   damageValue        = 5;
   damageType         = $BulletDamageType;

   aimDeflection      = 0.005;
   muzzleVelocity     = 50.0;
   totalTime          = 1.5;
   liveTime = 1.5;
   inheritedVelocityScale = 0;
   isVisible          = True;

//   tracerPercentage   = 1.0;
//   tracerLength       = 30;
};

RocketData Spike2 {
   bulletShapeName = "bullet.dts";
   explosionTag    = bulletExp0;

   mass            = 2.0;

   collideWithOwner = 1;
   ownerGraceMS = -1;
   collisionRadius = 0;

   damageClass      = 1;       // 0 impact, 1, radius
   damageValue      = 5;
   damageType       = $SpikeDamageType;

   explosionRadius = 3;

   muzzleVelocity   = 100.0;
   terminalVelocity = 100.0;
   acceleration     = 0.0;

   totalTime        = 3;
   liveTime         = 3;

   inheritedVelocityScale = 0;

   // rocket specific
//   trailType   = 1;
//   trailLength = 15;
//   trailWidth  = 0.3;

   soundId = SoundJetLight;
};

function SpikeGrenade::onAdd(%this) {
  schedule("SpikeGrenade::doSpike("@%this@");",2.0,%this);
  addToSet(MissionCleanup, %this);
}

function SpikeGrenade::onDamage(%this,%type,%value,%pos,%vec,%mom,%object) {}

function SpikeGrenade::doSpike(%this) {
  for (%i = 0; %i < 3; %i++) {
    schedule("SpikeGrenade::spike("@%this@");", %i*0.1);
  }
  schedule("Mine::detonate(" @ %this @ ");", 0.3);
}

function SpikeGrenade::spike(%this) {
  %pos = Vector::add(GameBase::getPosition(%this), "0 0 0.5");
  for (%i = 0; %i < 50; %i++) {
    %vec = Vector::randomRotVec(0,$PI/4,0,0,0,2*$PI, 1, 1);
    %proj = Projectile::spawnProjectile(Spike2, "1 0 0 " @ %vec @ "  0 0 1 " @ %pos, %this, 0);
  }
}