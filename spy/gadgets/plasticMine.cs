ItemData PlasticMineAmmo;

ItemImageData PlasticMineAmmoImage {
   shapeFile  = "grenade";
	mountPoint = 0;

	weaponType = 0; // Single Shot
	reloadTime = 1;
	fireTime = 1;

	ammoType = PlasticMineAmmo;

	accuFire = true;

	sfxFire = SoundThrowItem;
	sfxActivate = SoundPickUpWeapon;
};

ItemData PlasticMineAmmo {
   heading = $InvHeading[Gadget];
	description = "Plastique Explosive";
	className = "Tool";
   shapeFile  = "grenade";
	hudIcon = "blaster";
	shadowDetailMask = 4;
	imageType = PlasticMineAmmoImage;
	price = 50;
	showWeaponBar = false;//true;
};

$ItemDescription[PlasticMineAmmo] = "<jc><f1>Plastique Explosives:<f0> Potent explosives that can adhere to walls";

function PlasticMineAmmoImage::onFire(%player,%slot)
{
	if($matchStarted) {
		if(%player.throwTime < getSimTime() ) {
			Player::decItemCount(%player,PlasticMineAmmo);
			%obj = newObject("","Mine","PlasticMine");
		 	addToSet("MissionCleanup", %obj);
			GameBase::throw(%obj,%player,10,false);
			%player.throwTime = getSimTime() + 0.5;

			%client = Player::getClient(%player);
		}
	}
}

function PlasticMineAmmo::onMount(%player, %slot) { Weapon::displayDescription(%player, PlasticMineAmmo); }

registerGadget(PlasticMineAmmo, PlasticMineAmmo, 1);
linkGadget(PlasticMineAmmo);
setArmorAllowsItem(larmor, PlasticMineAmmo, 5);
setArmorAllowsItem(lfemale, PlasticMineAmmo, 5);
setArmorAllowsItem(SpyMale, PlasticMineAmmo, 5);
setArmorAllowsItem(SpyFemale, PlasticMineAmmo, 5);
setArmorAllowsItem(DMMale, PlasticMineAmmo, 5);
setArmorAllowsItem(DMFemale, PlasticMineAmmo, 5);



MineData PlasticMine
{
   mass = 0.3;
   drag = 1.0;
   density = 2.0;
	elasticity = 0;
	friction = 100.0;
	className = "Handgrenade";
   description = "Plastic Explosive";
   shapeFile = "grenade";
   shadowDetailMask = 4;
   explosionId = grenadeExp;
	explosionRadius = 10.0;
	damageValue = 400;
	damageType = $ShrapnelDamageType;
	kickBackStrength = 1000;
	triggerRadius = 0.5;
	maxDamage = 2;
};

function PlasticMine::onAdd(%this)
{
	%data = GameBase::getDataName(%this);
	schedule("PlasticMine::goBoom(" @ %this @ ");",5,%this);
}

function PlasticMine::goBoom(%this) {
  Mine::detonate(%this);
}
