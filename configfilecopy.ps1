# copy service-config file

Param(
  [string] $sourcePath,
  [string] $destinationPath
)

Write-Output "sourcePath: " $sourcePath
Write-Output "destinationPath: " $destinationPath

Copy-Item $sourcePath $destinationPath -Recurse -Force
