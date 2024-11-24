using Blazored.LocalStorage;
using Shared.Models.DTOs;
using System.Net.Http.Json;

namespace TTTFrontend.Services
{
	public class SceneService
	{
		private readonly HttpClient _httpClient;
		private readonly ISyncLocalStorageService _localStorage;
		private readonly string _baseUrl;

		public SceneService(HttpClient httpClient, ISyncLocalStorageService localStorage, IConfiguration configuration)
		{
			_httpClient = httpClient;
			_localStorage = localStorage;
			_baseUrl = configuration["ApiSettings:BaseUrl"] ?? throw new InvalidOperationException("API Base URL is not configured.");
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

		public async Task<List<SceneGetResponseDTO>> GetScenesAsync()
		{
			var response = await _httpClient.GetFromJsonAsync<List<SceneGetResponseDTO>>("api/scenes");
			return response ?? new List<SceneGetResponseDTO>();
		}
	}
}