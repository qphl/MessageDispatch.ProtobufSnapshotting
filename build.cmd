@echo off

SET VERSION=0.0.0
IF NOT [%1]==[] (set VERSION=%1)

SET TAG=0.0.0
IF NOT [%2]==[] (set TAG=%2)
SET TAG=%TAG:tags/=%

dotnet restore .\src\PharmaxoScientific.MessageDispatch.Snapshotting.sln -PackagesDirectory .\src\packages -Verbosity detailed

dotnet format .\src\PharmaxoScientific.MessageDispatch.Snapshotting.sln --severity warn --verify-no-changes -v diag
IF %errorlevel% neq 0 EXIT /B %errorlevel%

dotnet pack .\src\MessageDispatch.Snapshotting.Protobuf\MessageDispatch.Snapshotting.Protobuf.csproj -o .\dist -p:Version="%VERSION%" -p:PackageVersion="%VERSION%" -p:Tag="%TAG%" -c Release
