using System;

namespace Pokedex.Models
{
    public class PokemonModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Habitat { get; set; }

        public bool IsLegendary { get; set; }

        public string RawDescription { get; set; }

        public string Comments { get; set; }
    }
}
