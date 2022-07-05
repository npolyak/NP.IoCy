using NP.Utilities;
using NP.Utilities.Attributes;
using System;
using System.Linq;
using System.Reflection;

namespace NP.IoCy
{
    internal static class IoCUtils
    {
        public static ContainerItemResolvingKey ToKey(this Type typeToResolve, object? resolutionKey)
        {
            ContainerItemResolvingKey typeToResolveKey = new ContainerItemResolvingKey(typeToResolve, resolutionKey);

            return typeToResolveKey;
        }

        public static object CreateAndComposeObjFromMethod(this IoCContainer objectComposer, MethodInfo factoryMethodInfo)
        {
            object[] args = objectComposer.GetMethodParamValues(factoryMethodInfo).ToArray()!;

            object obj = factoryMethodInfo.Invoke(null, args)!;

            objectComposer.ComposeObject(obj);

            return obj;
        }

        public static object CreateAndComposeObjFromType(this IoCContainer objectComposer, Type resolvingType)
        {
            object? obj;
            ConstructorInfo constructorInfo =
                resolvingType.GetConstructors()
                              .FirstOrDefault(constr => constr.ContainsAttr<CompositeConstructorAttribute>())!;

            if (constructorInfo == null)
            {
                obj = Activator.CreateInstance(resolvingType)!;
            }
            else
            {
                obj =
                    Activator.CreateInstance
                    (
                        resolvingType,
                        objectComposer.GetMethodParamValues(constructorInfo).ToArray())!;
            }

            objectComposer.ComposeObject(obj);

            return obj;
        }
    }
}
