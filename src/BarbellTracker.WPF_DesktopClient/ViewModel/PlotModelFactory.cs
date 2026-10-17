using System.Linq;
using BarbellTracker.Adapter.Model;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace BarbellTracker.WPF_DesktopClient.ViewModel
{
    /// <summary>Creates the charts of a set. Keeps the OxyPlot details out of the view model.</summary>
    internal static class PlotModelFactory
    {
        public static PlotModel CreateVelocityPlot(PhysicalMotion motion, SetAnalysis analysis)
        {
            var model = new PlotModel { Title = "Geschwindigkeit (vertikal)" };
            model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "Zeit [s]" });
            model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Geschwindigkeit [m/s]" });

            // concentric phases of the repetitions as shaded areas
            foreach (var rep in analysis.Repetitions)
            {
                model.Annotations.Add(new RectangleAnnotation
                {
                    MinimumX = motion.Time[rep.StartIndex],
                    MaximumX = motion.Time[rep.EndIndex],
                    Fill = OxyColor.FromAColor(50, OxyColors.SteelBlue),
                    Text = rep.Number.ToString(),
                    TextVerticalAlignment = VerticalAlignment.Top,
                });
            }

            var series = new LineSeries { Title = "v", StrokeThickness = 2, Color = OxyColors.SteelBlue };
            for (int i = 0; i < motion.Count; i++)
            {
                series.Points.Add(new DataPoint(motion.Time[i], motion.Velocity[i].Y));
            }
            model.Series.Add(series);
            return model;
        }

        public static PlotModel CreateBarPathPlot(PhysicalMotion motion)
        {
            var model = new PlotModel { Title = "Hantelbahn", PlotType = PlotType.Cartesian };
            model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "Horizontal [m]" });
            model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Höhe [m]" });

            var x0 = motion.Position[0].X;
            var y0 = motion.Position.Min(p => p.Y);
            var series = new LineSeries { Title = "Bahn", StrokeThickness = 2, Color = OxyColors.OrangeRed };
            foreach (var p in motion.Position)
            {
                series.Points.Add(new DataPoint(p.X - x0, p.Y - y0));
            }
            model.Series.Add(series);
            return model;
        }
    }
}
