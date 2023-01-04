using NP.DependencyInjection.Interfaces;
using NP.IoC.CommonImplementations;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace NP.IoCy
{
    public class ContainerBuilder<TKey> : AbstractContainerBuilder<TKey>, IContainerBuilder<TKey>
    {
        public bool AllowOverrides { get; }

        private Dictionary<TKey, object?> ResolutionKeys { get; } = new Dictionary<TKey, object?>();

        internal IDictionary<FullContainerItemResolvingKey<TKey>, IResolvingCell> _cellMap =
            new Dictionary<FullContainerItemResolvingKey<TKey>, IResolvingCell>();

        private IResolvingCell AddCell
        (
            FullContainerItemResolvingKey<TKey> typeToResolveKey, 
            IResolvingCell resolvingCell)
        {
            Type resolvingType = typeToResolveKey.ResolvingType;
            lock (_cellMap)
            {
                bool addedCell = false;
                if (_cellMap.TryGetValue(typeToResolveKey, out var currentCell))
                {
                    // found an existing (ResolvingType, ResolutionKey) combination
                    if (currentCell is ResolvingMultiObjCell multiObjCell)
                    {
                        addedCell = true;
                        multiObjCell.AddCell(resolvingCell, resolvingType);
                    }
                    else if (!AllowOverrides)
                    {
                        throw new Exception($"ERROR: Trying to override already existing key '{typeToResolveKey.ToStr()}'. Unregister the old key first!");
                    }
                }
                else // did NOT find an existing (ResolvingType, ResolutionKey) combination
                {
                    // if the resolution key is not null (default) check that such key has not been already used. 

                    TKey resolutionKey = typeToResolveKey.KeyObject;

                    if (!resolutionKey.ObjEquals(default(TKey)))
                    {
                        if (ResolutionKeys.ContainsKey(resolutionKey))
                        {
                            throw new Exception($"Non default ResolutionKey {resolutionKey} has already been used");
                        }
                        else
                        {
                            ResolutionKeys.Add(resolutionKey, null);
                        }
                    }
                }

                if (!addedCell)
                {
                    _cellMap[typeToResolveKey] = resolvingCell;
                }

                return resolvingCell;
            }
        }

        public ContainerBuilder(bool allowOverrides = false)
        {
            AllowOverrides = allowOverrides;
        }

        public void RegisterMultiCell
        (
            Type resolvingType, 
            TKey resolutionKey = default
        )
        {
            AddCell(resolvingType.ToKey(resolutionKey!), new ResolvingMultiObjCell(resolvingType));
        }

        public override void RegisterType
        (
            Type resolvingType, 
            Type typeToResolve, 
            TKey resolutionKey = default)
        {
            resolvingType.CheckTypeDerivation(typeToResolve);
            AddCell(resolvingType.ToKey(resolutionKey!), new ResolvingTypeCell(false, typeToResolve));
        }


        public override void RegisterSingletonInstance
        (
            Type resolvingType,
            object instance, 
            TKey resolutionKey = default)
        {
            resolvingType.CheckTypeDerivation(instance.GetType());

            AddCell
            (
                resolvingType.ToKey(resolutionKey),
                new ResolvingObjSingletonCell(instance));
        }

        public override void RegisterSingletonType
        (
            Type resolvingType,
            Type typeToResolve,
            TKey resolutionKey = default)
        {

            resolvingType.CheckTypeDerivation(typeToResolve);

            AddCell
            (
                resolvingType.ToKey(resolutionKey),
                new ResolvingTypeCell(true, typeToResolve));
        }


        protected override void RegisterAttributedType
        (
            Type resolvingType, 
            Type typeToResolve, 
            TKey resolutionKey = default)
        {
            RegisterType(resolvingType, typeToResolve, resolutionKey);
        }

        protected override void RegisterAttributedSingletonType
        (
            Type resolvingType, 
            Type typeToResolve, 
            TKey resolutionKey = default)
        {
            RegisterSingletonType(resolvingType, typeToResolve, resolutionKey);
        }

        public void RegisterSingletonFactoryMethod<TResolving>
        (
            Func<TResolving> resolvingFunc,
            TKey resolutionKey = default
        )
        {
            Type resolvingType = typeof(TResolving);

            AddCell
            (
                resolvingType.ToKey(resolutionKey),
                new ResolvingFactoryMethodCell<TResolving>(true, resolvingFunc));
        }

        public void RegisterFactoryMethod<TResolving>
        (
            Func<TResolving> resolvingFunc,
            TKey resolutionKey = default
        )
        {
            Type resolvingType = typeof(TResolving);

            AddCell
            (
                resolvingType.ToKey(resolutionKey),
                new ResolvingFactoryMethodCell<TResolving>(false, resolvingFunc));
        }

        public override void RegisterSingletonFactoryMethodInfo
        (
            MethodBase factoryMethodInfo,
            Type? resolvingType = null,
            TKey resolutionKey = default)
        {
            resolvingType = factoryMethodInfo.GetAndCheckResolvingType(resolvingType);

            AddCell
            (
                resolvingType.ToKey(resolutionKey),
                new ResolvingMethodInfoCell(true, factoryMethodInfo));
        }


        public override void RegisterFactoryMethodInfo
        (
            MethodBase factoryMethodInfo,
            Type? resolvingType = null,
            TKey resolutionKey = default)
        {
            resolvingType = factoryMethodInfo.GetAndCheckResolvingType(resolvingType);

            AddCell
            (
                resolvingType.ToKey(resolutionKey),
                new ResolvingMethodInfoCell(false, factoryMethodInfo));
        }

        private void ModifyContainerBuilder(Action modificationAction, string errorMessage)
        {
            lock (_cellMap)
            {
                modificationAction();
            }
        }

        public void UnRegister(Type resolvingType, TKey resolutionKey = default)
        {
            FullContainerItemResolvingKey<TKey> resolvingTypeKey = resolvingType.ToKey(resolutionKey);

            if (!resolutionKey.ObjEquals(default(TKey)))
            {
                ResolutionKeys.Remove(resolutionKey);
            }

            string errorMessage =
                $"IoCy Programming Error: cannot remove key '{resolvingTypeKey}' since configuration has already been completed.";

            ModifyContainerBuilder(() => _cellMap.Remove(resolvingTypeKey), errorMessage);
        }

        public void UnRegister<TResolving>(TKey resolutionKey = default)
        {
            UnRegister(typeof(TResolving), resolutionKey);
        }

        public virtual IDependencyInjectionContainer<TKey> Build()
        {
            return new Container<TKey>(_cellMap);
        }
    }

    public class ContainerBuilder : ContainerBuilder<object?>, IContainerBuilder
    {
        public override IDependencyInjectionContainer<object?> Build()
        {
            return new Container(_cellMap);
        }
    }
}
