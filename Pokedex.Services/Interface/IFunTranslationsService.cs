using Pokedex.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokedex.Services.Interface
{
    public interface IFunTranslationsService
    {
        public Task<FunTranslation> TranslateWithShakespeare(string toTranslate);

        public Task<FunTranslation> TranslateWithYoda(string toTranslate);

    }
}
