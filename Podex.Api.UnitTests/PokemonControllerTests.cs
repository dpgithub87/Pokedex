using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using Pokedex.Controllers;
using Pokedex.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;


namespace Podex.Api.UnitTests
{
    public class PokemonControllerTests
    {
        private PokemonController _pokemonController;

        private Mock<ILogger<PokemonController>> _mockLogger;

        private Mock<IDistributedCache> _mockCache;

        private Mock<PokemonService> _mockPokemonService;

        [Fact]
        public void Get_Should_Return_ListOfPokemonNames()
        {
            // Arrange
            _mockLogger = new Mock<ILogger<PokemonController>>();

            _mockCache = new Mock<IDistributedCache>();

            _mockPokemonService = new Mock<PokemonService>();

            _mockPokemonService.Setup(ps => ps.GetPokemonNames())
                               .Returns(new List<string>() { "testPokemon1" });

            _pokemonController = new PokemonController(_mockLogger.Object
                , _mockCache.Object
                , _mockPokemonService.Object);

            // Act
            var result = _pokemonController.Get();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal("testPokemon1", result.FirstOrDefault());

        }
    }
}
