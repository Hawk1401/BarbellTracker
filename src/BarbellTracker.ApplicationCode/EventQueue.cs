using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbellTracker.ApplicationCode
{
    public class EventQueue
    {
        private List<EventQueueItem> Queue { get; set; }

        public EventQueue()
        {

            Queue = new List<EventQueueItem>();

        }


        public void Add(HashSet<Delegate> Delegates, object Agument)
        {
            var QueueIsEmpty = Queue.Count == 0;

            Queue.Add(new EventQueueItem(Delegates, Agument));

            if (QueueIsEmpty)
            {
                WorkOnQueue();
            }
        }

        public void WorkOnQueue()
        {
            while (Queue.Count > 0)
            {
                var first = Queue.First();

                first.Run();
                Queue.RemoveAt(0);
            }
        }
    }
}
