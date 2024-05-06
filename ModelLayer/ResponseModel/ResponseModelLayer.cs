﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.ResponseModel
{
    public class ResponseModelLayer<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; } = string.Empty;
        public T? Data { get; set; }    
    }
}
