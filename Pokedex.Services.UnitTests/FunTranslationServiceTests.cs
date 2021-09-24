using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using PokeApiNet;
using Pokedex.Models;
using Pokedex.Services.Interface;
using Pokedex.Services.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Pokedex.Services.UnitTests
{
    public class FunTranslationServiceTests
    {
        private Mock<ILogger<FunTranslationsService>> _mockLoggerFunTranslationService;    
        private FunTranslationsService _funTranslationsService;
        private HttpClient _mockClient;

        public FunTranslationServiceTests()
        {
            _mockLoggerFunTranslationService = new Mock<ILogger<FunTranslationsService>>();
        }


        #region "TranslateWithShakespeare"

        [Theory]
        [InlineData("testPokemon")]
        public void TranslateWithShakespeare_Should_Return_FunTranslation(string pokemonName)
        {
            // Arrange
            string responseJson = "{\"success\":{\"total\":1},\"contents\":{\"translated\": \"Lost a planet,  master obiwan has. translatedShakespeare\"" +
                                    ",\"text\": \"Master Obiwan has lost a planet.\",\"translation\": \"sakespeare\"}}";

            MockHttpClient(responseJson);

            _funTranslationsService = new FunTranslationsService(_mockClient, _mockLoggerFunTranslationService.Object);

            // Act
            var funTranslation = _funTranslationsService.TranslateWithShakespeare(pokemonName).Result;

            // Assert
            Assert.NotNull(funTranslation);
            Assert.Contains("translatedShakespeare", funTranslation.contents.translated);
        }

        #endregion

        #region "TranslateWithYoda"

        [Theory]
        [InlineData("testPokemon")]
        public void TranslateWithYoda_Should_Return_FunTranslation(string pokemonName)
        {
            // Arrange
            string responseJson = "{\"success\":{\"total\":1},\"contents\":{\"translated\": \"Lost a planet,  master obiwan has. translatedYoda\"" +
                                    ",\"text\": \"Master Obiwan has lost a planet.\",\"translation\": \"sakespeare\"}}";

            MockHttpClient(responseJson);

            _funTranslationsService = new FunTranslationsService(_mockClient, _mockLoggerFunTranslationService.Object);

            // Act
            var funTranslation = _funTranslationsService.TranslateWithYoda(pokemonName).Result;

            // Assert
            Assert.NotNull(funTranslation);
            Assert.Contains("translatedYoda", funTranslation.contents.translated);
        }

        #endregion

        private void MockHttpClient(string responseJson)
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent(responseJson),
               });
            _mockClient = new HttpClient(mockHttpMessageHandler.Object);
            _mockClient.BaseAddress = new Uri("https://mockuri/");
        }


    }
}
