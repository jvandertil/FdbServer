param (
    [string]$BuildConfiguration = "Release",
    [string]$VersionSuffix = "local",
    [string]$TestResultLogger = "console;verbosity=normal",
    [string]$TestResultOutputDirectory = "$PSScriptRoot\..\output\TestResults",
    [string]$OutputDirectory = "$PSScriptRoot\..\output\packages"
)

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

$buildDir = $PSScriptRoot
$sourceDir = "$PSScriptRoot\.."

Remove-Item -Recurse -Force "$sourceDir\src\FdbServer\bin","$sourceDir\src\FdbServer\obj" -ErrorAction SilentlyContinue
Remove-Item -Force "$OutputDirectory\*.nupkg" -ErrorAction SilentlyContinue

exec { & dotnet clean $sourceDir\FdbServer.sln }

exec { & dotnet restore --force $sourceDir\FdbServer.sln }

exec { & dotnet build --no-restore -c $BuildConfiguration --version-suffix=$VersionSuffix $sourceDir\FdbServer.sln }

exec { & dotnet test --logger=$TestResultLogger -r $TestResultOutputDirectory --no-build -c $BuildConfiguration --verbosity=normal $sourceDir\test\FdbServer.Tests\FdbServer.Tests.csproj }

exec { & dotnet pack -o $OutputDirectory --no-build --include-symbols -c $BuildConfiguration --version-suffix=$VersionSuffix $sourceDir\src\FdbServer\FdbServer.csproj }