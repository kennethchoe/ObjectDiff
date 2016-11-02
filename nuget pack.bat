%systemroot%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe /t:build /v:m /nologo /p:Configuration=Release ObjectDiff\ObjectDiff.csproj
nuget pack ObjectDiffLogger.nuspec
nuget push ObjectDiffLogger.*.nupkg
del ObjectDiffLogger.*.nupkg
pause