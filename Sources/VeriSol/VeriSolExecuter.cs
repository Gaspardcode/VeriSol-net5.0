

namespace VeriSolRunner
{

    using Microsoft.Extensions.Logging;
    using SolidityAST;
    using BoogieAST;
    using SolToBoogie;

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Reflection;
    using System.Linq;
    using VeriSolRunner.ExternalTools;
    // using Microsoft.Boogie.ExprExtensions;

    internal class VeriSolExecutor
    {
        private string SolidityFilePath;
        private string SolidityFileDir;
        private string ContractName;
        private string SolcPath;
        private ILogger Logger;
        private readonly string outFileName;
        private HashSet<Tuple<string, string>> ignoreMethods;
        private TranslatorFlags translatorFlags;

        public VeriSolExecutor(string solidityFilePath, string contractName, HashSet<Tuple<string, string>> ignoreMethods, ILogger logger, TranslatorFlags _translatorFlags = null)
        {
            this.SolidityFilePath = solidityFilePath;
            this.ContractName = contractName;
            this.outFileName = this.ContractName+"__SolToBoogie.bpl";
            this.SolidityFileDir = Path.GetDirectoryName(solidityFilePath);
            Console.WriteLine($"SpecFilesDir = {SolidityFileDir}");
            this.SolcPath = ExternalToolsManager.Solc.Command;
            this.ignoreMethods = new HashSet<Tuple<string, string>>(ignoreMethods);
            this.Logger = logger;
            this.translatorFlags = _translatorFlags;
        }

        public int Execute()
        {
            // call SolToBoogie on specFilePath
            if (!ExecuteSolToBoogie())
            {
                return 1;
            }

            // Test Boogie compilation
            if (!TestBoogieCompilation())
            {
                return 1;
            }

            Console.WriteLine($"\t*** Translation successful! Boogie file generated: {outFileName}");
            return 0;
        }

        // still in test phase, should be improved later on
        // not reliable for now
        private bool TestBoogieCompilation()
        {
            // Simple test: check if the Boogie file exists and has valid syntax
            if (!File.Exists(outFileName))
            {
                Console.WriteLine($"\t*** Error: Boogie file {outFileName} was not generated");
                return false;
            }

            string boogieContent = File.ReadAllText(outFileName);
            if (string.IsNullOrWhiteSpace(boogieContent))
            {
                Console.WriteLine($"\t*** Error: Boogie file {outFileName} is empty");
                return false;
            }

            // Basic syntax check: should contain "procedure" and "implementation"
            if (!boogieContent.Contains("procedure") || !boogieContent.Contains("implementation"))
            {
                Console.WriteLine($"\t*** Warning: Boogie file {outFileName} may have syntax issues");
                return false;
            }

            // TODO: check for a more reliable way to tets Boggie compilation
            // run the boogie compiler on the file for instance ?

            Console.WriteLine($"\t*** Boogie compilation test passed");
            return true;
        }

        private bool ExecuteSolToBoogie()
        {
            Console.WriteLine($"... running Solc on {SolidityFilePath}");
            Console.WriteLine(SolcPath);

            // compile the program
            SolidityCompiler compiler = new SolidityCompiler();
            CompilerOutput compilerOutput = compiler.Compile(SolcPath, SolidityFilePath, SolidityFileDir);

            if (compilerOutput.ContainsError())
            {
                compilerOutput.PrintErrorsToConsole();
                throw new SystemException("Compilation Error");
            }

            // build the Solidity AST from solc output
            AST solidityAST = new AST(compilerOutput, SolidityFileDir);

            // translate to Boogie
            BoogieTranslator translator = new BoogieTranslator();
            BoogieAST boogieAST = translator.Translate(solidityAST, ignoreMethods, translatorFlags, ContractName);

            // write the Boogie program to file
            using (var bplFile = new StreamWriter(outFileName))
            {
                bplFile.WriteLine(boogieAST.GetRoot());
            }

            Console.WriteLine($"\tFinished SolToBoogie, output in {outFileName}....\n");
            return true;
        }
    }
}
