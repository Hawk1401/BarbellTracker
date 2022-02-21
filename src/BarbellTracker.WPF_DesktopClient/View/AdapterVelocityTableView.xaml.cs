using BarbellTracker.WPF_DesktopClient.ViewModel;
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

namespace BarbellTracker.WPF_DesktopClient.View
{
    /// <summary>
    /// Interaction logic for AdapterVelocityTableView.xaml
    /// </summary>
    public partial class AdapterVelocityTableView : UserControl
    {
        public AdapterVelocityTableView()
        {
            InitializeComponent();
            this.DataContext = new AdapterVelocityTableViewModel();
        }
    }
}
