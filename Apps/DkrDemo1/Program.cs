using DkrDemo1;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.MapOpenApi();
    app.MapScalarApiReference(opts =>
    {
        opts
        .WithTitle("Docker Demo 1")
        .WithTheme(ScalarTheme.BluePlanet)
        .WithDefaultHttpClient(ScalarTarget.Node, ScalarClient.HttpClient);
    });

//}

CertificateTester.Validate("password");

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
