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
using WPF_UI_Plugin_MVVM.ViewModel;

namespace WPF_UI_Plugin_MVVM.View
{
    /// <summary>
    /// Interaction logic for PluginSettingsControl.xaml
    /// </summary>
    public partial class PluginSettingsControl : UserControl
    {
        public PluginSettingsControl()
        {
            InitializeComponent();
            this.DataContext = new PluginSettingsControlViewModel();
        }
    }
}
