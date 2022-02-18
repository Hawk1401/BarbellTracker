using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarbellTracker.DomainCode;

namespace BarbellTracker.ApplicationCode
{
    public enum Event
    {
        StartExtractVideoInfo,
        ExtracedVideoInfo,
        PluginLoaded,
        ActivatePlugin,
        DeactivatePlugin,
        ActivateService,
        DeactivateService,
        SelectFile,
        FileSelected,
        AdapterAdded
    }

    public class EventSystem
    {
        private static Dictionary<Event, Type> ArgDefinitonForEvents = new Dictionary<Event, Type>() {
            { Event.StartExtractVideoInfo, typeof(string) },
            { Event.ExtracedVideoInfo, typeof(TrackedInformation) },
            { Event.PluginLoaded, typeof(string) },
            { Event.ActivatePlugin, typeof(string) },
            { Event.DeactivatePlugin, typeof(string)},
            { Event.ActivateService, typeof(string) },
            { Event.DeactivateService, typeof(string) },
            { Event.SelectFile, null }, 
            { Event.FileSelected, typeof(string) },
            { Event.AdapterAdded, typeof(string) },
        };


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

        public static void Fire(object sender, Event @event, object arg)
        {
            Fire(sender, Default, @event, arg);
        }

        public static async void Fire(object sender, EventSystem system, Event @event, object arg)
        {
            try
            {
                if(ArgDefinitonForEvents[@event] != arg.GetType())
                {
                    throw new ArgumentException("bla");
                }

                await Task.Run(async () => await PushAsync(sender, system, @event, arg));
            }
            catch (Exception ex)
            {
                //Log.Error(system, ex);
            }
        }

        public static Task PushAsync(object sender, Event @event, object arg)
        {
            return PushAsync(sender, Default, @event, arg);
        }

        public static Task PushAsync(object sender, EventSystem system, Event @event, object arg)
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

            var originTask = originCallbacks.Any() ? new EventContext(system, @event, sender, arg, originCallbacks).Task : Task.CompletedTask;
            var allTask = allCallbacks.Any() ? new EventContext(system, @event, sender, arg, allCallbacks).Task : Task.CompletedTask;

            return Task.WhenAll(originTask, allTask);
        }
    }


}