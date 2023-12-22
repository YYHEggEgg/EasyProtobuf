[EN](Handbook.md) | 中文

# EasyProtobuf

本程序最初是专为了 Protobuf 操作设计的。但为方便您完成一些简便的日常任务，本程序也提供了一些工具类命令。

下文将会以 Protobuf 版本 `hk4e_3.4_gio-cmdid` 举例。如果您想获得与演示例相同的 Protobuf 版本，可以输入以下命令：

```shell
git clone --branch hk4e_3.4_gio-cmdid https://github.com/YYHEggEgg/mihomo-protos Protobuf-hk4e_3.4_gio-cmdid
```

然后复制 `config_example.json` 到 `config-hk4e_3.4_gio-cmdid.json`，并取消第 6 行 `ProtoRootNamespace` 的注释，就可以启动：

```shell
./run hk4e_3.4_gio-cmdid
```

## 目录

- [`dcurr` 命令](#dcurr-命令)
- [`gencur` 命令](#gencur-命令)
- [`convert` 命令](#convert-命令)
- [`ec2b` 命令](#ec2b-命令)

## Protobuf 操作

开启程序后，您将会遇到以下提示：

```log
12:09:09 <Info> Welcome to EasyProtobuf v1.0.0! Protobuf version: hk4e_3.4_gio-cmdid.
12:09:09 <Info:Configuration> Loading config...
12:09:11 <Info> ----------New Work (Protobuf version: hk4e_3.4_gio-cmdid)----------
12:09:11 <Info> Type the proto name or command here; 'help' for commands help.
```

此时只要输入 Proto 消息（message）的名称，即可启动一次 `protobuf` 向导。

```log
<Info:ProtobufCmd> Well done! The proto exists.
<Info:ProtobufCmd> Please type base64 encoded, HEX or JSON protobuf bin data (auto detect):
<Info:ProtobufCmd> You can also paste json data to get its serialized data.
```

有关同时支持多种输入类型的说明，请参阅 [`convert` 命令](#convert-命令)。  
有关粘贴内容作为其参数时的注意事项，请参阅 [`dcurr` 命令](#dcurr-命令)。

## `dcurr` 命令

`dcurr` 命令可以帮助您快速获取标准化 `query_cur_region`（别名 `query_gateway`）HTTP 响应的内容。

```sh
dcurr <key_id> <curr_json>
```

### 示例

以下命令解密一个使用 `key_id=4` 生成的响应：

```sh
dcurr 4 {"content":"KsDGYWDKC+fFQBIOPIKtKbXcN8eHT/wJ6zVEB+sgqoZzuoiN9ZqKJkzXRKDFEAzmqqLbqU5QzjDcsSrX6DCNVbqx93VG09wq3+lcUEgNuQlZ03iPP3V3Ojg8VB+Z5rPszh7Cy39rz/eTdq+aMZIfpSI/lkGZqh8Gb8c8o8lAlUxZI91cZkFR843ZHbOyJ8w9spSRmwNK2zBiIejvJY1NZgUdcbGezyOGaiw4S+rsc+fvHiqYrMSEruCKgHMM6uC0TNshrjnJ5B/dFWqDaJ0FZ2vHF6btTCj/7ublkaLlOskd+R14+m+G2Le7YodTZQzat4L2PJHTJ6OqNcwi94g9Vg==","sign":"UQ6ky8hLFPOYnuR8eI7DRCJE5lzLyx9EAmd6fddCKzN2FzBSURmALcEOrbwm9sVCLCDahO8KcoC+x8nnHggsJ1uCV5IORbVJcHKlgom16JQ3VuWzImxoI7q1L7QDjfA9AQD0gMommL4zJxXxhJPQmtDflrAAIpx+XAE6/qD6Yjh9WKxyql/ojt72vcEKYY3/+DMsX69aZ4aP8dtwcqyjaC6vDo2ESunmgKufL+Jp63bysdwFC7IQ4uEtE16E5IIBwR4BikFo8OlNZ79Uw8xT6wARXKzFD7REvRwBf0XuGdLzW2uxLmYRTpS5BEEHJNLbkuwz84yRneuATLZsVE3gDQ=="}
```

将产生以下输出：

```log

```

### 注解

`key_id` 是要使用的 RSA 密钥对编号（可在 `resources/rsakeys` 中找到）。

`curr_json` 是接收到 HTTP 响应的正文内容，JSON 中应带有 `content` 和 `sign`。特别注意由于 `curr_json` 必须是单行，如果复制了多行文本粘贴至控制台时需遵循以下原则：

- 如果使用本机上的控制台或 VSCode 集成终端，可直接按 Ctrl + V 粘贴；
- 如果使用 Windows Terminal，按 Ctrl + Alt + V 粘贴；
- 使用 VSCode Remote 或 SSH 等远程终端，请将文本转换为单行后粘贴。

`dcurr` 命令将会反序列化内容为符合 `QueryCurrRegionHttpRsp` 的 JSON，并使用本地的 RSA 公钥（ServerPub-Official）验证签名。因此，如果您遇到提示签名验证失败，则您本地的 RSA 公钥与生成 `query_cur_region` 内容的服务器使用的私钥并不匹配。

如果被加密的 Protobuf 消息不叫做 `QueryCurrRegionHttpRsp`，您可以更改 `#/CurrRegionCmds/BasedProto` 来进行适配。如果被加密的内容为纯文本，您可以



## `gencur` 命令

`gencur` 命令与 `dcurr` 命令类似，其反向使用提供的 `QueryCurrRegionHttpRsp` JSON 形式来生成有效的 `query_cur_region` 内容。

```sh
gencur <key_id> <protobuf_content>
```

### 示例

```sh
gencur 4 { "retcode": 1, "msg": "Not Found version config" }
```

```log

```

### 注解

参数 `<protobuf_content>` 的格式可能与其他方式获取到的不同：其除了首字母以外使用驼峰形式命名，而非在原 proto 中使用的下划线形式。例如 `region_info` 在其中命名为 `regionInfo`。

有关粘贴内容至控制台的注意事项，请参阅 [`dcurr` 命令](#dcurr-命令)。

## `convert` 命令

`convert` 命令可以将输入的内容在 base64 与 HEX（十六进制字符串）中互相转换。

```sh
convert <base64_data/hex_data>
```

### 示例

```sh
convert 01 23 45 67 89 AB CD EF
```

### 注解

输入类型将会自动检测。如果自动检测出现问题，您可以在输入的数据之前带上 `b-`、`h-` 或 `j-` 来强制指定输入的类型是 base64、HEX 还是 JSON。

有关粘贴内容至控制台时的注意事项，请参阅 [`dcurr` 命令](#dcurr-命令)。

## `ec2b` 命令

`ec2b` 命令可以通过 `client_secret_key`（通常也被称为 `dispatchSeed`）来获取 `server_secret_key`（通常也被称为 `dispatchKey`）。

```sh
ec2b get_key <content_bindata(base64/hex)>
```

### 示例

```sh
ec2b get_key RWMyYhAAAAAwWYAsJ4eUPYFvHj1nCbx+AAgAANWiRkndI305DQe3Tn34avtGlc3re7JUufEwL28on+GAskqBGAS1R72TrgFrOhG81Nh5C67LUHCLTvmjRJnVRoRiDpb61h6Pu3U+xJ5tPdPqZitheRzfPWfdHKvtnlKQT2Me38Pc/JnCGbMj9YkuhddbhnaoQwrIg6Q2tnNjo0dvVJFQgY3KT0C2yPIXvlg81K+dX3b5henGdE9TOe4UUrJBSK795z3IihWa9b9RFCIiU6L22U0GgpD1v/nCc0pGAAn8ML77wcoZqcNMGqc/P9XZXDFikZdAAZfiZr4QqnMZmQLxlNufyPkH7IdLOLy8Xd683wEMMA7TmXxH7w6PnXFDY2CiUtGa82g4PqdiSOIegUMBQvjz2FGdytAXcZEjpN2eUuHkD3hdGmvL7HCjHLaCQyZP0FaW2Xn89xC43D7wm86mb/C1CeDpZhQYintVfShb4at0dyzeQEkKN0n1TH+ARPOgAo6Sb58Q/6mFwgE5HNrTD3nQzaH0TlvNAHUjdFTqmX4NYif6p9Rc4rKORGpXHg8zQnOc079WkWexYnI9F4mcWu3ZFpw3TQ5UJ/1Jp121OkpKtBpUB/15mL+gMxWitgPWg9M59P1udabd2Yr160uF4FyZY+K6DNahJrUFFT6C4bwwxnEnBSKhCO9AUZPuzpdM09JWUImk1FeRnSalZ59+G9i8N415UCw8xl/I1nYBTllcFj8pyTYgchoQ7waTjLL9/zFKQMGoLr7JZIVTUmDdMW7zeV/nIs9g6m5Vn/0HuGlxt3wAR/fM8muvKUlMsyF/xwBwQz79HPquZ67MPMyr3PmN5Ec6Amh6bOUBWuAT39WRnZTvBD2xQQ8EGbNI7nN4UTnUScM6qNL10v0dfFwuzYSkhSXPqnj+B+fWBBTVeXGWZqcJZXOaaqdNcPmXF7+/z08505lvBM2gntOYY4N5X/yyDe7zCMQtY+FyBtey1kaFvXvE8KHpET7/MjSi3k8f3loQ2TZ6Et7KRvbj6bXEMR2mwFvw27vUfGe/jun/4q8tPGvpbDGkRxXVEe+bSezTKKxFOSzU+mEB98v5nKHvXj4AxJdpON3HWXnYClqch2PZN3OuDCY8jLCOeKV69kFkQTmqjIaZw7HHrrBu3j3lDQr+Eg+9QtVPNt+zVXNYyZRDkb2T6grh8AecaJI6oN4u+dL8pG3cinQY4wKF3lboJus/UWJ1Pg4795WB4dPwW4ZMB9WHaO4VjlTVSgBzRa3mUqe3xPXF1ei+H/yWeTNYI1BCzggjbhFWtQlFzUz2ONu9j/LJe88/52A8JxVIcM7ZphL+g8AjSfSeLQe3HPcE/wVS2YQLasC3ABsy7Ci34z3GmaaM1u1jGeWVxKTQjARrvYENppGS9nDCP3Q3ONzhtQQDEr0UhDz656seERi8RaWikeKn7Jq/+ave4aLFKNgBNudqNdGka9YrSgkG7UBS20G8rA5s9h1al2+VkoHCYHFHkMu7MUOdhtbXU45sgcqjk6+GHfYKUlWlGgPWJNISb6NCX7VI9UaTcGrrqL9gClF74PMzft7kJE8QDaQIDLEqL4GUhbPDZyzGINhh0RCOf5LbYh6z0hwg3UgCMAJYST9ShzHykplHMKoOeiI4mzTskju5uoiy5r+YOYFdkfJQ5s36rfKKfe7d++XytiCIZftFU+X1tDuXQjySb0j7VIy/nySlGxQqQYbZKe7hG5naYKvgDBaTiEC/b9MAcL+9pJ1n6Gu4D5qfxWXg6QewVIaqogcmbs9GDYdH8gdc/7/V1YU50KMmHAceKUoALYUuG5PcOrf4B46YdK6KLlUv/lm2rGQyB2NDQ9U0WbCpVINOr959w7LUf84D8e4IUuBb4U49g0cb55amglRzlxyNhd0Tf9+k+4kOwzsIt6tHuva5s8BOTcBKA7RfxIvzp2BnfdwImeNqcnir4IhLh3rvQYxUBFsfHAfzGvuVJOmCwOFWNAp2WyCl6KTSMXHHDFvL1J5TKiCajyjaSeuX0qpD5ho2JZfLluaTJzxAlJDXrRn3Va8qA/sx4KfmlbGegbN8uwpejs3EFPe078QkBLpQBKnqMAagIEKZ+gHj84Q/FA89RUKmFqm1m3QkgQ8u4Gwp6RQBQ1IriaFehLIhN+4JEAuq+KrEYw3x2ujN5C8JO/9Tf2/cV1ORm75U8qyqTOFoDBaMaJl/zekUOWIhM78qE65qnLr9DrS8ugAfGeay/IberydEHeE5aD9I5Kl8zUfbgJzIdf1L7MVswD+ZwGdxKz+9nWFS5vPNJ/ZV2rQJSExKuuymka+WohgwoehKnMDL0rjIjEhg5588c4fq9sJxyl2fLe2VHnQttAUKXROviZylC4/VPVHpqszU1X9tlkJXllDk3g09CKPoaHwfynDtdSxMMFVS5yyFjuU+SgzBUejAn4rc5PrJTXQRtd+wzrivFDyzDkL7bqBFqow3EkaPGvPW0L3ypmow22dzQ7jg+SDBhUvqoUqM4TVmWoEou/7bR/221WsVrEESMpFP8LJT2jgDlLfWvN6Wcxn4FRVxPT/M5z484J9nQJXjxQq7xF3WQ213dcgT63pYQVXLXnMR19N4Y1DQWVDnFbGiU96lX9yCIHHhJIQZXtFoVvGj/H2iunRfrpBxGSObEmHXUTr9iAHdV5l6wheBaiaaHDUvS4CF3B5cPJsJLTEnSkyemBeS5TphtiDiR422irfKxjO++a4nKhNLI9kuAayYye5C", "regionCustomConfigEncrypted": "GspvV70MjyzDc/V7Da382vBGoTcjxhSDFR7az7FDzKz4z7TQSCvqpIhCETbrMCIRYUHo9TEYt0aiQ3Sd7OhfEDVNJf8+O8mxw13o1KvVfOOA7rFfJWA5UqKwvjc9FxzYAv6TBlnMQj+XH1jClDT3UK2tvCLI7g5c4bcSKkoeKVxx6+kVxqi8/9iL2WSbSTJ9fdOtxsYuboxL9/xhjpw1MxGyYSjKk2T7EyBoJwLAb2TeG8Bd3gx6wiIOkvBn2GFZf0Gcy1SZOiYWk1GyE05lgeNQsHFgdabXak1mL/2LnxAOw5jEh00Ds9Im08StwZ0TQWmp+GBspnsdixrrkJIUUBVrmC7kS9w6xbIqF6k8O1qjZ0nGdL27QSvTFKtj58Li9N0MVWCtvVpbS25TpnDDcQFireFuAgqtmuLwFNLd4Ap1xMd0lCHmvq8LB2QDwHV/eB8v40BumkqO7qlV8zo0AySmdPb5raGg7X8y2purOdU4WYgPUNMuJ6Au0TbvVU9PZucjDtUppQacPIlqTvAzljwM8ERNVEZOLBlsW4Cz7YlsvmFvmXffYx6uN9pIxaBCLsklZnexwQZpzE7EfCnLaJVqtIfSmNReWREqfWkQRZGTYrs5Vd9su4ofqiY79qCxKedeRrRKJW4zWNky8kPojbjFqCzHKaXa+86gdzi/SqnYRjNHcNFEWObf8DzNGpPHnI7ZqNUnPdhki4u4rxVKhIzQYrsJ9EPJ9sShUr/QOJjfyodR1RDPMik0rIbgNsF/BWVFJfU7Zc+V9mDi2/9fCkKzuliOwcI3cVKGaXxDztrqGRORXUpux+QN+OMmiRCCJWO30rOmUQsH63SurDL0vH+uM7UcflEDt/MxayCOH2QQ0r5T88gmaVrDAz+hhFVGN7WNifom+GiZRN/rkQAuRIuRA8yP6aJRFkaCcjy/xMP4vqyHmAqfrdGJO+/peQ3PG2rlwYvNz+UR0mrKkAUCNjXjVCoesdsJCZz99tjSTvO7WWfKXN2R178+cwCPmiE8Jdm/gYIxCcVy82vxgSutOQITxVepw7/nI87/c6pOAN2jQLdsQStanlcp7t+gXBekvxBP0jR3kAkDvn8p8kiC13iFX3Lm8lY805YAiOScfKSh0/aeoX9g/v+ajo61Y/A=
```

### 注解

通常而言，您可以在 `QueryCurrRegionHttpRsp` 中找到 `client_secret_key`（也就是 `dispatchSeed`），但服务器实际上使用 `dispatchKey` 来作为最初的异或密钥。

有关同时支持多种输入类型的说明，请参阅 [`convert` 命令](#convert-命令)。  
有关粘贴内容作为其参数时的注意事项，请参阅 [`dcurr` 命令](#dcurr-命令)。
