using System;
using System.Collections.Generic;
using System.Text;

namespace com.simplesoft.bpworkspaces
{
    public class BPoint : IBPoint
    {
        public string FileName { get; set; }

        public int LineNumber { get; set; }

    }
}
