using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Unity;
using Unity.Injection;
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

    /// <summary>
    /// Internal class for holding on to necessary class info for future instantiation
    /// </summary>
    internal class VtsClassInfo
    {
        public Type ClassType { get; set; }
        public string ClassName { get; set; }
        public string ClassPrefixString { get; set; }
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

    // from http://stackoverflow.com/questions/8030538/how-to-implement-custom-jsonconverter-in-json-net-to-deserialize-a-list-of-base
    public class ConventionBasedConverter<TInterface> : JsonCreationConverter<TInterface> 
    {
        private static readonly UnityContainer _container = new UnityContainer();

        private readonly string _typeCategoryString;
        private readonly IDictionary<string, VtsClassInfo> _classInfoDictionary;
  

        public ConventionBasedConverter(string typeNamespace, string assemblyFullName, string typeCategoryString, IEnumerable<string> classPrefixStrings, string classBasename = null)
        {
            var interfaceType = typeof(TInterface);
            var classBasename1 = classBasename ?? interfaceType.Name.Substring(1);
            _typeCategoryString = typeCategoryString;

            var useDefaultConstructor = true;

            var classList =
                from classPrefixString in classPrefixStrings
                let className = typeNamespace + @"." + classPrefixString + classBasename1
                let classType = Type.GetType(className + "," + assemblyFullName, false, true)
                select new VtsClassInfo
                {
                    ClassType = classType,
                    ClassName = className,
                    ClassPrefixString = classPrefixString,
                };

            foreach (var item in classList)
            {
                if (!object.Equals(item.ClassType, null))
                {
                    _container.RegisterType(
                        interfaceType,
                        item.ClassType,
                        item.ClassPrefixString, // use the prefix string to register each class
                        useDefaultConstructor ? new InjectionMember[] { new InjectionConstructor() } : null);
                }
            }

            _classInfoDictionary = classList.ToDictionary(item => item.ClassPrefixString);
        }
        
        public static ConventionBasedConverter<TInterface> CreateFromEnum<TEnum>(string typeNamespace, string assemblyFullName, string classBasename = null)
        {
            return new ConventionBasedConverter<TInterface>(
                typeNamespace,
                assemblyFullName,
                typeof(TEnum).Name,
                // use convention to map class names to enum types  e.g. ThingyType.First will register to FirstThingy 
                EnumHelper.GetValues<TEnum>().Select(value => value.ToString()),
                classBasename);
        }

        protected override TInterface Create(Type objectType, JObject jObject)
        {
            if (!FieldExists(_typeCategoryString, jObject))
            {
                throw new Exception(String.Format("The given object type {0} is not supported!", objectType));
            }

            var classPrefixString = jObject[_typeCategoryString].ToString();
            
            // get name of Enum from interface (e.g. if it's "IThingy", get "ThingyType" Enum and generate names for all source classes, and then use the corresponding factory, possibly also using convention "ThingyFactory")
            var classInfo = _classInfoDictionary[classPrefixString];

            var classInstance = _container.Resolve<TInterface>(classInfo.ClassPrefixString);

            return classInstance;
        }

        private bool FieldExists(string fieldName, JObject jObject)
        {
            return jObject[fieldName] != null;
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
