using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pokedex.Models;
using Pokedex.Services.Interface;

namespace Pokedex.Services
{
    public class FunTranslationsService : IFunTranslationsService
    {
        private readonly HttpClient _httpClient;
        private readonly ITranslationStrategyResolver _strategyResolver;

        public FunTranslationsService(HttpClient client, ITranslationStrategyResolver strategyResolver)
        {
            _httpClient = client;
            _strategyResolver = strategyResolver;
        }

        public async Task<FunTranslation> Translate(string toTranslate, string strategyType)
        {
            var strategy = _strategyResolver.Resolve(strategyType);
            return await strategy.Translate(toTranslate);
        }
    }
}