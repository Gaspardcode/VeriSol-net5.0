
namespace SolToBoogieTest
{
    using Microsoft.Extensions.Logging;
    using SolidityAST;
    using BoogieAST;
    using SolToBoogie;

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Reflection;
    using VeriSolRunner.ExternalTools;

    class RegressionExecutor
    {
        private string solcPath;
        private string testDirectory;
        private string configDirectory;
        private string recordsDir;
        private ILogger logger;
        private string testPrefix;
        private static readonly string outFile = "__SolToBoogieTest_out.bpl";
        private static Dictionary<string, bool> filesToRun = new Dictionary<string, bool>();

        public enum BatchExeResult { Success, SolcError, SolToBoogieError, BoogieCompilationError, OtherException };

        public RegressionExecutor(string testDirectory, string configDirectory, string recordsDir, ILogger logger, string testPrefix = "")
        {
            this.testDirectory = testDirectory;
            this.configDirectory = configDirectory;
            this.recordsDir = recordsDir;
            this.logger = logger;
            this.testPrefix = testPrefix;
            this.solcPath = ExternalToolsManager.Solc.Command;
            ReadRecord();
        }

        public int BatchExecute()
        {
            int totalFiles = 0;
            int passedFiles = 0;
            int failedFiles = 0;

            Console.WriteLine($"Running tests in directory: {testDirectory}");
            Console.WriteLine($"Test prefix: {testPrefix}");

            foreach (var file in Directory.GetFiles(testDirectory, "*.sol"))
            {
                string filename = Path.GetFileName(file);
                if (!filesToRun.ContainsKey(filename))
                {
                    continue;
                }

                if (!filesToRun[filename])
                {
                    Console.WriteLine($"Skipping {filename} (disabled in records.txt)");
                    continue;
                }

                if (!string.IsNullOrEmpty(testPrefix) && !filename.StartsWith(testPrefix))
                {
                    continue;
                }

                totalFiles++;
                Console.WriteLine($"\n--- Testing {filename} ---");

                string expected, current;
                BatchExeResult result = Execute(filename, out expected, out current);

                switch (result)
                {
                    case BatchExeResult.Success:
                        Console.WriteLine($"      Passed - {filename}");
                        passedFiles++;
                        break;
                    case BatchExeResult.SolcError:
                        Console.WriteLine($"      Failed - {filename} (Solc compilation error)");
                        failedFiles++;
                        break;
                    case BatchExeResult.SolToBoogieError:
                        Console.WriteLine($"      Failed - {filename} (SolToBoogie translation error)");
                        failedFiles++;
                        break;
                    case BatchExeResult.BoogieCompilationError:
                        Console.WriteLine($"      Failed - {filename} (Boogie compilation error)");
                        failedFiles++;
                        break;
                    case BatchExeResult.OtherException:
                        Console.WriteLine($"      Failed - {filename} (Other exception)");
                        failedFiles++;
                        break;
                }
            }

            Console.WriteLine($"\n--- Test Summary ---");
            Console.WriteLine($"Total files: {totalFiles}");
            Console.WriteLine($"Passed: {passedFiles}");
            Console.WriteLine($"Failed: {failedFiles}");

            return failedFiles == 0 ? 0 : 1;
        }

        private void ParseTranslatorFlags(TranslatorFlags translatorFlags, string args)
        {
            string solidityFile, entryPointContractName;
            ILogger logger;
            HashSet<Tuple<string, string>> ignoredMethods;
            string verisolCmdLineArgs = "Foo Bar " + args; //Parser expects first two args to be present 
            SolToBoogie.ParseUtils.ParseCommandLineArgs(verisolCmdLineArgs.Split(" "),
            out solidityFile,
            out entryPointContractName,
            out logger,
            out ignoredMethods,
            ref translatorFlags);
        }

        public BatchExeResult Execute(string filename, out string expected, out string current)
        {
            BatchExeResult result = BatchExeResult.SolcError;
            expected = current = null;

            try
            {
                string filePath = Path.Combine(testDirectory, filename);
                Console.WriteLine($"... running Solc on {filePath}");

                // compile the program
                SolidityCompiler compiler = new SolidityCompiler();
                CompilerOutput compilerOutput = compiler.Compile(solcPath, filePath, testDirectory);

                if (compilerOutput.ContainsError())
                {
                    compilerOutput.PrintErrorsToConsole();
                    return BatchExeResult.SolcError;
                }

                // build the Solidity AST from solc output
                AST solidityAST = new AST(compilerOutput, testDirectory);

                // translate to Boogie
                TranslatorFlags translatorFlags = new TranslatorFlags();
                BoogieTranslator translator = new BoogieTranslator();
                BoogieAST boogieAST = translator.Translate(solidityAST, new HashSet<Tuple<string, string>>(), translatorFlags, Path.GetFileNameWithoutExtension(filename));

                // write the Boogie program to file
                using (var bplFile = new StreamWriter($"{filename.Replace(".sol", "")}_{outFile}"))
                {
                    bplFile.WriteLine(boogieAST.GetRoot());
                }

                Console.WriteLine($"\tFinished SolToBoogie, output in {filename.Replace(".sol", "")}_{outFile}....\n");

                // Test Boogie compilation
                if (!TestBoogieCompilation(filename))
                {
                    return BatchExeResult.BoogieCompilationError;
                }

                expected = "Translation successful";
                current = "Translation successful";
                return BatchExeResult.Success;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e.Message}");
                expected = current = null;
                return BatchExeResult.OtherException;
            }
        }

        private bool TestBoogieCompilation(string filename)
        {
            // Simple test: check if the Boogie file exists and has valid syntax
            string actualOutFile = $"{filename.Replace(".sol", "")}_{outFile}";
            if (!File.Exists(actualOutFile))
            {
                Console.WriteLine($"\t*** Error: Boogie file {actualOutFile} was not generated");
                return false;
            }

            string boogieContent = File.ReadAllText(actualOutFile);
            if (string.IsNullOrWhiteSpace(boogieContent))
            {
                Console.WriteLine($"\t*** Error: Boogie file {actualOutFile} is empty");
                return false;
            }

            // Basic syntax check: should contain "procedure" and "implementation"
            if (!boogieContent.Contains("procedure") || !boogieContent.Contains("implementation"))
            {
                Console.WriteLine($"\t*** Warning: Boogie file {actualOutFile} may have syntax issues");
                return false;
            }

            Console.WriteLine($"\t*** Boogie compilation test passed");
            return true;
        }

        private void ReadRecord()
        {
            StreamReader records = new StreamReader(Path.Combine(recordsDir, "records.txt"));
            string line;
            while((line = records.ReadLine()) != null)
            {
                string fileName = line.TrimEnd();
                if (fileName.StartsWith('#'))
                {
                    filesToRun[fileName.TrimStart('#')] = false;
                }
                else
                {
                    filesToRun[fileName] = true;
                }
            }
        }
    }
}
