using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using PokeApiNet;
using Pokedex.Models;
using Pokedex.Services.Interface;
using Pokedex.Services.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Pokedex.Services.UnitTests
{
    public class PokemonServiceTests
    {
        private Mock<IPokeApiNetService> _mockPokeApiNetService;

        private Mock<FunTranslationsService> _mockFunTranslationService;

        private Mock<ILogger<PokemonService>> _mockLoggerPokemonService;

        private Mock<IDistributedCache> _mockCache;

        private PokemonService _pokemonService;

        public PokemonServiceTests()
        {
            _mockPokeApiNetService = new Mock<IPokeApiNetService>();
            _mockLoggerPokemonService = new Mock<ILogger<PokemonService>>();
            _mockFunTranslationService = new Mock<FunTranslationsService>();
            _mockCache = new Mock<IDistributedCache>();
        }

        #region "GetPokemonList"
        [Fact]
        public void GetPokemonList_Should_Return_ListOfNames()
        {
            //Arrange
            SetupMockEmptyCache();

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
            _pokemonService = new PokemonService(_mockPokeApiNetService.Object, _mockFunTranslationService.Object, _mockLoggerPokemonService.Object, _mockCache.Object);

            //Act
            var resultPokemonNames = _pokemonService.GetPokemonNames();


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

            _mockPokeApiNetService.Setup(pac => pac.GetPokemonList())
                                  .Returns(Task.FromResult
                                  (new List<NamedApiResource<Pokemon>>()
                                  ));
            _pokemonService = new PokemonService(_mockPokeApiNetService.Object, _mockFunTranslationService.Object, _mockLoggerPokemonService.Object, _mockCache.Object);

            //Act
            var resultPokemonNames = _pokemonService.GetPokemonNames();


            //Assert
            Assert.NotNull(resultPokemonNames.Result);

            Assert.Empty(resultPokemonNames.Result);

        }

        #endregion

        #region "GetPokemonDetails - Endpoint 1"

        [Theory]
        [InlineData("testPokemon")]
        public void GetPokemonDetails_Should_Return_Pokemon_WithCache(string pokemonName)
        {
            // Arrange          
            SetupPokemonDataCache();

            _pokemonService = new PokemonService(_mockPokeApiNetService.Object, _mockFunTranslationService.Object, _mockLoggerPokemonService.Object, _mockCache.Object);

            // Act
            var pokemon = _pokemonService.GetPokemonDetails(pokemonName).Result;

            // Assert
            Assert.NotNull(pokemon);
            Assert.Equal("testPokemon", pokemon.Name);
            Assert.Equal("testHabitat", pokemon.Habitat);
            Assert.Equal("testDescription", pokemon.Description);
        }

        [Theory]
        [InlineData("pikachu")]
        public void GetPokemonDetails_Should_Return_Pokemon_WithoutCache(string pokemonName)
        {
            // Arrange           
            SetupMockEmptyCache();

            var habitat = new NamedApiResource<PokemonHabitat>() { Name = "testHabitat" };

            var flavorTextEntries = new List<PokemonSpeciesFlavorTexts>()
            {
                new PokemonSpeciesFlavorTexts()
                {
                    FlavorText = "testDescription",
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

            _pokemonService = new PokemonService(_mockPokeApiNetService.Object, _mockFunTranslationService.Object, _mockLoggerPokemonService.Object, _mockCache.Object);

            // Act
            var pokemon = _pokemonService.GetPokemonDetails(pokemonName).Result;

            // Assert

            Assert.NotNull(pokemon);
            Assert.Equal("pikachu", pokemon.Name);
            Assert.Equal("testHabitat", pokemon.Habitat);
            Assert.Equal("testDescription", pokemon.Description);

        }

        [Theory]
        [InlineData("pikachu")]
        public void GetPokemonDetails_Should_Return_Null_Habitat(string pokemonName)
        {
            // Arrange           
            SetupMockEmptyCache();

            var flavorTextEntries = new List<PokemonSpeciesFlavorTexts>()
            {
                new PokemonSpeciesFlavorTexts()
                {
                    FlavorText = "testDescription",
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

            _pokemonService = new PokemonService(_mockPokeApiNetService.Object, _mockFunTranslationService.Object, _mockLoggerPokemonService.Object, _mockCache.Object);

            // Act
            var pokemon = _pokemonService.GetPokemonDetails(pokemonName).Result;


            // Assert

            Assert.NotNull(pokemon);
            Assert.Equal("pikachu", pokemon.Name);
            Assert.Null(pokemon.Habitat);
        }

        [Fact]
        public void GetPokemonDescription_Should_Return_Null()
        {
            // Arrange

            SetupMockEmptyCache();

            var flavorTextEntries = new List<PokemonSpeciesFlavorTexts>()
            {
                new PokemonSpeciesFlavorTexts()
                {
                    FlavorText = "testDescriptionFrench",
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

            _pokemonService = new PokemonService(_mockPokeApiNetService.Object, _mockFunTranslationService.Object, _mockLoggerPokemonService.Object, _mockCache.Object);

            // Act
            var pokemon = _pokemonService.GetPokemonDescription(pokemonSpecies, pokemonModel);


            // Assert

            Assert.NotNull(pokemon);
            Assert.Null(pokemon.Description);
            Assert.Null(pokemon.RawDescription);
        }

        [Fact]
        public void GetPokemonDescription_Should_Return_Clean_Description()
        {
            // Arrange

            SetupMockEmptyCache();

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

            _pokemonService = new PokemonService(_mockPokeApiNetService.Object, _mockFunTranslationService.Object, _mockLoggerPokemonService.Object, _mockCache.Object);

            // Act
            var pokemon = _pokemonService.GetPokemonDescription(pokemonSpecies, pokemonModel);


            // Assert

            Assert.NotNull(pokemon);
            Assert.NotNull(pokemon.Description);
            Assert.NotNull(pokemon.RawDescription);

            Assert.DoesNotContain("\t\n\r\f", pokemon.Description);
            Assert.Contains("\t\n\r\f", pokemon.RawDescription);
        }

        #endregion

        #region "GetPokemonWithTranslations Endpoint 2"

        [Theory]
        [InlineData("testPokemon")]
        public void GetPokemonWithTranslations_Should_Return_Pokemon_WithCache(string pokemonName)
        {
            // Arrange          
            SetupPokemonDataCache();

            _pokemonService = new PokemonService(_mockPokeApiNetService.Object, _mockFunTranslationService.Object, _mockLoggerPokemonService.Object, _mockCache.Object);

            // Act
            var pokemon = _pokemonService.GetPokemonWithTranslations(pokemonName).Result;

            // Assert
            Assert.NotNull(pokemon);
            Assert.Equal("testPokemon", pokemon.Name);
            Assert.Equal("testHabitat", pokemon.Habitat);
            Assert.Equal("testDescription", pokemon.Description);
        }

        [Theory]
        [InlineData("testPokemon")]
        public void GetPokemonWithTranslations_Should_Return_Pokemon_Cave_YodaTranslation(string pokemonName)
        {
            //Arrange
            SetupMockEmptyCache();
            

            var habitat = new NamedApiResource<PokemonHabitat>() { Name = "cave" };//cave - one of the condition needed for Yoda translation

            var flavorTextEntries = new List<PokemonSpeciesFlavorTexts>()
            {
                new PokemonSpeciesFlavorTexts()
                {
                    FlavorText = "testDescription",
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

            _mockFunTranslationService.Setup(ft => ft.TranslateWithYoda(It.IsAny<string>()))
                                     .Returns(Task.FromResult(new FunTranslation()
                                     {
                                         contents = new Contents() { translated = "yodaTranslated" }
                                     }));

            _pokemonService = new PokemonService(_mockPokeApiNetService.Object, _mockFunTranslationService.Object, _mockLoggerPokemonService.Object, _mockCache.Object);

            //Act
            var pokemon = _pokemonService.GetPokemonWithTranslations(pokemonName).Result;

            //Assert
            Assert.NotNull(pokemon);
            Assert.Contains("yoda", pokemon.Comments.ToLower());

        }

        [Theory]
        [InlineData("testPokemon")]
        public void GetPokemonWithTranslations_Should_Return_Pokemon_Legendary_YodaTranslation(string pokemonName)
        {
            //Arrange
            SetupMockEmptyCache();


            var habitat = new NamedApiResource<PokemonHabitat>() { Name = "testHabitat" };

            var flavorTextEntries = new List<PokemonSpeciesFlavorTexts>()
            {
                new PokemonSpeciesFlavorTexts()
                {
                    FlavorText = "testDescription",
                    Language = new NamedApiResource<Language>(){ Name = "en" }
                }
            };

            var pokemonSpecies = new PokemonSpecies()
            {
                Name = "pikachu",
                Habitat = habitat,
                IsLegendary = true, // One of the condition needed for Yoda Translation
                FlavorTextEntries = flavorTextEntries
            };

            _mockPokeApiNetService.Setup(pac => pac.GetPokemonSpecies(It.IsAny<string>()))
                                                  .Returns(Task.FromResult(pokemonSpecies));

            _mockFunTranslationService.Setup(ft => ft.TranslateWithYoda(It.IsAny<string>()))
                                     .Returns(Task.FromResult(new FunTranslation()
                                     {
                                         contents = new Contents() { translated = "yodaTranslated" }
                                     }));

            _pokemonService = new PokemonService(_mockPokeApiNetService.Object, _mockFunTranslationService.Object, _mockLoggerPokemonService.Object, _mockCache.Object);

            //Act
            var pokemon = _pokemonService.GetPokemonWithTranslations(pokemonName).Result;

            //Assert
            Assert.NotNull(pokemon);
            Assert.Contains("yoda", pokemon.Comments.ToLower());

        }

        [Theory]
        [InlineData("testPokemon")]
        public void GetPokemonWithTranslations_Should_Return_Pokemon_Null_YodaTranslation(string pokemonName)
        {
            //Arrange
            SetupMockEmptyCache();

            FunTranslation funTranslation = null;

            var habitat = new NamedApiResource<PokemonHabitat>() { Name = "testHabitat" };

            var flavorTextEntries = new List<PokemonSpeciesFlavorTexts>()
            {
                new PokemonSpeciesFlavorTexts()
                {
                    FlavorText = "testDescription",
                    Language = new NamedApiResource<Language>(){ Name = "en" }
                }
            };

            var pokemonSpecies = new PokemonSpecies()
            {
                Name = "pikachu",
                Habitat = habitat,
                IsLegendary = true, // One of the condition needed for Yoda Translation
                FlavorTextEntries = flavorTextEntries
            };

            _mockPokeApiNetService.Setup(pac => pac.GetPokemonSpecies(It.IsAny<string>()))
                                                  .Returns(Task.FromResult(pokemonSpecies));

            _mockFunTranslationService.Setup(ft => ft.TranslateWithYoda(It.IsAny<string>()))
                                     .Returns(Task.FromResult(funTranslation)); // Return Null to check if Standard description is returned

            _pokemonService = new PokemonService(_mockPokeApiNetService.Object, _mockFunTranslationService.Object, _mockLoggerPokemonService.Object, _mockCache.Object);

            //Act
            var pokemon = _pokemonService.GetPokemonWithTranslations(pokemonName).Result;

            //Assert
            Assert.NotNull(pokemon);
            Assert.Equal("testDescription", pokemon.Description);
            Assert.Contains("standard", pokemon.Comments.ToLower());

        }

        [Theory]
        [InlineData("testPokemon")]
        public void GetPokemonWithTranslations_Should_Return_Pokemon_ShakespeareTranslations(string pokemonName)
        {
            //Arrange
            SetupMockEmptyCache();


            var habitat = new NamedApiResource<PokemonHabitat>() { Name = "notCave" };  // Condition - Not to have "Cave" for Shakespeare Translation

            var flavorTextEntries = new List<PokemonSpeciesFlavorTexts>()
            {
                new PokemonSpeciesFlavorTexts()
                {
                    FlavorText = "testDescription",
                    Language = new NamedApiResource<Language>(){ Name = "en" }
                }
            };

            var pokemonSpecies = new PokemonSpecies()
            {
                Name = "pikachu",
                Habitat = habitat,
                IsLegendary = false, // Condition mandatory for Shakespeare Translation
                FlavorTextEntries = flavorTextEntries
            };

            _mockPokeApiNetService.Setup(pac => pac.GetPokemonSpecies(It.IsAny<string>()))
                                                  .Returns(Task.FromResult(pokemonSpecies));

            _mockFunTranslationService.Setup(ft => ft.TranslateWithShakespeare(It.IsAny<string>()))
                                     .Returns(Task.FromResult(new FunTranslation()
                                     {
                                         contents = new Contents() { translated = "shakespeareTranslated" }
                                     }));

            _pokemonService = new PokemonService(_mockPokeApiNetService.Object, _mockFunTranslationService.Object, _mockLoggerPokemonService.Object, _mockCache.Object);

            //Act
            var pokemon = _pokemonService.GetPokemonWithTranslations(pokemonName).Result;

            //Assert
            Assert.NotNull(pokemon);
            Assert.Contains("shakespeare", pokemon.Comments.ToLower());

        }

        [Theory]
        [InlineData("testPokemon")]
        public void GetPokemonWithTranslations_Should_Return_Pokemon_Null_ShakespeareTranslations(string pokemonName)
        {
            //Arrange
            SetupMockEmptyCache();
            FunTranslation funTranslation = null;

            var habitat = new NamedApiResource<PokemonHabitat>() { Name = "notCave" };  // Condition - Not to have "Cave" for Shakespeare Translation

            var flavorTextEntries = new List<PokemonSpeciesFlavorTexts>()
            {
                new PokemonSpeciesFlavorTexts()
                {
                    FlavorText = "testDescription",
                    Language = new NamedApiResource<Language>(){ Name = "en" }
                }
            };

            var pokemonSpecies = new PokemonSpecies()
            {
                Name = "pikachu",
                Habitat = habitat,
                IsLegendary = false, // Condition mandatory for Shakespeare Translation
                FlavorTextEntries = flavorTextEntries
            };

            _mockPokeApiNetService.Setup(pac => pac.GetPokemonSpecies(It.IsAny<string>()))
                                                  .Returns(Task.FromResult(pokemonSpecies));

            _mockFunTranslationService.Setup(ft => ft.TranslateWithShakespeare(It.IsAny<string>()))
                                     .Returns(Task.FromResult(funTranslation));//Return Null to check if Standard description is returned

            _pokemonService = new PokemonService(_mockPokeApiNetService.Object, _mockFunTranslationService.Object, _mockLoggerPokemonService.Object, _mockCache.Object);

            //Act
            var pokemon = _pokemonService.GetPokemonWithTranslations(pokemonName).Result;

            //Assert
            Assert.NotNull(pokemon);
            Assert.Equal("testDescription", pokemon.Description);
            Assert.Contains("standard", pokemon.Comments.ToLower());

        }

        #endregion

        private void SetupMockEmptyCache()
        {
            // No data in cache;
            byte[] byteArray = null;

            _mockCache.Setup(ce => ce.GetAsync(It.IsAny<string>(), default))
                      .Returns(Task.FromResult(byteArray));
        }

        private void SetupPokemonDataCache()
        {
            var pokemonByteArray = CacheHelper.ToByteArray(
                new PokemonModel()
                {
                    Name = "testPokemon",
                    Habitat = "testHabitat",
                    Description = "testDescription",
                    IsLegendary = true                    
                });

            _mockCache.Setup(ce => ce.GetAsync(It.IsAny<string>(), default))
                      .Returns(Task.FromResult(pokemonByteArray));
        }
    }
}
