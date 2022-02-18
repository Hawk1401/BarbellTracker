using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarbellTracker.ApplicationCode;

namespace BarbellTracker.Services
{
    public interface IService
    {
        public string  Name { get; set; }
        public string Description { get; set; }

        public bool Activate();
        public bool Deactivate();

        public Task HandleTracking(EventContext eventContext);
    }
}
