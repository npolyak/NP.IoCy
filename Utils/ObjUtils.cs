using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IoCy.Utils
{
    public static class ObjUtils
    {
        public static bool ObjEquals(this object obj1, object obj2)
        {
            if (obj1 == obj2)
                return true;

            if ((obj1 != null) && (obj1.Equals(obj2)))
                return true;

            return false;
        }

        public static int GetHashCodeExtension(this object obj)
        {
            return obj?.GetHashCode() ?? 0; ;
        }

        public static string ToStr(this object obj)
        {
            if (obj == null)
                return string.Empty;

            return obj.ToString();
        }
    }
}
