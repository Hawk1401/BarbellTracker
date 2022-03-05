using BarbellTracker.ApplicationCode;
using BarbellTracker.ApplicationCode.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbellTracker.Adapter.Interface
{
    public interface ITrackerPlugin : IPlugin
    {
        public void StartTacking(StartExtractVideoInfo extracedVideoInfo);
    }
}
