using System.Windows.Controls;
using BarbellTracker.WPF_DesktopClient.ViewModel;

namespace BarbellTracker.WPF_DesktopClient.View
{
    /// <summary>
    /// Interaction logic for HeaderControl.xaml
    /// </summary>
    public partial class HeaderControl : UserControl
    {
        public HeaderControl()
        {
            InitializeComponent();
            this.DataContext = new HeaderControlViewModel();
        }
    }
}
