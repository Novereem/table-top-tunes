using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Shared.Factories;
using Shared.Models.Common;
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

        public AuthenticationService(HttpClient httpClient, 
            ISyncLocalStorageService localStorage, 
            NavigationManager navigationManager, 
            AuthenticationStateProvider authenticationStateProvider, 
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _navigationManager = navigationManager;
            _authenticationStateProvider = authenticationStateProvider as CustomAuthenticationStateProvider
    ?? throw new InvalidOperationException("AuthenticationStateProvider is not of type CustomAuthenticationStateProvider");
            _baseUrl = configuration["ApiSettings:BaseUrl"] ?? throw new InvalidOperationException("API Base URL is not configured.");
        }

        public async Task<ApiResponse<LoginResponseDTO>> LoginAsync(UserLoginDTO loginModel)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/Authentication/login", loginModel);
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponseDTO>>();

                if (apiResponse != null && apiResponse.Success && apiResponse.Data != null)
                {
                    var token = apiResponse.Data.Token;
                    _localStorage.SetItem("authToken", token);
                    _authenticationStateProvider.MarkUserAsAuthenticated(token);
                }

                return apiResponse ?? ApiResponseFactory.CreateFallbackResponse<LoginResponseDTO>();
            }
            catch
            {
                return ApiResponseFactory.CreateErrorResponse<LoginResponseDTO>("An error occured when logging in, please try again later.");
            }
        }

        public async Task<ApiResponse<object>> RegisterAsync(UserRegistrationDTO registerModel)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/Authentication/register", registerModel);
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

                if (apiResponse != null && apiResponse.Success)
                {
                    return apiResponse;
                }

                return ApiResponseFactory.CreateFallbackResponse<object>();
            }
            catch
            {
                return ApiResponseFactory.CreateErrorResponse<object>("An error occured when registering, please try again later.");
            }
        }

        public void Logout()
        {
            _localStorage.RemoveItem("authToken");
            _authenticationStateProvider.MarkUserAsLoggedOut();
            _navigationManager.NavigateTo("/login");
        }
    }
}