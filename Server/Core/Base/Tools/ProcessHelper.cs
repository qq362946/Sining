using System;
using System.Diagnostics;
using System.IO;

namespace Sining.Tools
{
    public static class ProcessHelper
    {
        public static Process Run(string fileName, string arguments, string workingDirectory = null,
            bool waitExit = false)
        {
            try
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

                if (!waitExit)
                {
                    startInfo.RedirectStandardOutput = false;
                    startInfo.RedirectStandardError = false;
                }

                var process = Process.Start(startInfo);
             
                if (!waitExit) return process;
                
                process?.WaitForExit();

                if (process?.ExitCode != 0)
                {
                    throw new Exception(
                        $"{process?.StandardOutput.ReadToEnd()} {process?.StandardError.ReadToEnd()}");
                }

                return process;
            }
            catch (Exception e)
            {
                throw new Exception($"dir: {Path.GetFullPath(workingDirectory)}, command: {fileName} {arguments}", e);
            }
        }
    }
}