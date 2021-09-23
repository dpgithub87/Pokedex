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

namespace Pokedex.Services
{
    public class FunTranslationsService : IFunTranslationsService
    {
        private HttpClient _httpClient { get; }
        private ILogger<FunTranslationsService> _loggerFunTranslationsService;

        public FunTranslationsService()
        {

        }
        public FunTranslationsService(HttpClient client, ILogger<FunTranslationsService> loggerFunTranslationsService)
        {
            _httpClient = client;
            _loggerFunTranslationsService = loggerFunTranslationsService;
        }

        public virtual async Task<FunTranslation> TranslateWithShakespeare(string toTranslate)
        {
            try
            {
                toTranslate = Regex.Replace(toTranslate, @"\t|\n|\r|\f", " ");

                toTranslate = HttpUtility.UrlEncode(toTranslate);
                             
                return await _httpClient.GetFromJsonAsync<FunTranslation>(
                              $"shakespeare.json?text=" + $"{toTranslate}");
            }
            catch(Exception ex)
            {
                _loggerFunTranslationsService.LogError("Exception occured on TranslateWithShakespeare, Exception Details: ", ex.Message, ex.InnerException?.Message);
                return null;
            }
        }

        public virtual async Task<FunTranslation> TranslateWithYoda(string toTranslate)
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
        
    }
}
