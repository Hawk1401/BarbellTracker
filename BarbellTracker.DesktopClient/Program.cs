using System;
using BarbellTracker.Adapter;
using BarbellTracker.Plugin.WPF_MVVM_UI;
using BarbellTracker.Plugins;

namespace BarbellTracker.DesktopClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var pluginManager = PluginManager.Instance;
            var uiAdapterManager = UIAdapterManager.Instance;

            pluginManager.AddPlugin(new VelocityToTable());
            uiAdapterManager.AddNewAdapter(new UIVideoAdapter());


            var wpf = new MainWindow();
        }
    }
}
