using Google.Protobuf;
using System.Diagnostics;
using System.Reflection;

namespace YYHEggEgg.EasyProtobuf;

#pragma warning disable CS8600, CS8602, CS8604
public class ProtoSerialize
{
    public static IMessage? Deserialize(Type prototype, byte[] protobin)
    {
        var parser_pro = prototype.GetProperty("Parser", BindingFlags.Static | BindingFlags.Public);
        var parse_get = parser_pro.GetGetMethod();
        var parser = parse_get.Invoke(null, null);

        Type parsertype = typeof(MessageParser<>).MakeGenericType(prototype);
        var parsefrom_method = parsertype.GetMethod("ParseFrom",
            BindingFlags.Instance | BindingFlags.Public,
            new Type[] { typeof(byte[]) });

        Debug.Assert(protobin.GetType() == typeof(byte[]));

        return (IMessage?)parsefrom_method.Invoke(parser, new object[] { protobin });
    }

    public static IMessage? Deserialize(string protoname, byte[] protobin)
    {
        Type prototype = Type.GetType(protoname);
        return Deserialize(prototype, protobin);
    }

    public static IMessage? Serialize(Type prototype, string protojson)
    {
        var parser_pro = prototype.GetProperty("Parser", BindingFlags.Static | BindingFlags.Public);
        var parse_get = parser_pro.GetGetMethod();
        var parser = parse_get.Invoke(null, null);

        Type parsertype = typeof(MessageParser<>).MakeGenericType(prototype);
        var parsefrom_method = parsertype.GetMethod("ParseJson",
            BindingFlags.Instance | BindingFlags.Public,
            new Type[] { typeof(string) });

        return (IMessage?)parsefrom_method.Invoke(parser, new object[] { protojson });
    }

    public static IMessage? Serialize(string protoname, string protojson)
    {
        Type prototype = Type.GetType(protoname);
        return Serialize(prototype, protojson);
    }
}
#pragma warning restore CS8600, CS8602, CS8604
