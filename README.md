# Pokedex
A Pokemon API written using C# that fetches Pokemon list from public API (https://pokeapi.co/) and does few translations based on a couple of Shakespeare / Yoda APIs (https://funtranslations.com). 

Endpoints available currently:
1. GET-  https://localhost:5001/swagger/
To get the latest OpenApi swagger.json file for the WebApi.Project 

2. GET-  https://localhost:5001/pokemon/{pokemonName}/
To fetch pokemon details from Pokeapi (species) and return along with "standard" description.

3. GET - https://localhost:5001/pokemon/
To fetch the list of Pokemon names, in case you don't know how a pokemon should be typed
Caching the Pokemon data would be key given the static nature of the same, Currently 

4. GET - https://localhost:5001/pokemon/translated/{pokemonName}
To fetch pokemon details from Pokeapi (species) and return along with translated description either Yoda in case of "cave" Habitat & "Legendary" pokemon or Sakespeare translation. To return Standard description if we encounter any issues in above said translations.

5. GET - https://localhost:5001/health
To check if the API is healthy - Provided by Microsoft Extensions Disagnostics Health Checks

Caching the Pokemon data would be key given the static nature of the data, Currently code implemented for Azure Redis cache and Distributed memory cache to run the application in local environments. This is also important as there are restrictions in the number of API calls made in both the integration endpoints.

## Prerequisites:
- Visual Studio Community 2019 with "Asp.NET and web development" workload
- Docker Desktop 4.0.1
- Docker Engine - v20.10.8
- OS: Windows 10 / Linux
- .Net Core 5.0
- httpie (optional for testing) - https://httpie.io/cli

## Usage
### 1. You can download as Zip or clone this repo using git

Using git:
> git clone https://github.com/dpgithub87/Pokedex

You can download it as .zip and extractit in a directory
>https://github.com/dpgithub87/Pokedex/archive/refs/heads/main.zip

### 2. To run in Developer Powershell
Navigate to the root folder and execute the below commands
```sh
dotnet build
cd Pokdex
dotnet run
```
The application will be available at https://localhost:5001/

Here is the swagger url: https://localhost:5001/swagger/index.html

### 3. To run in docker:
In Visual Studio 2019, please select Pokedex.Api as the startup project and please select the "docker" option and click F5, the application should be up and running in one of the available ports.
You can use the PodedexApi/Dockerfile in any build pipelines to build images that can be published to any cloud container registry, then deployed to the Kubernetes cluster by Release pipelines.
Sample Tech stack: Azure DevOps build pipeline, Release pipeline, Azure Container Registry & AKS

### 4. Git commit history added in the repo as requested - gitlog.txt
https://github.com/dpgithub87/Pokedex/blob/main/gitlog.txt

## Test
The test projects include unit tests and integration tests. Integration tests will query the PokeApi server for data via the PokeApiNet Nuget package and Shakespeare & Yoda APIs.
The unit tests run with mocked data.
Execute the below "test" command in root folder.
```sh
dotnet test
```

### Httpie test
Endpoint 1:
```sh
PS C:\Users\HP Laptop\source\repos\Pokedex\Pokedex\pokedex> http --verify=no https://localhost:5001/pokemon/mewtwo
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
Date: Fri, 24 Sep 2021 14:54:55 GMT
Server: Kestrel
Transfer-Encoding: chunked

{
    "comments": "Standard description",
    "description": "It was created by a scientist after years of horrific gene splicing and DNA engineering experiments.",
    "habitat": "rare",
    "isLegendary": true,
    "name": "mewtwo",
    "rawDescription": "It was created by\na scientist after\nyears of horrific\fgene splicing and\nDNA engineering\nexperiments."
}
```
Endpoint 2:
```sh
PS C:\Users\HP Laptop\source\repos\Pokedex\Pokedex\pokedex> http --verify=no https://localhost:5001/pokemon/translated/mewtwo
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
Date: Fri, 24 Sep 2021 14:59:18 GMT
Server: Kestrel
Transfer-Encoding: chunked

{
    "comments": "Yoda translated description",
    "description": "Created by a scientist after years of horrific gene splicing and dna engineering experiments,  it was.",
    "habitat": "rare",
    "isLegendary": true,
    "name": "mewtwo",
    "rawDescription": "It was created by\na scientist after\nyears of horrific\fgene splicing and\nDNA engineering\nexperiments."
}
```

## Production Design Decisions
To use Microservices architecture, Each of the integration touchpoints can be moved into a separate microservice. List of Microservices to follow:
- Console application to integrate with PokeApi.Co
- Console application to integrate with Shakespeare / Yoda FunTranslations API
- Web API that could expose two required endpoints
- Implement token-based authentication / authorisation mechanism

- Any additional microservices based on the complexity of the functionality (not required in this challenge)
- Need to have proper Database design for Microservices which requires database.
- Add model validations wherever needed


### Integration
- Message Queuing systems such as Azure Service Bus used for communication between microservices
- gPRC could be used as an alternative communication mechanism between microservices

### Cache
- Implement any distributed cache (such as Redis) to cache the upstream API responses
- Have to limit the maximum number of Pokemons based on the intended consumption of the API. Alternatively, you can go for higher subscription in any paas model cloud cache.

### Deployment & Observability
- Deploy it in any of the Kubernetes services such as AKS or EKS or private hosted Kubernetes
- Deploy any log collection agent such as Datadog
- Reports to show the health of the PODs, Services and the Cluster
- Any Alerting mechanism if threshold limits reach on the usage of the PODS / Cluster


