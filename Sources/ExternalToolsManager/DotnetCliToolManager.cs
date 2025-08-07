namespace VeriSolRunner.ExternalTools
{
    using System;
    using System.Diagnostics;
    using System.IO;

    internal class DotnetCliToolManager : ToolManager
    {
        internal string DependencyTargetPath
        {
            get
            {
                return Path.Combine(this.settings.CommandPath, this.settings.DependencyRelativePath);
            }
        }

        internal DotnetCliToolManager(ToolSourceSettings settings) : base(settings)
        {
        }

        internal override void EnsureExisted()
        {
            EnsureCommandPathExisted();
            if (!Exists())
            {
                InstallDotnetCliTool();
            }
            else
            {
                ExternalToolsManager.Log($"Skip installing tool {this.settings.Name} as we could find it under {this.settings.CommandPath}.");
            }
        }

        private string InstallDotnetCliTool()
        {
            var logStr = $"... Installing {this.settings.Name} as we could not find it from {this.settings.CommandPath}.";
            Console.WriteLine(logStr); // until we have better verbosity for printing
            ExternalToolsManager.Log(logStr);

            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.FileName = "dotnet";
            p.StartInfo.Arguments = $"tool install {this.settings.Name} --tool-path {this.settings.CommandPath} --version {this.settings.Version}";
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            string errorMsg = p.StandardError.ReadToEnd();

            p.StandardOutput.Close();
            p.StandardError.Close();

            if (!String.IsNullOrEmpty(errorMsg))
            {
                ExternalToolsManager.Log($"Installation failed: {errorMsg}");
            }
            else
            {
                ExternalToolsManager.Log("Done.");
            }

            return output;
        }

    }
}