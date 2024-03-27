using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using System.Security.Cryptography;
using System.Text;
using Xamarin.Essentials;

namespace Common.Services
{
    public interface IKeyStorageService
    {
        Task SaveKeyAsync(string key);
        Task<string> LoadKeyAsync();
    }
    public class NetKeyStorageService : IKeyStorageService
    {
        private readonly string _keyFilePath;

        public NetKeyStorageService(IConfiguration configuration)
        {
            _keyFilePath = configuration.GetValue<string>("KeyStorage:FilePath") ?? "userKey.dat";
        }

        public async Task SaveKeyAsync(string key)
        {
            byte[] encryptedKey = ProtectedData.Protect(Encoding.UTF8.GetBytes(key), null, DataProtectionScope.CurrentUser);
            await File.WriteAllBytesAsync(_keyFilePath, encryptedKey);
        }

        public async Task<string> LoadKeyAsync()
        {
            if (File.Exists(_keyFilePath))
            {
                byte[] encryptedKey = await File.ReadAllBytesAsync(_keyFilePath);
                byte[] decryptedKey = ProtectedData.Unprotect(encryptedKey, null, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(decryptedKey);
            }
            return null;
        }
    }
    public class MobileKeyStorageService : IKeyStorageService
    {
        public async Task SaveKeyAsync(string key)
        {
            await SecureStorage.SetAsync("user_key", key);
        }

        public async Task<string> LoadKeyAsync()
        {
            return await SecureStorage.GetAsync("user_key");
        }
    }
    public class WebKeyStorageService : IKeyStorageService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly string _storageKey;

        public WebKeyStorageService(IJSRuntime jsRuntime, IConfiguration configuration)
        {
            _jsRuntime = jsRuntime;
            _storageKey = configuration.GetValue<string>("KeyStorage:WebStorageKey") ?? "user_key";
        }

        public async Task SaveKeyAsync(string key)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", _storageKey, key);
        }

        public async Task<string> LoadKeyAsync()
        {
            return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", _storageKey);
        }
    }

}
