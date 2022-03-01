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
    }
}
