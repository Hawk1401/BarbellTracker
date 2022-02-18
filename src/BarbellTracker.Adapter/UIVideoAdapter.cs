using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarbellTracker.Adapter.Interface;

namespace BarbellTracker.Adapter
{
    public class UIVideoAdapter : IUIAdapter
    {
        public string Name { get; set; }

        public string VideoSource { get; set; }
    }
}
