@echo Off
set config=%1
if "%config%" == "" (
   set config=Release
)

REM Build
call %msbuild% src\cr-message-dispatch.sln /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false
if not "%errorlevel%"=="0" goto failure

REM Package
mkdir Build
cmd /c %nuget% pack "src\core\core.csproj" -symbols -o Build -p Configuration=%config%
if not "%errorlevel%"=="0" goto failure

cmd /c %nuget% pack "src\dispatchers.eventstore\dispatchers.eventstore.csproj" -symbols -o Build -p Configuration=%config%
if not "%errorlevel%"=="0" goto failure

cmd /c %nuget% pack "src\dispatchers.snapshotting.protobuf\dispatchers.snapshotting.protobuf.csproj" -symbols -o Build -p Configuration=%config%
if not "%errorlevel%"=="0" goto failure


:success
exit 0

:failure
exit -1