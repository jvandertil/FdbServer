# Taken from psake https://github.com/psake/psake

<#
.SYNOPSIS
  This is a helper function that runs a scriptblock and checks the PS variable $lastexitcode
  to see if an error occcured. If an error is detected then an exception is thrown.
  This function allows you to run command-line programs without having to
  explicitly check the $lastexitcode variable.
.EXAMPLE
  exec { svn info $repository_trunk } "Error executing SVN. Please verify SVN command-line client is installed"
#>
function Exec
{
    [CmdletBinding()]
    param(
        [Parameter(Position=0,Mandatory=1)][scriptblock]$cmd,
        [Parameter(Position=1,Mandatory=0)][string]$errorMessage = ($msgs.error_bad_command -f $cmd)
    )
    & $cmd
    if ($lastexitcode -ne 0) {
        throw ("Exec: " + $errorMessage)
    }
}

$CI_CONFIG = "$env:CI_CONFIG"
$CI_VERSION = "$env:CI_VERSION"

if ($CI_CONFIG -eq "")
{
    $CI_CONFIG = "Release"
}

$buildDir = $PSScriptRoot
$sourceDir = "$PSScriptRoot\.."
$outputDir = "$PSScriptRoot\..\output"

Remove-Item -Recurse -Force "$sourceDir\src\FdbServer\bin","$sourceDir\src\FdbServer\obj" -ErrorAction SilentlyContinue
Remove-Item -Force "$outputDir\*.nupkg" -ErrorAction SilentlyContinue

exec { & dotnet clean $sourceDir\FdbServer.sln }

exec { & dotnet restore --force $sourceDir\FdbServer.sln }

exec { & dotnet build --no-restore -c $CI_CONFIG --version-suffix=$CI_VERSION $sourceDir\FdbServer.sln }

exec { & dotnet test --no-build -c $CI_CONFIG --verbosity=normal $sourceDir\test\FdbServer.Tests\FdbServer.Tests.csproj }

exec { & dotnet pack -o $outputDir --no-build --include-symbols -c $CI_CONFIG --version-suffix=$CI_VERSION $sourceDir\src\FdbServer\FdbServer.csproj }