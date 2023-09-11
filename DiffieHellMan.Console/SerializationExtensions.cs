using System.Runtime.Serialization.Formatters.Binary;

namespace DiffieHellMan.Console;

public static class SerializationExtensions
{
    public static byte[] Serialize<T>(this T m)
    {
        var ms = new MemoryStream();
        try
        {
            var formatter = new BinaryFormatter();
#pragma warning disable SYSLIB0011
            formatter.Serialize(ms, m);
#pragma warning restore SYSLIB0011
            return ms.ToArray();
        }
        finally
        {
            ms.Close();
        }
    }
    public static T Deserialize<T>(this byte[] byteArray)
    {
        var ms = new MemoryStream(byteArray);
        try
        {
            var formatter = new BinaryFormatter();
#pragma warning disable SYSLIB0011
            return (T)formatter.Deserialize(ms);
#pragma warning restore SYSLIB0011
        }
        finally
        {
            ms.Close();
        }
    }
}