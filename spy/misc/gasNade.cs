ItemData GasGrenadeItem {
   heading = $InvHeading[Ammo];
	description = "Poison Gas Grenade";
	className = "Ammo";
   shapeFile  = "grenade";
	hudIcon = "blaster";
	shadowDetailMask = 4;
	price = 85;
	showWeaponBar = false;
};

function GasGrenadeItem::onUse(%player,%item) {
	if($matchStarted) {
		if(%player.throwTime < getSimTime()) {
			Player::decItemCount(%player,%item);
			%obj = newObject("", Mine, GasGrenade);
 	 	 	addToSet("MissionCleanup", %obj);
			%client = Player::getClient(%player);
			GameBase::throw(%obj,%player, 9 * %client.throwStrength, false);
			%player.throwTime = getSimTime() + 0.5;
			%obj.throwerClient = %client;
		}
	}
}

$ItemDescription[GasGrenadeItem] = "<jc><f1>Poison Gas Grenade:<f0> Expells clouds of poisonous gas";

registerGrenade(GasGrenadeItem, 1);
linkGrenade(GasGrenadeItem);
setArmorAllowsItem(larmor, GasGrenadeItem, 1);
setArmorAllowsItem(lfemale, GasGrenadeItem, 1);
//setArmorAllowsItem(SpyMale, GasGrenadeItem, 0);
//setArmorAllowsItem(SpyFemale, GasGrenadeItem, 0);
setArmorAllowsItem(DMMale, GasGrenadeItem, 1);
setArmorAllowsItem(DMFemale, GasGrenadeItem, 1);


MineData GasGrenade {
   mass = 0.3;
   drag = 1.0;
   density = 2.0;
	elasticity = 0.3;//0.15;
	friction = 0.8;
	className = "Grenade";
   description = "Smoke Grenade";
   shapeFile = "grenade";
   shadowDetailMask = 4;
	explosionRadius = 0;
	damageValue = 0;
	damageType = 0;
	kickBackStrength = 0;
	triggerRadius = 0.5;
	maxDamage = 200;
};

BulletData PoisonGas {
   bulletShapeName    = "tumult_large.dts";
   explosionTag       = noExp;

   damageClass        = 0;
   damageValue        = 0;
   damageType         = 0;

   muzzleVelocity     = 0.5;
   totalTime          = 7.0;
   liveTime           = 7.0;
   inheritedVelocityScale = 0;
   isVisible          = True;
   aimDeflection = $PI*2/5;

   soundId = SoundJetLight;
};

function GasGrenade::onAdd(%this) {
  %data = GameBase::getDataName(%this);
  schedule("GasGrenade::doGas(" @ %this @ ");",4.0,%this);
  addToSet(MissionCleanup, %this);
}

function GasGrenade::onDamage(%this,%type,%value,%pos,%vec,%mom,%object) {}

function GasGrenade::doGas(%this) {
  for (%i = 0; %i < 20; %i++) {
    schedule("GasGrenade::gas("@%this@");", %i, %this);
    schedule("GasGrenade::poison("@%this@");", %i, %this);
    schedule("GasGrenade::poison("@%this@");", %i + 0.2, %this);
    schedule("GasGrenade::poison("@%this@");", %i + 0.4, %this);
    schedule("GasGrenade::poison("@%this@");", %i + 0.6, %this);
    schedule("GasGrenade::poison("@%this@");", %i + 0.8, %this);
  }
  schedule("deleteObject("@%this@");", 20.5, %this);
}

function GasGrenade::gas(%this) {
  %pos = Vector::add(GameBase::getPosition(%this), "0 0 0.3");
  %pos = Vector::add(%pos, Vector::randomVec(-2,2,-2,2,-2,2));
  %proj = Projectile::spawnProjectile(PoisonGas, "1 0 0 0 0 1 0 -1 0 " @ %pos, -1, 0);//%this, 0);
  addToSet(MissionCleanup, %proj);
}

function GasGrenade::poison(%this) {
  %pos = GameBase::getPosition(%this);
  GameBase::applyRadiusDamage($PoisonGasDamageType, Vector::add(%pos, Vector::randomVec(-0.25,0.25,-0.25,0.25,0.5,0.75)),
                              8, 1.5, 0, %this.throwerClient);
}