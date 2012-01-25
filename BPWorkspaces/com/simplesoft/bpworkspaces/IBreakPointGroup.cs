using System;
namespace com.simplesoft.bpworkspaces
{
    interface IBreakPointGroup
    {
        System.Collections.Generic.List<BPoint> Breakpoints { get; set; }
        string GroupName { get; set; }
    }
}
