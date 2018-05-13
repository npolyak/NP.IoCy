using System;

namespace NP.IoCy.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MultiImplementsAttribute : ImplementsAttribute
    {
        public MultiImplementsAttribute(Type typeToResolve = null, object partKey = null) 
            :
            base(typeToResolve, true, partKey)
        {
            IsMulti = true;
        }
    }
}
