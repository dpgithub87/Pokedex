using Newtonsoft.Json.Bson;
using Pokedex.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pokedex.Services.Utils
{
    public static class CacheHelper
    {
        public static byte[] ToByteArray<T>(T obj)
        {
            if (obj == null)
                return null;           

            return JsonSerializer.SerializeToUtf8Bytes(obj,
                    new JsonSerializerOptions { WriteIndented = false, IgnoreNullValues = true });
        }

        public static T FromByteArray<T>(byte[] byteArray)
        {
            if (byteArray == null)
                return default(T);
           
            return JsonSerializer.Deserialize<T>(new ReadOnlySpan<byte>(byteArray));
        }

    }
}
