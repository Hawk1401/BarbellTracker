using BarbellTracker.Adapter.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbellTracker.Adapter.Model
{


    public class VectorCSVModel
    {
        private List<VectorCSVItem> Items;
        public VectorCSVModel()
        {
            Items = new List<VectorCSVItem>();
        }

        public static string GetHeader()
        {
            return "Time;Length;Vector";
        }

        public void AddItem(string time, double length, string vector)
        {
            Items.Add(new VectorCSVItem(time, length, vector));
        }

        public List<VectorCSVItem> GetTable()
        {
            List<VectorCSVItem> result = new List<VectorCSVItem>();
            foreach (var item in Items)
            {
                result.Add(item.Copy());
            }

            return result;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(GetHeader());

            foreach (var item in Items)
            {
                stringBuilder.AppendLine(item.ToString());
            }

            return stringBuilder.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is VectorCSVModel other)
            {
                if(this.Items.Count != other.Items.Count)
                {
                    return false;
                }

                for (int i = 0; i < this.Items.Count; i++)
                {
                    if (!this.Items[i].Equals(other.Items[i]))
                    {
                        return false;
                    }
                }

                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hash = 0;

            Items.ForEach(item => hash ^= item.GetHashCode());

            return  hash;
        }

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
                if(obj == null)
                {
                    return false;
                }

                if(obj is VectorCSVItem other)
                {
                    return this.Time == other.Time &&
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
}
