using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbellTracker.ApplicationCode
{
    public interface IEventSystem
    {
        public delegate void EventDelegate<T>(T item);
        public bool Fire(object o);
        public bool Subscribe<T>(EventDelegate<T> SingelDelegate);
        public bool Unsubscribe<T>(EventDelegate<T> SingelDelegate);
    }
}
