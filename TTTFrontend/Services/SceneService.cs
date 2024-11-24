using Blazored.LocalStorage;
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

		public async Task<bool> CreateSceneAsync(string sceneName)
		{
			try
			{
				var sceneDTO = new SceneCreateDTO
				{
					Name = sceneName
				};

				var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/scenes", sceneDTO);

				Console.WriteLine($"Response Status: {response.StatusCode}");
				Console.WriteLine($"Is Success: {response.IsSuccessStatusCode}");

				return response.IsSuccessStatusCode;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error creating scene: {ex.Message}");
				return false;
			}
		}

        public async Task<List<SceneListItemDTO>> GetScenesListAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<SceneListItemDTO>>("api/scenes");
            return response ?? new List<SceneListItemDTO>();
        }

        public async Task<SceneGetResponseDTO> GetSceneAsync(Guid sceneId)
        {
            var response = await _httpClient.GetFromJsonAsync<SceneGetResponseDTO>($"api/scenes/{sceneId}", _jsonOptions);
			Console.WriteLine(response.Name);
            return response;
        }
    }
}