
@echo off
color 0a
set base_dir=%~dp0
%base_dir:~0,2%
pushd %base_dir%
sc create "Sunup IO Server Control Panel" binPath= "%~dp0Sunup.ControlPanelWeb.exe" DisplayName= "Sunup IO Server Control Panel" start= auto
@echo START THE WINDOW SERVICE MANUALY!!!
popd
pause
