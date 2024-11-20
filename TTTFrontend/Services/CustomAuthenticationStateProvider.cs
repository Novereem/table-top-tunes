using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace TTTFrontend.Services
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ISyncLocalStorageService _localStorage;

        public CustomAuthenticationStateProvider(ISyncLocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = _localStorage.GetItem<string>("authToken");
            if (string.IsNullOrEmpty(token) || IsTokenExpired(token))
            {
                return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
            }

            var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
            var user = new ClaimsPrincipal(identity);
            return Task.FromResult(new AuthenticationState(user));
        }

        public bool IsTokenExpired(string token)
        {
            try
            {
                var payload = token.Split('.')[1];
                var jsonBytes = Convert.FromBase64String(PadBase64(payload));
                var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

                if (keyValuePairs != null && keyValuePairs.ContainsKey("exp"))
                {
                    if (long.TryParse(keyValuePairs["exp"].ToString(), out long exp))
                    {
                        var expirationTime = DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;
                        return expirationTime < DateTime.UtcNow;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing JWT token expiration: {ex.Message}");
                return true;
            }
        }

        private string PadBase64(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
                case 0: break;
            }
            return base64;
        }

        public void MarkUserAsAuthenticated(string token)
        {
            var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public void MarkUserAsLoggedOut()
        {
            _localStorage.RemoveItem("authToken");
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()))));
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = Convert.FromBase64String(PadBase64(payload));
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
        }
    }
}
