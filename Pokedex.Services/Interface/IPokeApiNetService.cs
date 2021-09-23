using PokeApiNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokedex.Services.Interface
{
    public interface IPokeApiNetService
    {
        public Task<List<NamedApiResource<Pokemon>>> GetPokemonList();

        public Task<PokemonSpecies> GetPokemonSpecies(string pokemonName);

    }
}
