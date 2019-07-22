using Jil;
using Lib.AppConfig;
using Lib.Logger;
using MorseCode.ITask;
using RunProcessAsTask;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.RClone
{
    public class RClone : IRClone
    {

        #region Properties

        protected readonly ILogWriter _log;

        public string RClonePath
        {
            get
            {
                return _rclonePath;
            }
        }
        protected readonly string _rclonePath;

        #endregion

        public RClone(ILogWriter log, string rcloneExePath)
        {
            _log = log;
            _rclonePath = rcloneExePath;
        }

        public Process StartDaemon(string logFilePath, IEnumerable<string> includes, IEnumerable<string> excludes)
        {
            var filterFile = GenTempFilterFile(includes, excludes);
            var rcloneArguments = $"rcd --rc-no-auth --log-level INFO --log-format '' --filter-from {filterFile} --log-file {logFilePath} --ignore-case --transfers 2";
            //--include \"{includeFiles}\"
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = _rclonePath;
            startInfo.Arguments = rcloneArguments;
            startInfo.CreateNoWindow = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;




            Process process = new Process();
            process.StartInfo = startInfo;
            process.EnableRaisingEvents = true;

            process.Start();
            _log.WriteLine($"Running: rclone {rcloneArguments}");

            return process;
        }

        private string GenTempFilterFile(IEnumerable<string> includes, IEnumerable<string> excludes)
        {
            var path = Path.GetTempFileName();
            var excludesPrefixed = excludes.Select(x => $"- {x}");
            var includesPrefixed = includes.Select(x => $"+ {x}");

            // Warning, the order of rules maters

            var lines = Enumerable.Concat(excludesPrefixed, includesPrefixed).ToList();
            
            // In the end forbid the rules-uncovered files
            lines.Add("- *");

            File.WriteAllLines(path, lines);
            return path;
        }

        public ITask<bool> IsDaemonRunning()
        {
            throw new NotImplementedException();
        }

        public async ITask KillDaemonIfExists()
        {

            try
            {
                var resp = await JSonRcCommand("core/pid");
                int pid = resp.pid;

                _log.WriteLine($"A non closed instance of rclone was found on pid: {pid}");
                Process.GetProcessById(pid).Kill();
            }
            catch { }
        }

        protected async ITask<string> JSonRcCommandString(string operation, object opParams = null)
        {
            // Handle json params
            var jsonOpParams = "{}";

            if (!(opParams is null))
            {
                using (var output = new StringWriter())
                {
                    Jil.JSON.Serialize(
                        opParams,
                        output
                    );
                    jsonOpParams = output.ToString().Replace("\"", "\\\"");
                }
            }


            var startInfo = new ProcessStartInfo();
            startInfo.FileName = _rclonePath;
            startInfo.Arguments = $"rc --json {jsonOpParams} {operation}";
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;

            var res = await ProcessEx.RunAsync(startInfo);


            if (res.ExitCode != 0)
            {
                var err =  res.StandardError;
                _log.WriteLine(err.First());
                throw new Exception($"Rclone rc command with args '{res.Process.StartInfo.Arguments}' failled", new Exception(String.Join(Environment.NewLine, res.StandardError)));
            }

            var strOutput = String.Join("",res.StandardOutput);

            return strOutput;
        }

        public async ITask<T> JSonRcCommand<T>(string operation, object opParams = null)
        {
            var json = await JSonRcCommandString(operation, opParams);
            var resp = Jil.JSON.Deserialize<T>(json, new Jil.Options(serializationNameFormat: Jil.SerializationNameFormat.CamelCase, dateFormat: DateTimeFormat.ISO8601));
            return resp;
        }

        public async ITask<dynamic> JSonRcCommand(string operation, object opParams = null)
        {

            var json = await JSonRcCommandString(operation, opParams);
            var resp = Jil.JSON.DeserializeDynamic(json, new Jil.Options(dateFormat: DateTimeFormat.ISO8601));
            return resp;
        }

        public Process ShowRCloneConfig()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = _rclonePath;
            startInfo.Arguments = " config";

            startInfo.WindowStyle = ProcessWindowStyle.Normal;



            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
            return process;
        }

    }
}
