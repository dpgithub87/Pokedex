using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using Pokedex.Controllers;
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

        [Fact]
        public void Get_Should_Return_ListOfPokemonNames()
        {
            // Arrange
            _mockLogger = new Mock<ILogger<PokemonController>>();

            _mockCache = new Mock<IDistributedCache>();

            _mockPokemonService = new Mock<PokemonService>();

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
    }
}
