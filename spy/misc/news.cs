// News file format:
//
// $News::numLines = 3;
// $News::line[0] = "This is the first line";
// $News::line[1] = "This is the second line";
// $News::line[2] = "This is the third line";
//
// Fonts:
//   <F0>Small white
//   <F1>Small dark green
//   <F2>Small cream
//   <F3>Small orange
//   <F4>Small red
//   <F5>Medium green
//   <F6>Medium cream
//   <F7>Medium orange
//   <F8>Large green
//   <F9>Large cream
//

$NewsConstants::NEWS_PATH = "config\\";
$NewsConstants::NEWS_FILE = "spyNews.cs";

function News::loadNews() {
  exec($NewsConstants::NEWS_FILE);
}

function News::showNews(%line) {
  if ($News::numLines == "") News::loadNews();
  for (%t = 0; %t < getNumTeams(); %t++) {
    for (%i = 0; %i < $News::numLines; %i++) {
      Team::setObjective(%t, %i + %line, $News::line[%i]);
    }
  }
}