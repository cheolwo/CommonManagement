using Common.ViewModel;

namespace Common.Command.Handlr
{
    public class FileHandler
    {
        private readonly string _fileStoragePath;

        public FileHandler(string fileStoragePath)
        {
            _fileStoragePath = fileStoragePath;
        }

        // 파일 업로드 처리
        public async Task<bool> UploadFileAsync(FileUploadModel fileModel)
        {
            if (fileModel == null || fileModel.FileBytes == null || fileModel.FileBytes.Length == 0)
            {
                throw new ArgumentException("File is empty.", nameof(fileModel));
            }

            try
            {
                // 파일 저장 경로 생성
                string filePath = Path.Combine(_fileStoragePath, fileModel.FileName);

                // 파일이 이미 존재하는지 확인
                if (File.Exists(filePath))
                {
                    throw new InvalidOperationException("File already exists.");
                }

                // 파일 바이트를 사용하여 파일 저장
                await File.WriteAllBytesAsync(filePath, fileModel.FileBytes);

                return true;
            }
            catch (Exception ex)
            {
                // 로깅이나 예외 처리
                Console.WriteLine($"Error uploading file: {ex.Message}");
                return false;
            }
        }

        // 기타 파일 처리 메서드 (예: 파일 다운로드, 파일 삭제 등)는 여기에 구현할 수 있습니다.
    }
}
