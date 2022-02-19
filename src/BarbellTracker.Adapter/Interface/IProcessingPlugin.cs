using BarbellTracker.ApplicationCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbellTracker.Adapter.Interface
{
    public interface IProcessingPlugin :IPlugin
    {

        public bool IsActiv();

        public Task Activate(EventContext eventContext);

        public Task Deactivate(EventContext eventContext);

        public Task ProcessData(EventContext eventContext);
    }
}
