using System;
using System.Collections.Generic;
using System.Text;

namespace SjaasCore.Service.Response
{
    public class SubmitJobResponse
    {
        public string RefId { get; }

        public SubmitJobResponse(string refId)
        {
            RefId = refId;
        }
    }
}
