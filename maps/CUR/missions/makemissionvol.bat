@ECHO OFF

DEL MissionPackWD.vol

CD ..

FOR %%Q IN (missions\*.cs,missions\*.mis,missions\*.dsc) DO vt.exe -q -lzh -rle missions\MissionPackWD.vol %%Q

cd missions

vtlist * MissionPackWD.vol
