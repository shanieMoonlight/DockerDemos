using Microsoft.AspNetCore.HttpOverrides;
using Movies.API.Setup;
using Movies.API.Setup.Data;
using Movies.API.Utils;
using Movies.Persistence.Cache.Redis.Setup;
using Movies.Persistence.Ef.Setup;
using Scalar.AspNetCore;

var _builder = WebApplication.CreateBuilder(args);
var _services = _builder.Services;
var _env = _builder.Environment;
var _configuration = _builder.Configuration;
var _logging = _builder.Logging;
var _startupData = new StartupData(_configuration);

//=========================//

// Robust AllowedOrigins parsing:
var _allowedOrigins = _startupData.GetAllowedOrigins()
    .Where(o => !string.IsNullOrWhiteSpace(o))
    .ToArray();

// Configure Forwarded Headers so the app can read X-Forwarded-For / X-Forwarded-Proto
// Plain English: this tells ASP.NET Core to trust headers set by the reverse proxy (nginx)
// so HttpContext.Connection.RemoteIpAddress and the scheme reflect the original client.
_services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    // In production, set KnownProxies or KnownNetworks to limit trusted proxies.
    // For local Docker Compose development we clear them so the middleware will accept forwarded headers from the Docker network.
    //options.KnownNetworks.Clear();
    //options.KnownProxies.Clear();
});

var redisConnection = Environment.GetEnvironmentVariable("ConnectionStrings__Redis");
var dbConnection = Environment.GetEnvironmentVariable("ConnectionStrings__PgDb");

MyConsole.WriteLine($"DOTNET_RUNNING_IN_CONTAINER: {Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER")} ");
MyConsole.WriteLine($"Version: {1}");
MyConsole.WriteLine($"Environment: {_env.EnvironmentName}");
MyConsole.WriteLine($"redisConnection: {redisConnection}");
MyConsole.WriteLine($"dbConnection: {dbConnection}");

MyConsole.WriteLine($"AllowedOrigins (cleaned): {_allowedOrigins.Length}");
for (int i = 0; i < _allowedOrigins.Length; i++)
{
    MyConsole.WriteLine($"AllowedOrigins {i}: {_allowedOrigins[i]}");
}


if (string.IsNullOrWhiteSpace(redisConnection))
{
    var inContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
    redisConnection = inContainer ? "redis-server:6379" : "localhost:6379";
}


if (string.IsNullOrWhiteSpace(dbConnection) && _env.IsDevelopment())
    dbConnection = _startupData.ConnectionStringsSection.GetPgDb();


_services.AddCors(options =>
    options.AddDefaultPolicy(policy => policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins(_allowedOrigins)
        )
);


// Add services to the container.
_services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
_services.AddOpenApi();

_services.AddMoviesApiServices();

_services.AddMoviesPersistenceEf(config =>
{
    config.ConnectionString = dbConnection;
});

_services.AddMoviesPersistenceCache_Redis(config =>
{
    config.RedisConnection = redisConnection;
});


MyConsole.WriteLine($"Environment: {_env.EnvironmentName}");
MyConsole.WriteLine($"SqlConnection: {_startupData.ConnectionStringsSection.GetSqlDb()}");
MyConsole.WriteLine($"PgConnection: {_startupData.ConnectionStringsSection.GetPgDb()}");
MyConsole.WriteLine($"RedisConnection: {_startupData.ConnectionStringsSection.GetRedis()}");


//=========================//
//======= Build Sé ========//
//=========================//

var app = _builder.Build();

// Use forwarded headers middleware early so downstream components see the original client info
app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
app.MapOpenApi();
app.MapScalarApiReference("api/scalar", opts =>
{
    opts
    .WithTitle("Movies API")
    .WithTheme(ScalarTheme.Solarized)
    .WithDefaultHttpClient(ScalarTarget.Node, ScalarClient.HttpClient);
});

// health endpoint used by Docker HEALTHCHECK
app.MapGet("/health", () => Results.Ok("Healthy"));

//app.UseHttpsRedirection();
app.UseCors(); // enable the default policy

app.UseAuthorization();

app.MapControllers();

await app.UseMoviesPersistenceEfAsync();

app.Run();
