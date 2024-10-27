using Pokedex.Services;
using PokeApiNet;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Pokedex.Services.Interface;
using Pokedex.Models;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;
using System.Net.Http.Headers;
using Microsoft.Extensions.Caching.Redis;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers(); 

IConfiguration Configuration = builder.Configuration;

builder.Services.AddSingleton<IConfiguration>(Configuration);
builder.Services.AddTransient<IPokemonService, PokemonService>();
builder.Services.AddSingleton<PokeApiClient>();

 builder.Services.AddHttpClient<ShakespeareTranslationStrategy>();
builder.Services.AddHttpClient<YodaTranslationStrategy>();

builder.Services.AddTransient<ITranslationStrategy, YodaTranslationStrategy>();
builder.Services.AddTransient<ITranslationStrategy, ShakespeareTranslationStrategy>();

builder.Services.AddSingleton<TranslationContext>();
// builder.Services.AddHttpClient<FunTranslationsService>(c=>
// {
//     c.BaseAddress = new Uri("https://api.funtranslations.com/translate/");
//     c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
// });

builder.Services.Configure<RedisCacheOptions>(options =>
{
    var redisConnStr = builder.Configuration.GetValue<string>("Redis:ConnectionString");

    if (!string.IsNullOrEmpty(redisConnStr))
    {
        options.Configuration = redisConnStr;
        options.InstanceName = "redis-pokedex";
    }
});

if (!string.IsNullOrEmpty(builder.Configuration.GetValue<string>("Redis:ConnectionString")))
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration.GetValue<string>("Redis:ConnectionString");
        options.InstanceName = "redis-pokedex";
    });
    Console.WriteLine("Redis Cache Initiated.");
}
else
{
    builder.Services.AddDistributedMemoryCache();
    Console.WriteLine("Distributed Memory Cache Initiated");
}

string redisConnStr = Configuration.GetValue<string>("Redis:ConnectionString");

if (!string.IsNullOrEmpty(redisConnStr)) // Can use Azure Redis cache instance in production / test environments
{
    builder.Services.AddDistributedRedisCache(options =>
    {
        options.Configuration = redisConnStr;
        options.InstanceName = "redis-pokedex";
    });
    Console.WriteLine("Redis Cache Initiated.");
}
else
{
    // For Local development environment when not using Redis
    builder.Services.AddDistributedMemoryCache();
    Console.WriteLine("Distributed Memory Cache Initiated");
}



builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

var app = builder.Build();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
