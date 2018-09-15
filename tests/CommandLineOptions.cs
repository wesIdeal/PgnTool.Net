using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace PgnTool.Net.Tests
{
    [TestClass]
    public class CommandLineOptionsTest
    {
        string directoryName = ".\\tmp.";
        const string validFileName = "BCE9.tmp";
        const string invalidFileName = "F31534AEFF14.tmp";
        private string GetFullPath(string fileName) => directoryName + "\\" + fileName;
        protected string GetValidPath => GetFullPath(validFileName);
        public string GetInvalidPath => GetFullPath(invalidFileName);
        [TestInitialize]
        public void Initialize()
        {
            directoryName = directoryName + Guid.NewGuid().ToString();
            Directory.CreateDirectory(directoryName);
            File.WriteAllText(GetValidPath, "hello.");
        }
        [TestCleanup]
        public void Cleanup()
        {
            foreach (var dir in Directory.EnumerateDirectories(".", "tmp.*"))
            {
                if (Directory.Exists(dir))
                {
                    foreach (var file in Directory.EnumerateFiles(dir))
                    {
                        File.Delete(file);
                    }
                    Directory.Delete(dir);
                }
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException), CommandLineOptions.invalidPathErrorMessage)]

        public void TestIsPathValid_InvalidPath()
        {
            var clOpt = new CommandLineOptions();
            clOpt.InputPgnPath = GetInvalidPath;
        }
        [TestMethod]


        public void TestIsPathValid_ValidPath()
        {
            var clOpt = new CommandLineOptions();
            clOpt.InputPgnPath = GetValidPath;
            Assert.AreEqual(clOpt.InputPgnPath, GetValidPath);
        }
    }
}
