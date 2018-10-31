pushd %~dp0\src\Voidwell.FileWell.Data
set ASPNETCORE_ENVIRONMENT=Development
dotnet ef migrations add filedbcontext.release.1 -v ^
    -c Voidwell.FileWell.Data.FileDbContext ^
    -o ./Migrations ^
    --msbuildprojectextensionspath ./../../build/Voidwell.FileWell.Data/Debug/obj
popd