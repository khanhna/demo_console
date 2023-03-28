using System.Security.Cryptography;
using System.Text;

namespace Demo_Console.Extensions;

public static class StringExtensions
{
    public static string CreateMD5(this string input)
    {
        // Use input string to calculate MD5 hash
        using var md5 = MD5.Create();
        var inputBytes = Encoding.ASCII.GetBytes(input);
        var hashBytes = md5.ComputeHash(inputBytes);

        // Convert the byte array to hexadecimal string
        var sb = new StringBuilder();
        foreach (var t in hashBytes)
        {
            sb.Append(t.ToString("X2"));
        }
        return sb.ToString();
    }
}