//============================================================================================
// ES Sfx Region reserves tags  400,000- 409,999
// 
// *IMPORTANT, PLEASE READ*
//     Do not define SimTag id zero.  Darkstar relies on the fact 
//     that SimTag id zero is never defined.
//
//     The SimTags are grouped into 3 sections: REGION, RESOURCE, and DATA.
//     A REGION holds one more more RESOURCE and DATA sections.
//     Tags in a DATA group will be translated during a foriegn language conversion.
//     Tags in the RESOURCE group will NOT.
//
//============================================================================================

///////////////////////////////////////////////////////////////////////////////
// SFX ID CONSTANTS  ( IDSFX )
///////////////////////////////////////////////////////////////////////////////
IDSFX_BEG_SFX              = 00450000; //, "-- Sound SFX RESOURCE group reserves tags 450,000 - 499,999 --";

//Weapons 0000-0499
IDSFX_BXPLO1			      = 00450001;
IDSFX_BXPLO2			      = 00450002;
IDSFX_BXPLO3			      = 00450003;
IDSFX_BXPLO4			      = 00450004;
IDSFX_DISCLOAD			      = 00450005;
IDSFX_DISCLOOP			      = 00450006;
IDSFX_SHELL1			      = 00450007;
IDSFX_SHELL2			      = 00450008;
IDSFX_DRYFIRE			      = 00450009;
IDSFX_EXPLO3			      = 00450010;
IDSFX_EXPLO4			      = 00450011;
IDSFX_FIRE				      = 00450012;
IDSFX_FLAG1				      = 00450013;
IDSFX_FLAG2				      = 00450014;
IDSFX_GRENADE			      = 00450015;
IDSFX_INJURED			      = 00450016;
IDSFX_LANDHURT			      = 00450017;
IDSFX_LAND_ON_GROUND	      = 00450018;
IDSFX_AUTOGUNCOOL		      = 00450019;
IDSFX_AUTOGUN			      = 00450020;
IDSFX_PU_AMMO			      = 00450021;
IDSFX_PU_HEALTH		      = 00450022;
IDSFX_PU_WEAPON		      = 00450023;
IDSFX_PLASMA1			      = 00450024;
IDSFX_PLASMA2			      = 00450025;
IDSFX_PLASMA3			      = 00450026;
IDSFX_PLAYER_DEATH	      = 00450027;
IDSFX_PLAYER_HIT		      = 00450028;
IDSFX_RICOCHE1			      = 00450029;
IDSFX_RICOCHE2			      = 00450030;
IDSFX_RICOCHE3			      = 00450031;
IDSFX_RIFLE1			      = 00450032;
IDSFX_RIFLE2			      = 00450033;
IDSFX_ROCKET			      = 00450034;
IDSFX_ROCKETA			      = 00450035;
IDSFX_ROCKET3			      = 00450036;
IDSFX_SNIPER			      = 00450037;
IDSFX_SNIPER2			      = 00450038;
IDSFX_JET				      = 00450039;
IDSFX_WEAPON2			      = 00450040;
IDSFX_WEAPON3			      = 00450041;
IDSFX_WEAPON4			      = 00450042;
IDSFX_WEAPON5			      = 00450043;
IDSFX_EMPTY				      = 00450044;
IDSFX_SNOW				      = 00450045;
IDSFX_RAIN				      = 00450046;
IDSFX_TELEPORT			      = 00450047;
IDSFX_RIFLE_RELOAD	      = 00450048;
IDSFX_PU_PACK			      = 00450049;
IDSFX_LFOOTLSOFT		      = 00450050;
IDSFX_LFOOTRSOFT		      = 00450051;
IDSFX_LFOOTLHARD		      = 00450052;
IDSFX_LFOOTRHARD		      = 00450053;
IDSFX_LFOOTLSNOW		      = 00450054;
IDSFX_LFOOTRSNOW		      = 00450055;
IDSFX_MFOOTLSOFT		      = 00450056;
IDSFX_MFOOTRSOFT		      = 00450057;
IDSFX_MFOOTLHARD		      = 00450058;
IDSFX_MFOOTRHARD		      = 00450059;
IDSFX_MFOOTLSNOW		      = 00450060;
IDSFX_MFOOTRSNOW		      = 00450061;
IDSFX_HFOOTLSOFT		      = 00450062;
IDSFX_HFOOTRSOFT		      = 00450063;
IDSFX_HFOOTLHARD		      = 00450064;
IDSFX_HFOOTRHARD		      = 00450065;
IDSFX_HFOOTLSNOW		      = 00450066;
IDSFX_HFOOTRSNOW		      = 00450067;
IDSFX_FLAGCAPTURE		      = 00450068;
IDSFX_FLAGRETURN		      = 00450069;
IDSFX_PLAYER_RES		      = 00450070;
IDSFX_GRAZED			      = 00450071;
IDSFX_NAILED			      = 00450072;
IDSFX_ENERGYDONATE	      = 00450073;
IDSFX_SHIELDHIT		      = 00450074;
IDSFX_GENERATOR		      = 00450075;
IDSFX_TURRET_SHOOT 	      = 00450076;
IDSFX_ENERGYEXP 	         = 00450077;
IDSFX_ROCKEXP	 	         = 00450078;
IDSFX_SHOCKEXP	 	         = 00450079;
IDSFX_TURRETEXP 	         = 00450080;
IDSFX_FLIER_SHOOT		      = 00450081;


IDSFX_GRENADE_DEFAULT   = 00450100;
IDSFX_GRENADE_STONE     = 00450101;
IDSFX_GRENADE_METAL     = 00450102;

// DOORS AND ELEVATORS
IDSFX_DOOR1             = 00450200;
IDSFX_DOOR2             = 00450201;
IDSFX_DOOR3             = 00450202;
IDSFX_DOOR4             = 00450203;
IDSFX_DOOR5             = 00450204;

//other...TBA

//Voice  7000-9999
IDSFX_VOICE_DOH			= 00470000;
IDSFX_VOICE_ATTACK		= 00470001;
IDSFX_VOICE_BASECLEAR	= 00470002;
IDSFX_VOICE_HAVEFLAG		= 00470003;
IDSFX_VOICE_HELP			= 00470004;
IDSFX_VOICE_INCOMMING	= 00470005;
IDSFX_VOICE_ISCLEAR		= 00470006;
IDSFX_VOICE_NEEDFLAG		= 00470007;
IDSFX_VOICE_UNDERATTACK	= 00470008;
IDSFX_VOICE_WAITSIGNAL	= 00470009;
IDSFX_VOICE_EESYM			= 00470010;

IDSFX_VOICE_CMD_ATTACK	= 00470011;
IDSFX_VOICE_CMD_DEFEND	= 00470012;
IDSFX_VOICE_CMD_REPAIR	= 00470013;
IDSFX_VOICE_CMD_GOTO  	= 00470014;
IDSFX_VOICE_CMD_RECON 	= 00470015;
IDSFX_VOICE_CMD_ESCORT	= 00470016;
IDSFX_VOICE_CMD_DEPLOY	= 00470017;
IDSFX_VOICE_CMD_GET   	= 00470018;
IDSFX_VOICE_CMD_TEMP1	= 00470019;
IDSFX_VOICE_CMD_TEMP2  	= 00470020;


IDSFX_VOICE_CMD_ATTACK2            = 00470030;
IDSFX_VOICE_CMD_DEFEND_OUR_BASE    = 00470031;
IDSFX_VOICE_CMD_MOVE_OUT           = 00470032;
IDSFX_VOICE_CMD_REGROUP            = 00470033;
IDSFX_VOICE_CMD_OUR_BASE_SECURE    = 00470034;
IDSFX_VOICE_CMD_OUR_BASE_TAKEN     = 00470035;
IDSFX_VOICE_CMD_BEING_ATTACKED     = 00470036;
IDSFX_VOICE_CMD_DEFENDIND_BASE     = 00470037;
IDSFX_VOICE_CMD_OUR_FLAG_SECURE    = 00470038;
IDSFX_VOICE_CMD_INCOMING_ENEMIES   = 00470039;
IDSFX_VOICE_CMD_ACKNOWLEDGED       = 00470040;
IDSFX_VOICE_CMD_GENERAL_YEAH       = 00470041;
IDSFX_VOICE_CMD_SHAZBOT            = 00470042;
IDSFX_VOICE_CMD_DAMIT              = 00470043;
IDSFX_VOICE_CMD_BASIC_HI           = 00470044;
IDSFX_VOICE_CMD_I_NO_KNOW          = 00470045;
IDSFX_VOICE_CMD_HOW_FEEL           = 00470046;
IDSFX_VOICE_CMD_MISSED_ME          = 00470047;
IDSFX_VOICE_CMD_COME_GET_SOME      = 00470048;
IDSFX_VOICE_CMD_HEY                = 00470049;
IDSFX_VOICE_CMD_I_HAVE_EFLAG       = 00470050;
IDSFX_VOICE_CMD_IS_BASE_CLEAR      = 00470051;
IDSFX_VOICE_CMD_RETREAT            = 00470052;

IDSFX_VOICE_CMD_TAKE_BOW           = 00470060;
IDSFX_VOICE_CMD_WAVE_HELLO         = 00470061;
IDSFX_VOICE_CMD_JUMPING_JACK       = 00470062;
IDSFX_VOICE_CMD_GET_DOWN           = 00470063;

//Profiles 460000-
IDPRF_2D					              = 00460000;
IDPRF_NEAR				              = 00460001;
IDPRF_MEDIUM			              = 00460002;
IDPRF_FAR				              = 00460003;
IDPRF_MEDIUM_LOOP		              = 00460004;
IDPRF_NEAR_LOOP		              = 00460005;
IDPRF_2D_LOOP			              = 00460006;
IDPRF_FOOTS				              = 00460007;

IDSFX_END_SFX                      = 00499999;