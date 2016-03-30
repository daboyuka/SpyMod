SoundProfileData Profile3dNearQuiet {
   baseVolume = -96;
   minDistance = 5.0;
   maxDistance = 10.0;
   flags = SFX_IS_HARDWARE_3D;
};

SoundData SoundFireStormbow {
   wavFileName = "Grenade.wav";//"ricoche2.wav";
   profile = Profile3dNearQuiet;
};
SoundData SoundLoadStormbow {
   wavFileName = "Pku_weap.wav";
   profile = Profile3dNearQuiet;
};

SoundData SoundLoadStormbow2 {
   wavFileName = "mortar_reload.wav";
   profile = Profile3dNearQuiet;
};

ItemData StormbowAmmo {
	description = "Stormbow Bolts";
	heading = $InvHeading[Ammo];
	className = "Ammo";
	shapeFile = "plasammo";
	shadowDetailMask = 4;
	price = 2;
};

BulletData StormbowBolt {
   bulletShapeName    = "bullet.dts";
   explosionTag       = AGP84BoltExp;

   damageClass        = 0;                 // 0 = impact, 1 = radius
   damageValue        = 30;
   damageType         = $BulletDamageType;

   muzzleVelocity     = 200.0;
   totalTime          = 1.25;
   liveTime           = 1.25;
   inheritedVelocityScale = 0.3;
   isVisible          = False;

   tracerLength = 10;
   tracerPercentage = 1;

   soundId = SoundJetLight;
};

ItemImageData StormbowImage2 {
	shapeFile = "energygun";
	mountPoint = 0;
	mountRotation = "0 1.57 2";
	mountOffset = "-0.3 0.05 0";
	reloadTime = 0.2;
	fireTime = 0;
	weaponType = 0;
};

ItemData Stormbow2 {
	shapeFile = "energygun";
	imageType = StormbowImage2;
};

ItemImageData StormbowImage3 {
	shapeFile = "energygun";
	mountPoint = 0;
	mountRotation = "0 -1.57 -2";
	mountOffset = "0.3 0.05 0";
	reloadTime = 0.2;
	fireTime = 0;
	weaponType = 0;
};

ItemData Stormbow3 {
	shapeFile = "energygun";
	imageType = StormbowImage3;
};

ItemImageData StormbowImage4 {
	shapeFile = "force";
	mountPoint = 0;
	mountRotation = "-1.57 0 0";
	mountOffset = "0 0.05 0";
};

ItemData Stormbow4 {
	shapeFile = "force";
	imageType = StormbowImage4;
};

ItemImageData StormbowImage5 {
	shapeFile = "force";
	mountPoint = 0;
	mountRotation = "-1.57 0 0";
	mountOffset = "0 -0.15 0";
};

ItemData Stormbow5 {
	shapeFile = "force";
	imageType = StormbowImage5;
};

ItemImageData StormbowImage {
	shapeFile = "breath";
	mountPoint = 0;
	mountOffset = "0 0 0.05";

	weaponType = 0;
	reloadTime = 0; 
	fireTime = 0.65;

	ammoType = LoadedAmmo;

	accuFire = false;
	firstPerson = false;

	sfxFire = SoundFireStormbow;
	sfxActivate = SoundPickUpWeapon;
};

ItemData Stormbow {
	description = "Stormbow";
	className = "Weapon";
	shapeFile = "paintgun";
	hudIcon = "mortar";
	heading = $InvHeading[Weapon];
	shadowDetailMask = 4;
	imageType = StormbowImage;
	price = 125;
	showWeaponBar = false;//true;
};



ItemImageData StormbowDisplayImage {
	shapeFile = "breath";
	firstPerson = false;
	ammoType = StormbowAmmo;
};

ItemData StormbowDisplay {
	description = "";
	className = "Tool";
	shapeFile = "breath";
	hudIcon = "ammopack";
	shadowDetailMask = 4;
	imageType = StormbowDisplayImage;
	showWeaponBar = true;
	showInventory = false;
};



$ClipSize[Stormbow] = 4;
$AmmoDisplay[Stormbow] = StormbowDisplay;
$ItemDescription[Stormbow] = "<jc><f1>Stormbow:<f0> A powerful, yet almost completely silent crossbow.";

function StormbowImage::onFire(%player, %slot) {
  if (Player::getItemCount(%player, LoadedAmmo) == 0) return;

  Weapon::fireProjectile(%player, StormbowBolt);
  Player::decItemCount(%player, LoadedAmmo);

  GameBase::playSound(%player, SoundFireStormbow2, 0);

  Player::trigger(%player, 3, true);
  Player::trigger(%player, 4, true);
  schedule("Player::trigger("@%player@",3,false);", 0.05);
  schedule("Player::trigger("@%player@",4,false);", 0.05);

  if (Player::getItemCount(%player, LoadedAmmo) == 0 && Player::getMountedItem(%player, 5) == "Stormbow4") {
    Stormbow::unloadBolt(%player);
  } else {
    Stormbow::unloadBolt(%player);
    schedule("Stormbow::loadBolt("@%player@");", 0.3);    
  }
}

function Stormbow::loadBolt(%player) {
  Player::mountItem(%player, Stormbow4, 5);
  Player::mountItem(%player, Stormbow5, 6);
  GameBase::playSound(%player, SoundLoadStormbow2, 1);
}
function Stormbow::unloadBolt(%player) {
  if (Player::getMountedItem(%player, 5) == "Stormbow4") {
    Player::unmountItem(%player, 5);
    Player::unmountItem(%player, 6);
  }
}

function Stormbow::onMount(%player, %slot) {
  Weapon::displayDescription(%player, Stormbow);
  Weapon::standardMount(%player, Stormbow);

  Player::mountItem(%player, Stormbow2, 3);
  Player::mountItem(%player, Stormbow3, 4);

  if (Player::getItemCount(%player, LoadedAmmo) > 0) {
    Stormbow::loadBolt(%player);
  }
}
function Stormbow::onUnmount(%player, %slot) {
  Weapon::standardUnmount(%player, Stormbow);
  Player::unmountItem(%player, 3);
  Player::unmountItem(%player, 4);
  Stormbow::unloadBolt(%player);
}

function Stormbow::onNoAmmo(%player) {
  Stormbow::reload(%player);
}

function Stormbow::reload(%player) {
  Weapon::standardReload2(%player, Stormbow,
                          "GameBase::playSound("@%player@",SoundLoadStormbow,0);Stormbow::loadBolt("@%player@");",
                          2);
}

registerWeapon(Stormbow, StormbowAmmo, 4);
linkWeapon(Stormbow);
setArmorAllowsItem(larmor, Stormbow, 16);
setArmorAllowsItem(lfemale, Stormbow, 16);
setArmorAllowsItem(SpyMale, Stormbow, 16);
setArmorAllowsItem(SpyFemale, Stormbow, 16);
setArmorAllowsItem(DMMale, Stormbow, 16);
setArmorAllowsItem(DMFemale, Stormbow, 16);
