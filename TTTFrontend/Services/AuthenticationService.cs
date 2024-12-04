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
        private readonly string _baseUrl;

        public AuthenticationService(HttpClient httpClient, ISyncLocalStorageService localStorage, NavigationManager navigationManager, AuthenticationStateProvider authenticationStateProvider, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _navigationManager = navigationManager;
            _authenticationStateProvider = authenticationStateProvider as CustomAuthenticationStateProvider
    ?? throw new InvalidOperationException("AuthenticationStateProvider is not of type CustomAuthenticationStateProvider");
            _baseUrl = configuration["ApiSettings:BaseUrl"] ?? throw new InvalidOperationException("API Base URL is not configured.");
        }

        public async Task<LoginResponseDTO> Login(UserLoginDTO loginModel)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/Authentication/login", loginModel);

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

            var errorResponse = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            var errorMessage = errorResponse != null && errorResponse.ContainsKey("message")
                ? errorResponse["message"]
                : "An unknown error occurred.";

            return new LoginResponseDTO
            {
                Success = false,
                ErrorMessage = errorMessage
            };
        }

        public async Task<RegisterResponseDTO> Register(UserRegistrationDTO registerModel)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/Authentication/register", registerModel);

            if (response.IsSuccessStatusCode)
            {
                return new RegisterResponseDTO
                {
                    Success = true
                };
            }

            var errorResponse = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            var errorMessage = errorResponse != null && errorResponse.ContainsKey("Message")
                ? errorResponse["Message"]
                : "An unknown error occurred.";

            return new RegisterResponseDTO
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