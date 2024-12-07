using Blazored.LocalStorage;
using Shared.Factories;
using Shared.Models.Common;
using Shared.Models.DTOs;
using System.Net.Http.Json;
using System.Text.Json;

namespace TTTFrontend.Services
{
	public class SceneService
	{
		private readonly HttpClient _httpClient;
		private readonly ISyncLocalStorageService _localStorage;
		private readonly string _baseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public SceneService(HttpClient httpClient, ISyncLocalStorageService localStorage, IConfiguration configuration, JsonSerializerOptions jsonOptions)
		{
			_httpClient = httpClient;
			_localStorage = localStorage;
			_baseUrl = configuration["ApiSettings:BaseUrl"] ?? throw new InvalidOperationException("API Base URL is not configured.");
            _jsonOptions = jsonOptions;
        }

        public async Task<ApiResponse<object>> CreateSceneAsync(string sceneName)
        {
            try
            {
                var sceneDTO = new SceneCreateDTO { Name = sceneName };
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/scenes", sceneDTO);
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

                return apiResponse ?? ApiResponseFactory.CreateFallbackResponse<object>();
            }
            catch
            {
                return ApiResponseFactory.CreateErrorResponse<object>($"An error occurred creating a scene, please try again later.");
            }
        }

        public async Task<ApiResponse<List<SceneListItemDTO>>> GetScenesListAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<SceneListItemDTO>>>($"{_baseUrl}/scenes", _jsonOptions);
                return response ?? ApiResponseFactory.CreateFallbackResponse<List<SceneListItemDTO>>();
            }
            catch
            {
                return ApiResponseFactory.CreateErrorResponse<List<SceneListItemDTO>>("An error occurred getting your scenes, please try again later.");
            }
        }

        public async Task<ApiResponse<SceneGetResponseDTO>> GetSceneAsync(Guid sceneId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<SceneGetResponseDTO>>($"{_baseUrl}/scenes/{sceneId}", _jsonOptions);
                return response ?? ApiResponseFactory.CreateFallbackResponse<SceneGetResponseDTO>();
            }
            catch
            {
                return ApiResponseFactory.CreateErrorResponse<SceneGetResponseDTO>("An error occurred getting the scene, please try again later.");
            }
        }
    }
}