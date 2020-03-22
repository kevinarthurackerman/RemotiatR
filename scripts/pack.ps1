Add-Type -AssemblyName System.IO.Compression.FileSystem

$root=(Get-Item -Path ".\..").FullName

Write-Output "Cleaning up and creating build directories"

$scriptsDir = "$($root)\scripts"
$nugetDir = "$($root)\nuget"
$packagesDir = "$($root)\packages"
$symbolsDir = "$($root)\symbols"

if(Test-Path $nugetDir){
    Write-Output "  Cleaning $($nugetDir)"
    Remove-Item $nugetDir\* -Recurse
}
else {
    Write-Output "  Creating $($nugetDir)"
    New-Item -ItemType Directory -Path $nugetDir | Out-Null 
}

if(Test-Path $packagesDir){
    Write-Output "  Cleaning $($packagesDir)"
    Remove-Item $packagesDir\* -Recurse
}
else {
    Write-Output "  Creating $($packagesDir)"
    New-Item -ItemType Directory -Path $packagesDir | Out-Null 
}

if(Test-Path $symbolsDir){
    Write-Output "  Cleaning $($symbolsDir)"
    Remove-Item $symbolsDir\* -Recurse
}
else {
    Write-Output "  Creating $($symbolsDir)"
    New-Item -ItemType Directory -Path $symbolsDir | Out-Null 
}

. "$($scriptsDir)\variables.ps1"

Write-Output "Checking for existing nuget packages to remove"
$buildOrder | ForEach-Object {
    . "$($scriptsDir)\variables.ps1"
    . "$($scriptsDir)\$($_).ps1"

    $packagePath = "$($env:USERPROFILE)\.nuget\packages\$($packageId)\$($version)"

    if(Test-Path $packagePath){
        Write-Output "  Removing package $($packagePath)"
        Remove-Item $packagePath -Recurse
    }
}

Write-Output "Building nuget packages"
$buildOrder | ForEach-Object {
    . "$($scriptsDir)\variables.ps1"
    . "$($scriptsDir)\$($_).ps1"

    dotnet restore $projectPath --force --no-cache --source ..\packages
    dotnet build $projectPath

    $packageName = "$($packageId).$($version)"
    $nupkgPath = "$($nugetDir)\$($packageName).nupkg"
    $snupkgPath = "$($nugetDir)\$($packageName).snupkg"

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
        -p:PackageLicenseExpression="MIT" `
        --output $root\nuget

    nuget add $nupkgPath -source $packagesDir

    $archive = [System.IO.Compression.ZipFile]::OpenRead($snupkgPath)
    $archive.Entries | Where-Object { $_.FullName -Like "*.pdb" } | ForEach-Object {
        $entryParent = Split-Path -Path (Split-Path -Path $_.FullName -Parent) -Leaf
        $entryTargetFilePath = "$($symbolsDir)\$($entryParent)\$($_.Name)"
        $entryDir = [System.IO.Path]::GetDirectoryName($entryTargetFilePath)

        if(!(Test-Path $entryDir )){
            New-Item -ItemType Directory -Path $entryDir | Out-Null 
        }

        [System.IO.Compression.ZipFileExtensions]::ExtractToFile($_, $entryTargetFilePath, $true);
    }
    $archive.Dispose()
}

Write-Output "Pack complete!"