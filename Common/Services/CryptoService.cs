using System.Security.Cryptography;
using System.Text;

namespace 계정Common.Services
{
    public static class CryptoService
    {
        // RSA 키 쌍 생성 메서드
        public static (string PublicKey, string PrivateKey) GenerateKeyPair()
        {
            using (var rsa = new RSACryptoServiceProvider(2048)) // 2048 비트 키 크기
            {
                rsa.PersistKeyInCsp = false; // 키를 CSP(암호 서비스 공급자)에 유지하지 않음

                var publicKey = rsa.ToXmlString(false); // 공개키만 추출
                var privateKey = rsa.ToXmlString(true); // 비밀키 포함하여 추출

                return (PublicKey: publicKey, PrivateKey: privateKey);
            }
        }
        // 일회용 세션키 생성
        public static string CreateSessionKey()
        {
            byte[] key = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(key);
            }
            return Convert.ToBase64String(key);
        }

        // 문서를 세션키로 암호화하는 메서드
        public static string EncryptDocumentWithSessionKey(string document, string sessionKey)
        {
            byte[] sessionKeyBytes = Convert.FromBase64String(sessionKey);
            byte[] documentBytes = Encoding.UTF8.GetBytes(document);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = sessionKeyBytes;
                int blockSizeInBytes = aesAlg.BlockSize / 8;
                byte[] iv = new byte[blockSizeInBytes];
                Buffer.BlockCopy(sessionKeyBytes, 0, iv, 0, blockSizeInBytes);
                aesAlg.IV = iv;
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(documentBytes, 0, documentBytes.Length);
                        csEncrypt.FlushFinalBlock();
                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
        }

        // 수신자의 공개키로 세션키를 암호화하는 메서드
        public static string EncryptSessionKeyWithRecipientPublicKey(string sessionKey, string publicKeyXml)
        {
            byte[] sessionKeyBytes = Encoding.UTF8.GetBytes(sessionKey);

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKeyXml);
                byte[] encryptedSessionKey = rsa.Encrypt(sessionKeyBytes, false);
                return Convert.ToBase64String(encryptedSessionKey);
            }
        }
    }

}
