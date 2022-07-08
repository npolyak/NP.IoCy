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

using NP.Utilities;
using System;

namespace NP.IoCy
{
    class ContainerItemResolvingKey
    {
        public Type TypeToResolve { get; }

        // allows resolution by object 
        // without this, a single type would always be 
        // resolved in a single way. 
        public object? KeyObject { get; }


        public ContainerItemResolvingKey(Type typeToResolve, object? keyObject)
        {
            this.TypeToResolve = typeToResolve;
            this.KeyObject = keyObject;
        }

        public override bool Equals(object? obj)
        {
            if (obj is ContainerItemResolvingKey target)
            {
                return this.TypeToResolve.ObjEquals(target.TypeToResolve) &&
                       this.KeyObject.ObjEquals(target.KeyObject);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return this.TypeToResolve.GetHashCode() ^ this.KeyObject.GetHashCodeExtension();
        }

        public override string ToString()
        {
            string result = $"{TypeToResolve.Name}";

            if (KeyObject != null)
            {
                result += $": {KeyObject.ToStr()}";
            }

            return result;
        }
    }
}
