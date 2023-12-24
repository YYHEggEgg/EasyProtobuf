EN | [中文](Handbook_CN.md)

# EasyProtobuf

This program was originally designed for Protobuf operations. But to help you complete some simple daily tasks, this program also provides some utility class commands.

The following will use the Protobuf version `hk4e_3.4_gio-cmdid` as an example. If you want to get the same Protobuf version as the demonstration example, you can enter the following command:

```shell
git clone --branch hk4e_3.4_gio-cmdid https://github.com/YYHEggEgg/mihomo-protos Protobuf-hk4e_3.4_gio-cmdid
```

Then copy `config_example.json` to `config-hk4e_3.4_gio-cmdid.json`, and uncomment the 6th line `ProtoRootNamespace`, you can start:

```shell
./run hk4e_3.4_gio-cmdid
```

## Table of Contents

- [Protobuf Operations](#protobuf-operations)
- [`dcurr` Command](#dcurr-command)
- [`gencur` Command](#gencur-command)
- [`convert` Command](#convert-command)
- [`ec2b` Command](#ec2b-command)
- [`mt19937` Command](#mt19937-command)
  - [`rsa` Subcommand (default)](#rsa-subcommand-default)
  - [`from-seed` Subcommand](#from-seed-subcommand)
- [`xor` Command](#xor-command)
  - [`set-key` Subcommand](#set-key-subcommand)
  - [`operate` Subcommand (default)](#operate-subcommand-default)
- [`rsakeyconv` Command](#rsakeyconv-command)

## Protobuf Operations

After starting the program, you will encounter the following prompts:

```log
12:09:09 <Info> Welcome to EasyProtobuf v1.0.0! Protobuf version: hk4e_3.4_gio-cmdid.
12:09:09 <Info:Configuration> Loading config...
12:09:11 <Info> ----------New Work (Protobuf version: hk4e_3.4_gio-cmdid)----------
12:09:11 <Info> Type the proto name or command here; 'help' for commands help.
```

At this time, just enter the Proto message (message) name to start a `protobuf` guide.

```log
12:09:31 <Info:ProtobufCmd> Well done! The proto exists.
12:09:31 <Info:ProtobufCmd> Please type base64 encoded, HEX or JSON protobuf bin data (auto detect):
12:09:31 <Info:ProtobufCmd> You can also paste json data to get its serialized data.
```

For instructions on supporting multiple input types at the same time, please refer to the [`convert` Command](#convert-command).  
For precautions when pasting content to the console, please refer to the [`dcurr` Command](#dcurr-command).

## `dcurr` Command

The `dcurr` command can help you quickly obtain the content of the standardized `query_cur_region` (alias `query_gateway`) HTTP response.

```sh
dcurr <key_id> <curr_json>
```

### Example

The following command decrypts a response generated using `key_id=4`:

```sh
dcurr 4 {"content":"KsDGYWDKC+fFQBIOPIKtKbXcN8eHT/wJ6zVEB+sgqoZzuoiN9ZqKJkzXRKDFEAzmqqLbqU5QzjDcsSrX6DCNVbqx93VG09wq3+lcUEgNuQlZ03iPP3V3Ojg8VB+Z5rPszh7Cy39rz/eTdq+aMZIfpSI/lkGZqh8Gb8c8o8lAlUxZI91cZkFR843ZHbOyJ8w9spSRmwNK2zBiIejvJY1NZgUdcbGezyOGaiw4S+rsc+fvHiqYrMSEruCKgHMM6uC0TNshrjnJ5B/dFWqDaJ0FZ2vHF6btTCj/7ublkaLlOskd+R14+m+G2Le7YodTZQzat4L2PJHTJ6OqNcwi94g9Vg==","sign":"UQ6ky8hLFPOYnuR8eI7DRCJE5lzLyx9EAmd6fddCKzN2FzBSURmALcEOrbwm9sVCLCDahO8KcoC+x8nnHggsJ1uCV5IORbVJcHKlgom16JQ3VuWzImxoI7q1L7QDjfA9AQD0gMommL4zJxXxhJPQmtDflrAAIpx+XAE6/qD6Yjh9WKxyql/ojt72vcEKYY3/+DMsX69aZ4aP8dtwcqyjaC6vDo2ESunmgKufL+Jp63bysdwFC7IQ4uEtE16E5IIBwR4BikFo8OlNZ79Uw8xT6wARXKzFD7REvRwBf0XuGdLzW2uxLmYRTpS5BEEHJNLbkuwz84yRneuATLZsVE3gDQ=="}
```

The following output will be produced:

```log
20:54:54 <Info:EasyInput> Detected Json input!
20:54:56 <Info:dcurr> Decrypted json content:
{ "retcode": 1, "msg": "Not Found version config" }
20:54:56 <Info:dcurr> Sign Verified OK!
20:54:56 <Info:SetClipBoard> Result copied to clipboard.
```

### Annotation

`key_id` is the number of the RSA key pair to be used (which can be found in `resources/rsakeys`). This command requires that the corresponding key pair has been defined in `ClientPri` and `ServerPub-Official`.

`curr_json` is the body content of the received HTTP response, and the JSON should contain `content` and `sign`. Please note that since `curr_json` must be single-line, the following principles must be followed when pasting multi-line text to the console:

- If you are using the console on your local machine or the integrated terminal of VSCode, you can paste directly by pressing Ctrl + V;
- If you are using Windows Terminal, press Ctrl + Alt + V to paste;
- For remote terminals such as VSCode Remote or SSH, please convert the text to a single line before pasting.

The `dcurr` command will deserialize the content into JSON that conforms to `QueryCurrRegionHttpRsp` and verify the signature using the local RSA public key (ServerPub-Official). Therefore, if you encounter a prompt that the signature verification failed, the local RSA public key does not match the private key used by the server that generated the `query_cur_region` content.

If the encrypted Protobuf message is not called `QueryCurrRegionHttpRsp`, you can change `#/CurrRegionCmds/BasedProto` to adapt. If the encrypted content is plain text, you can change `#/CurrRegionCmds/UseProtoCurr` to `false` to input/output text without Proto.

## `gencur` command

The `gencur` command is similar to the `dcurr` command, it uses the provided `QueryCurrRegionHttpRsp` JSON form to generate valid `query_cur_region` content.

```sh
gencur <key_id> <protobuf_content>
```

### Example

The following command uses the 4th key pair to encrypt the provided Protobuf content:

```sh
gencur 4 { "retcode": 1, "msg": "Not Found version config" }
```

The following output will be produced:

```log
20:59:50 <Info:EasyInput> Detected Json input!
20:59:51 <Info:gencur> Result:
{"content":"D+CYjyKhHN3BY6ythPR7Wr7KlMXqR2mzSv+fE2splpvH73PnKq+L3Db4dlASBaFVKVBTWmNu3Qnkn6c0+J0LvWb5YDKG0CtAHpW+domtPZHAp5+PSQZ8ujLOkc40xu+t6Hi1UVeHFcFlsLeb63AcVzvq4Nj2MlIkUTTfZQWGvaAcKzNqP8zJ0BAup5WHz61X8oECfJD85RDiJrt4k1j8Vi/3AMHCvdBfUXWcGtfd/JXp+b/0KqKDLv8G5dxiRqOdOsMAWx2Sr+XhCQQn+3TLc/upilYWNUnEf6nJAogitQBJ0l7H53jjbQVSsDVJUzGkh7lHtwlMu4tpgkD9feOX7Q==","sign":"qnm24S+DNCKopMK4XA8AaEXhtANmsckVULBsS9FUPMnOgOW2DTv3Sy4rpcNuv8ozutf/UyGCmBS03vXtNs/Hqp8Z86XsDcdHOQNvLnmYWrxgHJjQCpftw4psvAlenlzlED5oqpq5IZkKlvhPeUg5QTu9PZSidYd16P9MbbWcbLuAy+FiZJ7+RrmcwzEnNp91dg1tWG5E58dKOBGFDmLdh0FJqp6d3sAk0uuETEYcyYL3nYZA9maBtgobXUtS+g+9xLIOa8W/oeTLWdNRkOMWNAfl92Tye2KcZvNiBSShqQ30GRlCHcrrYTax+t7uPwFSLuCsZ3nMId5uMOS4WkxwbA=="}
20:59:51 <Info:SetClipBoard> Result copied to clipboard.
```

### Annotation

`key_id` is the number of the RSA key pair to be used (which can be found in `resources/rsakeys`). This command requires that the corresponding key pair has been defined in `ClientPri` and `ServerPri-Hosting`.

The format of the parameter `<protobuf_content>` may be different from that obtained in other ways: it uses camel case naming except for the first letter, instead of the underscore form used in the original proto. For example, `region_info` is named `regionInfo` in it.

For notes on pasting content to the console, please refer to the [`dcurr` command](#dcurr-command).

If the encrypted Protobuf message is not called `QueryCurrRegionHttpRsp`, you can change `#/CurrRegionCmds/BasedProto` to adapt. If the content to be encrypted is plain text, you can change `#/CurrRegionCmds/UseProtoCurr` to `false` to input/output text without Proto.

## `convert` command

The `convert` command can convert the input content between base64 and HEX (hexadecimal string).

```sh
convert <base64_data/hex_data>
```

### Example

The following example converts the provided HEX string to a Base64 string:

```sh
convert 01 23 45 67 89 AB CD EF
```

The following output will be produced:

```log
21:06:17 <Info:EasyInput> Detected Hex input!
21:06:17 <Info:convert> Converted to Base64 format, handled 8 bytes.
21:06:17 <Info:SetClipBoard> Result copied to clipboard.
```

The result will be automatically copied to the clipboard. The clipboard now contains the following content:

```txt
ASNFZ4mrze8=
```

### Annotation

The input type will be automatically detected. If there is a problem with automatic detection, you can add `b-`, `h-` or `j-` before the input data to forcibly specify that the input type is base64, HEX or JSON.

For notes on pasting content to the console, please refer to the [`dcurr` command](#dcurr-command).

## `ec2b` command

The `ec2b` command can obtain the `server_secret_key` (also known as `dispatchKey`) through the `client_secret_key` (also known as `dispatchSeed`).

```sh
ec2b get_key <content_bindata(base64/hex)>
```

### Example

The following command will decrypt an `Ec2b` dispatchSeed:

<details> <summary>Click to expand the example command</summary>

</details>

The following output will be produced:

<details> <summary>Click to expand the example output</summary>

</details>

### Annotation

Generally speaking, you can find the `client_secret_key` (that is, `dispatchSeed`) in `QueryCurrRegionHttpRsp`, but the server actually uses `dispatchKey` as the initial XOR key.

For instructions on supporting multiple input types at the same time, please refer to the [`convert` command](#convert-command).  
For notes on pasting content to the console, please refer to the [`dcurr` command](#dcurr-command).

## `mt19937` command

Generate a 4096-byte XOR key based on the given seed or RSA parameters `clientRandkey` and `serverRandKey`.

### `rsa` subcommand (default)

Generate a 4096-byte XOR key based on the given RSA parameters `clientRandkey` and `serverRandKey`.

```sh
mt19937 <--anime|--sleep>
        -k, --key <rsa_key_id>
        <RSA_Encrypted_clientRandKey_base64/hex> <RSA_Encrypted_serverRandKey_base64/hex>
```

#### Example

- TODO

#### Annotation

You must choose one of `--anime` or `--sleep` to use the generation algorithm you want.

`rsa_key_id` is the number of the RSA key pair to be used (which can be found in `resources/rsakeys`). This command requires that the corresponding key pair has been defined in `ClientPri` and `ServerPri-Hosting`.

### `from-seed` subcommand

Generate a 4096-byte XOR key directly based on the given seed (`final_seed`).

```sh
mt19937 from-seed <--anime|--sleep>
                  <uint64_seed|uint64_seed_HEX>
```

#### Example

The following command generates the corresponding XOR key with `6167` and the star dream algorithm.

```sh
mt19937 from-seed --anime 6167
```

<details> <summary>Click to expand the example output</summary>

</details>

#### Annotation

You must choose one of `--anime` or `--sleep` to use the generation algorithm you want.

`final_seed` is generally defined as the XOR result of `clientRandKey` and `serverRandKey`.

## `xor` command

Perform XOR operation, analyze and output the result.

### `set-key` subcommand

Set the default key for XOR operation.

```sh
xor set-key
    <xorkey>  The demanded default key for XOR command.
```

#### Example

The following command sets the default key to the key generated in the `mt19937` command - `from-seed` subcommand above.

<details> <summary>Click to expand the example command</summary>

</details>

The following prompt will be produced:

```log
21:38:49 <Info:EasyInput> Detected Hex input!
21:38:49 <Info:xor> Successfully set default key: 4096 bytes.
```

### `operate` subcommand (default)

Perform an XOR operation.

```sh
xor <value>                      The HEX / Base64 Content that should be decrypted.
    -k, --xorkey                 Set the default key for XOR command.
    --validate-startswith <bin>  Validate the result starts with a certain pattern.
    --validate-endswith <bin>    Validate the result ends with a certain pattern.
```

#### Example

The following example provides a KEY and performs an XOR operation:

<details> <summary>Click to expand the example command</summary>

```sh
xor 60227DB5B8DD34DC20FEFAA221B1DA9B32FDF248C37A4CE85FCDB94A755504EB13ED78EA34D90B08B4D7ABB8A495BC0999703BBCB2705909854304CD375DE70FA7FC6B6148D90435187EAB3A45D21B9AAEDE60EE7327686D21D68DC3B2B0A1
--validate-startswith 45 67
--validate-endswith 89 AB
-k 25457D73B8DD34DC20ADF27DE705C7A17EA4AB00861D2BAD38AA992B1B3124BF7A8A108455AB6228D7B8C5DFD6F4C87CF5114FD99209367CA52C6AED4233836AD58F1F0026BD6D5B7F5EC35532F26BFBCDB5059A5350071F4ABFE3A493390AED8CAE6B91
```

</details>

The following output will be produced:

```log
22:03:17 <Info:EasyInput> Detected Hex input!
22:03:17 <Info:EasyInput> Detected Hex input!
22:03:17 <Info:SetClipBoard> Result copied to clipboard.
22:03:17 <Info:xor> Successfully decrypted data, input: 95 bytes, key: 100 bytes.
22:03:17 <Info:EasyInput> Detected Hex input!
22:03:17 <Info:xor> Validate StartsWith OK: Decrypted value starts with the provided patter
n.
22:03:17 <Info:EasyInput> Detected Hex input!
22:03:17 <Info:xor> Validate EndsWith OK: Decrypted value ends with the provided pattern.
```

The XOR result will be automatically copied to the clipboard. The clipboard now contains the following content:

<details> <summary>Click to expand the example clipboard output</summary>

```txt
456700C600000000005308DFC6B41D3A4C59594845676745676720616E6420546967686E61726920636F6E67726174756C61746520796F75206F6E20756E6465727374616E64696E6720686F77207061636B657420776F726B696E672189AB
```

</details>

#### Annotation

`validate-startswith` and `validate-endswith` check whether the XOR result starts/ends with a specific Magic, which can be used to verify the accuracy of the KEY.

If the verification fails, the following output will be produced:

```log
22:00:26 <Info:EasyInput> Detected Hex input!
22:00:26 <Warn:xor> Validate StartsWith failed: Decrypted value's start mismatches the provided pattern.
22:00:26 <Info:EasyInput> Detected Hex input!
22:00:26 <Warn:xor> Validate EndsWith failed: Decrypted value's end mismatches the provided pattern.
```

In addition, you can set the default key for XOR through the `set-key` subcommand. You can try to execute the example command of the `set-key` subcommand above, then remove the `-k` parameter in the example provided here and execute it, and the output will be exactly the same.

If the `-k, --key` parameter is provided, the default KEY will not be used.

## `rsakeyconv` Command

Convert the provided RSA key (PEM or XML format) to any supported format.

```sh
rsakeyconv
  <input-key-filePath>
  (or --cb-in: Get input from the clipboard.)
  -o, --outkey [RSA Key Format Identifiers]

  Avaliable Format identifier: 'Public', 'Private', 'Xml', 'Pkcs1', 'Pkcs8'
```

### Example

The following command converts the key in resources to an XML public key format suitable for Patch:

```sh
rsakeyconv "resources/rsakeys/ServerPri-Hosting/5-pri.pem" -o Public Xml
```

It will prompt that the key has been copied to the clipboard. The clipboard now contains the following content:

```xml
<RSAKeyValue>
  <Modulus>xbbx2m1feHyrQ7jP+8mtDF/pyYLrJWKWAdEv3wZrOtjOZzeLGPzsmkcgncgoRhX4dT+1itSMR9j9m0/OwsH2UoF6U32LxCOQWQD1AMgIZjAkJeJvFTrtn8fMQ1701CkbaLTVIjRMlTw8kNXvNA/A9UatoiDmi4TFG6mrxTKZpIcTInvPEpkK2A7Qsp1E4skFK8jmysy7uRhMaYHtPTsBvxP0zn3lhKB3W+HTqpneewXWHjCDfL7Nbby91jbz5EKPZXWLuhXIvR1Cu4tiruorwXJxmXaP1HQZonytECNU/UOzP6GNLdq0eFDE4b04Wjp396551G99YiFP2nqHVJ5OMQ==</Modulus>
  <Exponent>AQAB</Exponent>
</RSAKeyValue>
```

You can try to enter the following command into the console, copy the following KEY to the clipboard and then execute the command:

<details> <summary>Click to expand the KEY to be copied to the clipboard</summary>

</details>

```sh
rsakeyconv --cb-in -o Public Xml
```

At this time, the output should be exactly the same as before.

### Annotation

This command supports any combination of `Public`, `Private` and `Pkcs1`, `Pkcs8`, `Xml`, except:

- Public key (`Public`) cannot be used to generate private key (`Private`).

The type of the input key will be automatically detected, so there is no need to specify. The type identifier of the RSA key output format can be separated by spaces.

About the format of the key, there are the following convenient insights:

- The one that starts with `-----BEGIN PUBLIC KEY-----`, `-----BEGIN PRIVATE KEY-----` is the `Pkcs8` key;
- The one that starts with `-----BEGIN RSA PUBLIC KEY-----`, `-----BEGIN RSA PRIVATE KEY-----` is the `Pkcs1` key;
- The above two are distinguished by `PUBLIC` or `PRIVATE` for public and private keys. For the `Xml` format, only the `Modulus` and `Exponent` elements are public keys; those that contain many elements and are long are private keys.
- Keys that cannot be viewed in plain text (such as `.der`) are not supported by this program.


