using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyIoCContainerTest
{
    public static class ReflectionUtils
    {
        public static string GetFullTypeStr(this Type type)
        {
            return type.Namespace + "." + type.Name;
        }

        public static bool TypesMatch(this Type sourceType, Type targetType)
        {
            return sourceType.GetFullTypeStr() == targetType.GetFullTypeStr();
        }

        public static IEnumerable<Type> GetSelfAndSuperTypes(this Type type)
        {
            if (type == null)
                yield break;

            yield return type;

            while (true)
            {
                type = type.BaseType;

                if (type == null)
                    yield break;

                yield return type;
            }
        }

        public static IEnumerable<Type> GetSelfSuperTypesAndInterfaces(this Type type)
        {
            List<Type> result = new List<Type>();

            if (type == null)
                return result;

            result.Add(type);

            result.AddRange(type.BaseType.GetSelfSuperTypesAndInterfaces());

            foreach (Type iface in type.GetInterfaces())
            {
                result.AddRange(iface.GetSelfSuperTypesAndInterfaces());
            }

            return result;
        }

        public static bool SelfOrSuperTypeMatches(this Type type, Type typeToMatch)
        {
            return type.GetSelfSuperTypesAndInterfaces().FirstOrDefault((t) => t.TypesMatch(typeToMatch)) != null;
        }
    }

}
