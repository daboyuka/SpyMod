//$DamageScales[type, damagetype] = scale

// The type for the damage scale is searched in this order:
//   1. Data Name (Juggernaught, DMMale)
//   2. Class Name (Generator, Armor)
//   3. Type Name (Vehicle, Player)
//   4. "" (these are the global damage scales)
//
// If a damage scale cannot be found in any of the levels, it is assumed to be 1.0 (no special modification to damage)

function DamageScale::getDamageScale(%obj, %dtype) {
  %name = GameBase::getDataName(%obj);
  %class = %name.className;
  %type = getObjectType(%obj);

  if ($DamageScales[%name, %dtype] != "") return $DamageScales[%name, %dtype];

  if ($DamageScales[%class, %dtype] != "") return $DamageScales[%class, %dtype];

  if ($DamageScales[%type, %dtype] != "") return $DamageScales[%type, %dtype];

  if ($DamageScales["", %dtype] != "") return $DamageScales["", %dtype];

  return 1.0;
}

// Global damage scales
$DamageScales["", $SuperAI::CheckDamageType] = 0;
$DamageScales["", $PoisonGasDamageType] = 0;
$DamageScales["", $SmokeBlindDamageType] = 0;
$DamageScales["", $DetectDamageType] = 0;

$DamageScales["", $KnifeDamageType] = 0.1;
$DamageScales["", $KnifeStabDamageType] = 0;

// Type level damage scales
$DamageScales[Mine, $MineDamageType] = 0.25;

// Class level damage scales
$DamageScales[Door, $ImpactDamageType] = 0;
$DamageScales[Door, $CrushDamageType] = 0;
$DamageScales[Door, $BulletDamageType] = 0;
$DamageScales[Door, $PlasmaDamageType] = 0;
$DamageScales[Door, $EnergyDamageType] = 0;
$DamageScales[Door, $ExplosionDamageType] = 1.0;
$DamageScales[Door, $MissileDamageType] = 1.0;
$DamageScales[Door, $DebrisDamageType] = 1.0;
$DamageScales[Door, $LaserDamageType] = 0;
$DamageScales[Door, $MortarDamageType] = 1.0;
$DamageScales[Door, $ElectricityDamageType] = 0;
$DamageScales[Door, $BlasterDamageType] = 0;
$DamageScales[Door, $MineDamageType] = 1.0;
$DamageScales[Door, $BombDamageType] = 1.0;
$DamageScales[Door, $KnifeStabDamageType] = 0;
$DamageScales[Door, $PoisonGasDamageType] = 0;
$DamageScales[Door, $KnifeDamageType] = 0;
$DamageScales[Door, $GrappleFallDamageType] = 0;
$DamageScales[Door, $AGPDamageType] = 1.0;
$DamageScales[Door, $BoomeringDamageType] = 0;

$DamageScales[KeypadDoor, $ImpactDamageType] = 0;
$DamageScales[KeypadDoor, $CrushDamageType] = 0;
$DamageScales[KeypadDoor, $BulletDamageType] = 0;
$DamageScales[KeypadDoor, $PlasmaDamageType] = 0;
$DamageScales[KeypadDoor, $EnergyDamageType] = 0;
$DamageScales[KeypadDoor, $ExplosionDamageType] = 1.0;
$DamageScales[KeypadDoor, $MissileDamageType] = 1.0;
$DamageScales[KeypadDoor, $DebrisDamageType] = 1.0;
$DamageScales[KeypadDoor, $LaserDamageType] = 0;
$DamageScales[KeypadDoor, $MortarDamageType] = 1.0;
$DamageScales[KeypadDoor, $ElectricityDamageType] = 0;
$DamageScales[KeypadDoor, $BlasterDamageType] = 0;
$DamageScales[KeypadDoor, $MineDamageType] = 1.0;
$DamageScales[KeypadDoor, $BombDamageType] = 1.0;
$DamageScales[KeypadDoor, $KnifeStabDamageType] = 0;
$DamageScales[KeypadDoor, $PoisonGasDamageType] = 0;
$DamageScales[KeypadDoor, $KnifeDamageType] = 0;
$DamageScales[KeypadDoor, $GrappleFallDamageType] = 0;
$DamageScales[KeypadDoor, $AGPDamageType] = 1.0;
$DamageScales[KeypadDoor, $BoomeringDamageType] = 0;

$DamageScales[Tree, $PlasmaDamageType] = 5.0;
$DamageScales[Tree, $BulletDamageType] = 0.1;
$DamageScales[Tree, $KnifeDamageType] = 0.01;

$DamageScales[Armor, $KnifeDamageType] = 1;
$DamageScales[Armor, $KnifeStabDamageType] = 1;
$DamageScales[Armor, $PoisonGasDamageType] = 1;

$DamageScales[Vehicle, $BulletDamageType] = 0.5;
$DamageScales[Vehicle, $PlasmaDamageType] = 0.5;

// Data name level damage scales are in their appropriate files (Juggernaught would be in vehicles\juggernaught.cs, etc.)