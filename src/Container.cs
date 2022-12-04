// (c) Nick Polyak 2018 - http://awebpros.com/
// License: MIT License (https://opensource.org/licenses/MIT)
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author of this software if something goes wrong. 
// 
// Also, please, mention this software in any documentation for the 
// products that use it.
//
using NP.DependencyInjection.Attributes;
using NP.DependencyInjection.Interfaces;
using NP.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NP.IoCy
{
    public class Container : IDependencyInjectionContainer
    {
        Dictionary<ContainerItemResolvingKey, IResolvingCell> _cellMap =
            new Dictionary<ContainerItemResolvingKey, IResolvingCell>();

        internal Container (Dictionary<ContainerItemResolvingKey, IResolvingCell> cellMap)
        {
            _cellMap.AddAll(cellMap);

            ComposeAllSingletonObjects();
        }

        private IResolvingCell? GetResolvingCellCurrentContainer(ContainerItemResolvingKey resolvingKey)
        {
            if (_cellMap.TryGetValue(resolvingKey, out IResolvingCell? resolvingCell))
            {
                return resolvingCell;
            }

            return null;
        }

        private IResolvingCell GetResolvingCell(ContainerItemResolvingKey resolvingKey)
        {
            IResolvingCell resolvingCell = GetResolvingCellCurrentContainer(resolvingKey)!;

            return resolvingCell;
        }

        private object ResolveCurrentObj(ContainerItemResolvingKey resolvingKey)
        {
            IResolvingCell resolvingCell = GetResolvingCell(resolvingKey);

            return resolvingCell?.GetObj(this)!;
        }

        private object ResolveCurrentObj(Type typeToResolve, object? resolutionKey = null)
        {
            return ResolveCurrentObj(typeToResolve.ToKey(resolutionKey));
        }


        private ContainerItemResolvingKey? GetTypeToResolveKey
        (
            ICustomAttributeProvider propOrParam,
            Type propOrParamType,
            bool returnNullIfNoPartAttr = true)
        {
            InjectAttribute injectAttr =
                propOrParam.GetAttr<InjectAttribute>();

            if (injectAttr == null)
            {
                if (returnNullIfNoPartAttr)
                {
                    return null;
                }
                else
                {
                    injectAttr = new InjectAttribute(propOrParamType);
                }
            }

            if (propOrParamType != null && injectAttr.ResolvingType != null)
            {
                if (!propOrParamType.IsAssignableFrom(injectAttr.ResolvingType))
                {
                    throw new ProgrammingError($"Actual type of a part should be a super-type of the type to resolve");
                }
            }

            Type? realPropOrParamType = injectAttr.ResolvingType ?? propOrParamType;

            return realPropOrParamType?.ToKey(injectAttr.ResolutionKey);
        }

        private ContainerItemResolvingKey? GetTypeToResolveKey(PropertyInfo propInfo)
        {
            return GetTypeToResolveKey(propInfo, propInfo.PropertyType);
        }

        private ContainerItemResolvingKey? GetTypeToResolveKey(ParameterInfo paramInfo)
        {
            return GetTypeToResolveKey(paramInfo, paramInfo.ParameterType, false);
        }

        private object? ResolveKey(ContainerItemResolvingKey key)
        {
            return Resolve(key);
        }


        // compose an object based in its properties' attributes
        public void ComposeObject(object obj)
        {
            Type objType = obj.GetType();

            foreach (PropertyInfo propInfo in 
                        objType.GetProperties
                        (
                            BindingFlags.Instance | 
                            BindingFlags.NonPublic | 
                            BindingFlags.Public))
            {
                if (propInfo.SetMethod == null)
                    continue;

                ContainerItemResolvingKey? propTypeToResolveKey = GetTypeToResolveKey(propInfo);

                if (propTypeToResolveKey == null)
                {
                    continue;
                }

                object? subObj = ResolveKey(propTypeToResolveKey);

                if (subObj != null)
                {
                    propInfo.SetMethod.Invoke(obj, new[] { subObj });
                }
            }
        }

        internal IEnumerable<object?> GetMethodParamValues(MethodBase constructorInfo)
        {
            foreach(var paramInfo in constructorInfo.GetParameters())
            {
                ContainerItemResolvingKey? propTypeToResolveKey = GetTypeToResolveKey(paramInfo);

                if (propTypeToResolveKey == null)
                {
                    yield return null;
                }

                yield return ResolveKey(propTypeToResolveKey!);
            }
        }

        internal object ConstructObject(Type type)
        {
            ConstructorInfo constructorInfo =
                type.GetConstructors()
                    .FirstOrDefault(constr => constr.ContainsAttr<CompositeConstructorAttribute>())!;

            if (constructorInfo == null)
            {
                return Activator.CreateInstance(type)!;
            }

            return Activator.CreateInstance(type, GetMethodParamValues(constructorInfo).ToArray())!;
        }

        private object Resolve(ContainerItemResolvingKey resolvingKey)
        {
            object resolvingObj = ResolveCurrentObj(resolvingKey);

            return resolvingObj;
        }

        private object ResolveImpl(ContainerItemResolvingKey resolvingKey)
        {
            object result = Resolve(resolvingKey);

            return result;
        }

        public object Resolve(Type resolvingType, object? resolutionKey = null)
        {
            ContainerItemResolvingKey resolvingTypeKey = resolvingType.ToKey(resolutionKey);

            return ResolveImpl(resolvingTypeKey);
        }

        private object ResolveImpl<TResolving>(object? resolutionKey)
        {
            Type resolvingType = typeof(TResolving);

            return Resolve(resolvingType, resolutionKey);
        }

        public TToResolve Resolve<TToResolve>(object? resolutionKey = null)
        {
            return (TToResolve) ResolveImpl<TToResolve>(resolutionKey);
        }

        private void ComposeAllSingletonObjects()
        {
            foreach(IResolvingCell resolvingCell in this._cellMap.Values)
            {
                if (resolvingCell.CellType != ResolvingCellType.Singleton)
                {
                    continue;
                }

                ComposeObject(resolvingCell.GetObj(this)!);
            }
        }
    }
}
