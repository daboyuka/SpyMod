ItemData LoadedAmmo {
	description = "Loaded Ammo";
	className = "Ammo";
	shapeFile = "plasammo";
	shadowDetailMask = 4;
	price = 2;
	showInventory = false;
};

ItemImageData LoadedAmmoDisplayImage {
	shapeFile = "breath";
	mountPoint = 0;

	weaponType = 0;
	reloadTime = 0; 
	fireTime = 0;

	ammoType = LoadedAmmo;

	accuFire = false;

	firstPerson = false;
};

ItemData LoadedAmmoDisplay {
	description = "";
	className = "StuffIsPie";
	shapeFile = "breath";
	hudIcon = "chain";
   //heading = "zAmmo currently loaded";
	shadowDetailMask = 4;
	imageType = LoadedAmmoDisplayImage;
	price = 0;
	showWeaponBar = true;
	showInventory = false;
};

$WeaponAmmo[LoadedAmmoDisplay] = LoadedAmmo;










GrenadeData DumbShell {
   bulletShapeName    = "grenade.dts";
   explosionTag       = energyExp;
   collideWithOwner   = True;
   ownerGraceMS       = 250;
   collisionRadius    = 0.2;
   mass               = 1.0;
   elasticity         = 0.01;

   damageClass        = 0;       // 0 impact, 1, radius
   damageValue        = 0;
   damageType         = $ShrapnelDamageType;

   maxLevelFlightDist = 0;
   totalTime          = 30.0;    // special meaning for grenades...
   liveTime           = 30.0;
   projSpecialTime    = 0.05;

   inheritedVelocityScale = 1;

   smokeName              = "smoke.dts";
};

function fireStuff(%pos, %targPt, %g, %v, %high) {
  %diff = Vector::sub(%targPt, %pos);
  %h = getWord(%diff, 2);
  %x = getWord(%diff, 0);
  %y = getWord(%diff, 1);
  %d = sqrt(%x*%x + %y*%y);
  %a = calcAngle(%d, %h, %g, %v, %high);

  %newV = Vector::resize(Vector::add(Vector::normalize(%x @ " " @ %y @ " 0"), "0 0 " @ (sin(%a)/cos(%a))), %v);
  Projectile::spawnProjectile(DumbShell, "1 0 0 0 1 0 0 0 1 " @ %pos, -1, %newV);
}

function killStuff(%high) {
  if (GameBase::getLOSInfo(Client::getControlObject(2049),500)) {
    fireStuff(GameBase::getPosition(2049), $los::position, -20, 100, %high);
  }
}