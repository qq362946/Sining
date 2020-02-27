using System;
using System.Diagnostics;

namespace Sining.ProtoBufTool
{
    public static class ProcessHelper
    {
        public static void Run(string fileName, string arguments,string workingDirectory = null, bool waitExit = true)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                WorkingDirectory = workingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            try
            {
                var process = Process.Start(startInfo);

                if (process == null || !waitExit) return;

                process.WaitForExit();
                
                if (process.ExitCode != 0)
                {
                    throw new Exception($"{process.StandardOutput.ReadToEnd()} {process.StandardError.ReadToEnd()}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}