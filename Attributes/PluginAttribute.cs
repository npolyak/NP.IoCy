using System;
using System.Runtime.InteropServices;

namespace NP.IoCy.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly, Inherited = true)]
    [ComVisible(true)]
    public class PluginAttribute : Attribute
    {
    }
}
