using Pokedex.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokedex.Services.Interface
{
   public interface IPokemonService
    {
        public Task<IEnumerable<string>> GetPokemonNames();

        public Task<PokemonModel> GetPokemonDetails(string pokemonName);

        public Task<PokemonModel> GetPokemonWithTranslations(string pokemonName);
    }
}
