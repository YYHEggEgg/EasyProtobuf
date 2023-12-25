# EasyProtobuf

A easy tool for protobuf related operations.

_Made by miHomo Software_

<div>
  <a href="https://discord.gg/NcAjuCSFvZ">
    <img alt="Discord - miHomo Software" src="https://img.shields.io/discord/1144970607616860171?label=Discord&logo=discord&style=for-the-badge">
  </a>
</div>

## What can it do?

- Protobuf
  - Convert Protobuf from JSON
  - Convert Base64 / HEX Protobuf to JSON
  - **Unknown Fields (not defined in your proto) detection**
- RSA
  - Basic RSA Encrypt / Decrypt / Sign / Verify
  - **Convert RSA Keys through different formats (including Private -> Public)**
  - `query_cur_region` decryption & generation
- More Dedicated Applications
  - MT19937 XOR Key Generate
  - Ec2b decrypt `dispatchSeed` -> `dispatchKey`
- Simple Tasks
  - Convert bytes in Base64 & HEX
  - XOR Decrypt data

## Requirements

- [.NET 6.0 Runtime](https://dotnet.microsoft.com/en-us/download)
- Network (for package restoration) (only during build process)

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
- Search demanded command in `Handbook.md` or just type the proto name
- Profit

## Notes
- **Don't directly paste `query_cur_region` content here!** It's RSA encrypted.  
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
