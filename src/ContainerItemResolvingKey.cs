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

        public bool IsMulti { get; }

        public ContainerItemResolvingKey(Type typeToResolve, object? keyObject, bool isMulti = false)
        {
            this.IsMulti = isMulti;
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
