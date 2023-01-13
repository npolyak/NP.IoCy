// (c) Nick Polyak 2022 - http://awebpros.com/
// License: MIT License (https://opensource.org/licenses/MIT)
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author of this software if something goes wrong. 
// 
// Also, please, mention this software in any documentation for the 
// products that use it.

using NP.DependencyInjection.Attributes;
using DynamicAssemblyLoadingTest.Implementations;
using DynamicAssemblyLoadingTest.Interfaces;
using System.Collections.Generic;

namespace Implementations
{
    [HasRegisterMethods]
    public static class ObjFactory
    {
        [RegisterMethod(resolutionKey:"TheOrg")]
        public static IOrg CreateOrg([Inject(resolutionKey: "TheConsoleLog")] ILog log)
        {
            return new Org(log);
        }


        [RegisterMethod(isSingleton:true, isMultiCell:true, resolutionKey:"MyStrs")]
        public static IEnumerable<string> GetStrs()
        {
            return new []
            {
                "Str1",
                "Str2"
            };
        }

        [RegisterMethod(isSingleton: true, isMultiCell: true, resolutionKey: "MyStrs")]
        public static IEnumerable<string> GetOtherStrs()
        {
            return new[]
            {
                "Str3",
                "Str4"
            };
        }
    }
}
