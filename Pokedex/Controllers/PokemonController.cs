using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pokedex.Models;
using Pokedex.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pokedex.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PokemonController : ControllerBase
    {

        private readonly ILogger<PokemonController> _logger;

        private readonly IDistributedCache _cache;

        private PokemonService _pokemonService;
        public PokemonController(ILogger<PokemonController> logger, IDistributedCache cache, PokemonService pokemonService)
        {
            _logger = logger;
            _cache = cache;
            _pokemonService = pokemonService;
        }

        /// <summary>
        /// Gets the list of Pokemon names from PokeApi service
        /// </summary>
        /// <returns>List of Pokemon names</returns>
        [HttpGet]
        public Task<IEnumerable<string>> Get()
        {
            try
            {
                _logger.LogInformation("Request received for GetPokemonNames");

                var result = _pokemonService.GetPokemonNames();

                _logger.LogInformation("Succesfully executed GetPokemonNames");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occured when fetching Pokemon Names, Error Message: ", ex.Message, ex.InnerException.Message);

                return null;
            }
        }

        /// <summary>
        /// Gets the Pokemon Details for a given pokemon name
        /// </summary>
        /// <param name="pokemonName"></param>
        /// <returns>Pokemon details</returns>
        [HttpGet]
        [Route("{pokemonName}")]
        public async Task<ActionResult<PokemonModel>> Get(string pokemonName)
        {
            try
            {
                return await _pokemonService.GetPokemonDetails(pokemonName);
            }
            catch (Exception ex)
            {
                _logger.LogError("Excpetion occurred when fetching Pokemon details, Error Message: ", ex.Message, ex.InnerException);

                return NotFound();
            }
        }

    }
}
