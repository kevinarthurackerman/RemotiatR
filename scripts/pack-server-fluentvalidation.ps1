. .\variables-server-fluentvalidation.ps1

Get-ChildItem ..\nuget -Filter "$packageId.$version.*" | ForEach-Object { Remove-Item $_.FullName }

dotnet pack $projectPath `
    -p:PackageId=$packageId `
    -p:PackageVersion=$version `
    -p:Version=$version `
    -p:Authors=$authors `
    -p:Copyright=$copyright `
    -p:PackageProjectUrl=$repoUrl `
    -p:RepositoryUrl=$repoUrl `
    -p:EmbedUntrackedSources=true `
    -p:IncludeSymbols=true `
    -p:SymbolPackageFormat=snupkg `
    --output ..\nuget