using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.RClone.ApiObjects
{
    public class Job : IJob
    {
        public Double Duration { get; set; }
        public DateTime EndTime { get; set; }
        public string Error { get; set; }
        public bool Finished { get; set; }
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public bool Success { get; set; }
    }
}
