focusServer();

%numItems = getNumItems();
for (%i = 0; %i < %numItems; %i++) {
  %item = getItemData(%i);
  if (%item.className == "Weapon" && $InvList[%item] != "" && $InvList[%item])
    MissionRegItem(Weapons, %item.description, %item, 1);

  if (%item.className == "Tool" && $InvList[%item] != "" && $InvList[%item])
    MissionRegItem(Gadgets, %item.description, %item, 1);

  MissionRegItem(Gadgets, "Parachute Pack", ParachutePack, 1);

  if (%item.className == "Ammo" && $InvList[%item] != "" && $InvList[%item]) {
    %amt = $SellAmmo[%item];
    if ($SellAmmo[%item] == "" || !$SellAmmo[%item]) %amt = 5;
    MissionRegItem(Ammo, %item.description, %item, %amt);
  }
}
MissionRegItem(Items, "Body Armor", BodyArmor, 1);

focusClient();

MissionRegObject(Generators, "Big Generator", MissionCreateObject, BigGenerator, "StaticShape", BigGenerator);
MissionRegObject(Generators, "Big Solar Panel", MissionCreateObject, BigSolarPanel, "StaticShape", BigSolarPanel);

MissionRegObject(KeypadDoors, KeypadDoorForceField4x8, MissionCreateObject, KeypadDoorForceField4x8, "Moveable", KeypadDoorForceField4x8);
MissionRegObject(KeypadDoors, KeypadDoorFive, MissionCreateObject, KeypadDoorFive, "Moveable", KeypadDoor1);
MissionRegObject(Keypads, Keypad1, MissionCreateObject, Keypad1, "StaticShape", Keypad1);

MissionRegObject(Triggers, ScriptTrigger, MissionCreateObject, ScriptTrigger, "Trigger", ScriptTrigger);
MissionRegObject(Triggers, RegionTrigger, MissionCreateObject, RegionTrigger, "Trigger", RegionTrigger);
MissionRegObject(Triggers, MessageTrigger, MissionCreateObject, MessageTrigger, "Trigger", MessageTrigger);
MissionRegObject(Triggers, DamageTrigger, MissionCreateObject, DamageTrigger, "Trigger", DamageTrigger);

MissionRegObject(Misc, "Big Satellite Dish", MissionCreateObject, BigSat, "StaticShape", BigSatDish);
MissionRegObject(Misc, Target6x6Octa, MissionCreateObject, Target6x6Octa, "Moveable", Target6x6Octa);
MissionRegObject(Misc, TargetPlayer, MissionCreateObject, TargetPlayer, "Moveable", TargetPlayer);
MissionRegObject(Misc, TargetSquare, MissionCreateObject, TargetSquare, "Moveable", TargetSquare);
MissionRegObject(Misc, TargetBlue, MissionCreateObject, TargetBlue, "Moveable", TargetBlue);
MissionRegObject(Misc, PoweredDamagingElectricalBeam, MissionCreateObject, PoweredDamagingElectricalBeam, "StaticShape", PoweredDamagingElectricalBeam);
MissionRegObject(Misc, LaserBeamEmitter, MissionCreateObject, LaserBeamEmitter, "StaticShape", LaserBeamEmitter);
MissionRegObject(Misc, Window, MissionCreateObject, Window, "StaticShape", Window);
MissionRegObject(Misc, DiscoEmitter, MissionCreateObject, DiscoEmitter, "StaticShape", DiscoEmitter);
MissionRegObject(Misc, InfoSwitch, MissionCreateObject, InfoSwitch, "StaticShape", InfoSwitch);
MissionRegObject(Misc, Octa6x6, MissionCreateObject, Octa6x6, "StaticShape", Octa6x6);
MissionRegObject(Misc, MovingAntenna, MissionCreateObject, MovingAntenna, "Moveable", MovingAntenna);
MissionRegObject(Misc, MainStation, MissionCreateObject, MainStation, "StaticShape", MainStation);

MissionRegObject(Vehicles, SpyScout, MissionCreateObject, "SpyScout", Flier, SpyScout, true);
MissionRegObject(Vehicles, Juggernaught, MissionCreateObject, "Juggernaught", Flier, Juggernaught, true);
MissionRegObject(Vehicles, Transport, MissionCreateObject, "Transport", Flier, Transport, true);
MissionRegObject(Vehicles, Tank1, MissionCreateObject, "Tank1", Flier, Tank, true);
MissionRegObject(Vehicles, Tank2, MissionCreateObject, "Tank2", Flier, Tank2, true);
MissionRegObject(Vehicles, SpyScoutRespawnPoint, CreateVehicleRespawnPoint, SpyScout, 60);//MissionCreateObject, "VehicleRespawnPoint", Marker, VehicleRespawnPoint);
MissionRegObject(Vehicles, JuggernaughtRespawnPoint, CreateVehicleRespawnPoint, Juggernaught, 120);//MissionCreateObject, "VehicleRespawnPoint", Marker, VehicleRespawnPoint);

function CreateVehicleRespawnPoint(%type, %time) {
  %x = MissionCreateObject("VehicleRespawnPoint", Marker, VehicleRespawnMarker);
  focusServer();
  GameBase::addPosition(%x, "0 0 0.5");
  %x.vehicleType = %type;
  %x.respawnTime = %time;
  %x.enabled = false;
  %x.noMultipleVehicles = true;
  focusClient();
}

MissionRegObject(Turrets, "Machine Gun Turret", MissionCreateObject, MachineGunTurret, "Turret", MachineGunTurret);