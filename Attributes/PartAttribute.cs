using System;

namespace NP.IoCy.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class PartAttribute : Attribute
    {
        public object PartKey { get; }

        internal protected bool IsMulti { get; protected set; }

        public PartAttribute(object partObjKey = null)
        {
            PartKey = partObjKey;

            IsMulti = false;
        }
    }
}
