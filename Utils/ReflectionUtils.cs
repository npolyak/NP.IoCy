using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
