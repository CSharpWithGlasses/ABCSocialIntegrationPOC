using System;
using System.Collections.Generic;
using System.Text;

namespace ABC.Shared.Models.Api
{
    public class ApiResponse<T>
    {
        public T Data { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
    }
}
