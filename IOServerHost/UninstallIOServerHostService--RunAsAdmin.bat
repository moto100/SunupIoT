
@echo off
color 0a
set base_dir=%~dp0
%base_dir:~0,2%
pushd %base_dir%
sc delete "Sunup IO Server Platform"
@echo STOP THE WINDOW SERVICE MANUALY!!!
popd
pause
