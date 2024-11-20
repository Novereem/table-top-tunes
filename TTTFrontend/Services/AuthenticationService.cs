using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Shared.Models.DTOs;
using System.Net.Http.Json;

namespace TTTFrontend.Services
{
    public class AuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly ISyncLocalStorageService _localStorage;
        private readonly NavigationManager _navigationManager;
        private readonly CustomAuthenticationStateProvider _authenticationStateProvider;

        public AuthenticationService(HttpClient httpClient, ISyncLocalStorageService localStorage, NavigationManager navigationManager, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _navigationManager = navigationManager;
            _authenticationStateProvider = (CustomAuthenticationStateProvider)authenticationStateProvider;
        }

        public async Task<LoginResponseDTO> Login(UserLoginDTO loginModel)
        {
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7041/api/Authentication/login", loginModel);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadFromJsonAsync<LoginResponseDTO>();
                if (responseData != null && !string.IsNullOrEmpty(responseData.Token))
                {
                    _localStorage.SetItem("authToken", responseData.Token);
                    _authenticationStateProvider.MarkUserAsAuthenticated(responseData.Token);
                    return new LoginResponseDTO
                    {
                        Success = true,
                        Token = responseData.Token
                    };
                }
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            return new LoginResponseDTO
            {
                Success = false,
                ErrorMessage = errorMessage
            };
        }

        public void Logout()
        {
            _localStorage.RemoveItem("authToken");
            _authenticationStateProvider.MarkUserAsLoggedOut();
            _navigationManager.NavigateTo("/login");
        }

        public string GetToken()
        {
            return _localStorage.GetItem<string>("authToken");
        }
    }
}