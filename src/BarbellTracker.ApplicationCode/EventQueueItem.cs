using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbellTracker.ApplicationCode
{
    public class EventQueueItem
    {
        public object Argument { get; init; }
        public HashSet<Delegate> Delegates { get; init; }

        public EventQueueItem(HashSet<Delegate> Delegates, object Argument)
        {
            this.Delegates = Delegates;
            this.Argument = Argument;

        }

        public void Run()
        {
            foreach (var Delegate in Delegates)
            {
                Delegate.DynamicInvoke(Argument);
            }
        }

    }
}
