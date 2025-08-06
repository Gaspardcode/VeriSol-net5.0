
namespace SolToBoogieTest
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using Microsoft.Extensions.Logging;
    using VeriSolRunner.ExternalTools;
    using System.Collections.Generic;

    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "test-solidity05")
            {
                // Run Solidity 0.5.0 test suite
                var testRunner = new Solidity05TestRunner();
                testRunner.RunAllTests();
                return 0;
            }

            // Original regression test logic
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: dotnet run <testDirectory> <configDirectory> [testPrefix]");
                Console.WriteLine("   or: dotnet run test-solidity05");
                return 1;
            }

            string testDirectory = args[0];
            string configDirectory = args[1];
            string testPrefix = args.Length > 2 ? args[2] : "";

            ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger logger = loggerFactory.CreateLogger("VeriSol");

            RegressionExecutor executor = new RegressionExecutor(testDirectory, configDirectory, testDirectory, logger, testPrefix);
            return executor.BatchExecute();
        }
        
    }
}
