using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TTTBackend.Tests.Factories;

namespace TTTBackend.Tests.EndpointTests
{
    public class AuthenticationEndpointTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public AuthenticationEndpointTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        //Registering
        [Fact]
        public async Task RegisterUserAsync_ShouldReturnSuccess()
        {
            var registrationDto = new
            {
                username = "testuser",
                email = "testuser@example.com",
                password = "TestPassword123!"
            };

            var response = await _client.PostAsJsonAsync("/api/Authentication/register", registrationDto);
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var responseContent = await response.Content.ReadAsStringAsync();
            responseContent.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldReturnConflictForDuplicateUser()
        {
            // Arrange
            var registrationDto = new
            {
                username = "existinguser1",
                email = "existinguser@example.com",
                password = "TestPassword123!"
            };

            // Register the user once
            await _client.PostAsJsonAsync("/api/Authentication/register", registrationDto);

            // Act: Try registering the same user again
            var response = await _client.PostAsJsonAsync("/api/Authentication/register", registrationDto);

            // Assert: Expect conflict status code (409)
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldReturnBadRequestForInvalidData()
        {
            // Arrange
            var invalidDto = new
            {
                username = "", // Invalid username
                email = "not-an-email", // Invalid email format
                password = "password"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/Authentication/register", invalidDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }


        //Logging in
        [Fact]
        public async Task Login_WithValidCredentials_ShouldReturnSuccess()
        {
            // Arrange
            var registrationDto = new
            {
                username = "testuser",
                email = "testuser@example.com",
                password = "TestPassword123!"
            };

            // Register the user first
            await _client.PostAsJsonAsync("/api/Authentication/register", registrationDto);

            var loginDto = new
            {
                username = "testuser",
                password = "TestPassword123!"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/Authentication/login", loginDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Optional: Validate response content (e.g., JWT token or success message)
            var responseContent = await response.Content.ReadAsStringAsync();
            responseContent.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Login_WithInvalidPassword_ShouldReturnUnauthorized()
        {
            // Arrange
            var registrationDto = new
            {
                username = "testuser",
                email = "testuser@example.com",
                password = "TestPassword123!"
            };

            // Register the user first
            await _client.PostAsJsonAsync("/api/Authentication/register", registrationDto);

            var loginDto = new
            {
                username = "testuser",
                password = "WrongPassword"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/Authentication/login", loginDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Login_WithNonExistentUser_ShouldReturnUnauthorized()
        {
            // Arrange
            var loginDto = new
            {
                username = "nonexistentuser",
                password = "AnyPassword"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/Authentication/login", loginDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Login_WithMissingFields_ShouldReturnBadRequest()
        {
            // Arrange
            var loginDto = new
            {
                username = "testuser"
                // Missing password field
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/Authentication/login", loginDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
