using System.Windows.Controls;
using System.Windows.Input;
using BarbellTracker.WPF_DesktopClient.ViewModel;

namespace BarbellTracker.WPF_DesktopClient.View
{
    /// <summary>
    /// Interaction logic for VideoAnalysisView.xaml
    /// The code behind only forwards the mouse positions (in video pixels) to the view model.
    /// </summary>
    public partial class VideoAnalysisView : UserControl
    {
        internal VideoAnalysisViewModel ViewModel { get; }

        public VideoAnalysisView()
        {
            InitializeComponent();
            ViewModel = new VideoAnalysisViewModel();
            DataContext = ViewModel;
        }

        private void VideoSurface_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            VideoSurface.CaptureMouse();
            ViewModel.BeginDrag(e.GetPosition(VideoSurface));
        }

        private void VideoSurface_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ViewModel.UpdateDrag(e.GetPosition(VideoSurface));
            }
        }

        private void VideoSurface_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ViewModel.UpdateDrag(e.GetPosition(VideoSurface));
            ViewModel.EndDrag();
            VideoSurface.ReleaseMouseCapture();
        }
    }
}
