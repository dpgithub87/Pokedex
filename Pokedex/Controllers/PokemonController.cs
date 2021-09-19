using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
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
        public PokemonController(ILogger<PokemonController> logger, IDistributedCache cache,PokemonService pokemonService)
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
        public IEnumerable<string> Get()
        {
            try
            {
                _logger.LogInformation("Request received for GetPokemonNames");
                return _pokemonService.GetPokemonNames();
            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
}
