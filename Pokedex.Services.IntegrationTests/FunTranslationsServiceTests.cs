using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;


namespace Pokedex.Services.IntegrationTests
{
    public class FunTranslationsServiceTests
    {

        private FunTranslationsService _FunTranslationsService;
        private HttpClient _httpClient;
        private Mock<ILogger<FunTranslationsService>> _mockIloggerFunTranslationService;

        [Fact]
        public void TranslateWithShakespeare_Should_Return_TranslatedText()
        {
            // Arrange
            _httpClient = new HttpClient() {            
                BaseAddress = new System.Uri(
                  "https://api.funtranslations.com/translate/")
            };
            _httpClient.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _mockIloggerFunTranslationService = new Mock<ILogger<FunTranslationsService>>();

            string sampleTextToConvert = "You gave Mr. Tim a hearty meal, but unfortunately what he ate made him die.";

            string expectedTextResult = "Thee did giveth mr. Tim a hearty meal,  but unfortunately what he did doth englut did maketh him kicketh the bucket.";

            _FunTranslationsService = new FunTranslationsService(_httpClient, _mockIloggerFunTranslationService.Object);

            // Act
            var result = _FunTranslationsService.TranslateWithShakespeare(sampleTextToConvert)?.Result;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1,result.success.total);
            Assert.Equal(expectedTextResult, result.contents.translated);          

        }
        [Fact]
        public void TranslateWithYoda_Should_Return_TranslatedText()
        {
            // Arrange
            _httpClient = new HttpClient()
            {
                BaseAddress = new System.Uri(
                  "https://api.funtranslations.com/translate/")
            };
            _httpClient.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _mockIloggerFunTranslationService = new Mock<ILogger<FunTranslationsService>>();

            string sampleTextToConvert = "Master Obiwan has lost a planet.";

            string expectedTextResult = "Lost a planet,  master obiwan has.";

            _FunTranslationsService = new FunTranslationsService(_httpClient, _mockIloggerFunTranslationService.Object);

            // Act
            var result = _FunTranslationsService.TranslateWithYoda(sampleTextToConvert)?.Result;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.success.total);
            Assert.Equal(expectedTextResult, result.contents.translated);

        }
    }
}
