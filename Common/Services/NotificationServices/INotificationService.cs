namespace Common.Services.NotificationServices
{
    public interface INotificationService
    {
        Task SendNotificationAsync(string recipient, string message);
    }
    public class EmailNotificationService : INotificationService
    {
        public async Task SendNotificationAsync(string recipient, string message)
        {
            // 이메일 전송 로직 구현
            Console.WriteLine($"Sending Email to {recipient}: {message}");
        }
    }
    public class KakaoNotificationService : INotificationService
    {
        public async Task SendNotificationAsync(string recipient, string message)
        {
            // 카카오톡 메시지 전송 로직 구현
            Console.WriteLine($"Sending Kakao message to {recipient}: {message}");
        }
    }


}
