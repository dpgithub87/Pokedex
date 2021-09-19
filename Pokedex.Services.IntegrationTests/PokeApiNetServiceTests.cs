using System;
using Xunit;

namespace Pokedex.Services.IntegrationTests
{
    public class PokeApiNetServiceTests
    {

        private PokeApiNetService _pokeApiNetService;

        [Fact]
        public void GetPokemonList_Should_Return_PokemonList()
        {
            // Arrange
            _pokeApiNetService = new PokeApiNetService();

            // Act
            var result = _pokeApiNetService.GetPokemonList().Result;

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);

        }
    }
}
