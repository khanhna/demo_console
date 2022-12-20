using System.Text;
using SHA3.Net;

namespace Demo_Console.Playground;

public class Playground
{
    public static void LittleChallengeSympli()
    {
        var placeholder =
            "cFZcRVE3MixXEmcZYnJeV1MzUEEzLSRGL1dWNmVgW0YQRiZiWCBQX1NcF10wFhMYZghdMRNQMmRQSmJDLVxdFjFYZVoiQFERVxAlX1ckUCEVQ15FLGETeC5SWV1RIh5hDB4cEWAoIyRHVmZLJ19VGVUvEVArICxeZkxXZWgZVVtWXS4jUm9fWkRXXXQ3Tl9JKi8cJVxUaCVMGTVeMFkYTypCNxIgWVJXU1VmU1ktXCdQFFtYLyQUFGZwVUAfClgzUVIeEXwrKCIUcSpZIVoealkgXVlqYS1dMRhVJCtAFEBCVyIwFiRNUBgYdlg3WBJJKiNTNVYZKiFNGS9SZFpWWTIXLV00FlheWFdmWUViQStaXxdIKzQUTSkTV0JRJVJhQl9bQhA0NCpWXyNVbBF8QBQiQ1QlKixcIRhMLChcFEdfWTE2T2xKVkBdWRQpXlxKamZWL1cZPytMGSBSJUUYWyAXf3Z8";
        using var shaAlg = Sha3.Sha3384();
        var hash = shaAlg.ComputeHash(Encoding.UTF8.GetBytes("SympliForYou"));
        var hashStr = ConvertByteToString(hash);
        Console.WriteLine(Dec(placeholder, hashStr));
    }
    
    public static string ConvertByteToString(byte[] array)
    {
        StringBuilder builder = new();
        for (var i = 0; i < array.Length; i++)
        {
            builder.Append(array[i].ToString("x2"));
            // if ((i % 4) == 3) builder.Append(" ");
        }

        return builder.ToString();
    }
    
    public static string StringXor(string firstStr, string secondStr)
    {
        var firstStrChars = firstStr.ToCharArray();
        var secStrChars = secondStr.ToCharArray();
        for (var i = 0; i < firstStr.Length; i++)
        {
            firstStrChars[i] ^= secStrChars[i];
        }
        return new string(firstStrChars);
    }
    
    public static string xorIt(string key, string input)
    {
        StringBuilder sb = new();
        for(var i=0; i < input.Length; i++)
            sb.Append((char)(input[i] ^ key[(i % key.Length)]));
        return sb.ToString();
    }
    
    static string Enc(string plaintext, string pad)
    {
        var data = Encoding.UTF8.GetBytes(plaintext);
        var key = Encoding.UTF8.GetBytes(pad);

        return Convert.ToBase64String(data.Select((b, i) => (byte)(b ^ key[i % key.Length])).ToArray());
    }

    static string Dec(string enctext, string pad)
    {
        var data = Convert.FromBase64String(enctext);
        var key = Encoding.UTF8.GetBytes(pad);

        return Encoding.UTF8.GetString(data.Select((b, i) => (byte)(b ^ key[i % key.Length])).ToArray());
    }
}