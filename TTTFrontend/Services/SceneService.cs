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

		public SceneService(HttpClient httpClient, ISyncLocalStorageService localStorage)
		{
			_httpClient = httpClient;
			_baseUrl = _httpClient.BaseAddress?.ToString() ?? throw new InvalidOperationException("API Base URL is not configured.");
			_localStorage = localStorage;
		}

		public async Task<bool> CreateSceneAsync(string sceneName)
		{
			try
			{
				var sceneDTO = new SceneCreateDTO
				{
					Name = sceneName
				};

				var response = await _httpClient.PostAsJsonAsync("api/scenes", sceneDTO);

				return response.IsSuccessStatusCode;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error creating scene: {ex.Message}");
				return false;
			}
		}
	}
}