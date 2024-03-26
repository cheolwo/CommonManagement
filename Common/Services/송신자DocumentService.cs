using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace Common.Actor.APIService
{
    public class DocumentUploadModel
    {
        public string EncryptedDocument { get; set; }
        public string EncryptedSessionKey { get; set; }
        public string RecipientId { get; set; }
        public string DocumentId { get; set; }
    }
    public class 송신자DocumentAPIService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public 송신자DocumentAPIService(IConfiguration configuration, 
            IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
        }

        // 1. 중개서버로부터 일회용 세션키를 요청하는 메서드
        public async Task<string> RequestSessionKeyFromIntermediaryServerAsync()
        {
            var url = _configuration["IntermediaryServer:SessionKeyUrl"];
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var sessionKey = await response.Content.ReadAsStringAsync();
            return sessionKey;
        }

        // 2. 문서를 세션키로 암호화하는 메서드
        public string EncryptDocumentWithSessionKey(string document, string sessionKey)
        {
            // Base64 인코딩된 세션키를 바이트 배열로 변환
            byte[] sessionKeyBytes = Convert.FromBase64String(sessionKey);

            // 문서 문자열을 바이트 배열로 변환
            byte[] documentBytes = Encoding.UTF8.GetBytes(document);

            // AES 암호화 객체 생성
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = sessionKeyBytes;

                // 초기화 벡터(IV) 설정, 여기서는 간단히 세션키에서 파생
                // 실제로는 무작위로 생성 및 별도로 관리
                int blockSizeInBytes = aesAlg.BlockSize / 8;
                byte[] iv = new byte[blockSizeInBytes];
                Buffer.BlockCopy(sessionKeyBytes, 0, iv, 0, blockSizeInBytes);
                aesAlg.IV = iv;

                // 암호화기 생성
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // 메모리 스트림을 사용하여 데이터 암호화
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(documentBytes, 0, documentBytes.Length);
                        csEncrypt.FlushFinalBlock();

                        // 암호화된 데이터를 Base64 문자열로 변환하여 반환
                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
        }

        // 3. 계정관리서버로부터 수신자의 공개키를 요청하는 메서드
        public async Task<string> RequestRecipientPublicKeyFromAccountManagementServerAsync(
            string recipientId)
        {
            var url = $"{_configuration["AccountManagementServer:Url"]}/{recipientId}/publickey";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var publicKey = await response.Content.ReadAsStringAsync();
            return publicKey;
        }

        // 4. 수신자의 공개키로 세션키를 암호화하는 메서드
        public string EncryptSessionKeyWithRecipientPublicKey(
            string sessionKey, string publicKeyXml)
        {
            // AES 세션키를 바이트 배열로 변환
            byte[] sessionKeyBytes = Encoding.UTF8.GetBytes(sessionKey);

            // RSA를 사용하여 공개키로 세션키 암호화
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                try
                {
                    // 수신자의 공개키로 RSA 개체 초기화
                    rsa.FromXmlString(publicKeyXml);

                    // 공개키로 세션키 암호화
                    byte[] encryptedSessionKey = rsa.Encrypt(sessionKeyBytes, false);

                    // 암호화된 세션키를 Base64 문자열로 반환
                    return Convert.ToBase64String(encryptedSessionKey);
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }

        // 5. 문서 및 암호화된 세션키를 중개관리서버에 전송하는 메서드
        public async Task SendEncryptedDocumentAndSessionKeyToIntermediaryServerAsync(
            string encryptedDocument, string encryptedSessionKey)
        {
            var url = _configuration["IntermediaryServer:DocumentUploadUrl"];
            var content = new StringContent(JsonConvert.SerializeObject(new { encryptedDocument, encryptedSessionKey }), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
        }
    }
}
