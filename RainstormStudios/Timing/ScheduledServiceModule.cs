using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainstormStudios.Timing
{
    [Author("Michael Unfried")]
    public interface ScheduledServiceModule
    {

        void BeginProcessing();
        void ForceTerminate();
    }
    [Author("Michael Unfried")]
    public enum ScheduledServiceModuleStatus : uint
    {
        Unknown = 0,
        Started,
        Stopped,
        Waiting,
        Paused,
        Running
    }
}
