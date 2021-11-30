using System.Windows.Controls;
using WPF_UI_Plugin_MVVM.ViewModel;

namespace WPF_UI_Plugin_MVVM.View
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
