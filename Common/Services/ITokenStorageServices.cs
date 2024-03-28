using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using System.Security.Cryptography;
using System.Text;
using Xamarin.Essentials;

namespace Common.Services
{
    public interface ITokenStorageService
    {
        Task SaveTokenAsync(string token);
        Task<string> LoadTokenAsync();
    }

    public class NetTokenStorageService : ITokenStorageService
    {
        private readonly string _tokenFilePath;

        public NetTokenStorageService(IConfiguration configuration)
        {
            _tokenFilePath = configuration.GetValue<string>(
                "TokenStorage:FilePath") ?? "token.dat";
        }

        public async Task SaveTokenAsync(string token)
        {
            byte[] encryptedToken = ProtectedData.Protect(
                Encoding.UTF8.GetBytes(token), null, DataProtectionScope.CurrentUser);
            await File.WriteAllBytesAsync(_tokenFilePath, encryptedToken);
        }

        public async Task<string> LoadTokenAsync()
        {
            if (File.Exists(_tokenFilePath))
            {
                byte[] encryptedToken = await File.ReadAllBytesAsync(_tokenFilePath);
                byte[] decryptedToken = ProtectedData.Unprotect(encryptedToken, null, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(decryptedToken);
            }
            return null;
        }
    }

    public class MobileTokenStorageService : ITokenStorageService
    {
        public async Task SaveTokenAsync(string token)
        {
            await SecureStorage.SetAsync("auth_token", token);
        }

        public async Task<string> LoadTokenAsync()
        {
            return await SecureStorage.GetAsync("auth_token");
        }
    }
    public class WebTokenStorageService : ITokenStorageService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly string _storageKey;

        public WebTokenStorageService(IJSRuntime jsRuntime, IConfiguration configuration)
        {
            _jsRuntime = jsRuntime;
            _storageKey = configuration.GetValue<string>(
                "TokenStorage:WebStorageKey") ?? "auth_token";
        }

        public async Task SaveTokenAsync(string token)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", _storageKey, token);
        }

        public async Task<string> LoadTokenAsync()
        {
            var token = await _jsRuntime.InvokeAsync<string>(
                "localStorage.getItem", _storageKey);
            return token;
        }
    }

}