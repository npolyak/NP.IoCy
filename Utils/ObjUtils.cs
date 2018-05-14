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
