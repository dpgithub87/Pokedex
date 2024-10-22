using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pokedex.Models;

namespace Pokedex.Services.Interface
{
    public interface ITranslationStrategy
    {
        Task<FunTranslation> Translate(string text);
    }
}