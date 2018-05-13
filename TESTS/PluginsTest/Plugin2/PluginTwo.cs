using Interfaces;
using NP.IoCy.Attributes;
using System;

namespace Plugin2
{
    [MultiImplements(typeof(IPlugin))]
    public class PluginTwo : IPlugin
    {
        public void PrintMessage()
        {
            Console.WriteLine("I am PluginTwo");
        }
    }
}
