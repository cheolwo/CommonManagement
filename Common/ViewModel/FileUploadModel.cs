using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.ViewModel
{
    public class FileUploadModel
    {
        // 파일의 바이트 배열
        public byte[] FileBytes { get; set; }

        // 파일 이름
        public string FileName { get; set; }

        // MIME 타입 (예: "image/jpeg", "application/pdf")
        public string ContentType { get; set; }

        // (선택사항) 파일을 업로드할 때 추가적인 메타데이터 또는 설명
        public string Description { get; set; }

        // (선택사항) 파일 저장에 사용될 다른 속성들
        // 예를 들어, 사용자 ID나 파일과 연관된 엔티티의 ID 등
        public string UserId { get; set; }
        public int RelatedEntityId { get; set; }

        public FileUploadModel(byte[] fileBytes, string fileName, string contentType)
        {
            FileBytes = fileBytes;
            FileName = fileName;
            ContentType = contentType;
        }

        // 필요에 따라 다른 생성자나 메서드를 추가할 수 있습니다.
    }

}
