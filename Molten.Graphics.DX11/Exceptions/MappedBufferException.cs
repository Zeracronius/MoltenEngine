﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten
{
    public class MappedBufferException : Exception
    {
        public MappedBufferException(string message) : base(message) { }
    }
}
