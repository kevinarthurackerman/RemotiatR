Add-Type -AssemblyName System.IO.Compression.FileSystem

$root=(Get-Item -Path ".\..").FullName

$scriptsDir = "$($root)\scripts"
$nugetDir = "$($root)\nuget"
$packagesDir = "$($root)\packages"
$symbolsDir = "$($root)\symbols"

if(Test-Path $nugetDir){
    Remove-Item $nugetDir\* -Recurse
}
else {
    New-Item -ItemType Directory -Path $nugetDir | Out-Null 
}

if(Test-Path $packagesDir){
    Remove-Item $packagesDir\* -Recurse
}
else {
    New-Item -ItemType Directory -Path $packagesDir | Out-Null 
}

if(Test-Path $symbolsDir){
    Remove-Item $symbolsDir\* -Recurse
}
else {
    New-Item -ItemType Directory -Path $symbolsDir | Out-Null 
}

dotnet build $projectPath

Get-ChildItem $scriptsDir -Filter variables-*.ps1 -Recurse | ForEach-Object {
    . .\variables.ps1
    . .\$_

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