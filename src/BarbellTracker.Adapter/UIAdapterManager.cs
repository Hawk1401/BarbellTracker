using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarbellTracker.Adapter.Interface;
using BarbellTracker.ApplicationCode;
using BarbellTracker.ApplicationCode.Event;

namespace BarbellTracker.Adapter
{
    //SOLID => S
    // 
    public class UIAdapterManager
    {
        private Dictionary<string, IUIAdapter> AdapterStorage = new Dictionary<string, IUIAdapter>();

        IEventSystem eventSystem;
        public UIAdapterManager(IEventSystem eventSystem)
        {
            this.eventSystem = eventSystem;
        }

        public bool TryGetUIAdapterByName(string name, out IUIAdapter adapter)
        {
            if (AdapterStorage.ContainsKey(name))
            {
                adapter = AdapterStorage[name];
                return true;
            }
            adapter = null;
            return false;
        }

        public void AddNewAdapter(IUIAdapter adapter)
        {
            if (AdapterStorage.ContainsKey(adapter.Name))
            {
                AdapterStorage[adapter.Name] = adapter;
            }
            else
            {
                AdapterStorage.Add(adapter.Name, adapter);
            }

            eventSystem.Fire(new AdapterAdded() { AdapterName = adapter.Name });
        }

        public bool TryIsType(string adapterName, Type type)
        {
            try
            {
                return AdapterStorage[adapterName].GetType() == type;

            }catch(KeyNotFoundException e)
            {
                return false;
            }
        }
    }
}
