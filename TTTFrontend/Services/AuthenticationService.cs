using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Shared.Models.DTOs;
using System.Net.Http.Json;

namespace TTTFrontend.Services
{
    public class AuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly ISyncLocalStorageService _localStorage;
        private readonly NavigationManager _navigationManager;

        public AuthenticationService(HttpClient httpClient, ISyncLocalStorageService localStorage, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _navigationManager = navigationManager;
        }

        public async Task<LoginResponseDTO> Login(UserLoginDTO loginModel)
        {
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7041/api/Authentication/login", loginModel);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadFromJsonAsync<LoginResponseDTO>();
                if (responseData != null && !string.IsNullOrEmpty(responseData.Token))
                {
                    // Return success response with token
                    return new LoginResponseDTO
                    {
                        Success = true,
                        Token = responseData.Token
                    };
                }
            }

            // If the login was unsuccessful, capture the error message
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
            _navigationManager.NavigateTo("/login");
        }

        public string GetToken()
        {
            return _localStorage.GetItem<string>("authToken");
        }
    }
}