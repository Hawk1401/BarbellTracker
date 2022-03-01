namespace BarbellTracker.Adapter.Model
{
    public class VectorCSVItem
    {
        public string Time { get; init; }
        public string Length { get; init; }
        public string Vector { get; init; }

        public VectorCSVItem(string time, double length, string vector)
        {
            this.Time = time;
            this.Vector = vector;
            this.Length = length.ToString("0.###");
        }

        private VectorCSVItem(string time, string length, string vector)
        {
            this.Time = time;
            this.Vector = vector;
            this.Length = length;
        }

        public VectorCSVItem Copy()
        {
            return new VectorCSVItem(Time, Length, Vector);
        }
        public override string ToString()
        {
            return $"{Time};{Length};{Vector}";
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is VectorCSVItem other)
            {
                return 
                    this.Time == other.Time &&
                    this.Length == other.Length &&
                    this.Vector == other.Vector;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Time.GetHashCode() ^ Length.GetHashCode() ^ Vector.GetHashCode();
        }
    }
}
