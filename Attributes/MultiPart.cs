using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IoCy.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class MultiPartAttribute : PartAttribute
    {
        public MultiPartAttribute(object partObjKey = null) : base(partObjKey)
        {
            IsMulti = true;
        }
    }
}
