using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarbellTracker.Adapter.Interface;
using BarbellTracker.ApplicationCode;

namespace BarbellTracker.Adapter
{
    public class PluginManager
    {
        public static PluginManager Instance = new PluginManager();

        private readonly List<IPlugin> PluginsList;
        private PluginManager()
        {
            PluginsList = new List<IPlugin>();
        }


        public List<IProcessingPlugin> GetProcessingPlugins()
        {
            return PluginsList.Where(x => x is IProcessingPlugin).Select(x => x as IProcessingPlugin).ToList(); // performance not the best O(n + m); n=list size m=IUIplugins count
        }
        public List<ITrackerPlugin> GetTrackerPlugins()
        {
            return PluginsList.Where(x => x is ITrackerPlugin).Select(x => x as ITrackerPlugin).ToList(); // performance not the best O(n + m); n=list size m=IUIplugins count
        }


        public bool TryGetProcessingPluginByName(string name, out IProcessingPlugin processingPlugin)
        {
            foreach (var _plugin in PluginsList)
            {
                if(_plugin is IProcessingPlugin _processingPlugin && _processingPlugin.Name == name)
                {
                    processingPlugin = _processingPlugin;
                    return true;
                }
            }

            processingPlugin = null;
            return false;
        }
        public bool TryGetTrackerPluginByName(string name, out ITrackerPlugin trackerPlugin)
        {
            foreach (var _plugin in PluginsList)
            {
                if (_plugin is ITrackerPlugin _trackerPlugin && _trackerPlugin.Name == name)
                {
                    trackerPlugin = _trackerPlugin;
                    return true;
                }
            }

            trackerPlugin = null;
            return false;
        }

        public void AddPlugin(IPlugin plugin)
        {
            PluginsList.Add(plugin);
            EventSystem.Fire(this, Event.PluginLoaded, plugin.Name);
        }
    }
}
