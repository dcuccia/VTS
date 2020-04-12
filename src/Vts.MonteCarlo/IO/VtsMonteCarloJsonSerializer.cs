using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Vts.IO;

namespace Vts.MonteCarlo.IO
{
    public static class VtsMonteCarloJsonSerializer
    {
        static VtsMonteCarloJsonSerializer()
        {
            var knownConverters = new List<JsonConverter>
            {
                new ConventionBasedConverter2<ISourceInput>("SourceType"),
                new ConventionBasedConverter2<ITissueInput>("TissueType"),
                new ConventionBasedConverter2<ITissueRegion>("TissueRegionType"),
                new ConventionBasedConverter2<IDetectorInput>("TallyType"),
                new ConventionBasedConverter2<IDetector>("TallyType"),
            };

            VtsJsonSerializer.KnownConverters.AddRange(knownConverters);
        }

        public static void PreInitialize()
        {
        }

        public static string WriteToJson<T>(this T myObject)
        {
            return VtsJsonSerializer.WriteToJson(myObject);
        }

        public static void WriteToJsonFile<T>(this T myObject, string filename)
        {
            VtsJsonSerializer.WriteToJsonFile(myObject, filename);
        }

        public static T ReadFromJson<T>(this string myString)
        {
            return VtsJsonSerializer.ReadFromJson<T>(myString);
        }

        public static T ReadFromJsonFile<T>(string filename)
        {
            return VtsJsonSerializer.ReadFromJsonFile<T>(filename);
        }
    }
}
