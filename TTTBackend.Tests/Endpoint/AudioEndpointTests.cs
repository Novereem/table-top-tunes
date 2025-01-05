using Shared.Models.Common;
using Shared.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TTTBackend.Tests.Factories;
using System.Net;
using FluentAssertions;
using Shared.Models;
using Microsoft.Extensions.DependencyInjection;


namespace TTTBackend.Tests.EndpointTests
{
    public class AudioEndpointTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly ApplicationDbContext _db;

        public AudioEndpointTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            var scope = factory.Services.CreateScope();
            _db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        }

        public void Dispose()
        {
            _db.Users.RemoveRange(_db.Users);
            _db.SaveChanges();
        }

        [Fact]
        public async Task CreateAudio_WithValidToken_ShouldReturnSuccess()
        {
            // Arrange: Register and login a user to obtain a token
            var registrationDto = new
            {
                username = "testuser",
                email = "testuser@example.com",
                password = "TestPassword123!"
            };
            var register = await _client.PostAsJsonAsync("/api/Authentication/register", registrationDto);

            var loginDto = new
            {
                username = "testuser",
                password = "TestPassword123!"
            };
            var loginResponse = await _client.PostAsJsonAsync("/api/Authentication/login", loginDto);
            string loginBody = await loginResponse.Content.ReadAsStringAsync();
            Console.WriteLine($"Login Status: {loginResponse.StatusCode}, Body: {loginBody}");
            var apiResponse = await loginResponse.Content.ReadFromJsonAsync<ApiResponse<LoginResponseDTO>>();
            apiResponse.Should().NotBeNull();
            apiResponse!.Success.Should().BeTrue();
            apiResponse.Data.Should().NotBeNull();

            var token = apiResponse.Data!.Token;

            // Add token to the Authorization header
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Prepare audio creation
            var audioDto = new
            {
                name = "Test Audio"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/Audio", audioDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var createdAudio = await response.Content.ReadFromJsonAsync<ApiResponse<AudioFileResponseDTO>>();
            createdAudio.Should().NotBeNull();
            createdAudio!.Data.Name.Should().Be("Test Audio");
        }

        [Fact]
        public async Task GetAllAudio_WithValidToken_ShouldReturnSuccess()
        {
            // Arrange: Register and login a user to obtain a token
            var registrationDto = new
            {
                username = "testuser",
                email = "testuser@example.com",
                password = "TestPassword123!"
            };
            await _client.PostAsJsonAsync("/api/Authentication/register", registrationDto);

            var loginDto = new
            {
                username = "testuser",
                password = "TestPassword123!"
            };
            var loginResponse = await _client.PostAsJsonAsync("/api/Authentication/login", loginDto);
            var apiResponse = await loginResponse.Content.ReadFromJsonAsync<ApiResponse<LoginResponseDTO>>();
            apiResponse.Should().NotBeNull();
            apiResponse!.Success.Should().BeTrue();
            apiResponse.Data.Should().NotBeNull();

            var token = apiResponse.Data!.Token;

            // Add token to the Authorization header
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Create Audio
            var audioDto = new
            {
                name = "Test Audio"
            };
            await _client.PostAsJsonAsync("/api/Audio", audioDto);

            // Act
            var response = await _client.GetAsync("/api/Audio");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var audioListResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<AudioFileListItemDTO>>>();
            audioListResponse.Should().NotBeNull();
            audioListResponse!.Success.Should().BeTrue();
            audioListResponse.Data.Should().NotBeNullOrEmpty();
            audioListResponse.Data!.First().Name.Should().Be("Test Audio");
        }

        [Fact]
        public async Task AssignAudioToScene_WithValidData_ShouldReturnSuccess()
        {
            // Arrange: Register and login a user to obtain a token
            var registrationDto = new
            {
                username = "testuser",
                email = "testuser@example.com",
                password = "TestPassword123!"
            };
            await _client.PostAsJsonAsync("/api/Authentication/register", registrationDto);

            var loginDto = new
            {
                username = "testuser",
                password = "TestPassword123!"
            };
            var loginResponse = await _client.PostAsJsonAsync("/api/Authentication/login", loginDto);
            var apiResponse = await loginResponse.Content.ReadFromJsonAsync<ApiResponse<LoginResponseDTO>>();
            apiResponse.Should().NotBeNull();
            apiResponse!.Success.Should().BeTrue();
            apiResponse.Data.Should().NotBeNull();

            var token = apiResponse.Data!.Token;

            // Add token to the Authorization header
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Create the scene
            var sceneDto = new
            {
                name = "Test Scene"
            };
            var createSceneResponse = await _client.PostAsJsonAsync("/api/Scenes", sceneDto);
            var createdScene = await createSceneResponse.Content.ReadFromJsonAsync<ApiResponse<SceneCreateResponseDTO>>();

            // Create Audio
            var audioDto = new
            {
                name = "Test Audio"
            };
            var createAudioResponse = await _client.PostAsJsonAsync("/api/Audio", audioDto);
            var createdAudio = await createAudioResponse.Content.ReadFromJsonAsync<ApiResponse<AudioFileResponseDTO>>();

            // Prepare assign request
            var assignDto = new
            {
                audioFileId = createdAudio!.Data.Id,
                sceneId = createdScene!.Data.Id,
                type = "MusicTrack"
            };

            // Act
            var response = await _client.PutAsJsonAsync("/api/Audio/assign", assignDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
