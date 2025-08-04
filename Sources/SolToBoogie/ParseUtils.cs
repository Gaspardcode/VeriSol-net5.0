using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using BoogieAST;

namespace SolToBoogie
{
    /// <summary>
    /// Helper class for parsing command line arguments
    /// </summary>
    public static class ParseUtils
    {
        // TODO: extract into a VerificationFlags structure 
        public static void ParseCommandLineArgs(string[] args, out string solidityFile, out string entryPointContractName, out ILogger logger, out HashSet<Tuple<string, string>> ignoredMethods, ref TranslatorFlags translatorFlags)
        {
            //Console.WriteLine($"Command line args = {{{string.Join(", ", args.ToList())}}}");
            solidityFile = args[0];
            // Debug.Assert(!solidityFile.Contains("/"), $"Illegal solidity file name {solidityFile}"); //the file name can be foo/bar/baz.sol
            entryPointContractName = args[1];
            Debug.Assert(!entryPointContractName.Contains("/"), $"Illegal contract name {entryPointContractName}");

            ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole()); //  new LoggerFactory().AddConsole(LogLevel.Information);
            logger = loggerFactory.CreateLogger("VeriSol");
            ignoredMethods = new HashSet<Tuple<string, string>>();
            foreach (var arg in args.Where(x => x.StartsWith("/ignoreMethod:")))
            {
                Debug.Assert(arg.Contains("@"), $"Error: incorrect use of /ignoreMethod in {arg}");
                Debug.Assert(arg.LastIndexOf("@") == arg.IndexOf("@"), $"Error: incorrect use of /ignoreMethod in {arg}");
                var str = arg.Substring("/ignoreMethod:".Length);
                var method = str.Substring(0, str.IndexOf("@"));
                var contract = str.Substring(str.IndexOf("@") + 1);
                ignoredMethods.Add(Tuple.Create(method, contract));
            }

            foreach (var arg in args.Where(x => x.StartsWith("/SliceFunctions:")))
            {
                var str = arg.Substring("/SliceFunctions:".Length);
                String[] fns = str.Split(",");
                translatorFlags.PerformFunctionSlice = true;
                foreach (String fn in fns)
                {
                    translatorFlags.SliceFunctionNames.Add(fn);
                }
            }
            
            if (args.Any(x => x.StartsWith("/ignoreMethod:")))
            {
                Console.WriteLine($"Ignored method/contract pairs ==> \n\t {string.Join(",", ignoredMethods.Select(x => x.Item1 + "@" + x.Item2))}");
            }
            translatorFlags.GenerateInlineAttributes = true;
            if (args.Any(x => x.Equals("/noInlineAttrs")))
            {
                translatorFlags.GenerateInlineAttributes = false;
            }
            if (args.Any(x => x.Equals("/break")))
            {
                Debugger.Launch();
            }
            if (args.Any(x => x.Equals("/omitSourceLineInfo")))
            {
                translatorFlags.NoSourceLineInfoFlag = true;
            }
            if (args.Any(x => x.Equals("/omitDataValuesInTrace")))
            {
                translatorFlags.NoDataValuesInfoFlag = true;
            }
            if (args.Any(x => x.Equals("/useModularArithmetic")))
            {
                translatorFlags.UseModularArithmetic = true;
            }
            if (args.Any(x => x.Equals("/omitUnsignedSemantics")))
            {
                translatorFlags.NoUnsignedAssumesFlag = true;
            }
            if (args.Any(x => x.Equals("/omitAxioms")))
            {
                translatorFlags.NoAxiomsFlag = true;
            }
            if (args.Any(x => x.Equals("/omitHarness")))
            {
                translatorFlags.NoHarness = true;
            }

            if (args.Any(x => x.Equals("/omitBoogieHarness")))
            {
                translatorFlags.NoBoogieHarness = true;
            }

            if (args.Any(x => x.Equals("/createMainHarness")))
            {
                translatorFlags.CreateMainHarness = true;
            }

            if (args.Any(x => x.Equals("/noCustomTypes")))
            {
                translatorFlags.NoCustomTypes = true;
            }

            if (args.Any(x => x.Equals("/modelReverts")))
            {
                translatorFlags.ModelReverts = true;
            }

            if (args.Any(x => x.Equals("/instrumentGas")))
            {
                translatorFlags.InstrumentGas = true;
            }

            var stubModels = args.Where(x => x.StartsWith("/stubModel:"));
            if (stubModels.Count() > 0)
            {
                Debug.Assert(stubModels.Count() == 1, $"At most 1 /stubModel:s expected, found {stubModels.Count()}");
                var stubModel = stubModels.First().Substring("/stubModel:".Length);
                Debug.Assert(stubModel.Equals("skip") || stubModel.Equals("havoc") || stubModel.Equals("callback"), $"Argument of /stubModel:s should be skip, havoc, or callback, found {stubModel}");
                translatorFlags.ModelOfStubs = stubModel;
            }
        }

    }
}
