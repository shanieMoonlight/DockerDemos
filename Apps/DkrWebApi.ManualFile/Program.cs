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

var sclrTheme = app.Environment.IsDevelopment() ? ScalarTheme.BluePlanet : ScalarTheme.Mars;
app.MapOpenApi();
app.MapScalarApiReference(opts =>
{
    opts
    .WithTitle("DkrWebApi.ManualFile")
    .WithTheme(sclrTheme)
    .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
});
//}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
