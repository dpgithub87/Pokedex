using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pokedex.Models;
using Pokedex.Services.Interface;

namespace Pokedex.Services
{
    public class TranslationContext
    {
        private readonly IEnumerable<ITranslationStrategy> _translationStrategies;
        private IPokemonService _pokemonService;
        public TranslationContext(IEnumerable<ITranslationStrategy> translationStrategies,IPokemonService pokemonService)
        {
            _translationStrategies = translationStrategies;
            _pokemonService = pokemonService;
 
        }

        public async Task<FunTranslation> Translate(string pokemonName)
        {
            var pokemon = await _pokemonService.GetPokemonDetails(pokemonName);
            foreach (var strategy in _translationStrategies)
            {
                if (await strategy.IsResolved(pokemon))
                {
                    return await strategy.Translate(pokemon.Description);
                }
            }
            throw new InvalidOperationException("No applicable translation strategy found.");
        }
    }
}
