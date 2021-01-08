dotnet clean "src\Vts.sln" --configuration Release

dotnet build "src\Vts.sln" --configuration Release

dotnet test "src\Vts.sln" --configuration Release --no-build --no-restore

dotnet publish "src\Vts.sln" --configuration Release
