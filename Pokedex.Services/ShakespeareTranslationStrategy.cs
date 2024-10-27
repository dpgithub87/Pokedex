using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pokedex.Models;
using Pokedex.Services.Interface;

namespace Pokedex.Services
{
    public class ShakespeareTranslationStrategy : ITranslationStrategy
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TranslationContext> _logger;
        private IPokemonService _pokemonService;
        private const string baseUrl = "https://api.funtranslations.com/translate/";
        public ShakespeareTranslationStrategy(HttpClient client, ILogger<TranslationContext> logger, IPokemonService PokemonService)
        {
            _httpClient = client;
            _logger = logger;
            _pokemonService = PokemonService;
            
        }

        public async Task<FunTranslation> Translate(string text)
        {
            try
            {
                text = Regex.Replace(text, @"\t|\n|\r|\f", " ");
                text = HttpUtility.UrlEncode(text);
               
                return await _httpClient.GetFromJsonAsync<FunTranslation>($"{baseUrl}shakespeare.json?text={text}");
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occurred on Shakespeare translation, Exception Details: ", ex.Message, ex.InnerException?.Message);
                return null;
            }
        }
        
        public async Task<bool> IsResolved(PokemonModel PokemonName )
        {
            //Default Translation method
            return true;
        }
        
    }
}