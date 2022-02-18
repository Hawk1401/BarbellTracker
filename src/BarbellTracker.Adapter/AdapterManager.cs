using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarbellTracker.Adapter.Interface;
using BarbellTracker.ApplicationCode;

namespace BarbellTracker.Adapter
{
    public class AdapterManager
    {
        public static AdapterManager Instance = new AdapterManager();
        Dictionary<string, IUIAdapter> AdapterStorage = new Dictionary<string, IUIAdapter>();


        public IUIAdapter GetAdapterByName(string name)
        {
            return AdapterStorage[name];
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

            EventSystem.Fire(this, Event.AdapterAdded, adapter.Name);
        }

        public bool IsType(string adapterName, Type type)
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
