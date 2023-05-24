
## Build nuget package
dotnet pack -c Release

## Deploy 

dotnet nuget push bin/Release/<name-of-nupkg-file> --api-key=<api-key> -s https://swwao.orbit.au.dk/nuget/v3/index.json


* List: https://swwao.orbit.au.dk/nuget/