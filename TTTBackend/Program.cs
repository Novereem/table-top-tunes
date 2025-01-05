using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System.Text;
using DotNetEnv;
using TTTBackend.Data;
using TTTBackend.Services.CommonServices;
using Shared.Interfaces.Services;
using TTTBackend.Services;
using Shared.Interfaces.Data;
using Serilog;
using Shared.Interfaces.Services.CommonServices;
using TTTBackend.Services.Helpers;
using Shared.Interfaces.Services.Helpers;
using System.Text.Json;
using Shared.Factories;
using Shared.Models.Common;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    //.WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();


// Load environment variables
Env.Load();

// Services Registration
builder.Services.AddScoped<IPasswordHashingService, PasswordHashingService>();
builder.Services.AddSingleton<IUserClaimsService, UserClaimsService>();

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IAuthenticationData, AuthenticationData>();
builder.Services.AddScoped<ISceneService, SceneService>();
builder.Services.AddScoped<ISceneData, SceneData>();
builder.Services.AddScoped<IAudioService, AudioService>();
builder.Services.AddScoped<IAudioData, AudioData>();

builder.Services.AddScoped<ISceneServiceHelper, SceneServiceHelper>();
builder.Services.AddScoped<IAuthenticationServiceHelper, AuthenticationServiceHelper>();
builder.Services.AddScoped<IAudioServiceHelper, AudioServiceHelper>();

// CORS Policy
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowFrontend", policy =>
	{
		policy.WithOrigins("https://localhost:7040")
			  .AllowAnyMethod()
			  .AllowAnyHeader()
			  .AllowCredentials();
	});
});

// JWT Configuration
var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
if (string.IsNullOrEmpty(secretKey))
{
	throw new InvalidOperationException("JWT secret key is not set in the .env file.");
}

var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "https://localhost:7041";
var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "https://localhost:7040";

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = issuer,
		ValidAudience = audience,
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
	};

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception is SecurityTokenExpiredException)
            {
                // Create the invalid session response
                var response = ApiResponseFactory.CreateInvalidSessionResponse<object>();
                var jsonResponse = JsonSerializer.Serialize(response);

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                return context.Response.WriteAsync(jsonResponse);
            }

            return Task.CompletedTask;
        }
    };
});

// Database Configuration
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
var useInMemory = Environment.GetEnvironmentVariable("USE_IN_MEMORY") == "true";

if (useInMemory)
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseInMemoryDatabase("K6TestDb"));
}
else
{
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("DB_CONNECTION_STRING is not set.");
    }
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 23))));
}

// Add remaining services
builder.Services.AddAuthorization();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Database Initialization
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Only run migrations etc. if *not* using InMemory
    if (!string.Equals(dbContext.Database.ProviderName,
                       "Microsoft.EntityFrameworkCore.InMemory",
                       StringComparison.OrdinalIgnoreCase))
    {
        var connection = dbContext.Database.GetDbConnection();
        var originalConnectionString = connection.ConnectionString;
        var noDbConnectionString = originalConnectionString.Replace("Database=tttdatabase;", "");
        connection.ConnectionString = noDbConnectionString;

        try
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "CREATE DATABASE IF NOT EXISTS `tttdatabase`;";
            command.ExecuteNonQuery();
        }
        finally
        {
            connection.Close();
        }

        connection.ConnectionString = originalConnectionString;
        connection.Open();
        dbContext.Database.Migrate();
        connection.Close();
    }
}

// Middleware
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }