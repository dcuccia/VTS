using System.Collections.Generic;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Detectors;

namespace Vts.MonteCarlo
{
    public class PostProcessorInput
    {
        /// <summary>
        /// IList of IDetectorInput
        /// </summary>
        public IList<IDetectorInput> DetectorInputs;
        /// <summary>
        /// string input folder
        /// </summary>
        public string InputFolder;
        /// <summary>
        /// string identifying database SimulationInput filename
        /// </summary>
        public string DatabaseSimulationInputFilename;
        /// <summary>
        /// string identifying output filename
        /// </summary>
        public string OutputName;

        /// <summary>
        /// constructor for post-processor input
        /// </summary>
        /// <param name="detectorInputs">list of detector inputs</param>
        /// <param name="inputFolder">input folder name, where database file(s), etc. reside</param>
        /// <param name="databaseSimulationInputFilename">filename of simulation input file that generated database to be post-processed</param>
        /// <param name="outputName"></param>
        public PostProcessorInput(
            IList<IDetectorInput> detectorInputs,
            string inputFolder,
            string databaseSimulationInputFilename,
            string outputName)
        {
            DetectorInputs = detectorInputs;
            InputFolder = inputFolder;
            DatabaseSimulationInputFilename = databaseSimulationInputFilename;
            OutputName = outputName;
        }

        /// <summary>
        /// default constructor
        /// </summary>
        public PostProcessorInput()
            : this(
                //VirtualBoundaryType.DiffuseReflectance,
                new List<IDetectorInput>
                    {
                        new ROfRhoDetectorInput
                        {
                            Rho = new DoubleRange(0.0, 40.0, 201), // rho: nr=200 dr=0.2mm used for workshop)
                        }
                    },
                "results",
                "infile",
                "ppresults"
                ) {}

        /// <summary>
        /// Method to read this class from JSON file.
        /// </summary>
        /// <param name="filename">string file name</param>
        /// <returns>PostProcessorInput</returns>
        public static PostProcessorInput FromFile(string filename)
        {
            return FileIO.ReadFromJson<PostProcessorInput>(filename);
        }
        /// <summary>
        /// Method to write this class to JSON file.
        /// </summary>
        /// <param name="filename">string file name</param>
        public void ToFile(string filename)
        {
            FileIO.WriteToJson(this, filename);
        }
        /// <summary>
        /// Method to read this class from file in Resources
        /// </summary>
        /// <param name="filename">filename to be read</param>
        /// <param name="project">project where file resides</param>
        /// <returns>PostProcessorInput</returns>
        public static PostProcessorInput FromFileInResources(string filename, string project)
        {
            return FileIO.ReadFromJsonInResources<PostProcessorInput>(filename, project);
        }
    }
}
