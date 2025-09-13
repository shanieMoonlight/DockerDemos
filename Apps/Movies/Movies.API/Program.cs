using Movies.API.Setup;
using Movies.API.Setup.Data;
using Movies.API.Utils;
using Movies.Persistence.Cache.Redis.Setup;
using Movies.Persistence.Ef.Setup;
using Scalar.AspNetCore;
using System.Diagnostics;

var _builder = WebApplication.CreateBuilder(args);
var _services = _builder.Services;
var _env = _builder.Environment;
var _configuration = _builder.Configuration;
var _logging = _builder.Logging;
var _startupData = new StartupData(_configuration);

//=========================//


foreach (var origin in _startupData.GetAllowedOrigins())
{
    Debug.WriteLine(origin);
}


var redisConnection = Environment.GetEnvironmentVariable("ConnectionStrings__Redis");
var dbConnection = Environment.GetEnvironmentVariable("ConnectionStrings__PgDb");

MyConsole.WriteLine($"Version: {1}");
MyConsole.WriteLine($"Environment: {_env.EnvironmentName}");
MyConsole.WriteLine($"redisConnection: {redisConnection}");
MyConsole.WriteLine($"dbConnection: {dbConnection}");
MyConsole.WriteLine($"DOTNET_RUNNING_IN_CONTAINER: {Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER")}");


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
            .WithOrigins(_startupData.GetAllowedOrigins())
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

//_services.AddMoviesPersistenceCache_Redis(config =>
//{
//    config.RedisConnection = redisConnection;
//});


MyConsole.WriteLine($"Environment: {_env.EnvironmentName}");
MyConsole.WriteLine($"SqlConnection: {_startupData.ConnectionStringsSection.GetSqlDb()}");
MyConsole.WriteLine($"PgConnection: {_startupData.ConnectionStringsSection.GetPgDb()}");
MyConsole.WriteLine($"RedisConnection: {_startupData.ConnectionStringsSection.GetRedis()}");


//=========================//
//======= Build Sé ========//
//=========================//

var app = _builder.Build();

// Configure the HTTP request pipeline.
app.MapOpenApi();
app.MapScalarApiReference(opts =>
{
    opts
    .WithTitle("Movies API")
    .WithTheme(ScalarTheme.Mars)
    .WithDefaultHttpClient(ScalarTarget.Node, ScalarClient.HttpClient);
});

//app.UseHttpsRedirection();
app.UseCors(); // enable the default policy

app.UseAuthorization();

app.MapControllers();

await app.UseMoviesPersistenceEfAsync();

app.Run();
