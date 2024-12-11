using Shared.Enums;
using Shared.Factories;
using Shared.Models.Common;
using Shared.Models.DTOs;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

public class UserSoundsService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly JsonSerializerOptions _jsonOptions;

    public UserSoundsService(HttpClient httpClient, IConfiguration configuration, JsonSerializerOptions jsonOptions)
    {
        _httpClient = httpClient;
        _baseUrl = configuration["ApiSettings:BaseUrl"] ?? throw new InvalidOperationException("API Base URL is not configured.");
        _jsonOptions = jsonOptions;
    }

    public async Task<ApiResponse<List<AudioFileListItemDTO>>> GetUserSoundsAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<AudioFileListItemDTO>>>($"{_baseUrl}/audio", _jsonOptions);
            return response ?? ApiResponseFactory.CreateFallbackResponse<List<AudioFileListItemDTO>>();
        }
        catch
        {
            return ApiResponseFactory.CreateErrorResponse<List<AudioFileListItemDTO>>("An error occurred getting your sounds, please try again later.");
        }
    }

    public async Task<ApiResponse<object>> CreateAudioFileAsync(string audioFileName)
    {
        try
        {
            var audioFileDTO = new AudioFileCreateDTO { Name = audioFileName };
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/audio", audioFileDTO);
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

            return apiResponse ?? ApiResponseFactory.CreateFallbackResponse<object>();
        }
        catch
        {
            return ApiResponseFactory.CreateErrorResponse<object>("An error occurred creating the audio file. Please try again later.");
        }
    }

    public async Task<ApiResponse<AudioFileResponseDTO>> AddSoundToScene(Guid soundId, Guid sceneId, AudioType audioType)
    {
        try
        {
            var audioFileAssignDTO = new AudioFileAssignDTO { AudioFileId = soundId, SceneId = sceneId, Type = audioType };
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/audio/assign", audioFileAssignDTO);
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<AudioFileResponseDTO>>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            });

            return apiResponse ?? ApiResponseFactory.CreateFallbackResponse<AudioFileResponseDTO>();
        }
        catch
        {
            return ApiResponseFactory.CreateErrorResponse<AudioFileResponseDTO>("An error occurred getting the scene, please try again later.");
        }
    }
}