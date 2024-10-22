using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pokedex.Models;
using Pokedex.Services.Interface;

namespace Pokedex.Services
{
    public class FunTranslationImplService:IFunTranslationImplInterface
    {
        private readonly IPokemonService _pokemonService;
        private readonly IFunTranslationsService _funTranslationsService;
        public FunTranslationImplService(IPokemonService pokemonService, IFunTranslationsService funTranslationsService)
        {
            _pokemonService = pokemonService;
            _funTranslationsService = funTranslationsService;            
        }    

        async Task<FunTranslation> IFunTranslationImplInterface.funTranslationImpl(string PokemonName)
        {
            var pokemonDetails = await _pokemonService.GetPokemonDetails(PokemonName);
            if (pokemonDetails.Habitat == "grassland" || !(pokemonDetails.IsLegendary) )
            {
                return await _funTranslationsService.Translate(pokemonDetails.Description,"yoda");
            }
            else
            {
                return await _funTranslationsService.Translate(pokemonDetails.Description,"shakespeare");
            }
        }
    }
}