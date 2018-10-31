pushd %~dp0\src\Voidwell.FileWell.Data
set ASPNETCORE_ENVIRONMENT=Development
dotnet ef database update -v ^
    -c Voidwell.FileWell.Data.FileDbContext ^
    --msbuildprojectextensionspath ./../../build/Voidwell.FileWell.Data/Debug/obj
popd