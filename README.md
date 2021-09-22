# Pokedex
A Pokemon API written using C# that fetches Pokemon list from public API (https://pokeapi.co/) and do few translations based on couple of Shakespeare / Yoda APIs (https://funtranslations.com).

## Prerequisites:
- Visual Studio Community 2019 with "Asp.NET and web development" workload
- Docker Desktop 4.0.1
- Docker Engine - v20.10.8
- OS: Windows 10 / Linux
- .Net Core 5.0

## Usage
- git clone 
### To run in Docker:

### To run in Visual Studio 2019
- Open the Solution file
- Select Solution Configuration: Debug, Platform: Any CPU, StartUp Project: Podex.Api
- Build and Execute it using the "F5"


## Test
The test projects includes unit tests and integration tests. Integration tests will query the PokeApi server for data via PokeApiNet Nuget package and Shakespeare & Yoda APIs.
The unit tests run off of mocked data.

dotnet test --filter "Category = Unit"
dotnet test --filter "Category = Integration"

## Production Design Decisions
To use Microservices architecture, Each of the integration touch point can be moved into a separate microservice. List of Microservices to follow:
- Console application to integrate with PokeApi.Co
- Console application to integrate with Shakespeare / Yoda FunTranslations API
- Web API that could expose two required endpoints
- Any additional microservices based on the complexity of the functionality (not required in this challenge)

### Integration 
- Message Queuing system such as Azure Service Bus used for communication between microservices
- gPRC could be used as an alternative communication mechanism between microservices

### Cache
- Implement any distributed cache (such as Redis) to cache the upstream API responses
### Deployment & Observability
- Deploy it in any of the Kubernetes service such as AKS or EKS or private hosted Kubernetes
- Deploy any log collection agent such as Datadog
- Reports to show the health of the PODs, Services and the Cluster
- Any Alerting mechanism if the threshold limits reach on usage of the PODS / Cluster



