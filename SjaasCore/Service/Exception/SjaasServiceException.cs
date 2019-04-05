using System;
using System.Collections.Generic;
using System.Text;

namespace SjaasCore.Service.Exception
{
    public class SjaasServiceException : System.Exception
    {
        public SjaasServiceException()
        {
        }

        public SjaasServiceException(string message)
            : base(message)
        {
        }

        public SjaasServiceException(string message, System.Exception inner)
            : base(message, inner)
        {
        }
    }
}
