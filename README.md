# EasyProtobuf

A easy tool for protobuf related operations.

## Feature

- Convert Protobuf from JSON
- Convert Base64 / HEX Protobuf to JSON
- **Unknown Fields (not defined in your proto) detection**

## Requirements

- [.NET 6.0 Runtime](https://dotnet.microsoft.com/en-us/download)
- Network (to nuget.org)

## Build

1. Give your protos a name as the `protobuf_version`, e.g. `hk4e_3.6_live`.
2. Create a directory here with name of `Protobuf-$(protobuf_version)`, e.g. `Protobuf-hk4e_3.6_live`.
3. Put `*.proto` files inside. e.g. `Protobuf-hk4e_3.6_live/...`. Files under `Protos` sub-directory is also accepted.
4. Start `./publish` with `protobuf_version`, e.g:

   ```sh
   ./publish hk4e_3.6_live
   ```

5. Check Output at `EasyProtobuf Build`

## Usage

- Build First
- Start with `./run`
- Type the proto name or `help` to get command list
- Paste base64 string / HEX string / Json string (support auto-detect)
- Profit

## Notes
- **Don't paste `query_cur_region` content here!** It's RSA encrypted.  
  You can use `dcurr` and `gencur` command to do related options.
- You can copy the built assets to anywhere, without neither original protos nor compiled code. But along with everything under the folder!
- If your protos contains `package` option, please enter the namespace into `config-<protobuf>.json`. e.g. If your proto has: 

  ```proto
  package miHomo.Protos;
  ```

  Then just use:

  ```json
  {
    // ...
    "EasyProtobufProgram": {
      "ProtoRootNamespace": "MiHomo.Protos"
    }
  }
  ```
