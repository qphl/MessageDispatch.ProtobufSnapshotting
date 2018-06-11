@echo off

SET TARGET="Default"
IF NOT [%1]==[] (set TARGET="%1")

SET BUILDMODE="Release"
IF NOT [%2]==[] (set BUILDMODE="%2")

SET VERSION=0.0.0
IF NOT [%3]==[] (set VERSION=%3)

dotnet pack src -o ../../dist -p:target=%TARGET% -p:Version=%VERSION% -p:PackageVersion=%VERSION% -c %BUILDMODE%