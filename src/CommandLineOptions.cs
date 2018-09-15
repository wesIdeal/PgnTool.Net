using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgnTool
{
    interface ICommandLineOptions
    {
        [Value(0, HelpText = "You must supply a path to the pgn file you wish to manipulate.", Required = true)]
        string InputPgnPath { get; set; }

        [Option('o', "output" , HelpText = "You must supply a path to the pgn file you wish to manipulate.", Required = false, Default ="")]
        string OutputPgnPath { get; set; }



    }

    public class CommandLineOptions : ICommandLineOptions
    {
        private string _inputPgnPath = string.Empty;
        public const string invalidPathErrorMessage = "Error: Cannot locate input file specified.";
        public string InputPgnPath
        {
            get
            {
                return _inputPgnPath;
            }

            set
            {
                if (!IsPathValid(value))
                {
                    throw new ArgumentException(invalidPathErrorMessage);
                }
                _inputPgnPath = value;
            }
        }
        public string OutputPgnPath { get; set; }

        
        private bool IsPathValid(string value)
        {
            return File.Exists(value);
        }
    }
    [Verb("crop", HelpText = "Removes all moves. Useful for tactics pgns containing starting positions for each entry.")]
    public class CropOptions : CommandLineOptions
    {
        private int _movesToKeep = 0;
        [Option('m', "moves", Default = 0, HelpText = "Moves to keep in each entry. This keeps n  moves, cropping the rest.", Required = false)]
        public int MovesToKeep { get { return _movesToKeep; } set { _movesToKeep = value; } }
    }
}
