using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IoCy.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ImplementsAttribute : Attribute
    {
        public bool IsSingleton { get; set; } = false;

        public Type TypeToResolve { get; set; }

        public object PartKey { get; } = null;

        internal protected bool IsMulti { get; set; }

        public ImplementsAttribute(bool isSingleton = false, object partKey = null)
        {
            IsSingleton = isSingleton;
            PartKey = partKey;
            IsMulti = false;
        }

        public ImplementsAttribute(Type typeToResolve, bool isSingleton = false, object partKey = null) : 
            this(isSingleton, partKey)
        {
            TypeToResolve = typeToResolve;
        }
    }
}
