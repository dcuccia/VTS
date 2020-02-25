using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.IO;

namespace Vts.Test.IO
{
    #region Dummy classes for tests below
    public class CompositeThingy
    {
        public IThingy OneThingy { get; set; }
        public IList<IThingy> SomeThingies { get; set; }
        public IThingy[] MoreThingies { get; set; }
        public FirstThingy[] FirstThingies { get; set; }
        public SecondThingy[] SecondThingies { get; set; }
    }

    public interface IThingy
    {
        string ThingyType { get; }
    }

    public enum ThingyType
    {
        First,
        Second
    }

    public class FirstThingy : IThingy
    {
        public string ThingyType { get { return "First"; } }
    }

    public class SecondThingy : IThingy
    {
        public string ThingyType { get { return "Second"; } }
    }
    #endregion

    [TestFixture]
    public class VtsJsonSerializerTests
    {
        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        private List<string> listOfTestGeneratedFiles = new List<string>()
        {
            "VtsJsonSerializerTests_file1.txt", 
            "VtsJsonSerializerTests_file2.txt", 
            "VtsJsonSerializerTests_file3.txt", 
            "VtsJsonSerializerTests_file4.txt"
        };
        /// <summary>
        /// clear all generated folders and files
        /// </summary>
        [OneTimeSetUp]
        [OneTimeTearDown]
        public void clear_folders_and_files()
        {
            foreach (var file in listOfTestGeneratedFiles)
            {
                FileIO.FileDelete(file);
            }
        }

        [Test]
        public void validate_file_exists_after_writing()
        {
            VtsJsonSerializer.WriteToJsonFile(new[] { "Hello", "World" }, "VtsJsonSerializerTests_file1.txt");
            Assert.IsTrue(FileIO.FileExists("VtsJsonSerializerTests_file1.txt"));
        }

        [Test]
        public void validate_deserialization_from_string()
        {
            var jsonSerialized = VtsJsonSerializer.WriteToJson(new[] { "Hello", "Dolly" });
            var objectDeserialized = VtsJsonSerializer.ReadFromJson<string[]>(jsonSerialized);
            Assert.IsTrue(objectDeserialized != null);
            Assert.IsTrue(objectDeserialized.Length > 0);
            Assert.AreEqual(objectDeserialized[0], "Hello");
            Assert.AreEqual(objectDeserialized[1], "Dolly");
        }

        [Test]
        public void validate_deserialization_from_file()
        {
            VtsJsonSerializer.WriteToJsonFile(new[] { "Hello", "Sailor" }, "VtsJsonSerializerTests_file2.txt");
            var objectDeserialized = VtsJsonSerializer.ReadFromJsonFile<string[]>("VtsJsonSerializerTests_file2.txt");
            Assert.IsTrue(objectDeserialized != null);
            Assert.IsTrue(objectDeserialized.Length > 0);
            Assert.AreEqual(objectDeserialized[0], "Hello");
            Assert.AreEqual(objectDeserialized[1], "Sailor");
        }

        [Test]
        public void validate_deserialization_of_interface_object_from_string()
        {
            var exampleEnumType = typeof(FirstThingy);
            VtsJsonSerializer.KnownConverters.Add(ConventionBasedConverter<IThingy>.CreateFromEnum<ThingyType>(
                exampleEnumType.Namespace,
                exampleEnumType.Assembly.FullName));
            var compositeThingy = new CompositeThingy
            {
                OneThingy = new FirstThingy(),
                SomeThingies = new List<IThingy> { new FirstThingy(), new SecondThingy() },
                MoreThingies = new IThingy[] { new FirstThingy(), new SecondThingy() },
                FirstThingies = new [] { new FirstThingy(), new FirstThingy() },
                SecondThingies = new [] { new SecondThingy(), new SecondThingy() },

            };
            var jsonSerialized = VtsJsonSerializer.WriteToJson(compositeThingy);
            var thingyDeserialized = VtsJsonSerializer.ReadFromJson<CompositeThingy>(jsonSerialized);
            Assert.IsTrue(thingyDeserialized != null);
        }

        [Test]
        public void validate_serialization_of_IntRange()
        {
            var range = new IntRange(10, 20, 11);
            var jsonSerialized = VtsJsonSerializer.WriteToJson(range);
            Assert.IsTrue(jsonSerialized != null && jsonSerialized.Length > 0);
        }

        [Test]
        public void validate_deserialization_of_IntRange()
        {
            var range = new IntRange(10, 20, 11);
            var jsonSerialized = VtsJsonSerializer.WriteToJson(range);
            var intRangeDeserialized = VtsJsonSerializer.ReadFromJson<IntRange>(jsonSerialized);
            Assert.IsTrue(intRangeDeserialized != null);
            Assert.AreEqual(intRangeDeserialized.Start, 10);
            Assert.AreEqual(intRangeDeserialized.Stop, 20);
            Assert.AreEqual(intRangeDeserialized.Count, 11);
            Assert.AreEqual(intRangeDeserialized.Delta, 1);
        }

        [Test]
        public void validate_serialization_of_OpticalProperties()
        {
            var op = new OpticalProperties(0.011, 1.1, 0.99, 1.44);
            var jsonSerialized = VtsJsonSerializer.WriteToJson(op);
            Assert.IsTrue(jsonSerialized != null && jsonSerialized.Length > 0);
        }

        [Test]
        public void validate_deserialization_of_OpticalProperties()
        {
            Func<double, double, bool> areRoughlyEqual = (a, b) => Math.Abs(a - b) < 0.001;
            var op = new OpticalProperties(0.011, 1.1, 0.99, 1.44);
            var jsonSerialized = VtsJsonSerializer.WriteToJson(op);
            var opDeserialized = VtsJsonSerializer.ReadFromJson<OpticalProperties>(jsonSerialized);
            Assert.IsTrue(opDeserialized != null);
            Assert.IsTrue(areRoughlyEqual(opDeserialized.Mua, 0.011));
            Assert.IsTrue(areRoughlyEqual(opDeserialized.Musp, 1.1));
            Assert.IsTrue(areRoughlyEqual(opDeserialized.G, 0.99));
            Assert.IsTrue(areRoughlyEqual(opDeserialized.N, 1.44));
        }
    }
}
