using System;
using System.Collections.Generic;
using System.Text;

namespace com.simplesoft.bpworkspaces
{
    public class BreakPointGroup : IBreakPointGroup
    {
        public string GroupName { get; set; }
        public List<BPoint> Breakpoints { get; set; }

    }
}
