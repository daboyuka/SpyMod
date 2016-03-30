$Time::MONTHS = "31 28 31 30 31 30 31 31 30 31 30 31";
$Time::LEAP_MONTHS = "31 29 31 30 31 30 31 31 30 31 30 31";

$Time::MONTH_NAMES[0] = "January";
$Time::MONTH_NAMES[1] = "February";
$Time::MONTH_NAMES[2] = "March";
$Time::MONTH_NAMES[3] = "April";
$Time::MONTH_NAMES[4] = "May";
$Time::MONTH_NAMES[5] = "June";
$Time::MONTH_NAMES[6] = "July";
$Time::MONTH_NAMES[7] = "August";
$Time::MONTH_NAMES[8] = "September";
$Time::MONTH_NAMES[9] = "October";
$Time::MONTH_NAMES[10] = "November";
$Time::MONTH_NAMES[11] = "December";

$Time::CUM_MONTHS = "";
$Time::CUM_LEAP_MONTHS = "";

%last = 0;
%lastL = 0;
for (%i = 0; %i < 12; %i++) {
  $Time::CUM_MONTHS = $Time::CUM_MONTHS @ tern(%i!=0," ","") @ (%last += getWord($Time::MONTHS, %i));
  $Time::CUM_LEAP_MONTHS = $Time::CUM_LEAP_MONTHS @ tern(%i!=0," ","") @ (%lastL += getWord($Time::LEAP_MONTHS, %i));
}

$Time::startupTime = -1;
$Time::startupIntegerTime = -1;

function Time::initTime() {
  BanList::add("TIME", 10);
  BanList::export("config\\timeInit.cs");
  $ConsoleWorld::DefaultSearchPath = $ConsoleWorld::DefaultSearchPath;
  function BanList::addAbsolute(%a, %b) {
    $Time::startupTime = ((%b|0)-10)|0;
  }
  exec("timeInit.cs");
  $Time::startupIntegerTime = getIntegerTime(true);
  File::delete("config\\timeInit.cs");
}

function Time::getTime() {
  if ($Time::startupTime == -1) return -1;
  return ($Time::startupTime + ((getIntegerTime(true)-$Time::startupIntegerTime)/31.25))|0;
}

//
// ALL DATE VALUES ARE 0-BASED, EXCEPT YEAR.
//   (examples: Year 2006 = 2006, January 5th = 4, 12AM = 0, 12PM = 12)
//
//  Should work for DST, maybe...
//

function Time::getDate() {
  %seconds = Time::getTime();

  if (%seconds == -1) return -1;

  %minutes = floor(((%seconds|0) / 60)|0);
  %seconds = (%seconds|0) % 60;

  %hours = floor(((%minutes|0) / 60)|0)-4;
  %minutes = (%minutes|0) % 60;

  %days = floor(((%hours|0) / 24)|0);
  %hours = (%hours|0) % 24;

  %year = 1973;
  %days -= 365*2+366;

  %year += floor(%days / (365*3+366)) * 4;
  %days %= (365*3+366);

  %year += floor(%days / 365);
  %days %= 365;

  return %year @ " " @ %days @ " " @ %hours @ " " @ %minutes @ " " @ %seconds;
}

function Time::isLeapYear(%year) {
  return (%year%4==0) && ((%year%100!=0) || (%year%400==0));
}

function Time::getMonth(%year, %day) {
  %ily = Time::isLeapYear(%year);
  for (%i = 0; %i < 12; %i++) {
    if (%day < getWord(tern(%ily, $Time::CUM_LEAP_MONTHS, $Time::CUM_MONTHS), %i)) return %i;
  }
  return -1;
}

function Time::getDayOfMonth(%year, %day) {
  %ily = Time::isLeapYear(%year);
  for (%i = 0; %i < 12; %i++) {
    if (%day < getWord(tern(%ily, $Time::CUM_LEAP_MONTHS, $Time::CUM_MONTHS), %i)) {
      if (%i == 0) return %day;
      else         return %day - getWord(tern(%ily, $Time::CUM_LEAP_MONTHS, $Time::CUM_MONTHS), %i-1);
    }
  }
  return -1;
}

function Time::formatDate() {
  %date = Time::getDate();
  if (%date == -1) return -1;

  %year = getWord(%date, 0);
  %day = getWord(%date, 1);
  %hour = getWord(%date, 2);
  %minutes = getWord(%date, 3);
  %seconds = getWord(%date, 4);

  %month = Time::getMonth(%year, %day);
  %monthName = $Time::MONTH_NAMES[%month];

  %dayOfMonth = Time::getDayOfMonth(%year, %day) + 1;
  %ordinalDay = %dayOfMonth @ getSuffix(%dayOfMonth);

  %ampm = tern(%hour >= 12, "PM", "AM");
  %hour12 = tern(%hour%12 != 0, %hour%12, 12);

  %minutes2Dig = tern(%minutes >= 10, %minutes, "0" @ %minutes);
  %seconds2Dig = tern(%seconds >= 10, %seconds, "0" @ %seconds);

  %str = strcat(%monthName, " ", %ordinalDay, ", ", %year, " ", %hour12, ":", %minutes2Dig, ":", %seconds2Dig, " ", %ampm);
  return %str;
}

function Time::generateTime(%year, %month, %day, %hour, %minute, %second) {
  %ily = Time::isLeapYear(%year);
  %monthBase = "0 " @ tern(%ily, $Time::CUM_LEAP_MONTHS, $Time::CUM_MONTHS);

  %yearDays = 365 * (%year - 1970);
  %yearDays += floor((%year-1)/4) - floor(1970/4);
  %yearDays -= floor((%year-1)/100) - floor(1970/100);
  %yearDays += floor((%year-1)/400) - floor(1970/400);

  %time = (%second + 60 * %minute + 3600 * %hour)|0;
  %time = %time + (3600 * 24 * (%yearDays + getWord(%monthBase,%month) + %day))|0;
  %time = %time + 14400|0;

  return %time;
}

function Time::getIntegerTime(%time) {
  if ($Time::startupTime == -1) return -1;
  return ($Time::startupIntegerTime + (%time - $Time::startupTime)*31.25)|0;
}