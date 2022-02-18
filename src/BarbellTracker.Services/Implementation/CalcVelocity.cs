using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarbellTracker.ApplicationCode;

namespace BarbellTracker.Services.Implementation
{
    public class CalcVelocity : IService
    {
        public string Name { get; set; }
        public string Description { get; set; }


        public CalcVelocity()
        {
            EventSystem.Subscribe(Event.ExtracedVideoInfos, HandleTracking);

            EventSystem.Fire(this, EventSystem.Default, Event.ExtracedVideoInfos, new object()));
        }

        public Task HandleTracking(EventContext eventContext)
        {
            throw new NotImplementedException();
        }
    }
}
