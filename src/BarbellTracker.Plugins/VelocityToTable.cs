using BarbellTracker.Adapter.Interface;
using BarbellTracker.ApplicationCode;
using BarbellTracker.DomainCode;
using BarbellTracker.Services.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbellTracker.Plugins
{
    public class VelocityToTable : IProcessingPlugin
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public VelocityCalculator velocityCalculator;

        public VelocityToTable()
        {
            EventSystem.Subscribe(Event.ActivatePlugin, Activate);
            EventSystem.Subscribe(Event.DeactivatePlugin, Deactivate);
            velocityCalculator = VelocityCalculator.Instance;
        }

        public bool IsActiv()
        {
            throw new NotImplementedException();
        }

        public async Task ProcessData(EventContext eventContext)
        {
            var Velocity = velocityCalculator.GetVelocity(eventContext.Arg as TrackedInformation);
        }

        public async Task Activate(EventContext eventContext)
        {
            EventSystem.Subscribe(Event.ActivatePlugin, Activate);

        }

        public async Task Deactivate(EventContext eventContext)
        {
            EventSystem.Unsubscribe(Event.ActivatePlugin, Activate);

        }
    }
}
