using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Pokedex.Services.Interface;

namespace Pokedex.Services
{
    public class TranslationStrategyResolver : ITranslationStrategyResolver
    {
        private readonly IServiceProvider _serviceProvider;

        public TranslationStrategyResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ITranslationStrategy Resolve(string strategyType)
        {
            return strategyType.ToLower() switch
            {
                "shakespeare" => _serviceProvider.GetService<ShakespeareTranslationStrategy>(),
                "yoda" => _serviceProvider.GetService<YodaTranslationStrategy>(),
                _ => throw new ArgumentException("Invalid translation strategy type", nameof(strategyType))
            };
        } 
        
    }
}