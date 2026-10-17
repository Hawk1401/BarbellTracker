using System.Windows.Controls;
using BarbellTracker.WPF_DesktopClient.ViewModel;

namespace BarbellTracker.WPF_DesktopClient.View
{
    /// <summary>Interaction logic for HistoryView.xaml</summary>
    public partial class HistoryView : UserControl
    {
        public HistoryView()
        {
            InitializeComponent();
            DataContext = new HistoryViewModel();
        }
    }
}
