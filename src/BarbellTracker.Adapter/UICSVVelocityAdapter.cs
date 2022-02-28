using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarbellTracker.Adapter.Model;

namespace BarbellTracker.Adapter
{
    public class UICSVVelocityAdapter : Interface.IUIAdapter
    {
        public string Name { get; set; }

        public List<VectorCSVModel> Table { get; set; }
    }
}
