using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace MySchool.App.Helpers
{

    public class JwtAuthenticationStateProvider : AuthenticationStateProvider
    {

        private string _token = string.Empty;

        ILocalStorageService LocalStorage;

        public JwtAuthenticationStateProvider(ILocalStorageService localStorage)
        {
            LocalStorage = localStorage;
        }

        public async Task Login(string token)
        {
            // Set the token.
            _token = token;

            // Store the token in local storage.
            await LocalStorage.SetItemAsync("token", token);

            // Notify the application that the authentication state has changed
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task LogoutAsync()
        {
            // Remove the token.
            _token = string.Empty;

            // Remove the token from local storage.
            await LocalStorage.RemoveItemAsync("token");

            // Notify the application that the authentication state has changed
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // Get the token from local storage.
            _token = await LocalStorage.GetItemAsync<string>("token");

            var identity = string.IsNullOrWhiteSpace(_token)
                ? new ClaimsIdentity()
                : new ClaimsIdentity(ParseClaimsFromJwt(_token), "jwt");

            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }

}
