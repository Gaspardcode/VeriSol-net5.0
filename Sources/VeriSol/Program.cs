
namespace VeriSolRunner
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using Microsoft.Extensions.Logging;
    using SolToBoogie;
    using VeriSolRunner.ExternalTools;

    /// <summary>
    /// Top level application to run VeriSol for Solidity to Boogie translation
    /// </summary>
    class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length < 2)
            {
                ShowUsage();
                return 1;
            }

            ExternalToolsManager.EnsureAllExisted();

            string solidityFile, entryPointContractName;
            ILogger logger;
            HashSet<Tuple<string, string>> ignoredMethods;
            TranslatorFlags translatorFlags = new TranslatorFlags();
            SolToBoogie.ParseUtils.ParseCommandLineArgs(args,
                out solidityFile,
                out entryPointContractName,
                out logger,
                out ignoredMethods,
                ref translatorFlags);

            var verisolExecuter =
                new VeriSolExecutor(
                    Path.Combine(Directory.GetCurrentDirectory(), solidityFile), 
                    entryPointContractName,
                    ignoredMethods,
                    logger,
                    translatorFlags);
            return verisolExecuter.Execute();
        }


        private static void ShowUsage()
        {
            Console.WriteLine("VeriSol: Solidity to Boogie translation tool");
            Console.WriteLine("Usage:  VeriSol <relative-path-to-solidity-file> <top-level-contractName> [options]");
            Console.WriteLine("options:");

            Console.WriteLine("\n------ Controls translation --------\n");

            Console.WriteLine("   /ignoreMethod:<method>@<contract>: Ignores translation of the method within contract, and only generates a declaration");
            Console.WriteLine("                           multiple such pairs can be specified, ignored set is the union");
            Console.WriteLine("                           a wild card '*' can be used for method, would mean all the methods of the contract");
            Console.WriteLine("   /noInlineAttrs          do not generate any {:inline x} attributes");
            Console.WriteLine("   /stubModel:<s>          the model of an unknown procedure or fallback. <s> can be either");
            Console.WriteLine("                           skip      // treated as noop");
            Console.WriteLine("                           havoc     // completely scramble the entire global state");
            Console.WriteLine("                           callback  // treated as a non-deterministic callback into any of the methods of any contract");
            Console.WriteLine("   /useModularArithmetic   uses modular arithmetic for unsigned integers (experimental), default unbounded integers");
        }

        private static string GetSolcNameByOSPlatform()
        {
            string solcName = null;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                solcName = "solc.exe";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                solcName = "solc-static-linux";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                solcName = "solc";
            }
            else
            {
                throw new SystemException("Cannot recognize OS platform");
            }
            return solcName;
        }
    }
}
