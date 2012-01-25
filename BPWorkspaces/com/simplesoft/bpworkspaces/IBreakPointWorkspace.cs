using System;
namespace com.simplesoft.bpworkspaces
{
    interface IBreakPointWorkspace
    {
        System.Collections.Generic.List<com.simplesoft.bpworkspaces.BreakPointGroup> BreakPointGroups { get; set; }
    }
}
