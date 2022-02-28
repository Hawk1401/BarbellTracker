using BarbellTracker.ApplicationCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbellTracker.Adapter.Interface
{
    public interface ITrackerPlugin : IPlugin
    {
        public Task StartTacking(EventContext eventContext);
    }
}
