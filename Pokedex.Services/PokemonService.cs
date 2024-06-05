using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using PokeApiNet;
using Pokedex.Models;
using Pokedex.Services.Interface;
using Pokedex.Services.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Pokedex.Services
{
    public class PokemonService : IPokemonService
    {
        private IPokeApiNetService _pokeApiNetService;
        private FunTranslationsService _funTranslationService;
        private ILogger<PokemonService> _loggerPokemonService;
        private IDistributedCache _cache;
        public PokemonService()
        {

        }
        public PokemonService(IPokeApiNetService pokeApiNetService, FunTranslationsService funTranslationService, ILogger<PokemonService> loggerPokemonService, IDistributedCache cache)
        {
            _pokeApiNetService = pokeApiNetService;
            _funTranslationService = funTranslationService;
            _loggerPokemonService = loggerPokemonService;
            _cache = cache;
        }

        /// <summary>
        ///  Gets the list of Pokemon names from PokeAPI
        /// </summary>
        /// <returns>Pokemon names</returns>
        public virtual async Task<IEnumerable<string>> GetPokemonNames()
        {
            var resultLst = await _pokeApiNetService.GetPokemonList();
            return resultLst.Select(pmn => pmn.Name);
        }

        /// <summary>
        /// Get the basic pokemon details from PokeAPI given a name of Pokemon
        /// </summary>
        /// <param name="pokemonName"></param>
        /// <returns></returns>
        public virtual async Task<PokemonModel> GetPokemonDetails(string pokemonName)
        {
            try
            {
                PokemonModel resultPokemon = new PokemonModel();

                // Check if Pokemon exists in the cache
                var cachePokemon = await _cache.GetAsync(pokemonName);
                if (cachePokemon != null)
                {
                    // Fetch from Cache
                    resultPokemon = CacheHelper.FromByteArray<PokemonModel>(cachePokemon);
                }
                else
                {
                    // Fetch from API
                    var pokemonSpecies = await _pokeApiNetService.GetPokemonSpecies(pokemonName);

                    resultPokemon.Name = pokemonSpecies.Name;
                    resultPokemon.Habitat = pokemonSpecies.Habitat?.Name;
                    resultPokemon.IsLegendary = pokemonSpecies.IsLegendary;

                    // Get the clean description as well as the raw description
                    resultPokemon = GetPokemonDescription(pokemonSpecies, resultPokemon);

                    // Insert into Cache with an hour sliding expiry
                    await _cache.SetAsync(
                        pokemonName,
                        CacheHelper.ToByteArray<PokemonModel>(resultPokemon),
                        new DistributedCacheEntryOptions
                        {
                            SlidingExpiration = TimeSpan.FromHours(1)
                        }
                        );
                }
                return resultPokemon;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets both raw description & clean description from PokemonSpecies object.
        /// </summary>
        /// <param name="pokemonSpecies"></param>
        /// <param name="pokemonModel"></param>
        /// <returns>Pokemodel onject filled with description</returns>
        public virtual PokemonModel GetPokemonDescription(PokemonSpecies pokemonSpecies, PokemonModel pokemonModel)
        {
            // Fetch any of the English descriptions.
            string description = pokemonSpecies.FlavorTextEntries?.FirstOrDefault(te => te.Language.Name == "en")?.FlavorText;

            if (!String.IsNullOrEmpty(description))
            {
                pokemonModel.RawDescription = description;

                pokemonModel.Description = Regex.Replace(description, @"\t|\n|\r|\f", " ");

                pokemonModel.Comments = "Standard description";
            }

            return pokemonModel;
        }

        /// <summary>
        /// Gets the pokemon details with translations either with Yoda or Shakespeare API based on few conditions
        /// </summary>
        /// <param name="pokemonName"></param>
        /// <returns>PokemonModel object filled with either translated description or standard description</returns>
        public virtual async Task<PokemonModel> GetPokemonWithTranslations(string pokemonName)
        {
            PokemonModel pokemonModel;
            // Check if Pokemon (Translated version) exists in the cache
            var cachePokemon = await _cache.GetAsync($"{pokemonName}T");
            if (cachePokemon != null)
            {
                // Fetch from Cache
                pokemonModel = CacheHelper.FromByteArray<PokemonModel>(cachePokemon);
            }
            else
            {
                pokemonModel = await GetPokemonDetails(pokemonName);

                List<IFunTranslationsStrategyService> funs =
                {
                    new ShakespeareFunTranslationsStrategyService(),
                    new YodaFunTranslationsStrategyService()
                };
                foreach(IFunTranslationsStrategyService fun in funs)
                {
                    _funTranslationService = fun.isResolved(pokemonModel);
                    if _funTranslationService != null
                        break;
                }
                    
                var translatedResponse = await _funTranslationService.MakeFunTranslation(pokemonModel.Description);
                if (translatedResponse is null)
                {
                    return pokemonModel; // Standard description if not able to translate
                }

                //Translated description
                pokemonModel.Description = translatedResponse.contents.translated;
                pokemonModel.Comments = _funTranslationService.FunType() + " translated description";
                
                // Insert into Cache with an hour sliding expiry
                await _cache.SetAsync(
                    $"{pokemonName}T",
                    CacheHelper.ToByteArray<PokemonModel>(pokemonModel),
                    new DistributedCacheEntryOptions
                    {
                        SlidingExpiration = TimeSpan.FromHours(1)
                    }
                    );
            }
            return pokemonModel;
        }
    }
}
