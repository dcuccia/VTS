using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Vts.IO
{
    public static class VtsJsonSerializer
    {
#if DEBUG
        public static MemoryTraceWriter TraceWriter = new MemoryTraceWriter();
#endif
        public static string WriteToJson<T>(this T myObject)
        {
            var settings = new JsonSerializerSettings
            {
                //added temporarily to help serialize the sources that use interfaces
                TypeNameHandling = TypeNameHandling.None,
                ObjectCreationHandling = ObjectCreationHandling.Replace
            };
#if DEBUG
            settings.TraceWriter = TraceWriter;
#endif
            settings.Converters.Add(new StringEnumConverter());
            string json = JsonConvert.SerializeObject(
                myObject,
                Formatting.Indented,
                settings);
#if DEBUG
            Console.WriteLine(TraceWriter);
#endif
            return json;
        }

        public static void WriteToJsonFile<T>(this T myObject, string filename)
        {
            var settingsJson = WriteToJson(myObject);

            using (var stream = StreamFinder.GetFileStream(filename, FileMode.Create))
            using (var sw = new StreamWriter(stream))
            {
                sw.Write(settingsJson);
            }
        }

        public static List<JsonConverter> KnownConverters = new List<JsonConverter>();

        public static T ReadFromJson<T>(this string myString)
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new StringEnumConverter());
            foreach (var jsonConverter in KnownConverters)
            {
                serializer.Converters.Add(jsonConverter);
            }
            serializer.NullValueHandling = NullValueHandling.Ignore;
            //added temporarily to help serialize the sources that use interfaces
            serializer.TypeNameHandling = TypeNameHandling.None;
            serializer.ObjectCreationHandling = ObjectCreationHandling.Replace;
#if DEBUG
            serializer.TraceWriter = TraceWriter;
#endif

            T deserializedProduct = default(T);
            using (var sr = new StringReader(myString))
            using (var reader = new JsonTextReader(sr))
            {
                deserializedProduct = serializer.Deserialize<T>(reader);
            }
#if DEBUG
            Console.WriteLine(TraceWriter);
#endif
            return deserializedProduct;
        }

        public static T ReadFromJsonFile<T>(string filename)
        {
            using (var stream = StreamFinder.GetFileStream(filename, FileMode.Open))
            using (var sr = new StreamReader(stream, false))
            {
                var json = sr.ReadToEnd();

                return ReadFromJson<T>(json);
            }
        }
    }

    // from http://stackoverflow.com/questions/8030538/how-to-implement-custom-jsonconverter-in-json-net-to-deserialize-a-list-of-base
    public class ConventionBasedConverter2<TInterface> : JsonCreationConverter<TInterface>
    {
        private static Lazy<IServiceCollection> ServiceCollection { get; } = new Lazy<IServiceCollection>(GetServiceCollection);
        private static Lazy<IServiceProvider> ServiceProvider { get; } = new Lazy<IServiceProvider>(GetServiceProvider);

        private static IServiceCollection GetServiceCollection()
        {
            var collection = new ServiceCollection()
                .Scan(scan =>
                    scan.FromApplicationDependencies()
                        .AddClasses(classes => classes.AssignableTo<TInterface>())
                            .AsSelfWithInterfaces()
                            .WithTransientLifetime()
                );

            return collection;
        }

        private static IServiceProvider GetServiceProvider()
        {
            var collection = ServiceCollection.Value;
            var serviceProvider = collection.BuildServiceProvider();
            return serviceProvider;
        }

        private readonly string _typeCategoryString;

        public ConventionBasedConverter2(string jsonTypeCategory)
        {
            _typeCategoryString = jsonTypeCategory;
        }

        protected override TInterface Create(Type objectType, JObject jObject)
        {
            Type GetTypeToUse()
            {
                if (!objectType.IsInterface)
                    return objectType;

                if (!_typeCategoryString.EndsWith("Type") || !jObject.ContainsKey(_typeCategoryString))
                    return null;

                var typeName =
                    jObject[_typeCategoryString] +
                    typeof(TInterface).Name.Substring(1);

                var concreteTypeToUse = ServiceCollection.Value
                    .FirstOrDefault(sc => sc.ImplementationType.Name == typeName)
                    .ImplementationType;

                return concreteTypeToUse;
            }

            var typeToUse = GetTypeToUse();

            if (typeToUse == null)
                return default;

            var classInstance = (TInterface)ServiceProvider.Value.GetRequiredService(typeToUse);

            return classInstance;
        }
    }

    public abstract class JsonCreationConverter<T> : JsonConverter
    {
        /// <summary>
        /// Create an instance of objectType, based properties in the JSON object
        /// </summary>
        /// <param name="objectType">type of object expected</param>
        /// <param name="jObject">contents of JSON object that will be deserialized</param>
        /// <returns></returns>
        protected abstract T Create(Type objectType, JObject jObject);

        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Load JObject from stream
            JObject jObject = JObject.Load(reader);

            // Create target object based on JObject
            T target = Create(objectType, jObject);

            // Populate the object properties
            serializer.Populate(jObject.CreateReader(), target);

            return target;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
