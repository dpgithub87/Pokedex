using PokeApiNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pokedex.Services
{
    /// <summary>
    /// Wrapper class that fetches Pokemon details from PokeAPI via PokeAPINet Nuget package
    /// Client Cache enabled by default
    /// </summary>
    public class PokeApiNetService
    {
        private PokeApiClient _pokeApiClient;

        public PokeApiNetService()
        {

        }
        public PokeApiNetService(PokeApiClient pokeApiClient)
        {
            _pokeApiClient = pokeApiClient;
        }

        public virtual async Task<List<NamedApiResource<Pokemon>>> GetPokemonList()
        {
            var responsePokemonLst = await _pokeApiClient.GetNamedResourcePageAsync<Pokemon>();

            return responsePokemonLst.Results;
        }       

        public virtual async Task<PokemonSpecies> GetPokemonSpecies(string pokemonName)
        {
            return await _pokeApiClient.GetResourceAsync<PokemonSpecies>(pokemonName);
        }

    }
}
