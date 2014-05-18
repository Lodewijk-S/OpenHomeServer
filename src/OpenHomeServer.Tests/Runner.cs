using System;
using System.Diagnostics;
using System.IO;

namespace OpenHomeServer.Tests
{
    public static class Runner
    {
        public static void Main(params string[] args)
        {
            var fixie = new Process();
            fixie.StartInfo.FileName = @"fixie.console.exe";
            fixie.StartInfo.UseShellExecute = false;
            fixie.StartInfo.RedirectStandardInput = true;
            fixie.StartInfo.Arguments = Path.Combine(typeof(Runner).Assembly.Location).ToString();
            fixie.Start();
            fixie.WaitForExit();

            if (fixie.ExitCode != 0)
            {
                Console.ReadKey();
            }
        }
    }
}
