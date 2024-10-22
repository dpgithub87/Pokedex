using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pokedex.Models;

namespace Pokedex.Services.Interface
{
    public interface IFunTranslationsService
    {
        Task<FunTranslation> Translate(string text, string strategyType);
        //Task<FunTranslation> TranslateWithShakespeare(string toTranslate);
        //Task<FunTranslation> TranslateWithYoda(string toTranslate);
    }
}