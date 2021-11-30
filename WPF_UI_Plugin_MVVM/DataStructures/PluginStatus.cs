using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_UI_Plugin_MVVM.DataStructures
{
    class PluginStatus
    {
        public PluginStatus(string pluginName, bool status)
        {
            this.PluginName = pluginName;
            this.Status = status;
        }
        public string PluginName { get; set; }
        public bool Status { get; set; }
        // kein ToString override, da nicht gebraucht ;)
    }
}
