using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using Pokedex.Controllers;
using Pokedex.Models;
using Pokedex.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;


namespace Podex.Api.UnitTests
{
    public class PokemonControllerTests
    {
        private PokemonController _pokemonController;

        private Mock<ILogger<PokemonController>> _mockLogger;

        private Mock<IDistributedCache> _mockCache;

        private Mock<PokemonService> _mockPokemonService;

        public PokemonControllerTests()
        {
            _mockLogger = new Mock<ILogger<PokemonController>>();

            _mockCache = new Mock<IDistributedCache>();

            _mockPokemonService = new Mock<PokemonService>();
        }

        [Fact]
        public void Get_Should_Return_ListOfPokemonNames()
        {
            // Arrange
            _mockPokemonService.Setup(ps => ps.GetPokemonNames())
                               .Returns(Task.FromResult(
                                   new List<string>()
                                   {
                                       "testPokemon1"
                                   }.AsEnumerable()
                                   ));

            _pokemonController = new PokemonController(_mockLogger.Object
                , _mockCache.Object
                , _mockPokemonService.Object);

            // Act
            var response = _pokemonController.Get();

            // Assert
            Assert.NotNull(response);
            Assert.NotEmpty(response.Result);
            Assert.Equal("testPokemon1", response.Result.FirstOrDefault());
        }

        [Theory]
        [InlineData("testPokemon")]
        public void GetPokemon_Should_Return_PokemonDetails(string pokemon)
        {
            // Arrange
            _mockPokemonService.Setup(ps => ps.GetPokemonDetails(It.IsAny<string>()))
                               .Returns(Task.FromResult(
                                   new PokemonModel()
                                   {
                                       Name = "testPokemon"
                                   }
                                   ));

            _pokemonController = new PokemonController(_mockLogger.Object
                , _mockCache.Object
                , _mockPokemonService.Object);

            // Act
            var response = _pokemonController.GetPokemon(pokemon).Result;

            // Assert
            Assert.NotNull(response);            
            Assert.Equal("testPokemon", response.Value.Name);
        }

        [Theory]
        [InlineData("testPokemon")]
        public void GetPokemonWithTranslations_Should_Return_PokemonDetails(string pokemon)
        {
            // Arrange
            _mockPokemonService.Setup(ps => ps.GetPokemonWithTranslations(It.IsAny<string>()))
                               .Returns(Task.FromResult(
                                   new PokemonModel()
                                   {
                                       Name = "testPokemon"
                                   }
                                   ));

            _pokemonController = new PokemonController(_mockLogger.Object
                , _mockCache.Object
                , _mockPokemonService.Object);

            // Act
            var response = _pokemonController.GetPokemonWithTranslations(pokemon).Result;

            // Assert
            Assert.NotNull(response);
            Assert.Equal("testPokemon", response.Value.Name);
        }
    }
}
