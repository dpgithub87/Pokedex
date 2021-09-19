using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokedex.Services
{
   public class PokemonService
    {
        private PokeApiNetService _pokeApiNetService;
        public PokemonService()
        {

        }
        public PokemonService(PokeApiNetService pokeApiNetService)
        {
            _pokeApiNetService = pokeApiNetService;
        }

        public virtual IEnumerable<string> GetPokemonNames()
        {
            var resultLst = _pokeApiNetService.GetPokemonList().Result;
            return resultLst.Select(pmn => pmn.Name).ToList();           
        }
    }
}
