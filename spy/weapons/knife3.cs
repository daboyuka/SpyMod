SoundData SoundFireKnife {
   wavFileName = "throwitem.wav";
   profile = Profile3dNear;
};

SoundData SoundKnifeExplosion {
   wavFileName = "dryfire1.wav";
   profile     = Profile3dNear;
};

ExplosionData KnifeExplosion {
   shapeName = "laserhit.dts";
   //soundId   = SoundKnifeExplosion;

   faceCamera = true;
   randomSpin = true;

   timeScale = 0.4;

   timeZero = 0.250;
   timeOne  = 0.850;
};


RocketData KnifeSlashEffect {
   bulletShapeName  = "breath.dts";
   explosionTag     = noExp;//SoundKnifeExplosion;
   collisionRadius  = 0.0;
   mass             = 2.0;

   damageClass      = 0;       // 0 impact, 1, radius
   damageValue      = 0;
   damageType       = 0;

   muzzleVelocity   = 25.0/2;
   terminalVelocity = 25.0/2;
   totalTime        = 0.04*2;
   liveTime         = 0.04*2;
   inheritedVelocityScale = 1;

   // rocket specific
   trailType   = 1;
   trailLength = 100;
   trailWidth  = 1;
};



RepairEffectData KnifeTriggerBolt {
   bitmapName       = "bar00.bmp";
   boltLength       = 0;
   segmentDivisions = 1;
   beamWidth        = 0;

   updateTime   = 4000;
   skipPercent  = 0.6;
   displaceBias = 0.15;
};

ItemImageData KnifeStab1Image {
	shapeFile = "force";
	mountPoint = 0;
	mountRotation = "1.57 0 0";
	mountOffset = "-0.1 0 -0.05";
};

ItemData KnifeStab1 {
	shapeFile = "breath";
	imageType = KnifeStab1Image;
};

ItemImageData KnifeStab2Image {
	shapeFile = "force";
	mountPoint = 0;
	mountRotation = "1.57 0 0";
	mountOffset = "-0.1 0.3 -0.05";
};

ItemData KnifeStab2 {
	shapeFile = "breath";
	imageType = KnifeStab2Image;
};

ItemImageData KnifeM1Image {
	shapeFile = "force";
	mountPoint = 0;
	mountRotation = "0 3.14 0";
	mountOffset = "-0.1 -0.15 0.1";

	weaponType = 0;
	reloadTime = 0; 
	fireTime = 0.05;//0.25;//0.45;

	maxEnergy = 0;
	minEnergy = 0;

	accuFire = true;

//	sfxFire = SoundFireKnife;
};

ItemData KnifeM1 {
	shapeFile = "breath";
	imageType = KnifeM1Image;
};

ItemImageData KnifeM2Image {
	shapeFile = "breath";
	mountPoint = 0;

	weaponType = 0;
	reloadTime = 0; 
	fireTime = 0.2;

	maxEnergy = 0;
	minEnergy = 0;

	accuFire = true;

	firstPerson = false;
};

ItemData KnifeM2 {
	shapeFile = "breath";
	imageType = KnifeM2Image;
};

ItemImageData KnifeImage {
	shapeFile = "breath";
	mountPoint = 0;
	mountOffset = "-0.1 0.85 0";

	weaponType = 2;
	reloadTime = 0.25;
	fireTime = 0;
	projectileType = KnifeTriggerBolt;

        minEnergy = 0;
        maxEnergy = 0;

	accuFire = true;

	firstPerson = false;
	isVisible = false;
};

ItemData Knife {
	description = "Knife";
	className = "Weapon";
	shapeFile = "force";
	hudIcon = "mortar";
	heading = $InvHeading[Weapon];
	shadowDetailMask = 4;
	imageType = KnifeImage;
	price = 125;
	showWeaponBar = false;
};

$ItemDescription[Knife, 0] = "<jc><f1>Knife, Slash Mode:<f0> Hacks and slashes your opponent to pieces";
$ItemDescription[Knife, 1] = "<jc><f1>Knife, Stab Mode:<f0> Stab your enemy; the faster you are moving, the more it hurts";
$NumModes[Knife] = 2;

$Knife::slash::range = 1;
$Knife::slash::damage = 60;

//$Knife::slash::positions[0] = "-1.5 0.5";
//$Knife::slash::positions[1] = "-0.5 1";
//$Knife::slash::positions[2] = "0.5 1";
//$Knife::slash::positions[3] = "1.5 0.5";
function KnifeM1Image::onFire(%player, %slot) {
  %trans = GameBase::getMuzzleTransform(%player);
  %transX = Matrix::subMatrix(%trans, 3, 4, 3, 1, 0, 0);
  %transY = Matrix::subMatrix(%trans, 3, 4, 3, 1, 0, 1);
  %transZ = Matrix::subMatrix(%trans, 3, 4, 3, 1, 0, 2);
  %transPos = Matrix::subMatrix(%trans, 3, 4, 3, 1, 0, 3);

  %newSlashPos = Vector::add(%transPos, %transY);
  if (%player.lastSlashPos == "") {

  } else {
    %vec = Vector::sub(%newSlashPos, %player.lastSlashPos);
    Projectile::spawnProjectile(KnifeSlashEffect, "1 0 0 " @ %vec @ " 0 0 1 " @ %player.lastSlashPos, -1, 0);
    if (getLOSInfo(%player.lastSlashPos, %newSlashPos, ~0) && $los::object != %player && getObjectType($los::object) == "Player") {
      GameBase::applyDamage($los::object, $KnifeDamageType, $Knife::slash::damage, $los::position, "0 0 0", "0 0 0", %player);
    }
  }
  %player.lastSlashPos = %newSlashPos;

  //%slashVec = Vector::getFromRot(Vector::randomVec2("0 0 0", "0 0 " @ 2*$PI));
  //%slashVec = getWord(%slashVec, 0) @ " 0 " @ getWord(%slashVec, 1);

  //%slashVec = radnomItems(2, "1 0 0", "-1 0 0");
  //%newSlash = Matrix::subMatrix(Matrix::mul(%trans, 3, 4, %slashVec @ " 0", 4, 1), 4, 1, 3, 1);

  //%slashPos1 = Vector::add(Vector::add(%transPos, Vector::mul(%newSlash, -1.5)), Vector::mul(%transY, 1));
  //%slashPos1 = Vector::add(%transPos, Vector::mul(%transY, 1));
  //%slashPos2 = Vector::add(%transPos, Vector::mul(%transY, 1));

  //Projectile::spawnProjectile(KnifeSlashEffect, "1 0 0 " @ %velPlane @ " 0 0 1 " @ %slashPos1, %player, Item::getVelocity(%player));
  //Projectile::spawnProjectile(KnifeSlashEffect, "1 0 0 " @ %velPlane @ " 0 0 1 " @ %slashPos2, %player, Item::getVelocity(%player));

  GameBase::playSound(%player, SoundFireKnife, 0);
}



function KnifeTriggerBolt::onAcquire(%this, %player, %target) {
  %player.slashTransStart = GameBase::getMuzzleTransform(%player);
  if (Weapon::getMode(%player, Knife) == 1) Player::mountItem(%player, KnifeStab2, 4);
}

function KnifeTriggerBolt::onRelease(%this, %player) {
  if (Player::isDead(%player)) return;

  %startTrans = %player.slashTransStart;
  %endTrans = GameBase::getMuzzleTransform(%player);

  %startTransY = Matrix::subMatrix(%startTrans, 3, 4, 3, 1, 0, 1);
  %endTransY = Matrix::subMatrix(%endTrans, 3, 4, 3, 1, 0, 1);

  %pos = Matrix::subMatrix(%endTrans, 3, 4, 3, 1, 0, 3);
  %zVec = Vector::normalize(Vector::cross(%startTransY, %endTransY));
  %yVec = Vector::normalize(Vector::add(%startTransY, %endTransY));
  %xVec = Vector::cross(%yVec, %zVec);

  //Projectile::spawnProjectile(RedLaser, "1 0 0 " @ %yVec @ " 0 0 1 " @ %pos, -1, 0);

  %slashMat = %xVec @ " " @ %yVec @ " " @ %zVec @ " " @ %pos;

  %set = newObject("", SimSet);
  for (%i = -2; %i <= 2; %i++) {
    %slashVec = Vector::getFromRot("0 0 " @ %i*($PI/4), $Knife::slash::range) @ " 1";
    %slashPoint = Matrix::mul(%slashMat, 3, 4, %slashVec, 4, 1);

    if (%lastSlashPoint != "") {
      %vec = Vector::sub(%slashPoint, %lastSlashPoint);

      Projectile::spawnProjectile(KnifeSlashEffect, "1 0 0 " @ %vec @ " 0 0 1 " @ %lastSlashPoint, -1, 0);

      if (getLOSInfo(%lastSlashPoint, %slashPoint, ~0) && $los::object != %player && getObjectType($los::object) == "Player") {
        addToSet(%set, $los::object);
      }
    }

    %lastSlashPoint = %slashPoint;
  }

  for (%i = 0; %i < Group::objectCount(%set); %i++) {
    %o = Group::getObject(%set, %i);
    GameBase::applyDamage(%o, $KnifeDamageType, $Knife::slash::damage, %pos, "0 0 0", "0 0 0", %player);
  }
  deleteObject(%set);

  GameBase::playSound(%player, SoundFireKnife, 0);

  %player.slashTransStart = "";
  if (Weapon::getMode(%player, Knife) == 1) Player::mountItem(%player, KnifeStab1, 4);
}
function KnifeTriggerBolt::checkDone(%this, %player) {}



function Knife::onMount(%player, %slot) {
  Weapon::displayDescription(%player, Knife);
  %mode = Weapon::getMode(%player, Knife);
  if (%mode == 0) Player::mountItem(%player, KnifeM1, 3);
  if (%mode == 1) {
    Player::mountItem(%player, KnifeM2, 3);
    Player::mountItem(%player, KnifeStab1, 4);
  }
  if (%mode == 2) Player::mountItem(%player, KnifeM3, 3);
  Weapon::standardMount(%player, Knife);
}

function Knife::onUnmount(%player, %slot) {
  Weapon::clearDescription(%player);
  Player::trigger(%player, 0, false);
  Player::trigger(%player, 3, false);
  Player::unmountItem(%player, 3);
  Player::unmountItem(%player, 4);
  Weapon::standardUnmount(%player, Knife);
}



function Knife::onNoAmmo(%player) {}

function Knife::onModeChanged(%player, %mode) {
  Weapon::displayDescription(%player, Knife);
  if (%mode == 0) {
    Player::mountItem(%player, KnifeM1, 3);
    Player::unmountItem(%player, 4);
  }
  if (%mode == 1) {
    Player::mountItem(%player, KnifeM2, 3);
    Player::mountItem(%player, KnifeStab1, 4);
  }
}



registerWeapon(Knife, Knife, 1);
linkWeapon(Knife);
setArmorAllowsItem(larmor, Knife, 1);
setArmorAllowsItem(lfemale, Knife, 1);
setArmorAllowsItem(SpyMale, Knife, 1);
setArmorAllowsItem(SpyFemale, Knife, 1);
setArmorAllowsItem(DMMale, Knife, 1);
setArmorAllowsItem(DMFemale, Knife, 1);