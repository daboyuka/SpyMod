LaserData GeomDraw::RedLaser {
   laserBitmapName   = "repairAdd.bmp";
   hitName           = "breath.dts";

   damageConversion  = 0.0;
   baseDamageType    = 0;

   beamTime = 10000;

   lightRange        = 0;
   lightColor        = { 0,0,0 };

   detachFromShooter = true;
};

LaserData GeomDraw::BlueLaser {
   laserBitmapName   = "flyerflame1.bmp";
   hitName           = "breath.dts";

   damageConversion  = 0.0;
   baseDamageType    = 0;

   beamTime = 10000;

   lightRange        = 0;
   lightColor        = { 0,0,0 };

   detachFromShooter = true;
};

LaserData GeomDraw::GreenLaser {
   laserBitmapName   = "paintPulse.bmp";
   hitName           = "breath.dts";

   damageConversion  = 0.0;
   baseDamageType    = 0;

   beamTime = 10000;

   lightRange        = 0;
   lightColor        = { 0,0,0 };

   detachFromShooter = true;
};

StaticShapeData GeomDraw::LaserBlocker {
	shapeFile = "grenade";
	maxDamage = 10000000;
};

// GeomDraw::blocker stuff: controls a singleton GeomDraw::LaserBlocker for popping up temporarily to block laser shots
$GeomDraw::blocker = "";
function GeomDraw::getBlocker() {
	if (!$GeomDraw::blocker || !getGroup($GeomDraw::blocker)) {
		$GeomDraw::blocker = newObject("", StaticShape, GeomDraw::LaserBlocker, true);
		addToSet(MissionCleanup, $GeomDraw::blocker);
	}
	return $GeomDraw::blocker;
}
function GeomDraw::block(%pos) { GameBase::setPosition(GeomDraw::getBlocker(), %pos); }
function GeomDraw::unblock()   { GeomDraw::block("0 0 -100000"); }

function GeomDraw::line(%from, %to, %kind, %clean) {
	if (%clean == "") %clean = MissionCleanup;
	if (%kind == "") %kind = GeomDraw::RedLaser;
	
	GeomDraw::block(%to);
	
	%look = Matrix::lookAt(%from, %to, "0 0 1");
	%laser = Projectile::spawnProjectile(%kind, %look, -1, 0);
	addToSet(%clean, %laser);
	
	GeomDraw::unblock();
	
	return %laser;
}

// GeomDraw::path "vec1 vec2 ..." ?closed ?kind ?clean
function GeomDraw::path(%path, %closed, %kind, %clean) {
	%first = Matrix::subMatrix(%path, 3, 999, 3, 1, 0, 0);
	
	%prev = %first;
	for (%i = 1; true; %i++) {
		%cur = Matrix::subMatrix(%path, 3, 999, 3, 1, 0, %i);
		if (%cur == "-1 -1 -1") break;
		
		GeomDraw::line(%prev, %cur, %kind, %clean);
		%prev = %cur;
	}
	
	if (%closed) GeomDraw::line(%prev, %first, %kind, %clean);
}