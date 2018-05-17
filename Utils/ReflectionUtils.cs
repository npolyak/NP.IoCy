// (c) Nick Polyak 2018 - http://awebpros.com/
// License: Apache License 2.0 (http://www.apache.org/licenses/LICENSE-2.0.html)
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author of this software if something goes wrong. 
// 
// Also, please, mention this software in any documentation for the 
// products that use it.

using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NP.IoCy.Utils
{
    public static class ReflectionUtils
    {
        public static Type GetBaseTypeOrFirstInterface(this Type type)
        {
            Type result = type.BaseType;

            if (result == typeof(object))
            {
                result = type.GetInterfaces().FirstOrDefault();
            }

            return result;
        }

        public static string GetCurrentExecutablePath()
        {
            string currentExecutablePath =
                Assembly.GetEntryAssembly().Location;

            return Path.GetDirectoryName(currentExecutablePath);
        }
    }
}
