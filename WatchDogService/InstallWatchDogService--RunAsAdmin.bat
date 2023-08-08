
@echo off
color 0a
set base_dir=%~dp0
%base_dir:~0,2%
pushd %base_dir%
installUtil.exe "Sunup.WatchDogService.exe"
popd
pause
