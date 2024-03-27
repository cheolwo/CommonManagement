using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO
{
    // 수정된 CreateEncryptDocumentDto 클래스 예시 (추가 필드 포함)
    public class CreateEncryptDocumentDto
    {
        public CreateEncryptDocumentDto(string senderId, string recipientId, string encryptedDocument, string encryptedSessionKey, 
            string hMAC, string encryptedHMACSessionKey)
        {
            EncryptedDocument = encryptedDocument;
            EncryptedSessionKey = encryptedSessionKey;
            HMAC = hMAC;
            EncryptedHMACSessionKey = encryptedHMACSessionKey;
            RecipientId = recipientId;
            SenderId = senderId;
        }
        public string SenderId { get; set; }    
        public string RecipientId { get; set; }
        public string EncryptedDocument { get; set; }
        public string EncryptedSessionKey { get; set; }
        public string HMAC { get; set; } // HMAC 값 추가
        public string EncryptedHMACSessionKey { get; set; } // HMAC으로 암호화된 세션키 추가

    }

}
