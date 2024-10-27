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
        Task<List<string>> GetPokemonNames();
        Task<PokemonModel> GetPokemonDetails(string name);
    }
}
