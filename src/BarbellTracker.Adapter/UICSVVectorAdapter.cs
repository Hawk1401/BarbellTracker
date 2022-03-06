using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarbellTracker.Adapter.Model;

namespace BarbellTracker.Adapter
{
    public class UICSVVectorAdapter : Interface.IUIAdapter
    {
        public string Name { get; set; }

        public VectorCSVModel CSV { get; set; }
    }
}
