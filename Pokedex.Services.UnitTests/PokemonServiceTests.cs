using Moq;
using PokeApiNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Pokedex.Services.UnitTests
{
    public class PokemonServiceTests
    {
        private Mock<PokeApiNetService> _mockPokeApiNetService;

        private PokemonService pokemonService;

        [Fact]
        public void GetPokemonList_Should_Return_ListOfNames()
        {
            //Arrange
            _mockPokeApiNetService = new Mock<PokeApiNetService>();

            _mockPokeApiNetService.Setup(pac => pac.GetPokemonList())
                                  .Returns(Task.FromResult
                                  (new List<NamedApiResource<Pokemon>>()
                                  {
                                      new NamedApiResource<Pokemon>()
                                      {
                                          Name = "testPokemon1"
                                      }
                                  }
                                  ));
            pokemonService = new PokemonService(_mockPokeApiNetService.Object);

            //Act
            var resultLst = pokemonService.GetPokemonNames();


            //Assert
            Assert.NotNull(resultLst);

            var count = resultLst.Select(x => x.Count()).Count();
            Assert.Equal(1, count);

            Assert.NotNull(resultLst.FirstOrDefault());

            Assert.Contains("testPokemon1", resultLst.FirstOrDefault());

        }

        [Fact]
        public void GetPokemonList_Should_Return_ZeroPokemon()
        {
            //Arrange
            _mockPokeApiNetService = new Mock<PokeApiNetService>();

            _mockPokeApiNetService.Setup(pac => pac.GetPokemonList())
                                  .Returns(Task.FromResult
                                  (new List<NamedApiResource<Pokemon>>()
                                  ));
            pokemonService = new PokemonService(_mockPokeApiNetService.Object);

            //Act
            var resultLst = pokemonService.GetPokemonNames();


            //Assert
            Assert.NotNull(resultLst);

            var count = resultLst.Select(x => x.Count()).Count();
            Assert.Equal(0, count);

        }


    }
}
