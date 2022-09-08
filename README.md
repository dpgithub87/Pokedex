# Pokedex
A Pokemon API written using C# that fetches Pokemon list from public API (https://pokeapi.co/) and does few translations based on a couple of Shakespeare / Yoda APIs (https://funtranslations.com). 
Caching the Pokemon data would be key given the static nature of the data, Currently code implemented for Azure Redis cache and Distributed memory cache to run the application in local environments. This is also important as there are restrictions in the number of API calls made in both the integration endpoints.

## Endpoints available currently:
1. GET-  http://localhost:5000/swagger/
To get the latest OpenApi swagger.json file for the Pokedex.Api - WebApi.Project 

2. GET-  http://localhost:5000/pokemon/{pokemonName}/
To fetch pokemon details from Pokeapi (species) and return along with "standard" description.

3. GET - http://localhost:5000/pokemon/
To fetch the list of Pokemon names, in case you don't know how a pokemon should be typed.

4. GET - http://localhost:5000/pokemon/translated/{pokemonName}
To fetch pokemon details from Pokeapi (species) and return along with translated description either **Yoda** in case of {"cave" Habitat & "Legendary" pokemon} or **Shakespeare** translation. To return Standard description if we encounter any issues in above mentioned translations.

5. GET - http://localhost:5000/health
To check if the API is healthy - Provided by Microsoft Extensions Disagnostics Health Checks


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
```sh
git clone https://github.com/dpgithub87/Pokedex
```
Zip link:
https://github.com/dpgithub87/Pokedex/archive/refs/heads/main.zip

### 2. To run in Developer Powershell VS 2019 or any console command prompt
Navigate to the root folder and execute the below commands
```sh
dotnet build
cd Pokedex
dotnet run
```
The application will be available at http://localhost:5000/


### 3. To run in docker:
- In Visual Studio 2019, please select Pokedex.Api as the startup project and please select the "docker" option and click F5, the application should be up and running in one of the available ports.
- In Commandline tool, you can navigate to root folder and execute the below commands to get interactive docker run  (keeps STDIN open), Environment variable "Development" is required only to run in dev environment
```sh
docker build -t pokedex:v1 -f .\Pokedex\Dockerfile .
docker run -it --rm -p 5000:80 -e "ASPNETCORE_ENVIRONMENT=Development" pokedex:v1
```
- If you want to run docker in a disconnected fashion, please use the below command. Please note, you don't need to build if you had already built the image.
```sh
docker build -t pokedex:v1 -f .\Pokedex\Dockerfile .
docker run -dp 5000:80 -e "ASPNETCORE_ENVIRONMENT=Development" pokedex:v1
```
Note:
For first time run, this will take time as it downloads base aspnet 5.0 image as well as the SDK 5.0 image, Subsequent runs will be faster as it uses the cached images unless there are any modifications.
The "docker run" command connects localhost system port 5000 to docker container port 80

### 4. Git commit history added in the repo as requested - gitlog.txt
https://github.com/dpgithub87/Pokedex/blob/main/gitlog.txt

## Test
The test projects include unit tests and integration tests. Integration tests will query the PokeApi server for data via the PokeApiNet Nuget package and Shakespeare & Yoda APIs.
The unit tests run with mocked data.
Execute the below "test" command by navigating to root folder in CLI.
```sh
dotnet test
```

### Httpie test
Endpoint 1:
```sh
C:\Users\HP Laptop>http http://localhost:5000/pokemon/mewtwo/
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
Date: Fri, 24 Sep 2021 23:30:43 GMT
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
C:\Users\HP Laptop>http http://localhost:5000/pokemon/translated/mewtwo/
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
Date: Fri, 24 Sep 2021 23:30:54 GMT
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

### Docker Image creation via pipelines
You can use the PodedexApi/Dockerfile in any build pipelines to build images that can be published to any cloud container registry, then deployed to the Kubernetes cluster by Release pipelines.
Sample Tech stack: Azure DevOps build pipeline, Release pipeline, Azure Container Registry & AKS

### Deployment & Observability
- Deploy it in any of the Kubernetes services such as AKS or EKS or private hosted Kubernetes
- Deploy any log collection agent such as Datadog
- Reports to show the health of the PODs, Services and the Cluster
- Any Alerting mechanism if threshold limits reach on the usage of the PODS / Cluster


