using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarbellTracker.ApplicationCode.EventModel;
using BarbellTracker.DomainCode;

namespace BarbellTracker.ApplicationCode
{
    // SOLID => O
    // neue Eventsystem ohne probleme möglich eine neues Event über das Eventsystem zu schicken
    // mit der alten Implementierung hätte man ein neuen Werte einem Enum hinzufügen müssen was es komplexer macht für erweiterungen
    //
    public class EventSystem : IEventSystem
    {
        //public delegate void EventDelegate<T>(T item);

        private Dictionary<Type, HashSet<Delegate>> Map = new Dictionary<Type, HashSet<Delegate>>();

        private object _lock = new object();

        public bool Fire(object o)
        {
            return FireAsync(o);
        }
        private EventQueue EventQueue = new EventQueue();
        private bool FireAsync(object o)
        {
            var type = o.GetType();
            HashSet<Delegate> Delegates;

            lock (_lock)
            {
                if (!Map.TryGetValue(type, out var registered))
                {
                    return false;
                }

                // Snapshot, so handlers can (un)subscribe while the event is being dispatched
                Delegates = new HashSet<Delegate>(registered);
            }

            EventQueue.Add(Delegates, o);
            return true;
        }



        public bool Subscribe<T>(IEventSystem.EventDelegate<T> SingelDelegate)
        {
            lock (_lock)
            {
                Type type = typeof(T);

                if (!Map.ContainsKey(type))
                {
                    Map.Add(type, new HashSet<Delegate>());
                }

                return Map[type].Add(SingelDelegate);
            }
        }

        public bool Unsubscribe<T>(IEventSystem.EventDelegate<T> SingelDelegate)
        {
            lock (_lock)
            {
                Type type = typeof(T);

                if (!Map.ContainsKey(type))
                {
                    return false;
                }

                return Map[type].Remove(SingelDelegate);
            }
        }
    }


}