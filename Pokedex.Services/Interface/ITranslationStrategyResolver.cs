using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pokedex.Services.Interface
{
    public interface ITranslationStrategyResolver
    {
        ITranslationStrategy Resolve(string strategyType);
    }
}