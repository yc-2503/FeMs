using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeMs.Share
{
    public class ApiResponse<T>
    {
        public string? Message { get; set; }

        public bool Status { get; set; }

        public T? Result { get; set; }
    }
    public class ApiResponse: ApiResponse<object>
    {
    }
}
