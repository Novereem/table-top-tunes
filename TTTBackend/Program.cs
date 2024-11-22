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

var builder = WebApplication.CreateBuilder(args);

// Load environment variables
DotNetEnv.Env.Load();

// Services Registration
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IAuthenticationData, AuthenticationData>();
builder.Services.AddScoped<IPasswordHashingService, PasswordHashingService>();

builder.Services.AddScoped<ISceneService, SceneService>();
builder.Services.AddScoped<ISceneData, SceneData>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserData, UserData>();

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
		ValidIssuer = "https://localhost:7041",
		ValidAudience = "https://localhost:7040",
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
	};
});

// Database Configuration
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
if (string.IsNullOrEmpty(connectionString))
{
	throw new InvalidOperationException("DB_CONNECTION_STRING is not set in the .env file.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 23))));

// Add remaining services
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Database Initialization
using (var scope = app.Services.CreateScope())
{
	var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

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