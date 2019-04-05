using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SjaasCore.Service.Exception
{
    public class SjaasServiceValidationException : System.Exception
    {
        public SjaasServiceValidationException()
        {
        }

        public SjaasServiceValidationException(string message)
            : base(message)
        {
        }

        public SjaasServiceValidationException(string message, System.Exception inner)
            : base(message, inner)
        {
        }
    }
}
