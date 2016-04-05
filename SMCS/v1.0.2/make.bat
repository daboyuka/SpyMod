@echo off

rmdir /S /Q SMCSInstall
mkdir SMCSInstall
mkdir SMCSInstall\config
mkdir SMCSInstall\base
mkdir SMCSInstall\base\missions

COPY spymodOptions.gui SMCSInstall\config\
COPY spymodScript.cs SMCSInstall\config\
COPY spymodScriptDSC.cs SMCSInstall\base\missions\spymodScriptDSC.dsc
