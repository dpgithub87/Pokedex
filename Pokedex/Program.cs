using Pokedex.Services;
using PokeApiNet;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Pokedex.Services.Interface;
using Pokedex.Models;
using Microsoft.Extensions.Configuration;
using System.IO;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers(); 

builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

builder.Services.Configure<ConfigParameters>(builder.Configuration.GetSection("Configurations"));

// Register services
builder.Services.AddSingleton<PokeApiClient>();
builder.Services.AddTransient<IPokemonService, PokemonService>();

builder.Services.AddHttpClient<FunTranslationsService>();
builder.Services.AddTransient<ShakespeareTranslationStrategy>();
builder.Services.AddTransient<YodaTranslationStrategy>();
builder.Services.AddTransient<ITranslationStrategyResolver, TranslationStrategyResolver>();
builder.Services.AddTransient<IFunTranslationsService,FunTranslationsService>();
builder.Services.AddTransient<IFunTranslationImplInterface,FunTranslationImplService>();
var app = builder.Build();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
