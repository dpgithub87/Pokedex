using Pokedex.Models;
using Pokedex.Services;

namespace Pokedex.Services.Interface
public interface IFunTranslationsStrategyService
{
    public Task<FunTranslation> MakeFunTranslation(string toTranslate);
    public IFunTranslationsStrategyService isResolved(Pokemon pokemon);
}