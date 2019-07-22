using Lib.Logger;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lib.Tools.LiveFileReader
{
    public class LiveFileReader : ILiveFileReader
    {
        protected readonly string _filePath;

        public event EventHandler<string> OnNewLine = delegate { };

        protected BackgroundWorker _backgroundWorker = new BackgroundWorker();

        protected readonly ILogWriter _logWriter;

        public LiveFileReader(ILogWriter logWriter, string filePath)
        {
            _logWriter = logWriter;
            _filePath = filePath;

            _backgroundWorker.WorkerReportsProgress = true;
            _backgroundWorker.ProgressChanged += (sender, arg) =>
            {
                string logLine = arg.UserState as string;
                HandleNewLine(logLine);
                OnNewLine(null, logLine);
            };

            _backgroundWorker.DoWork += (e, a) =>
            {
                ReadNewLines();
            };
        }

        protected void ReadNewLines()
        {
            var wh = new AutoResetEvent(false);
            var fsw = new FileSystemWatcher(".");
            fsw.Filter = "file-to-read";
            fsw.EnableRaisingEvents = true;
            fsw.Changed += (s, e) => wh.Set();

            var fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using (var sr = new StreamReader(fs))
            {
                var s = "";
                while (true)
                {
                    s = sr.ReadLine();
                    if (s != null)
                        _backgroundWorker.ReportProgress(0, s);
                    else
                        wh.WaitOne(1000);
                }
            }

            wh.Close();
        }

        public void StartWatching()
        {
            _backgroundWorker.RunWorkerAsync();
            _logWriter.WriteLine($"Starting to watch the following file: '{_filePath}'");
        }

        public void StopWatching()
        {
            _backgroundWorker.CancelAsync();
        }

        protected virtual void HandleNewLine(string line) { }
    }
}
