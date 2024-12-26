using System.Diagnostics;
using System.Security.Cryptography;

namespace TestWebService;

public class UnitTest1 {
    public static string GenerateSecureKey() {
        using (var rng = new RNGCryptoServiceProvider())
        {
            var randomBytes = new byte[32];
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }

    [Fact]
    public void Test1() {
        
        var key = GenerateSecureKey();

        Debug.WriteLine(key);
    }
}