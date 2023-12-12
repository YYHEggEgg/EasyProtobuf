# Generatd by ChatGPT
# Converted from sh to ps1 by ChatGPT
# 1. 接受参数并赋值给变量 protocol_ver，若未传参则提示用户并退出
if ($args.Count -eq 0) {
  Write-Host "Please give in the param!"
  exit 1
} else {
  $protocol_ver = $args[0]
}

# 2. 检查目录 Protobuf-$protocol_ver 是否存在，不存在则提示用户并退出
if (!(Test-Path "Protobuf-$protocol_ver")) {
  Write-Host "Protobuf-$protocol_ver doesn't exist!"
  exit 1
}

# 3. 创建文件夹 Protobuf-$protocol_ver/Compiled
New-Item -ItemType Directory -Force -Path "Protobuf-$protocol_ver/Compiled"

# 4. 编译proto文件
# Write-Host "Start compiling protos..."
# protoc --proto_path="Protobuf-$protocol_ver/Protos" --csharp_out="Protobuf-$protocol_ver/Compiled" "Protobuf-$protocol_ver/Protos/*.proto"

# 5. 创建文件夹
New-Item -ItemType Directory -Force -Path "EasyProtobuf Build/$protocol_ver/bin"

# 6. Call dotnet build and pass the version attribute
Write-Host "Start building program..."
dotnet publish EasyProtobuf.csproj -c Release -p:protobuf_version=$protocol_ver -o "EasyProtobuf Build/$protocol_ver/bin"

# 7. Check if the build was successful or not
if ($LASTEXITCODE -eq 0) {
  Write-Host "Publish Successful."
} else {
  Write-Host "Publish Failed."
  exit 1
}

# 8. Copy the config and resources
if (!(Test-Path -Path "config-$protocol_ver.json" -PathType Leaf)) {
  Copy-Item "config_example.json" "config-$protocol_ver.json"
}
Copy-Item "config-$protocol_ver.json" "EasyProtobuf Build/$protocol_ver/config-$protocol_ver.json" -force
Copy-Item resources "EasyProtobuf Build/$protocol_ver/resources" -recurse -force

# 9. Write run.sh and run-win.bat
Set-Content -Path "EasyProtobuf Build/$protocol_ver/run" -Value @"
#!/bin/bash
export EASYPROTOBUF_PROTOCOL_VERSION=$protocol_ver
dotnet ./bin/EasyProtobuf.dll $*
"@
Write-Host "run shell script Generated at EasyProtobuf Build/$protocol_ver."

Set-Content -Path "EasyProtobuf Build/$protocol_ver/run.bat" -Value @"
set EASYPROTOBUF_PROTOCOL_VERSION=$protocol_ver
dotnet ./bin/EasyProtobuf.dll %*
"@
Write-Host "run.bat Generated at EasyProtobuf Build/$protocol_ver."

# Display success message and exit
Write-Host "Done!"
exit 0