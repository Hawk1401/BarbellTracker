﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbellTracker.Adapter.Interface
{
    public interface IPlugin
    {
        public string Name { get;  set; }
        public string Description { get; set; }
    }
}
