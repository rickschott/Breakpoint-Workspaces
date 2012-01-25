using System;
using System.Collections.Generic;
using System.Text;

namespace com.simplesoft.bpworkspaces
{
    public class BreakPointWorkspace : IBreakPointWorkspace
    {
        public List<BreakPointGroup> BreakPointGroups { get; set; }
    }
}
