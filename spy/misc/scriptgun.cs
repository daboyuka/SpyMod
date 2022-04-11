
RepairEffectData ScriptgunTriggerBolt {
   bitmapName       = "bar00.bmp";
   boltLength       = 0;
   segmentDivisions = 1;
   beamWidth        = 0;
   updateTime   = 1000;
   skipPercent  = 0.6;
   displaceBias = 0.15;
};

ItemImageData ScriptgunImage {
	shapeFile = "paintgun";
	mountPoint = 0;

	weaponType = 2;
	reloadTime = 0;
	fireTime = 0;
	projectileType = ScriptgunTriggerBolt;

	accuFire = false;
	sfxActivate = SoundPickUpWeapon;
};

ItemData Scriptgun {
	description = "Scriptgun";
	className = "Tool";
	shapeFile = "paintgun";
	hudIcon = "mortar";
	heading = $InvHeading[Gadget];
	shadowDetailMask = 4;
	imageType = ScriptgunImage;
};

$ItemDescription[Scriptgun] = "<jc><f1>Scriptgun:<f0> function undefined";

// To configure the scriptgun, call Scriptgun::setModes(%desc0, %desc1, ...)
// to set number of modes (non-empty args) and mode descriptions, then redefine:
function sgpress(%player, %mode, %lookvec, %hitobj, %hitpos, %hitnorm) {}
function sgrelease(%player, %mode, %lookvec, %hitobj, %hitpos, %hitnorm) {}

function Scriptgun::setModes(%d0, %d1, %d2, %d3, %d4, %d5, %d6, %d7, %d8, %d9) {
	for (%i = 0; %d[%i] != ""; %i++) $ItemDescription[Scriptgun, %i] = "<jc><f1>Scriptgun:<f0> " @ %d[%i];
	$NumModes[Scriptgun] = %i;
}

function Scriptgun::reset() {
	Scriptgun::setModes();
	function sgpress(%player, %mode, %lookvec, %hitobj, %hitpos, %hitnorm) { echo(%hitobj, " ", %hitpos); }
	function sgrelease(%player, %mode, %lookvec, %hitobj, %hitpos, %hitnorm) {}
}
Scriptgun::reset();

function Scriptgun::cast(%this, %isclick) {
	%mode = Weapon::getMode(%this, Scriptgun);
	%muzzle = GameBase::getMuzzleTransform(%this);
	%lookvec = Matrix::subMatrix(%muzzle, 3, 4, 3, 1, 0, 1);
	if (GameBase::getLOSInfo(%this, 9999)) {
		%hitobj = $los::object;
		%hitpos = $los::position;
		%hitnorm = $los::normal;
	}
	if (%isclick) sgpress(%this, %mode, %lookvec, %hitobj, %hitpos, %hitnorm);
	else          sgrelease(%this, %mode, %lookvec, %hitobj, %hitpos, %hitnorm);
}
function ScriptgunTriggerBolt::onAcquire(%blah, %this, %target) { Scriptgun::cast(%this, true); }
function ScriptgunTriggerBolt::onRelease(%blah, %this, %target) { Scriptgun::cast(%this, false); }
function ScriptgunTriggerBolt::checkDone(%blah, %this) {}

function Scriptgun::onModeChanged(%player) { Weapon::displayDescription(%player, Scriptgun); }
