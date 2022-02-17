using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public List<IUIPlugin> GetUIPlugins()
        {
            return PluginsList.Where(x => x is IUIPlugin).Select(x => x as IUIPlugin).ToList(); // perfamance not the best O(n + m); n=list size m=IUIplugins count
        }
    }
}
