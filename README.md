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
3. Create a sub directory named `Protos` and put `*.proto` files inside. e.g. `Protobuf-hk4e_3.6_live/Protos/...`.
4. Start `./publish` with `protobuf_version`, e.g:

   ```sh
   ./publish hk4e_3.6_live
   ```

5. Check Output at `EasyProtobuf Build`

## Usage

- Build First
- Start with `./run`
- Type the proto name
- Paste base64 string / HEX string / Json string (support auto-detect)
- Profit

## Notes
- **Don't paste `query_cur_region` content here!** It's RSA encrypted.
- You can copy the built assets to anywhere, without neither original protos nor compiled code. But along with `bin` folder and two `run` scripts!
- If your protos contains `package` or `option csharp_namespace`, remove them.
  like: 

  ```
  package CrepeSR.Proto;
  option csharp_namespace = "Grasscutter.Proto";
  ```

  You may open VSCode, select "replace all" and remove them.      
  If you want to use them with package name, just type the name with namespace:

  ```log
  09:39:48 <Info> ----------New Work (Protobuf version: hkrpg_0.7_live)----------
  09:39:48 <Info> Type the proto name here:
  CrepeSR.Proto.PlayerLoginCsReq
  09:40:29 <Info> Well done! The proto exists.
  ```
