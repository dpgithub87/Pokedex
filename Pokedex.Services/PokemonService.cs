using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokeApiNet;
using Pokedex.Models;
using Pokedex.Services.Interface;

namespace Pokedex.Services
{
    public class PokemonService : IPokemonService
    {
        private readonly PokeApiClient _pokeApiClient;

        public PokemonService(PokeApiClient pokeApiClient)
        {
            _pokeApiClient = pokeApiClient;
        }

        public async Task<List<string>> GetPokemonNames()
        {
            var pokemonList = await _pokeApiClient.GetNamedResourcePageAsync<Pokemon>();
            return pokemonList.Results.Select(p => p.Name).ToList();
        }

        public virtual async Task<PokemonModel> GetPokemonDetails(string pokemonName)
        {
            var PokeDetails = await _pokeApiClient.GetResourceAsync<PokemonSpecies>(pokemonName);
            var flavorTextEntry = PokeDetails.FlavorTextEntries.FirstOrDefault(x => x.Language.Name == "en");
            var pokemonModel = new PokemonModel
            {
                Name = PokeDetails.Name,
                Description = flavorTextEntry?.FlavorText,
                Habitat = PokeDetails.Habitat?.Name, // Or any other property as needed
                IsLegendary = PokeDetails.IsLegendary,
                RawDescription = PokeDetails.FlavorTextEntries.FirstOrDefault()?.FlavorText,
                Comments = "This is pokemon Service"
            };
            return pokemonModel;
        }
    }
}
