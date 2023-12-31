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

# 5. Call dotnet build and pass the version attribute
Write-Host "Start building program..."
dotnet build EasyProtobuf.csproj -c Debug -p:protobuf_version="$protocol_ver"

# 6. Check if the build was successful or not
if ($LASTEXITCODE -eq 0) {
  Write-Host "MSBuild Successful."
} else {
  Write-Host "MSBuild Failed."
  exit 1
}

# 7. Use the default config
if (!(Test-Path -Path "config-$protocol_ver.json" -PathType Leaf)) {
  Copy-Item "config_example.json" "config-$protocol_ver.json"
}

# 8. Set Protobuf Version and Enable HotPatch
$env:EASYPROTOBUF_PROTOCOL_VERSION=$protocol_ver
$env:COMPLUS_ForceENC=1

# 9. Run 
dotnet run --no-build