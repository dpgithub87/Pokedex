using Microsoft.Extensions.Logging;
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

        private Mock<ILogger<PokemonService>> _mockLoggerPokemonService;

        private PokemonService pokemonService;

        [Fact]
        public void GetPokemonList_Should_Return_ListOfNames()
        {
            //Arrange
            _mockPokeApiNetService = new Mock<PokeApiNetService>();
            _mockLoggerPokemonService = new Mock<ILogger<PokemonService>>();

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
            pokemonService = new PokemonService(_mockPokeApiNetService.Object, _mockLoggerPokemonService.Object);

            //Act
            var resultPokemonNames = pokemonService.GetPokemonNames();


            //Assert
            Assert.NotNull(resultPokemonNames.Result);

            var count = resultPokemonNames.Result.Select(x => x.Count()).Count();
            Assert.Equal(1, count);

            Assert.NotNull(resultPokemonNames.Result.FirstOrDefault());

            Assert.Contains("testPokemon1", resultPokemonNames.Result.FirstOrDefault());

        }

        [Fact]
        public void GetPokemonList_Should_Return_ZeroPokemon()
        {
            //Arrange
            _mockPokeApiNetService = new Mock<PokeApiNetService>();
            _mockLoggerPokemonService = new Mock<ILogger<PokemonService>>();


            _mockPokeApiNetService.Setup(pac => pac.GetPokemonList())
                                  .Returns(Task.FromResult
                                  (new List<NamedApiResource<Pokemon>>()
                                  ));
            pokemonService = new PokemonService(_mockPokeApiNetService.Object, _mockLoggerPokemonService.Object);

            //Act
            var resultPokemonNames = pokemonService.GetPokemonNames();


            //Assert
            Assert.NotNull(resultPokemonNames.Result);

            Assert.Empty(resultPokemonNames.Result);

        }

        [Theory]
        [InlineData("pikachu")]
        public void GetPokemonDetails_Should_Return_Pokemon(string pokemonName)
        {
            // Arrange
            _mockPokeApiNetService = new Mock<PokeApiNetService>();

            _mockLoggerPokemonService = new Mock<ILogger<PokemonService>>();

            var habitat = new NamedApiResource<PokemonHabitat>() { Name = "testHabitat" };

            var flavorTextEntries = new List<PokemonSpeciesFlavorTexts>()
            {
                new PokemonSpeciesFlavorTexts()
                {
                    FlavorText = "testFlavorText",
                    Language = new NamedApiResource<Language>(){ Name = "en" }
                } 
            };

            var pokemonSpecies = new PokemonSpecies()
            {
                Name = "pikachu",
                Habitat = habitat,
                IsLegendary = false,
                FlavorTextEntries = flavorTextEntries
            };

            _mockPokeApiNetService.Setup(pac => pac.GetPokemonSpecies(It.IsAny<string>()))
                                                   .Returns(Task.FromResult(pokemonSpecies));

            pokemonService = new PokemonService(_mockPokeApiNetService.Object, _mockLoggerPokemonService.Object);

            // Act
            var pokemon = pokemonService.GetPokemonDetails(pokemonName).Result;


            // Assert

            Assert.NotNull(pokemon);
            Assert.Equal("pikachu", pokemon.Name);
            Assert.Equal("testHabitat", pokemon.Habitat);
            Assert.Equal("testFlavorText", pokemon.Description);

        }

        [Theory]
        [InlineData("pikachu")]
        public void GetPokemonDetails_Should_Return_Null_Habitat(string pokemonName)
        {
            // Arrange
            _mockPokeApiNetService = new Mock<PokeApiNetService>();

            _mockLoggerPokemonService = new Mock<ILogger<PokemonService>>();           
                      
            var flavorTextEntries = new List<PokemonSpeciesFlavorTexts>()
            {
                new PokemonSpeciesFlavorTexts()
                {
                    FlavorText = "testFlavorText",
                    Language = new NamedApiResource<Language>(){ Name = "en" }
                }
            };

            var pokemonSpecies = new PokemonSpecies()
            {
                Name = "pikachu",
                Habitat = null,
                IsLegendary = false,
                FlavorTextEntries = flavorTextEntries
            };

            _mockPokeApiNetService.Setup(pac => pac.GetPokemonSpecies(It.IsAny<string>()))
                                                   .Returns(Task.FromResult(pokemonSpecies));

            pokemonService = new PokemonService(_mockPokeApiNetService.Object, _mockLoggerPokemonService.Object);

            // Act
            var pokemon = pokemonService.GetPokemonDetails(pokemonName).Result;


            // Assert

            Assert.NotNull(pokemon);
            Assert.Equal("pikachu", pokemon.Name);
            Assert.Null(pokemon.Habitat);            
        }

        [Fact]
        public void GetPokemonDescription_Should_Return_Null()
        {
            // Arrange
            _mockPokeApiNetService = new Mock<PokeApiNetService>();

            _mockLoggerPokemonService = new Mock<ILogger<PokemonService>>();

            var flavorTextEntries = new List<PokemonSpeciesFlavorTexts>()
            {
                new PokemonSpeciesFlavorTexts()
                {
                    FlavorText = "testFlavorText",
                    Language = new NamedApiResource<Language>(){ Name = "fr" }
                }
            };

            var pokemonSpecies = new PokemonSpecies()
            {
                Name = null,
                Habitat = null,
                IsLegendary = false,
                FlavorTextEntries = flavorTextEntries
            };

            var pokemonModel = new Models.PokemonModel();

            pokemonService = new PokemonService(_mockPokeApiNetService.Object, _mockLoggerPokemonService.Object);

            // Act
            var pokemon = pokemonService.GetPokemonDescription(pokemonSpecies, pokemonModel);


            // Assert

            Assert.NotNull(pokemon);           
            Assert.Null(pokemon.Description);
            Assert.Null(pokemon.RawDescription);
        }

        [Fact]
        public void GetPokemonDescription_Should_Return_Clean_Description()
        {
            // Arrange
            _mockPokeApiNetService = new Mock<PokeApiNetService>();

            _mockLoggerPokemonService = new Mock<ILogger<PokemonService>>();

            var flavorTextEntries = new List<PokemonSpeciesFlavorTexts>()
            {
                new PokemonSpeciesFlavorTexts()
                {
                    FlavorText = "testFlavorText\t\n\r\f",
                    Language = new NamedApiResource<Language>(){ Name = "en" }
                }
            };

            var pokemonSpecies = new PokemonSpecies()
            {
                Name = null,
                Habitat = null,
                IsLegendary = false,
                FlavorTextEntries = flavorTextEntries
            };

            var pokemonModel = new Models.PokemonModel();

            pokemonService = new PokemonService(_mockPokeApiNetService.Object, _mockLoggerPokemonService.Object);

            // Act
            var pokemon = pokemonService.GetPokemonDescription(pokemonSpecies, pokemonModel);


            // Assert

            Assert.NotNull(pokemon);
            Assert.NotNull(pokemon.Description);
            Assert.NotNull(pokemon.RawDescription);

            Assert.DoesNotContain("\t\n\r\f", pokemon.Description);
            Assert.Contains("\t\n\r\f", pokemon.RawDescription);
        }
    }
}
