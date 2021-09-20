using PokeApiNet;
using System;
using Xunit;

namespace Pokedex.Services.IntegrationTests
{
    public class PokeApiNetServiceTests
    {

        private PokeApiNetService _pokeApiNetService;
        private PokeApiClient _pokeApiClient;

        [Fact]
        public void GetPokemonList_Should_Return_PokemonList()
        {
            // Arrange
            _pokeApiClient = new PokeApiClient();
            _pokeApiNetService = new PokeApiNetService(_pokeApiClient);

            // Act
            var result = _pokeApiNetService.GetPokemonList().Result;

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);

        }
    }
}
