using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainstormStudios.Timing
{
    public interface ScheduledServiceModule
    {

        void BeginProcessing();
        void ForceTerminate();
    }
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
