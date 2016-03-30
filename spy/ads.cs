$Ads::AD_TIME = 300;
$Ads::NUM_ADS = 3;

$Ads::AD[0] = "Want to learn more about SpyMod? Check out the SpyMod website at spymod.cjb.net!";
$Ads::AD_COLOR[0] = 1;

$Ads::AD[1] = "Have some questions about SpyMod? Head over to spymod.cjb.net for answers!";
$Ads::AD_COLOR[1] = 1;

$Ads::AD[2] = "Where's a good place to talk with other SpyModers? Click the SpyMod forum link at spymod.cjb.net!";
$Ads::AD_COLOR[2] = 1;

$Ads::currentAd = 0;
function Ads::doAd() {
  if (!$Mission::noAds) {
    messageAll($Ads::AD_COLOR[$Ads::currentAd], $Ads::AD[$Ads::currentAd]);
    $Ads::currentAd = ($Ads::currentAd+1)%$Ads::NUM_ADS;
  }
  Ads::scheduleAd();
}

function Ads::scheduleAd() {
  schedule("Ads::doAd();", $Ads::AD_TIME);
}