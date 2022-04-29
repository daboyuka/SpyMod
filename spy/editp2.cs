function teleToLOS(%clientId) {
	%player = Client::getControlObject(%clientId);
	if (GameBase::getLOSInfo(%player, 2000)) {
		%pos = GameBase::getPosition(%player);
		%boxPos = getBoxCenter(%player);
		%boxPosToPos = Vector::sub(%pos, %boxPos);
		
		%teleTo = Vector::add($los::position, Vector::resize($los::normal, 1.5));
		%teleTo = Vector::add(%teleTo, %boxPosToPos);
		GameBase::setPosition(%player, %teleTo);
	}
}
