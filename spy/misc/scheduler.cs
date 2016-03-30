// $Scheduler::task[%id]       // The script line to evaluate
// $Scheduler::taskITime[%id]  // The integer time at which to execute it
// $Scheduler::lastTaskID      // The ID for the last task scheduled

// $Scheduler::taskList[%i]    // The %i'th task pending
// $Scheduler::taskListSize    // The number of tasks pending

function Scheduler::checkTask(%id) {
  if ($Scheduler::task[%id] == "") return;


  %curTime = getIntegerTime(true);
  %schTime = $Scheduler::taskITime[%id];

  if (%curTime >= %schTime) {
    eval($Scheduler::task[%id]);
    Scheduler::deleteTask(%id);
  } else {
    %deltaSec = (%schTime - %curTime)/31.25;
    if (%deltaSec > 3600) schedule("Scheduler::checkTask("@%id@");", 3600);
    else                  schedule("Scheduler::checkTask("@%id@");", %deltaSec);
  }
}

function Scheduler::deleteTask(%id) {
  $Scheduler::task[%id] = "";
  $Scheduler::taskITime[%id] = "";

  for (%i = 0; %i < $Scheduler::taskListSize; %i++) {
    if ($Scheduler::taskList[%i] == %id) {
      $Scheduler::taskListSize--;
      $Scheduler::taskList[%i] = $Scheduler::taskList[$Scheduler::taskListSize];
      break;
    }
  }
}

function Scheduler::addTask(%eval, %itime) {
  %id = ($Scheduler::lastTaskID++)|0;
  $Scheduler::task[%id] = %eval;
  $Scheduler::taskITime[%id] = %itime;

  $Scheduler::taskListSize++;
  $Scheduler::taskList[$Scheduler::taskListSize - 1] = %id;

  Scheduler::checkTask(%id);
  return %id;
}

function Scheduler::rescheduleTasks() {
  // Iterate backwards so that if any task is deleted, we won't miss the new one that is swapped into it's place
  for (%i = $Scheduler::taskListSize - 1; %i >= 0; %i--) {
    Scheduler::checkTask($Scheduler::taskList[%i]);
  }
}

// public API

function cancelSchedule(%id) {
  Scheduler::deleteTask(%id);
}

// %time = integer time
function pschedule(%command, %delta) {
  Scheduler::addTask(%command, getIntegerTime(true) + %delta);
}

// %time = system clock time
function pscheduleAtTime(%command, %time) {
  Scheduler::addTask(%command, %time);
}

function resetScheduler() {
  deleteObject(ConsoleScheduler);
  newObject(ConsoleScheduler, SimConsoleScheduler);
  Scheduler::rescheduleTasks();
}
