﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using Vts.Common;
using Vts.IO;

namespace Vts.Test.IO
{
    [TestFixture]
    public class FileIOTests
    {
        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        List<string> listOfTestGeneratedFolders = new List<string>()
        {
            "fileiotest.folder",
            "folder1",
            "folder2",
            "folder3",
            "folder4",
            "sourcetest"
        };
        List<string> listOfTestGeneratedFiles = new List<string>()
        {
            "file1.txt",
            "file2.txt",
            "file3.txt",
            "file4.txt",
            "file5.txt",
            "file6.txt",
            "file7.txt",
            "file7.xml",
            "file8.xml",
            "array1",
            "array1.txt",
            "array2",
            "array3",
            "array4",
            "array5",
            "array6",
            "scalar",
            "2darray",
            "2darray.txt",
            "floatarray",
            "floatarray.txt",
            "complexarray",
            "complexarray.txt",
            "bytearray",
            "bytearray.txt",
            "ushortarray",
            "ushortarray.txt",
            "embeddedresourcefile.txt",
            "resourcefile.txt",
            "AOfXAndYAndZ",
            "position.txt"
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
            foreach (var folder in listOfTestGeneratedFolders)
            {
                FileIO.DeleteDirectory(folder);
            }
        }

        [Test]
        public void validate_clear_directory()
        {
            const string folder = "fileiotest.folder";
            if (!FileIO.DirectoryExists(folder))
            {
                FileIO.CopyFolderFromEmbeddedResources(folder, "", Assembly.GetExecutingAssembly().FullName, true);
                Assert.IsTrue(FileIO.FileExists(Path.Combine(folder, "embeddedresourcefile.txt")));
            }
            else
            {
                if (!FileIO.FileExists(Path.Combine(folder, "embeddedresourcefile.txt")))
                {
                    var name = Assembly.GetExecutingAssembly().FullName;
                    var assemblyName = new AssemblyName(name).Name;
                    FileIO.CopyFileFromEmbeddedResources(assemblyName + ".Resources.embeddedresourcefile.txt", Path.Combine(folder, "embeddedresourcefile.txt"), name);
                }
            }
            Assert.IsTrue(FileIO.FileExists(Path.Combine(folder, "embeddedresourcefile.txt")));
            FileIO.ClearDirectory(folder);
            Assert.IsFalse(FileIO.FileExists(Path.Combine(folder, "embeddedresourcefile.txt")));
        }

        [Test]
        public void validate_clone()
        {
            var i = new Position { X = 2, Y = 5, Z = 9 };
            var iCloned = i.Clone();
            Assert.AreEqual(iCloned.X, i.X);
            Assert.AreEqual(iCloned.Y, i.Y);
            Assert.AreEqual(iCloned.Z, i.Z);
        }

        [Test]
        public void validate_copy_stream()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            var stream1 = StreamFinder.GetFileStreamFromResources("Resources/streamfindertest/resourcefile.txt", assemblyName);
            var stream2 = StreamFinder.GetFileStream("file5.txt", FileMode.CreateNew);
            Assert.IsNotNull(stream1);
            FileIO.CopyStream(stream1, stream2);
            Assert.IsNotNull(stream2);
            Assert.AreEqual(stream1, stream2);
            stream1.Close();
            stream2.Close();
        }

        [Test]
        public void validate_create_directory()
        {
            const string folder = "folder2";
            FileIO.CreateDirectory(folder);
            Assert.IsTrue(FileIO.DirectoryExists(folder));
        }

        [Test]
        public void validate_create_empty_directory()
        {
            const string folder = "fileiotest.folder";
            if (!FileIO.DirectoryExists(folder))
            {
                FileIO.CopyFolderFromEmbeddedResources(folder, "", Assembly.GetExecutingAssembly().FullName, true);
                Assert.IsTrue(FileIO.FileExists(Path.Combine(folder, "embeddedresourcefile.txt")));
            }
            else
            {
                if (!FileIO.FileExists(Path.Combine(folder, "embeddedresourcefile.txt")))
                {
                    var name = Assembly.GetExecutingAssembly().FullName;
                    var assemblyName = new AssemblyName(name).Name;
                    FileIO.CopyFileFromEmbeddedResources(assemblyName + ".Resources.fileiotest.embeddedresourcefile.txt", "embeddedresourcefile.txt", name);
                }
            }
            Assert.IsTrue(FileIO.FileExists(Path.Combine(folder, "embeddedresourcefile.txt")));
            FileIO.CreateEmptyDirectory(folder);
            Assert.IsFalse(FileIO.FileExists(Path.Combine(folder, "embeddedresourcefile.txt")));
        }


        [Test]
        public void validate_read_array_from_binary_in_resources_with_size_parameter()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            string dataLocation = "Resources/fileiotest/";
            int size = 100;
            var data = (double[])FileIO.ReadArrayFromBinaryInResources<double>
                (dataLocation + @"ROfRho", assemblyName, size);
            Assert.IsTrue(Math.Abs(data[2] - 0.052445) < 0.000001);
        }

        [Test]
        public void validate_read_array_from_binary_in_resources_without_parameter_dimensions()
        {
            // ReadArrayFromBinaryInResources without parameter dimensions calls ReadFromJsonInResources
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            string dataLocation = "Resources/fileiotest/";
            var data = (double[])FileIO.ReadArrayFromBinaryInResources<double>
                (dataLocation + @"ROfRho", assemblyName);
            Assert.IsTrue(Math.Abs(data[2] - 0.052445) < 0.000001);
        }

        [Test]
        public void validate_read_from_binary_in_resources()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            double data;
            data = (double)FileIO.ReadFromBinaryInResources<double>(
                "Resources/fileiotest/binarydbl", assemblyName);
            Assert.AreEqual(data, 10);
        }

        [Test] 
        public void validate_read_from_binary_in_resources_custom()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            double ReadMap(BinaryReader b) => b.ReadDouble();
            // the following method has a "yield return" so won't load until accessed
            var arrayRead = FileIO.ReadFromBinaryInResourcesCustom<double>(
                "Resources/fileiotest/ROfRho", assemblyName, ReadMap);
            Assert.IsTrue(Math.Abs(arrayRead.Skip(2).Take(1).First() - 0.052445) < 0.000001);
        }


        [Test] 
        public void validate_read_from_binary_custom()
        {
            IEnumerable<double> arrayWritten = Enumerable.Range(15, 3).Select(x => (double)x);
            void WriteMap(BinaryWriter b, double s) => b.Write(s);
            FileIO.WriteToBinaryCustom<double>(arrayWritten, "array6", WriteMap);
            double ReadMap(BinaryReader b) => b.ReadDouble();
            // the following method has a "yield return" so won't load until accessed
            var listRead = FileIO.ReadFromBinaryCustom<double>("array6", ReadMap);
            var arrayRead = listRead.Take(3).ToArray();
            Assert.AreEqual(arrayRead[1], 16);
        }

        [Test]
        public void validate_read_from_binary_stream()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            int size = 100;
            var arrayWritten = new double[size];
            // read file from resources and write it so that can be read in
            arrayWritten = (double[])FileIO.ReadArrayFromBinaryInResources<double>
                ("Resources/fileiotest/ROfRho", assemblyName, size);
            FileIO.WriteToBinary<double[]>(arrayWritten, "array5");
            double[] arrayRead = new double[100];
            using (Stream stream = StreamFinder.GetFileStream("array5", FileMode.Open))
            {
                arrayRead = FileIO.ReadFromBinaryStream<double[]>(stream);
            }
            Assert.IsTrue(Math.Abs(arrayRead[2] - 0.052445) < 0.000001);
        }

        [Test]
        public void validate_read_from_json()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            FileIO.CopyFileFromResources("Resources/fileiotest/position.txt", "position.txt", assemblyName);
            var pos = FileIO.ReadFromJson<Position>("position.txt");
            Assert.AreEqual(pos.X, 5);
            Assert.AreEqual(pos.Y, 10);
            Assert.AreEqual(pos.Z, 15);
        }

        [Test]
        public void validate_read_from_json_in_resources()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            var pos = FileIO.ReadFromJsonInResources<Position>("Resources/fileiotest/position.txt", assemblyName);
            Assert.AreEqual(pos.X, 5);
            Assert.AreEqual(pos.Y, 10);
            Assert.AreEqual(pos.Z, 15);
        }

        [Test]
        public void validate_read_from_json_stream()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            // create a JSON stream
            var stream = StreamFinder.GetFileStreamFromResources("Resources/fileiotest/position.txt", assemblyName);
            var pos = FileIO.ReadFromJsonStream<Position>(stream);
            Assert.AreEqual(pos.X, 5);
            Assert.AreEqual(pos.Y, 10);
            Assert.AreEqual(pos.Z, 15);
            stream.Close();
        }

        [Test]
        public void validate_read_from_stream()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            Position pos;
            // read file from resources and write it so that can be read in
            var xml = FileIO.ReadFromXMLInResources<Position>("Resources/fileiotest/file7.xml", assemblyName);
            FileIO.WriteToXML<Position>(xml, "file7.xml");
            using (Stream stream = StreamFinder.GetFileStream("file7.xml", FileMode.Open))
            {
                pos = FileIO.ReadFromStream<Position>(stream);
            }
            Assert.AreEqual(pos.X, 2);
            Assert.AreEqual(pos.Y, 4);
            Assert.AreEqual(pos.Z, 6);
        }

        [Test]
        public void validate_read_from_xml_in_resources()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            var pos = FileIO.ReadFromXMLInResources<Position>("Resources/fileiotest/file7.xml", assemblyName);
            Assert.AreEqual(pos.X, 2);
            Assert.AreEqual(pos.Y, 4);
            Assert.AreEqual(pos.Z, 6);
        }


        [Test]
        public void validate_write_json_to_stream()
        {
            var pos = new Position(2, 4, 6);
            Stream stream = StreamFinder.GetFileStream("file6.txt", FileMode.Create);
            FileIO.WriteJsonToStream(pos, stream);
            var pos2 = FileIO.ReadFromJson<Position>("file6.txt");
            Assert.AreEqual(pos2.X, 2.0);
            Assert.AreEqual(pos2.Y, 4.0);
            Assert.AreEqual(pos2.Z, 6.0);
        }

        [Test]
        public void validate_write_scalar_value_to_binary_and_read_scalar_value_from_binary()
        {
            // write scalar using Action=WriteMap and validate file exists and non-zero length
            var scalar = 11;
            void WriteMap(BinaryWriter b, int s) => b.Write(s);
            FileIO.WriteScalarValueToBinary<int>(scalar, "scalar", WriteMap);
            Assert.IsTrue(FileIO.FileExists("scalar"));
            Assert.IsTrue(new FileInfo("scalar").Length != 0);
            // then read what what written using func ReadMap and validate value
            int ReadMap(BinaryReader b) => b.Read();
            var data = FileIO.ReadScalarValueFromBinary<int>("scalar", ReadMap);
            Assert.AreEqual(data, 11);
        }

        [Test]
        public void validate_write_to_binary_and_read_from_binary()
        {
            double[] array = new double[3] { 4.0, 5.0, 6.0 };
            FileIO.WriteToBinary(array, "array2");
            Assert.IsTrue(FileIO.FileExists("array2"));
            Assert.IsTrue(new FileInfo("array2").Length != 0);
            var data = FileIO.ReadFromBinary<double[]>("array2");
            Assert.AreEqual(data[0], 4.0);
        }

        [Test]
        public void validate_write_2D_array_to_binary_and_read_from_binary()
        {
            var array = new double[,]
            {
                {1D, 2D},
                {3D, 4D}
            };
            FileIO.WriteToBinary(array, "2darray");
            Assert.IsTrue(FileIO.FileExists("2darray"));
            Assert.IsTrue(new FileInfo("2darray").Length != 0);
            var data = FileIO.ReadFromBinary<double[,]>("2darray");
            Assert.AreEqual(data[0, 1], 2D);
        }

        [Test]
        public void validate_write_to_binary_custom()
        {
            IEnumerable<double> arrayWritten = Enumerable.Range(7, 3).Select(x => (double) x);          
            void WriteMap(BinaryWriter b, double s) => b.Write(s);
            FileIO.WriteToBinaryCustom<double>(arrayWritten, "array3", WriteMap);
            Assert.IsTrue(FileIO.FileExists("array3"));
            Assert.IsTrue(new FileInfo("array3").Length != 0);
        }

        [Test]
        public void validate_write_to_binary_stream_and_read_from_binary_stream()
        {
            // first create stream, write array, validate written and close stream
            double[] array = new double[3] { 10, 11, 12 };
            Stream streamWrite = StreamFinder.GetFileStream("array4", FileMode.Create);
            FileIO.WriteToBinaryStream(array, streamWrite);
            Assert.IsNotNull(streamWrite);
            Assert.IsTrue(FileIO.FileExists("array4"));
            Assert.IsTrue(new FileInfo("array4").Length != 0);
            streamWrite.Close();
            // then open stream, read array, validate values and close stream
            Stream streamRead = StreamFinder.GetFileStream("array4", FileMode.Open);
            var data = FileIO.ReadFromBinaryStream<double[]>(streamRead);
            Assert.AreEqual(data[0], 10);
            streamRead.Close();
        }

        [Test]
        public void validate_write_to_json()
        {
            var pos = new Position(2, 4, 6);
            FileIO.WriteToJson(pos, "file7.txt");
            var pos2 = FileIO.ReadFromJson<Position>("file7.txt");
            Assert.AreEqual(pos2.X, 2.0);
            Assert.AreEqual(pos2.Y, 4.0);
            Assert.AreEqual(pos2.Z, 6.0);
        }

        [Test]
        public void validate_write_to_xml_and_read_from_xml()
        {
            var pos = new Position(2, 4, 6);
            FileIO.WriteToXML<Position>(pos, "file7.xml");
            Assert.IsTrue(FileIO.FileExists("file7.xml"));
            Assert.IsTrue(new FileInfo("file7.xml").Length != 0);
            var pos2 = FileIO.ReadFromXML<Position>("file7.xml");
            Assert.AreEqual(pos2.X, 2.0);
            Assert.AreEqual(pos2.Y, 4.0);
            Assert.AreEqual(pos2.Z, 6.0);
        }

        [Test]
        public void validate_write_to_xml_stream()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            var xmlFile = FileIO.ReadFromXMLInResources<Position>("Resources/fileiotest/file7.xml", assemblyName);
            Stream stream = StreamFinder.GetFileStream("file8.xml", FileMode.Create);
            FileIO.WriteToXMLStream(xmlFile, stream);
            Assert.IsNotNull(stream);
            Assert.IsTrue(FileIO.FileExists("file8.xml"));
            Assert.IsTrue(new FileInfo("file8.xml").Length != 0);
            stream.Close();
        }

        [Test]
        public void validate_copy_file_from_embedded_resources()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            FileIO.CopyFileFromEmbeddedResources(assemblyName + ".Resources.fileiotest.embeddedresourcefile.txt", "embeddedresourcefile.txt", name);
            Assert.IsTrue(FileIO.FileExists("embeddedresourcefile.txt"));
        }

        [Test]
        public void validate_copy_binary_file_from_embedded_resources()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            FileIO.CopyFileFromEmbeddedResources(assemblyName + ".Resources.sourcetest.AOfXAndYAndZ", "AOfXAndYAndZ", name);
            Assert.IsTrue(FileIO.FileExists("AOfXAndYAndZ"));
        }

        [Test]
        public void validate_copy_file_from_resources()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            FileIO.CopyFileFromResources("Resources/streamfindertest/resourcefile.txt", "resourcefile.txt", assemblyName); 
            Assert.IsTrue(FileIO.FileExists("resourcefile.txt"));
        }

        [Test]
        public void validate_copy_folder_from_embedded_resources()
        {
            var folder = "fileiotest.folder";
            FileIO.CopyFolderFromEmbeddedResources(folder, "", Assembly.GetExecutingAssembly().FullName, true);
            Assert.IsTrue(FileIO.FileExists(Path.Combine(folder, "embeddedresourcefile.txt")));
        }

        [Test]
        public void validate_copy_folder_containing_binary_from_embedded_resources()
        {
            var folder = "sourcetest";
            FileIO.CopyFolderFromEmbeddedResources(folder, "", Assembly.GetExecutingAssembly().FullName, true);
            Assert.IsTrue(FileIO.FileExists(Path.Combine(folder, "AOfXAndYAndZ")));
            Assert.IsTrue(FileIO.FileExists(Path.Combine(folder, "AOfXAndYAndZ.txt")));
            Assert.IsTrue(FileIO.FileExists(Path.Combine(folder, "input.txt")));
        }

        [Test]
        public void validate_file_exists()
        {
            const string file = "file1.txt";
            FileIO.WriteToTextFile("Text", file);
            Assert.IsTrue(FileIO.FileExists(file));
        }

        [Test]
        public void validate_directory_exists()
        {
            const string folder = "folder1";
            FileIO.CreateDirectory(folder);
            Assert.IsTrue(FileIO.DirectoryExists(folder));
        }

        [Test]
        public void validate_file_delete()
        {
            const string file = "file2.txt";
            FileIO.WriteToTextFile("Text", file);
            FileIO.FileDelete(file);
            Assert.IsFalse(FileIO.FileExists(file));
        }

        [Test]
        public void validate_directory_delete()
        {
            const string folder = "folder3";
            FileIO.CreateDirectory(folder);
            Assert.IsTrue(FileIO.DirectoryExists(folder));
            FileIO.DeleteDirectory(folder);
            Assert.IsFalse(FileIO.DirectoryExists(folder));
        }

        [Test]
        public void validate_write_text_to_file()
        {
            StringBuilder myString = new StringBuilder();
            myString.AppendLine("This is a test string");
            FileIO.WriteToTextFile(myString.ToString(), "file3.txt");
            Assert.IsTrue(FileIO.FileExists("file3.txt"));
        }

        [Test]
        public void validate_write_text_to_stream()
        {
            StringBuilder myString = new StringBuilder();
            myString.AppendLine("This is a test string");
            Stream stream = StreamFinder.GetFileStream("file4.txt", FileMode.Create);
            FileIO.WriteTextToStream(myString.ToString(), stream);
            Assert.IsNotNull(stream);
        }

        /// <summary>
        /// This unit test first verifies WriteArrayToBinary works successfully the
        /// reads back array with ReadArrayFromBinary and checks values
        /// </summary>
        [Test]
        public void validate_write_double_array_to_binary_and_read_array_from_binary()
        {
            var array = new double[3] { 1.0, 2.0, 3.0 };
            FileIO.WriteArrayToBinary(array, "array1", true);
            Assert.IsTrue(FileIO.FileExists("array1"));
            Assert.IsTrue(new FileInfo("array1").Length != 0);
            Assert.IsTrue(FileIO.FileExists("array1.txt"));
            var data = (double[])FileIO.ReadArrayFromBinary<double>("array1", 3);
            Assert.AreEqual(data[0], 1.0);
        }

        /// <summary>
        /// This unit test first verifies WriteArrayToBinary works successfully the
        /// reads back array with ReadArrayFromBinary and checks values
        /// </summary>
        [Test]
        public void validate_write_floating_point_array_to_binary_and_read_array_from_binary()
        {
            var array = new float[3] { 1.0F, 2.0F, 3.0F };
            FileIO.WriteArrayToBinary(array, "floatarray", true);
            Assert.IsTrue(FileIO.FileExists("floatarray"));
            Assert.IsTrue(new FileInfo("floatarray").Length != 0);
            Assert.IsTrue(FileIO.FileExists("floatarray.txt"));
            var data = (float[])FileIO.ReadArrayFromBinary<float>("floatarray", 3);
            Assert.AreEqual(data[0], 1.0F);
        }

        /// <summary>
        /// This unit test first verifies WriteArrayToBinary works successfully the
        /// reads back array with ReadArrayFromBinary and checks values
        /// </summary>
        [Test]
        public void validate_write_complex_array_to_binary_and_read_array_from_binary()
        {
            var array = new Complex[2] { new Complex(0.85, 0.0), new Complex(0.3, 0.0) };
            FileIO.WriteArrayToBinary(array, "complexarray", true);
            Assert.IsTrue(FileIO.FileExists("complexarray"));
            Assert.IsTrue(new FileInfo("complexarray").Length != 0);
            Assert.IsTrue(FileIO.FileExists("complexarray.txt"));
            var data = (Complex[])FileIO.ReadArrayFromBinary<Complex>("complexarray", 2);
            Assert.AreEqual(data[1], new Complex(0.3, 0.0));
        }

        /// <summary>
        /// This unit test first verifies WriteArrayToBinary works successfully the
        /// reads back array with ReadArrayFromBinary and checks values
        /// </summary>
        [Test]
        public void validate_write_ushort_array_to_binary_and_read_array_from_binary()
        {
            var array = new ushort[2] { 5, 7 };
            FileIO.WriteArrayToBinary(array, "ushortarray", true);
            Assert.IsTrue(FileIO.FileExists("ushortarray"));
            Assert.IsTrue(new FileInfo("ushortarray").Length != 0);
            Assert.IsTrue(FileIO.FileExists("ushortarray.txt"));
            var data = (ushort[])FileIO.ReadArrayFromBinary<ushort>("ushortarray", 2);
            Assert.AreEqual(data[1], 7);
        }

        /// <summary>
        /// This unit test first verifies WriteArrayToBinary works successfully the
        /// reads back array with ReadArrayFromBinary and checks values
        /// </summary>
        [Test]
        public void validate_write_byte_array_to_binary_and_read_array_from_binary()
        {
            var array = new byte[5] { 1, 0, 0, 1, 0 };
            FileIO.WriteArrayToBinary(array, "bytearray", true);
            Assert.IsTrue(FileIO.FileExists("bytearray"));
            Assert.IsTrue(new FileInfo("bytearray").Length != 0);
            Assert.IsTrue(FileIO.FileExists("bytearray.txt"));
            var data = (byte[])FileIO.ReadArrayFromBinary<byte>("bytearray", 5);
            Assert.AreEqual(data[1], 0);
        }
    }
}
