using PokeApiNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pokedex.Services
{
    /// <summary>
    /// Fetches Pokemon details from PokeAPI via PokeAPINet Nuget package
    /// PokeAPINet NugetPackage has cache enabled by default
    /// </summary>
    public class PokeApiNetService
    {
        private PokeApiClient _pokeApiClient;

        public PokeApiNetService()
        {
            _pokeApiClient = new PokeApiClient();
        }

        public virtual async Task<List<NamedApiResource<Pokemon>>> GetPokemonList()
        {
            var responsePokemonLst = await _pokeApiClient.GetNamedResourcePageAsync<Pokemon>(-1, 0);

            return responsePokemonLst.Results;
        }

    }
}
