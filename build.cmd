@echo off

SET MAJOR=0
IF NOT [%1]==[] (set MAJOR=%1)

SET MINOR=0
IF NOT [%2]==[] (set MINOR=%2)

set PATCH=0
IF NOT [%3]==[] (set PATCH=%3)

set VERSION=%MAJOR%.%MINOR%.%PATCH%

set SUFFIX=
IF NOT [%4]==[] (set SUFFIX=%4)

set FULLVERSION=%VERSION%

IF NOT [%SUFFIX%]==[] (set FULLVERSION="%FULLVERSION%-%SUFFIX%")

dotnet pack src -o ../../dist -p:Version=%VERSION% -p:PackageVersion=%FULLVERSION% -c Release