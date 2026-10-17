using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace BarbellTracker.Adapter.Model
{
    public class SetCSVModel
    {
        private readonly List<string> rows = new();

        public static string GetHeader()
        {
            return "Rep;Duration [s];Eccentric Duration [s];ROM [m];Mean Velocity [m/s];Peak Velocity [m/s];Mean Propulsive Velocity [m/s];Bar Path Deviation [m];Velocity Loss [%]";
        }

        public void AddRepetition(Repetition rep)
        {
            rows.Add(string.Join(";",
                rep.Number.ToString(CultureInfo.InvariantCulture),
                Format(rep.ConcentricDuration),
                Format(rep.EccentricDuration),
                Format(rep.RangeOfMotion),
                Format(rep.MeanVelocity),
                Format(rep.PeakVelocity),
                Format(rep.MeanPropulsiveVelocity),
                Format(rep.BarPathDeviation),
                Format(rep.VelocityLossPercent)));
        }

        public IReadOnlyList<string> Rows => rows;

        private static string Format(double value)
        {
            return value.ToString("0.###", CultureInfo.InvariantCulture);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine(GetHeader());
            foreach (var row in rows)
            {
                builder.AppendLine(row);
            }
            return builder.ToString();
        }

        public override bool Equals(object obj)
        {
            return obj is SetCSVModel other && rows.SequenceEqual(other.rows);
        }

        public override int GetHashCode()
        {
            return rows.Aggregate(0, (hash, row) => hash ^ row.GetHashCode());
        }
    }
}
