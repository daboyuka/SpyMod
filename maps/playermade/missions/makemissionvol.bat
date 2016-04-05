@ECHO OFF

DEL MissionPackPlayerMade.vol

CD ..

FOR %%Q IN (missions\*.mis,missions\*.dsc) DO vt.exe -q -lzh -rle missions\MissionPackPlayerMade.vol %%Q

@ECHO ON

vtlist * missions\MissionPackPlayerMade.vol

cd missions