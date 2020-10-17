rmdir CoverageResults /s /q

dotnet test ./src/DesignPatternSamples.sln ^
/p:CollectCoverage=true ^
/p:CoverletOutput=../../CoverageResults/ ^
/p:MergeWith="../../CoverageResults/coverage.json" ^
/p:CoverletOutputFormat=\"cobertura,json,opencover\" ^
-m:1

reportgenerator ^
-reports:"CoverageResults/coverage.cobertura.xml" ^
-targetdir:"CoverageResults/Report" ^
-reporttypes:Html;HTMLSummary

start "" "./CoverageResults/Report/index.html"

pause