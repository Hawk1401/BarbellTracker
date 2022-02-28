using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbellTracker.Services
{
    public class KeyAlreadyExist : Exception
    {
        public KeyAlreadyExist()
        {
        }

        public KeyAlreadyExist(string message)
            : base(message)
        {
        }

        public KeyAlreadyExist(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
