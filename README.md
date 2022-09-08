# ToolITs

dotnet publish --sc --os win -p:PublishSingleFile=true
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true  
dotnet publish -r win-x64 -p:PublishSingleFile=true --self-contained true


---
implement oracle NLS language
https://docs.microsoft.com/EN-US/dotnet/core/runtime-config/globalization
Setting name	Values
runtimeconfig.json	System.Globalization.Invariant	false - access to cultural data
true - run in invariant mode
MSBuild property	InvariantGlobalization	false - access to cultural data
true - run in invariant mode
Environment variable	DOTNET_SYSTEM_GLOBALIZATION_INVARIANT	0 - access to cultural data
1 - run in invariant mode

dotnet add package Microsoft.Extensions.Configuration.Json --version 7.0.0-preview.7.22375.6
dotnet add package Microsoft.Extensions.Configuration.FileExtensions --version 7.0.0-preview.7.22375.6
dotnet add package System.Configuration.ConfigurationManager --version 7.0.0-preview.7.22375.6
dotnet add package Microsoft.Extensions.Configuration --version 7.0.0-preview.7.22375.6


add region ctrl + k + s 
ctrl + alt + enter full screen 
