namespace VeriSolRunner.ExternalTools
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    public class ToolSourceSettings
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public Dictionary<string, string> DownloadURLs { get; set; }
        public Dictionary<string, string> ExePathsWithinZip { get; set; }
        public string DependencyRelativePath { get; set; }  = GetDefaultPath();
        public string CommandPath { get; set; } = GetDefaultPath();

        private static string GetDefaultPath()
        {
            var assemblyPath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(ToolSourceSettings)).Location);
            if (assemblyPath == null)
            {
                // Fallback to current directory if assembly path is null
                return Directory.GetCurrentDirectory();
            }
            
            // Check if the path contains .store
            if (assemblyPath.Contains(".store"))
            {
                return assemblyPath.Split(new string[] { @".store" }, StringSplitOptions.None)[0];
            }
            else
            {
                // If no .store, just use the assembly directory
                return assemblyPath;
            }
        }
    }
}
