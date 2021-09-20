using System;
using System.Net.Http;
using Xunit;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Pokedex.Api.IntegrationTests
{
    public class PokemonControllerTests
    {
        private readonly HttpClient _httpClient;

        private readonly TestServer _testServer;

        public PokemonControllerTests()
        {
            // Arrange
            _testServer = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>());
            _httpClient = _testServer.CreateClient();
        }

        [Fact]
        public async Task Get_Should_Return_ListOfPokemonNames()
        {
            // Act
            var response = await _httpClient.GetAsync("/pokemon");
          //  response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            //Assert
            Assert.NotEmpty(responseString);

        }
    }
}
