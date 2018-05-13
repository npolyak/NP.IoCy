using Interfaces;
using NP.IoCy.Attributes;
using System;

namespace Plugin1
{
    [MultiImplements]
    public class PluginOne : IPlugin
    {
        public void PrintMessage()
        {
            Console.WriteLine("I am PluginOne");
        }
    }
}
