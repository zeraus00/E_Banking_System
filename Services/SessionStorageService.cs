using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Exceptions;
using ViewModels;

namespace Services
{
    public class SessionStorageService
    {
        private readonly ProtectedSessionStorage _sessionStorage;

        public SessionStorageService(ProtectedSessionStorage sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }
        public async Task StoreSessionAsync<T>(string sessionScheme, T sessionObject) where T : class
        {
            try
            {
                await _sessionStorage.SetAsync(sessionScheme, sessionObject);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"""

                        EXCEPTION: {ex.Message}  


                    """);
            }
        }
        public async Task<T> FetchSessionAsync<T>(string sessionScheme) where T : class
        {
            return (await _sessionStorage.GetAsync<T>(sessionScheme)).Value ?? throw new SessionNotFoundException(sessionScheme);
        }

        public async Task DeleteSessionAsync(string sessionScheme)
        {
            await _sessionStorage.DeleteAsync(sessionScheme);
        }
    }
}
