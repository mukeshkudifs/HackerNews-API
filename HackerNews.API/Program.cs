
using HackerNews.API.Helpers;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var configuration = builder.Configuration;

string hackerNewsBaseUri = configuration["HackerNewsAPI:BaseUri"] ?? string.Empty;


builder.Services.ConfigureHttpClient(hackerNewsBaseUri);
builder.Services.AddMemoryCache();
builder.Services.ConfigureServices();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost");
    });
});


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Hacker News API");
    });
}




app.UseAuthorization();


app.MapControllers();

app.UseCors();

app.Run();

