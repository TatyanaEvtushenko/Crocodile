﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    interface IConverter<T>
    {
        byte[] ConvertToBytes(T data);
        T ConvertToObject(byte[] data);
    }
}
