using Interfaces;
using NP.IoCy;
using System.Collections.Generic;

namespace PluginsTest
{
    class Program
    {
        static void Main(string[] args)
        {
            IoCContainer container = new IoCContainer();

            container.InjectPluginsFromFolder("Plugins");

            container.CompleteConfiguration();

            IEnumerable<IPlugin> plugins = container.MultiResolve<IPlugin>();

            foreach(IPlugin plugin in plugins)
            {
                plugin.PrintMessage();
            }
        }
    }
}
