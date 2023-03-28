using System.Security.Cryptography;
using System.Text;

namespace Althea.Infrastructure.Helpers;

public static class CryptoHelper
{
    private const char SEPARATE = '_';

    public static string Md5(char separate = SEPARATE, params string[] args)
    {
        var md5      = MD5.Create();
        var str      = string.Join(SEPARATE, args);
        var bytes    = Encoding.UTF8.GetBytes(str);
        var newBytes = md5.ComputeHash(bytes);

        return Convert.ToHexString(newBytes);
    }
}
