@echo off

SET VERSION=0.0.0
IF NOT [%1]==[] (set VERSION=%1)

dotnet pack src/protobufSnapshotting/protobufSnapshotting.csproj -o ../../dist -p:Version="%VERSION%" -p:PackageVersion="%VERSION%" -c Release