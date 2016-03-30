//
// Timeline structure:
//
// Root group {
//   "Firstscene <TIME>" {
//     "Subscene <TIME>" {
//       "Event1 <TIME>" {
//         
//       }
//     }
//   }
//   "Secondscene <TIME>" {
//   }
// }
//

if ($Cinematic::nextMovieID == "")
  $Cinematic::nextMovieID = 0;

function Cinematic::nextMovieID() {
  return ($Cinematic::nextMovieID++) - 1;
}

function Cinematic::newMovie(%timeline) {
  %movieID = Cinematic::nextMovieID();
  %movie = newObject("Movie" @ %movieID, SimGroup);

  addToSet(%movie, %timeline);
  %movie.timeline = %timeline;
  %timeline.isTimeline = true;

  %movie.running = false;

  return %movie;
}

function Cinematic::newMovieFromFile(%timelineFile) {
  setInstantGroup(0);

  %oldcount = Group::objectCount(0);
  exec(%timelineFile);
  %newcount = Group::objectCount(0);

  if (%newcount == %oldcount + 1) {
    %timeline = Group::getObject(0, Group::objectCount(0) - 1);
    return Cinematic::newMovie(%timeline);
  } else {
    echo("ERROR: Timeline from file " @ %timelineFile @ " did not load!");
    return -1;
  }
}

function Cinematic::deleteMovie(%movie, %deleteTimeline) {
  if (!%deleteTimeline) removeFromGroup(%movie, %movie.timeline);

  if (Cinematic::isMovieRunning(%movie))
    Cinematic::stopMovie(%movie);

  deleteObject(%movie);
}

function Cinematic::startMovie(%movie) {
  if (%movie.running) return;
  %movie.running = true;
}

function Cinematic::stopMovie(%movie) {
  if (!%movie.running) return;
  %movie.running = false;
}

function Cinematic::getEventTimeRelative(%event) {
  %time = getWord(Object::getName(%event), 1);
  if (%time == -1) %time = 0;
  return %time;
}

function Cinematic::getEventTimeAbsolute(%event) {
  %time = 0;
  %cur = %event;
  while (%cur != %movie.timeline && %cur > 0 && !%cur.isTimeline) {
    %time += Cinematic::getEventTimeRelative(%cur);
    %cur = getGroup(%cur);
  }

  if (%cur == 0 || %cur == 2048) {
    echo("ERROR: Cinematic::getEventTimeAbsolute traversed invalid scene structure; reached root group before timeline group");
    return -1;
  }

  return %time;
}

function Cinematic::getFirstEvent(%movie) {
  return Group::getFirstDescendant(%movie.timeline);
}

function Cinematic::getNextEvent(%movie, %event) {
  return Group::getNextLeaf(%movie.timeline, %event);
}
