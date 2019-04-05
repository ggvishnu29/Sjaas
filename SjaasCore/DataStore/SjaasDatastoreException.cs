using System;
using System.Collections.Generic;
using System.Text;

namespace SjaasCore.DataStore
{
    class SjaasDatastoreException : Exception
    {
        public SjaasDatastoreException()
        {
        }

        public SjaasDatastoreException(string message)
            : base(message)
        {
        }

        public SjaasDatastoreException(string message, System.Exception inner)
            : base(message, inner)
        {
        }
    }
}
