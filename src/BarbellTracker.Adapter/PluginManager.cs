using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarbellTracker.Adapter.Interface;

namespace BarbellTracker.Adapter
{
    public class PluginManager
    {
        public static PluginManager Instance = new PluginManager();

        private readonly List<IPlugin> PluginsList;
        public PluginManager()
        {
            PluginsList = new List<IPlugin>();
        }


        public List<IProcessingPlugin> GetProcessingPlugins()
        {
            return PluginsList.Where(x => x is IProcessingPlugin).Select(x => x as IProcessingPlugin).ToList(); // performance not the best O(n + m); n=list size m=IUIplugins count
        }

        public static void mylittelTest()
        {
            var plugins = PluginManager.Instance.GetProcessingPlugins();

            var n = plugins[0].Name;
        }
    }
}
