using Google.Protobuf;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using TextCopy;
using YYHEggEgg.Logger;

namespace YYHEggEgg.EasyProtobuf;

internal class Program
{
    public readonly static string? protobuf_version = Environment.GetEnvironmentVariable("EASYPROTOBUF_PROTOCOL_VERSION");

    static void Main(string[] args)
    {
        Log.Initialize(new LoggerConfig(
            max_Output_Char_Count: -1,
            use_Console_Wrapper: true,
            use_Working_Directory: true,
            debug_LogWriter_AutoFlush: true
        ));
        ConsoleWrapper.ShutDownRequest += (_, _) =>
        {
            Log.Info($"Thanks for using EasyProtobuf! Exiting...");
            Environment.Exit(1);
        };
        Log.Info($"Welcome to easyprotobuf! Protobuf version: {protobuf_version}.");

        Log.Info("Initializing Regex workers, it will take some time...");
        base64worker = new("^.*[g-zG-Z].*$", RegexOptions.Compiled | RegexOptions.Singleline,
            TimeSpan.FromMilliseconds(2000));
        hexworker = new("^([A-Z0-9]|[a-z0-9])*$", RegexOptions.Compiled | RegexOptions.Singleline,
            TimeSpan.FromMilliseconds(3000));

        while (true)
        {
            Log.Info($"----------New Work (Protobuf version: {protobuf_version})----------");
            Log.Info("Type the proto name here:");
            string? protoname = ConsoleWrapper.ReadLine();
            if (protoname == null)
            {
                Log.Erro("Proto type not found! You may check if your protos has 'package' or 'option csharp_namespace' definition.");
                continue;
            }

            Type? prototype = null;
            try
            {
                prototype = Type.GetType(protoname);
            }
            catch (Exception ex)
            {
                Log.Erro(ex.ToString());
            }
            if (prototype == null)
            {
                Log.Erro("Proto type not found!");
                continue;
            }
            Log.Info("Well done! The proto exists.");

            Log.Info("Please type base64 encoded or HEX protobuf bin data (auto detect):");
            Log.Info("You can also paste json data to get its serialized data.");

            Log.Info("If you want to input a long bin data and don't want to waste too much time " +
                "on auto-detecting base64, HEX or json, add '<color=Yellow>b-</color>', '<color=Yellow>h-</color>' or '<color=Yellow>j-</color>' at the start.");
            string? input = ConsoleWrapper.ReadLine();
            try
            {
                byte[]? bytes = null;
                string? extjson = null;
                #region Strictly defined
                if (input.StartsWith("h-"))
                {
                    Log.Info("User defined HEX input!");
                    bytes = Convert.FromHexString(input.Substring(2));
                }
                else if (input.StartsWith("b-"))
                {
                    Log.Info("User defined Base64 input!");
                    bytes = Convert.FromBase64String(input.Substring(2));
                }
                else if (input.StartsWith("j-"))
                {
                    Log.Info("User defined Json input!");
                    extjson = input.Substring(2);
                }
                #endregion
                #region Auto detect
                else
                {
                    string str = input.Trim();
                    if ((str.StartsWith('{') && str.EndsWith('}')) ||
                        (str.StartsWith('[') && str.EndsWith(']')))
                    {
                        Log.Info("Detected Json input!");
                        extjson = str;
                    }
                    else if (isBase64(str))
                    {
                        Log.Info("Detected Base64 input!");
                        bytes = Convert.FromBase64String(str);
                    }
                    else
                    {
                        Log.Info("Detected Hex input!");
                        bytes = Convert.FromHexString(str);
                    }
                }
                #endregion

                string? res = null;
                IMessage? msg = null;
                if (bytes != null)
                {
                    msg = ProtoSerialize.Deserialize(prototype, bytes);
                    if (msg == null)
                    {
                        Log.Info($"Deserialized message = null");
                        Log.Warn("Serialization/Deserialization probably failed!");
                        continue;
                    }
                    res = JsonFormatter.Default.Format(msg);
                    Log.Info($"Converted Json:{Environment.NewLine}{res}");
                }
                else if (extjson != null)
                {
                    msg = ProtoSerialize.Serialize(prototype, extjson);
                    if (msg == null)
                    {
                        Log.Info($"Serialized message = null");
                        Log.Warn("Serialization/Deserialization probably failed!");
                        continue;
                    }
                    res = Convert.ToBase64String(msg.ToByteArray());
                    Log.Info($"Serialized Base64:{Environment.NewLine}{res}");
                }

                if (res == null || msg == null)
                {
                    Log.Warn("Serialization/Deserialization probably failed!");
                }
                else
                {
                    Tools.SetClipBoard(res);

                    var unksize = Tools.GetUnknownFieldsSize(msg, prototype);
                    if (unksize != 0)
                    {
                        Log.Warn($"Message has unknown fields that aren't defined in your proto: {unksize}/{msg.CalculateSize()} bytes. Please go to protobuf decode-raw tools for more information.");
                    }
                }
            }
            catch (Exception ex)
            {
                LogTrace.ErroTrace(ex);
                continue;
            }
        }
    }

#pragma warning disable CS8618
    // 只要字符串包含g/G以后的字母（“以后”遵循字母表顺序，无论大小写，包含g/G）一个及以上，则一定为base64串
    static Regex base64worker;
    // 如果字符串的字母只同时为大写/小写（也就是说，字符串中除去数字以后，剩下的字符要么全为大写要么全为小写），则判定为HEX串
    static Regex hexworker;
#pragma warning restore CS8618

    static bool isBase64(string input)
    {
        if (input.EndsWith('=')) return true;
        if (input.Contains('+') || input.Contains('/')) return true;
        try
        {
            if (base64worker.IsMatch(input)) return true;
        }
        catch (RegexMatchTimeoutException) { }

        try
        {
            if (!hexworker.IsMatch(input)) return true;
        }
        catch (RegexMatchTimeoutException ex)
        {
            throw new RegexMatchTimeoutException("Auto-detect input format timeout of 5s. " +
                "Please explicitly specifiy the input format by adding '<color=Yellow>b-</color>' or '<color=Yellow>h-</color>' at the start and retry.", ex);
        }
        return false;
    }
}
