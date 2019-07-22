using System;

namespace Lib.RClone.ApiObjects
{
    public interface IJob
    {
        double Duration { get; set; }
        DateTime EndTime { get; set; }
        string Error { get; set; }
        bool Finished { get; set; }
        int Id { get; set; }
        DateTime StartTime { get; set; }
        bool Success { get; set; }
    }
}