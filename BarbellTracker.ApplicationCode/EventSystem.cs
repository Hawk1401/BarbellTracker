using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbellTracker.ApplicationCode
{
    public enum Event
    {
        ExtracedVideoInfos,
        PluginLoaded
    }

    public class EventSystem
    {
        private readonly object m_sync = new object();
        private readonly Dictionary<Event, List<Func<EventContext, Task>>> m_registry = new Dictionary<Event, List<Func<EventContext, Task>>>();


        public static EventSystem Default { get; } = new EventSystem("Default");

        /// <summary>
        /// All event systems will fire/push to this instance as well
        /// </summary>
        public static EventSystem All { get; } = new EventSystem("All");

        public string Name { get; }
        public EventSystem(string name)
        {
            Name = name;

            foreach (var @evn in Enum.GetValues<Event>())
            {
                m_registry.Add(@evn, new List<Func<EventContext, Task>>());
            }
        }

        public static void Subscribe(Event @event, Func<EventContext, Task> callback)
        {
            Subscribe(Default, @event, callback);
        }

        public static void Subscribe(EventSystem system, Event @event, Func<EventContext, Task> callback)
        {
            lock (system.m_sync)
            {
                system.m_registry[@event].Add(callback);
            }
        }

        public static void Unsubscribe(Event @event, Func<EventContext, Task> callback)
        {
            Unsubscribe(Default, @event, callback);
        }

        public static void Unsubscribe(EventSystem system, Event @event, Func<EventContext, Task> callback)
        {
            lock (system.m_sync)
            {
                system.m_registry[@event].Remove(callback);
            }
        }

        public static void Fire(object sender, Event @event, params object[] args)
        {
            Fire(sender, Default, @event, args);
        }

        public static async void Fire(object sender, EventSystem system, Event @event, params object[] args)
        {
            try
            {
                await Task.Run(async () => await PushAsync(sender, system, @event, args));
            }
            catch (Exception ex)
            {
                //Log.Error(system, ex);
            }
        }

        public static Task PushAsync(object sender, Event @event, params object[] args)
        {
            return PushAsync(sender, Default, @event, args);
        }

        public static Task PushAsync(object sender, EventSystem system, Event @event, params object[] args)
        {
            if (system == All)
                throw new Exception("The all event system is a subscribe only event system");

            Func<EventContext, Task>[] originCallbacks = null;
            Func<EventContext, Task>[] allCallbacks = null;

            lock (system.m_sync)
            {
                originCallbacks = system.m_registry[@event].ToArray();
                allCallbacks = All.m_registry[@event].ToArray();
            }

            var originTask = originCallbacks.Any() ? new EventContext(system, @event, sender, args, originCallbacks).Task : Task.CompletedTask;
            var allTask = allCallbacks.Any() ? new EventContext(system, @event, sender, args, allCallbacks).Task : Task.CompletedTask;

            return Task.WhenAll(originTask, allTask);
        }
    }
}