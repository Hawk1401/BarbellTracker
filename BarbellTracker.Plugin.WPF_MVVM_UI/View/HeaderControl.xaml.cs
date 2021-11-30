using System.Windows.Controls;
using BarbellTracker.Plugin.WPF_MVVM_UI.ViewModel;

namespace BarbellTracker.Plugin.WPF_MVVM_UI.View
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
