
namespace VeriSolRunner.ExternalTools
{
    using System;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    public static class ExternalToolsManager
    {
        private static ILogger logger;

        public static ToolManager Solc { get; private set; }

        static ExternalToolsManager()
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole()); // AddConsole(LogLevel.Information)
            logger = loggerFactory.CreateLogger("VeriSol.ExternalToolsManager");

            IConfiguration toolSourceConfig = new ConfigurationBuilder()
                .AddJsonFile("toolsourcesettings.json", true, true)
                .Build();

            var solcSourceSettings = new ToolSourceSettings();
            toolSourceConfig.GetSection("solc").Bind(solcSourceSettings);
            
            // Debug output
            Console.WriteLine($"Solc settings - Name: {solcSourceSettings.Name}, CommandPath: {solcSourceSettings.CommandPath}");
            
            Solc = new SolcManager(solcSourceSettings);
        }

        internal static void Log(string v)
        {
            logger.LogDebug(v);
        }

        public static void EnsureAllExisted()
        {
            Solc.EnsureExisted();
        }
    }
}
