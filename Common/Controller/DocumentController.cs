//using System.Security.Cryptography;
//using System.Text;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//[ApiController]
//[Route("api/[controller]")]
//public class DocumentController : ControllerBase
//{
//    private readonly IDocumentService _documentService;
//    private readonly IUserService _userService; // 사용자 공개키를 조회하는 서비스

//    public DocumentController(IDocumentService documentService, IUserService userService)
//    {
//        _documentService = documentService;
//        _userService = userService;
//    }

//    [Authorize]
//    [HttpGet("document")]
//    public async Task<IActionResult> GetDocument(string documentId)
//    {
//        var username = User.Identity.Name;
//        var hasAccess = CheckUserAccessToDocument(username, documentId);

//        if (!hasAccess)
//        {
//            return Forbid();
//        }

//        var document = _documentService.GetDocumentById(documentId);
//        var userPublicKey = _userService.GetUserPublicKey(username);

//        var encryptedContent = EncryptDocumentWithPublicKey(document.Content, userPublicKey);
//        return File(encryptedContent, "application/octet-stream", document.FileName);
//    }

//    private bool CheckUserAccessToDocument(string username, string documentId)
//    {
//        // 접근 권한 확인 로직 구현
//        return true;
//    }

//    private byte[] EncryptDocumentWithPublicKey(byte[] content, string publicKey)
//    {
//        using (var rsa = new RSACryptoServiceProvider())
//        {
//            rsa.FromXmlString(publicKey); // 공개키를 RSA에 설정
//            var encryptedData = rsa.Encrypt(content, false); // 문서 내용 암호화
//            return encryptedData;
//        }
//    }
//}
