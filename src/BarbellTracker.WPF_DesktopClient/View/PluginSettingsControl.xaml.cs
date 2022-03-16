using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BarbellTracker.ApplicationCode;
using BarbellTracker.WPF_DesktopClient.ViewModel;

namespace BarbellTracker.WPF_DesktopClient.View
{
    /// <summary>
    /// Interaction logic for PluginSettingsControl.xaml
    /// </summary>
    public partial class PluginSettingsControl : UserControl
    {
        private IEventSystem eventSystem;
        public PluginSettingsControl(IEventSystem eventSystem)
        {
            InitializeComponent();
            this.eventSystem = eventSystem;
            this.DataContext = new PluginSettingsControlViewModel(eventSystem);
    }
}
