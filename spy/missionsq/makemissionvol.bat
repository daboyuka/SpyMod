@ECHO OFF

DEL MissionPack.vol

CD ..

FOR %%Q IN (missions\*.cs,missions\*.mis,missions\*.dsc) DO vt.exe -q -lzh -rle missions\MissionPack.vol %%Q

@ECHO ON

vtlist * missions\MissionPack.vol

cd missions