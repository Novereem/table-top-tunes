using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Shared.Models.Common;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace TTTFrontend.Services
{
    public class AuthorizationMessageHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;
        private readonly NavigationManager _navigationManager;
        private readonly CustomAuthenticationStateProvider _authenticationStateProvider;

        public AuthorizationMessageHandler(
            ILocalStorageService localStorage,
            NavigationManager navigationManager,
            CustomAuthenticationStateProvider authenticationStateProvider)
        {
            _localStorage = localStorage;
            _navigationManager = navigationManager;
            _authenticationStateProvider = authenticationStateProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _authenticationStateProvider.MarkUserAsLoggedOut();
                await _localStorage.RemoveItemAsync("authToken");
                _navigationManager.NavigateTo("/login", true);
            }

            return response;
        }
    }
}
