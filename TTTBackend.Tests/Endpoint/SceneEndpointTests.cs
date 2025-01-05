using System.Net.Http.Headers;
using System.Net.Http.Json;
using TTTBackend.Tests.Factories;
using System.Net;
using FluentAssertions;
using Shared.Models.Common;
using Shared.Models.DTOs;
using Shared.Models;
using Microsoft.Extensions.DependencyInjection;

namespace TTTBackend.Tests.EndpointTests
{
    public class SceneEndpointTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly ApplicationDbContext _db;

        public SceneEndpointTests(CustomWebApplicationFactory factory)
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
        public async Task CreateScene_WithValidToken_ShouldReturnSuccess()
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
            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Deserialize the response to extract the token
            var apiResponse = await loginResponse.Content.ReadFromJsonAsync<ApiResponse<LoginResponseDTO>>();
            apiResponse.Should().NotBeNull();
            apiResponse!.Success.Should().BeTrue();
            apiResponse.Data.Should().NotBeNull();
            var token = apiResponse.Data!.Token;

            // Add the token to the Authorization header
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Prepare the scene to create
            var sceneDto = new
            {
                name = "Test Scene"
            };

            // Act: Create the scene
            var response = await _client.PostAsJsonAsync("/api/Scenes", sceneDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var responseContent = await response.Content.ReadAsStringAsync();
            responseContent.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task CreateScene_WithInvalidToken_ShouldReturnUnauthorized()
        {
            // Arrange: Set an invalid token
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "invalid-token");

            var sceneDto = new
            {
                name = "Test Scene"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/Scenes", sceneDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetAllScenes_WithValidToken_ShouldReturnSuccess()
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

            // Prepare: Create a scene
            var sceneDto = new
            {
                name = "Test Scene"
            };
            var createResponse = await _client.PostAsJsonAsync("/api/Scenes", sceneDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            // Act: Retrieve all scenes
            var response = await _client.GetAsync("/api/Scenes");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Assert: Validate response structure and data
            var scenesResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<Scene>>>();
            scenesResponse.Should().NotBeNull();
            scenesResponse!.Success.Should().BeTrue();
            scenesResponse.Data.Should().NotBeNullOrEmpty();

            var createdScene = scenesResponse.Data!.FirstOrDefault(s => s.Name == "Test Scene");
            createdScene.Should().NotBeNull();
            createdScene!.Name.Should().Be("Test Scene");
        }

        [Fact]
        public async Task GetAllScenes_WithInvalidToken_ShouldReturnUnauthorized()
        {
            // Arrange: Set an invalid token
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "invalid-token");

            // Act
            var response = await _client.GetAsync("/api/Scenes");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetSceneById_WithInvalidId_ShouldReturnNotFound()
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

            // Act
            var response = await _client.GetAsync($"/api/Scenes/{Guid.NewGuid()}"); // Use a random GUID

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}