
/////////////////////
// AI script macros
/////////////////////

//
// SuperAI::enemySearchMacro
//

function SuperAI::enemySearchMacro(%aiName) {
  return SuperAI::statePeriodic(%aiName, "SuperAI::checkForEnemies", -1, true);
}

function SuperAI::checkForEnemies(%aiName) {
  if (SuperAI::isDead(%aiName)) return;

  %obj = SuperAI::getOwnedObject(%aiName);
  %pos = getBoxCenter(%obj);//Matrix::subMatrix(GameBase::getMuzzleTransform(%obj), 3, 4, 3, 1, 0, 3);
  %trans = GameBase::getMuzzleTransform(%obj);
  %muzX = Matrix::subMatrix(%trans, 3, 4, 3, 1, 0, 0);
  %muzY = Matrix::subMatrix(%trans, 3, 4, 3, 1, 0, 1);
  %muzZ = Matrix::subMatrix(%trans, 3, 4, 3, 1, 0, 2);
  %range = AI::getVar(%aiName, "spotDist"); // Native AI variable
  %team = GameBase::getTeam(%obj);

  %enemies = newObject("", SimSet);
  for (%c = BaseRep::getFirst(); %c != -1; %c = BaseRep::getNext(%c)) {
    if (%c == AI::getID(%aiName)) continue;

    if (getNumTeams() > 1 && Client::getApparentTeam(%c) == %team) continue;

    %obj2 = Client::getOwnedObject(%c);
    if (%obj2 != -1) {
      %pos2 = getBoxCenter(%obj2);
      %vec = Vector::sub(%pos2, %pos);
      %vec = Vector::normalize(%vec);

      if (Vector::dot(%vec, %vec) < %range*%range) {
        %viewVec = Vector::dot(%vec, %muzX) @ " " @ Vector::dot(%vec, %muzY) @ " " @ Vector::dot(%vec, %muzZ);
        %rot = Vector::getRot(%viewVec);

        if (GameBase::getLOSInfo(%obj, %range, %rot)) {
            //%o = newObject("", StaticShape, LOSer, true);
            //GameBase::setPosition(%o, Vector::add(%pos, "0 0 1.3"));
          if ($los::object == %obj2) {
            if (SuperAI::fireEvent(%aiName, "checkTarget", 1, %obj2)) addToSet(%enemies, %obj2);
            //%p = Projectile::spawnProjectile(RedLaser, "1 0 0 " @ Vector::sub($los::position, Vector::add(%pos, "0 0 1.3")) @ " 0 0 1 " @ Vector::add(%pos, "0 0 1.3"), %o, 0);
          } //else %p = Projectile::spawnProjectile(BlueLaser, "1 0 0 " @ Vector::sub($los::position, Vector::add(%pos, "0 0 1.3")) @ " 0 0 1 " @ Vector::add(%pos, "0 0 1.3"), %o, 0);
            //deleteObject(%o);
            //schedule("deleteObject(" @ %p @ ");", 4);
        }
      }
    }
  }

  if (Group::objectCount(%enemies) > 0) SuperAI::fireEvent(%aiName, "enemiesFound", 1, %enemies);

  deleteObject(%enemies);

  return (0.5 + getRandom() * 0.5)/3;
}

//
// SuperAI::quickTurnTo[Target]
//

function SuperAI::quickTurnToTarget(%aiName) {
  SuperAI::quickTurnTo(%aiName, GameBase::getPosition(AI::getTarget(%aiName)));
}

function SuperAI::quickTurnTo(%aiName, %pos2) {
  %pos1 = SuperAI::getPosition(%aiName);

  %dirRot = getWord(Vector::getRot(Vector::sub(%pos2, %pos1)), 2);
  %aiRot = getWord(SuperAI::getRotation(%aiName), 2);

  if (abs(%dirRot - %aiRot) > $PI/24) SuperAI::setRotation(%aiName, "0 0 " @ %dirRot);
}

