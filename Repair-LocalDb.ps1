# Repairs a crashed/corrupted MSSQLLocalDB instance and recreates EventHighwayDb.
# Run this whenever you see: "Local Database Runtime error occurred (0x89c50108)"

Write-Host "Stopping LocalDB..."
sqllocaldb stop MSSQLLocalDB -k 2>&1 | Out-Null

# Kill any lingering sqlservr processes
Get-Process -Name "sqlservr" -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Sleep -Seconds 1

# Clean up orphaned registry entries (no DataDirectory)
$key = "HKCU:\SOFTWARE\Microsoft\Microsoft SQL Server\UserInstances"
$removed = 0
Get-ChildItem $key -ErrorAction SilentlyContinue | ForEach-Object {
    $vals = Get-ItemProperty $_.PSPath
    if ([string]::IsNullOrEmpty($vals.DataDirectory)) {
        Remove-Item $_.PSPath -Force -Recurse
        $removed++
    }
}
Write-Host "Removed $removed orphaned registry entries."

# Rename stale data directory if master.mdf is missing
$dataPath = "$env:LOCALAPPDATA\Microsoft\Microsoft SQL Server Local DB\Instances\MSSQLLocalDB"
if ((Test-Path $dataPath) -and -not (Test-Path "$dataPath\master.mdf")) {
    $stamp = Get-Date -Format "yyyyMMddHHmmss"
    Rename-Item $dataPath "${dataPath}_old_$stamp" -Force -ErrorAction SilentlyContinue
    Write-Host "Renamed stale data directory."
}

# Delete and recreate the instance
sqllocaldb delete MSSQLLocalDB 2>&1 | Out-Null
sqllocaldb create MSSQLLocalDB 2>&1 | Out-Null
sqllocaldb start MSSQLLocalDB 2>&1 | Out-Null

$state = (sqllocaldb info MSSQLLocalDB | Select-String "State:").ToString().Trim()
Write-Host "Instance $state"

# Delete stale MDF files from user profile (prevent CREATE DATABASE conflict)
Remove-Item "$env:USERPROFILE\EventHighwayDb.mdf"     -Force -ErrorAction SilentlyContinue
Remove-Item "$env:USERPROFILE\EventHighwayDb_log.ldf"  -Force -ErrorAction SilentlyContinue
Remove-Item "$env:USERPROFILE\EventHighwayDB.mdf"      -Force -ErrorAction SilentlyContinue
Remove-Item "$env:USERPROFILE\EventHighwayDB_log.ldf"  -Force -ErrorAction SilentlyContinue

# Re-apply migrations
Write-Host "Applying EF migrations..."
Push-Location $PSScriptRoot
dotnet ef database update --project EventHighway.Core --startup-project EventHighway.Core
Pop-Location

Write-Host "Done. LocalDB is ready."
