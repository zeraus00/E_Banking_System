using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Exceptions;
using ViewModels.Sessions;

namespace Services.SessionsManagement
{
    public class SessionStorageService
    {
        private readonly ProtectedSessionStorage _sessionStorage;

        public SessionStorageService(ProtectedSessionStorage sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }
        public async Task StoreSessionAsync<T>(string sessionScheme, T sessionObject) where T : SessionModel
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
        public async Task<T> FetchSessionAsync<T>(string sessionScheme) where T : SessionModel
        {
            var result = await _sessionStorage.GetAsync<T>(sessionScheme);
            T? sessionObject = result.Success ? result.Value : throw new SessionNotFoundException(sessionScheme);
            return sessionObject ?? throw new SessionNotFoundException(sessionScheme);
        }

        public async Task DeleteSessionAsync(string sessionScheme)
        {
            await _sessionStorage.DeleteAsync(sessionScheme);
        }
    }
}
