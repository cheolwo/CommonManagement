using System.Security.Cryptography;
using System.Text;

namespace Common.Services
{
    public class HMACService
    {
        // HMAC 생성
        public static string CreateHMAC(string data, string sessionKey)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(sessionKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return Convert.ToBase64String(hash);
        }

        // 세션키를 사용자의 개인키로 암호화 (클라이언트 측 구현 예시)
        public static string EncryptSessionKey(string sessionKey, RSAParameters privateKeyParameters)
        {
            using var rsa = RSA.Create();
            rsa.ImportParameters(privateKeyParameters);
            var encryptedKey = rsa.Encrypt(Encoding.UTF8.GetBytes(sessionKey), 
                RSAEncryptionPadding.OaepSHA256);
            return Convert.ToBase64String(encryptedKey);
        }

        // 세션키를 사용자의 공개키로 복호화 (서버 측 구현 예시)
        public static string DecryptSessionKey(string encryptedSessionKey, RSAParameters publicKeyParameters)
        {
            using var rsa = RSA.Create();
            rsa.ImportParameters(publicKeyParameters);
            var decryptedKey = rsa.Decrypt(Convert.FromBase64String(encryptedSessionKey), 
                RSAEncryptionPadding.OaepSHA256);
            return Encoding.UTF8.GetString(decryptedKey);
        }

        // HMAC 검증
        public static bool VerifyHMAC(string hmac, string data, string sessionKey)
        {
            var computedHmac = CreateHMAC(data, sessionKey);
            return hmac == computedHmac;
        }
        public static RSAParameters StringToRSAParameters(string keyString)
        {
            using var rsa = RSA.Create();
            rsa.FromXmlString(keyString); // FromXmlString 확장 메서드를 사용해야 할 수도 있음
            return rsa.ExportParameters(true);
        }
        public static string RSAParametersToString(RSAParameters parameters)
        {
            using var rsa = RSA.Create();
            rsa.ImportParameters(parameters);
            return rsa.ToXmlString(true); // ToXmlString 확장 메서드를 사용해야 할 수도 있음
        }
        public static string EncryptSessionKeyWithStringKey(string sessionKey, string privateKeyString)
        {
            var privateKeyParams = StringToRSAParameters(privateKeyString);
            using var rsa = RSA.Create();
            rsa.ImportParameters(privateKeyParams);
            var encryptedKey = rsa.Encrypt(Encoding.UTF8.GetBytes(sessionKey), RSAEncryptionPadding.OaepSHA256);
            return Convert.ToBase64String(encryptedKey);
        }

        public static string DecryptSessionKeyWithStringKey(string encryptedSessionKey, string publicKeyString)
        {
            var publicKeyParams = StringToRSAParameters(publicKeyString);
            using var rsa = RSA.Create();
            rsa.ImportParameters(publicKeyParams);
            var decryptedKey = rsa.Decrypt(Convert.FromBase64String(encryptedSessionKey), RSAEncryptionPadding.OaepSHA256);
            return Encoding.UTF8.GetString(decryptedKey);
        }

    }
}
