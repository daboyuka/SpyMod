ItemData BodyArmor {
	description = "Body Armor";
	className = "Repair";
	shapeFile = "ammo1";
   heading = "eMiscellany";
	shadowDetailMask = 4;
  	price = 2;
   validateShape = true;
   validateMaterials = true;
};

function BodyArmor::onCollision(%this,%object) {
	if (getObjectType(%object) == "Player") {
		%armor = Player::getArmor(%object);
		if (GameBase::getEnergy(%object) < %armor.maxEnergy) {
			%energy = GameBase::getEnergy(%object);
			GameBase::setEnergy(%object, min(%energy + (10 * 100 * Item::getCount(%this)), %armor.maxEnergy));
			%item = Item::getItemData(%this);
			Item::playPickupSound(%this);
			Item::respawn(%this);
		}
	}
}