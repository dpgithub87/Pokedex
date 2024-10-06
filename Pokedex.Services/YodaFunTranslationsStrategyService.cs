using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pokedex.Models;
using Pokedex.Services.Interface;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
s
namespace Pokedex.Services
public class YodaFunTranslationsStrategyService : IFunTranslationsStrategyService
{
    private readonly IFunTranslationsStrategyService _funTranslation;
    private readonly HttpClient _httpClient;
    private static readonly string _funType = "yoda";
    public YodaFunTranslationsStrategyService(IFunTranslationsStrategyService funTranslation, HTTPClient httpClient, ILogger<YodaFunTranslationsStrategyService> logger)
    {
        _funTranslation = new YodaFunTranslationsStrategyService();
        _httpClient = httpClient;
        _loggerFunTranslationsService = logger;
    }
    public Task<FunTranslation> MakeFunTranslation(string toTranslate)
    {
        try
        {
            toTranslate = Regex.Replace(toTranslate, @"\t|\n|\r|\f", " ");

            toTranslate = HttpUtility.UrlEncode(toTranslate);

            return await _httpClient.GetFromJsonAsync<FunTranslation>(
                $"yoda.json?text=" + $"{toTranslate}");
        }
        catch (Exception ex)
        {
            _loggerFunTranslationsService.LogError("Exception occured on TranslateWithShakespeare, Exception Details: ", ex.Message, ex.InnerException?.Message);
            return null;
        }
    }

    public IFunTranslationsStrategyService isResolved(Pokemon pokemon)
    {
        if (pokemonModel.Habitat == "cave" || pokemonModel.IsLegendary)
        {
            return _funTranslation;
        }
        else
        {
            return null;
        }
    }
    public string FunType()
    {
        return _funType;
    }
}