using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace Common.Services
{
    public class 수신자DocumentService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public 수신자DocumentService(IConfiguration configuration, 
            IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
        }

        // 1. 중개 관리 서버로부터 암호화된 문서와 세션키 목록을 조회
        public async Task<IEnumerable<EncryptedDocument>> GetEncryptedDocumentsAsync(
            string recipientId)
        {
            var url = $"{_configuration["IntermediaryServer:EncryptedDocumentsUrl"]}?recipientId={recipientId}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<EncryptedDocument>>(content);
        }

        // 2. 비밀키로 암호화된 세션키를 복호화
        public string DecryptSessionKeyWithPrivateKey(
            string encryptedSessionKey, string privateKeyXml)
        {
            byte[] encryptedSessionKeyBytes = Convert.FromBase64String(encryptedSessionKey);

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(privateKeyXml);
                byte[] decryptedSessionKeyBytes = rsa.Decrypt(encryptedSessionKeyBytes, false);
                return Encoding.UTF8.GetString(decryptedSessionKeyBytes);
            }
        }
        // 3. 세션키로 암호화된 문서를 복호화
        public string DecryptDocumentWithSessionKey(
            string encryptedDocument, string sessionKey)
        {
            // Base64 인코딩된 세션키와 문서를 바이트 배열로 변환
            byte[] sessionKeyBytes = Convert.FromBase64String(sessionKey);
            byte[] encryptedDocumentBytes = Convert.FromBase64String(encryptedDocument);

            // AES 복호화 객체 생성
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = sessionKeyBytes;

                // 초기화 벡터(IV) 설정, 예제에서는 간단화를 위해 세션키에서 파생
                // 실제 애플리케이션에서는 보안을 위해 별도로 안전하게 관리되어야 함
                int blockSizeInBytes = aesAlg.BlockSize / 8;
                byte[] iv = new byte[blockSizeInBytes];
                Buffer.BlockCopy(sessionKeyBytes, 0, iv, 0, blockSizeInBytes);
                aesAlg.IV = iv;

                // 복호화기 생성
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // 메모리 스트림을 사용하여 암호화된 데이터 복호화
                using (MemoryStream msDecrypt = new MemoryStream(encryptedDocumentBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // 복호화된 데이터를 문자열로 읽음
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }

    public class EncryptedDocument
    {
        public string DocumentId { get; set; }
        public string EncryptedSessionKey { get; set; }
        public string EncryptedContent { get; set; }
    }
}
