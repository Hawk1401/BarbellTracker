using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbellTracker.ApplicationCode
{
    public class EventContext
    {
        private readonly List<Action> m_doneActions = new List<Action>();

        internal EventContext(EventSystem system, Event @event, object sender, object[] args, Func<EventContext, Task>[] callbacks, EventSystem origin = null)
        {
            System = system;
            Origin = origin;
            Event = @event;
            Sender = sender;
            Args = args;
            Task = Task.WhenAll(callbacks.Select(c => c(this)));
            InvokeDoneTasks();
        }

        // Current event system
        public EventSystem System { get; }

        // Origin event system
        public EventSystem Origin { get; }

        /// <summary>
        /// Raiser of event
        /// </summary>
        public object Sender { get; }

        /// <summary>
        /// Event
        /// </summary>
        public Event Event { get; }

        /// <summary>
        /// Event arguments
        /// </summary>
        public object[] Args { get; }

        /// <summary>
        /// Completition task of all subscribers (most likely to be null in event handler)
        /// </summary>
        public Task Task { get; }

        /// <summary>
        /// Fire event after current context is handled by all subscribers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="event"></param>
        /// <param name="args"></param>
        public void FireWhenDone(object sender, Event @event, params object[] args)
        {
            WhenDone(() => EventSystem.PushAsync(sender, System, @event, args));
        }

        /// <summary>
        /// Execute action after current context is handled by all subscribers
        /// </summary>
        /// <param name="action"></param>
        public void WhenDone(Action action)
        {
            lock (m_doneActions)
            {
                m_doneActions.Add(action);
            }
        }

        private async void InvokeDoneTasks()
        {
            await Task.Run(async () =>
            {
                await Task;

                lock (m_doneActions)
                {
                    foreach (var doneAction in m_doneActions)
                    {
                        try
                        {
                            doneAction();
                        }
                        catch (Exception ex)
                        {
                            //Log.Error(this, ex);
                        }
                    }
                }
            });
        }
    }
}
