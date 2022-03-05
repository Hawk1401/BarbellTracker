using BarbellTracker.ApplicationCode;
using BarbellTracker.ApplicationCode.Event;
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

        public void Activate(ActivatePlugin activatePlugin);

        public void Deactivate(DeactivatePlugin deactivatePlugin);

        public void ProcessData(ExtracedVideoInfo extracedVideoInfo);
    }
}
