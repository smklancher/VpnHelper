cd /d %~dp0

dotnet publish -r win-x64 -c Release -p:PublishSingleFile=true --self-contained -p:PublishTrimmed=true -p:EnableCompressionInSingleFile=true -o:.\Publish\SelfContained
dotnet publish -r win-x64 -c Release -p:PublishSingleFile=true --no-self-contained -o:.\Publish\FrameworkDependentSingleFile

pause