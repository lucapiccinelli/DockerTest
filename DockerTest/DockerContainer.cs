using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Xunit.Abstractions;

namespace DockerTest
{
    public class DockerContainer
    {
        private readonly ITestOutputHelper _output;
        public DockerRunParams Parameters { get; }
        private readonly ProcessStartInfo _processInfo;
        private readonly string _dockerExecutable;
        private string _containerId;

        public DockerContainer(DockerRunParams parameters, ITestOutputHelper output = null)
        {
            _output = output;
            Parameters = parameters;
            _containerId = string.Empty;
            _dockerExecutable = GetPath("docker.exe");
            var arguments = parameters.Get();
            _processInfo = new ProcessStartInfo(_dockerExecutable, arguments)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
        }

        public string Run()
        {
            var process = Process.Start(_processInfo);
            process?.WaitForExit();
            _containerId = process?.StandardOutput.ReadLine() ?? string.Empty;
            return _containerId;
        }

        public void Stop()
        {
            ProcessStartInfo info = new ProcessStartInfo(_dockerExecutable, $"rm -f {_containerId}")
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };
            Process.Start(info)?.WaitForExit();
        }

        public string Log(bool outputErrors = false) => LogCommand($"logs {_containerId}", outputErrors);

        public string LogCommand(string command, bool outputErrors = false)
        {
            ProcessStartInfo info = new ProcessStartInfo(_dockerExecutable, command)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = outputErrors
            };
            var process = Process.Start(info);
            int attempt = 0;
            int maxAttempsts = 150;
            while ((!process?.HasExited ?? false) && attempt++ < maxAttempsts)
            {
                Thread.Sleep(10);
            }

            if (attempt > maxAttempsts)
            {
                process?.Kill();
                return "";
            }

            StringBuilder output = new StringBuilder(process?.StandardOutput.ReadToEnd());
            if (outputErrors)
            {
                output.Append(process?.StandardError.ReadToEnd());
            }
            process?.WaitForExit();
            string log = output.ToString();
            _output?.WriteLine(log);
            return log;
        }

        private string GetPath(string executableName) =>
            Environment.GetEnvironmentVariable("PATH")
                ?.Split(';')
                .Select(x => Path.Combine(x, executableName))
                .FirstOrDefault(File.Exists)
            ?? string.Empty;

        public void WaitForLog(string command, string logValue, int times = 1, long timeout = 60000)
        {
            var attempRetrySleep = 1000;
            while (!ContainsTimes(command, logValue, times))
            {
                _output?.WriteLine(command);
                Thread.Sleep(attempRetrySleep);
            }
            _output?.WriteLine($"Found: {logValue}, {times} times");
        }

        private bool ContainsTimes(string command, string logValue, int times) => 
            LogCommand(command)
                .Split('\n')
                .Count(line => line.Contains(logValue)) == times;
    }
}