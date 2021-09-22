using Microsoft.Extensions.Logging;
using PokeApiNet;
using Pokedex.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Pokedex.Services
{
    public class PokemonService
    {
        private PokeApiNetService _pokeApiNetService;
        private FunTranslationsService _funTranslationService;
        private ILogger<PokemonService> _loggerPokemonService;
        public PokemonService()
        {

        }
        public PokemonService(PokeApiNetService pokeApiNetService,FunTranslationsService funTranslationService, ILogger<PokemonService> loggerPokemonService)
        {
            _pokeApiNetService = pokeApiNetService;
            _funTranslationService = funTranslationService;
            _loggerPokemonService = loggerPokemonService;
        }

        public virtual async Task<IEnumerable<string>> GetPokemonNames()
        {
            var resultLst = await _pokeApiNetService.GetPokemonList();
            return resultLst.Select(pmn => pmn.Name);
        }

        public virtual async Task<PokemonModel> GetPokemonDetails(string pokemonName)
        {           
            try
            {
                PokemonModel resultPokemon = new PokemonModel();
                         
                var pokemonSpecies = await _pokeApiNetService.GetPokemonSpecies(pokemonName);

                resultPokemon.Name = pokemonSpecies.Name;
                resultPokemon.Habitat = pokemonSpecies.Habitat?.Name;
                resultPokemon.IsLegendary = pokemonSpecies.IsLegendary;

                // Get the clean description as well as the raw description
                resultPokemon = GetPokemonDescription(pokemonSpecies, resultPokemon);

                return resultPokemon;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public virtual PokemonModel GetPokemonDescription(PokemonSpecies pokemonSpecies, PokemonModel pokemonModel)
        {
            string description = pokemonSpecies.FlavorTextEntries?.FirstOrDefault(te => te.Language.Name == "en")?.FlavorText;

            if (!String.IsNullOrEmpty(description))
            {
                pokemonModel.RawDescription = description;

                pokemonModel.Description = Regex.Replace(description, @"\t|\n|\r|\f", " ");
            }

            return pokemonModel;
        }

        public virtual async Task<PokemonModel> GetPokemonWithTranslations(string pokemonName)
        {
            var pokemonModel = await GetPokemonDetails(pokemonName);            
           
            if (pokemonModel.Habitat == "cave" || pokemonModel.IsLegendary)
            {
                var translatedResponse = await _funTranslationService.TranslateWithYoda(pokemonModel.Description);
                if(translatedResponse is null)
                { 
                    return pokemonModel; 
                }
                pokemonModel.Description = translatedResponse.contents.translated;
            }
            else
            {
                var translatedResponse = await _funTranslationService.TranslateWithShakespeare(pokemonModel.Description);
                if (translatedResponse is null)
                {
                    return pokemonModel;
                }
                pokemonModel.Description = translatedResponse.contents.translated;
            }
            return pokemonModel;
        }
    }
}
