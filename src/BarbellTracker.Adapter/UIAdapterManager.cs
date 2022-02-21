using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarbellTracker.Adapter.Interface;
using BarbellTracker.ApplicationCode;

namespace BarbellTracker.Adapter
{
    public class UIAdapterManager
    {
        public static UIAdapterManager Instance = new UIAdapterManager();
        Dictionary<string, IUIAdapter> AdapterStorage = new Dictionary<string, IUIAdapter>();


        public bool TryGetUIAdapterByName(string name, IUIAdapter adapter)
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

            EventSystem.Fire(this, Event.AdapterAdded, adapter.Name);
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
