using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pokedex.Models;
using Pokedex.Services;
using Pokedex.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pokedex.Controllers
{
    [ApiController]
    
    public class PokemonController : ControllerBase
    {

        private readonly IPokemonService _pokemonService;
        private readonly IFunTranslationsService _funTranslationsService;
        private readonly IFunTranslationImplInterface _funTranslationsImpl;
        public PokemonController(IPokemonService pokemonService, IFunTranslationsService funTranslationsService, IFunTranslationImplInterface funTranslationImplInterface)
        {
            _pokemonService = pokemonService;
            _funTranslationsService = funTranslationsService;
            _funTranslationsImpl = funTranslationImplInterface;
        }
        [Route("/getpokemonName")]
        [HttpGet]
        public async Task<IActionResult> GetPokemonNames()
        {
            var pokemonNames = await _pokemonService.GetPokemonNames();
            return Ok(pokemonNames);
        }  

        [Route("/getPokemonDetails")]
        [HttpGet()]
        public async Task<IActionResult> getpokemonDetail([FromHeader] string pokemonName)
        {
            var pokemonDetails = await _pokemonService.GetPokemonDetails(pokemonName);
            
            return Ok(pokemonDetails);
        }

        [Route("/Translate")]
        [HttpGet()]
        public async Task<IActionResult> funTranslation([FromHeader] string pokemonName)
        {
            return Ok(await _funTranslationsImpl.funTranslationImpl(pokemonName));
        }

    }
}
