using NP.DependencyInjection.Interfaces;
using NP.IoC.CommonImplementations;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NP.IoCy
{
    public class ContainerBuilder : AbstractContainerBuilder, IContainerBuilder
    {
        private Dictionary<FullContainerItemResolvingKey, IResolvingCell> _cellMap =
            new Dictionary<FullContainerItemResolvingKey, IResolvingCell>();

        private IResolvingCell AddCell(FullContainerItemResolvingKey typeToResolveKey, IResolvingCell resolvingCell)
        {
            lock (_cellMap)
            {
                _cellMap[typeToResolveKey] = resolvingCell;

                return resolvingCell;
            }
        }


        public override void RegisterType
        (
            Type resolvingType, 
            Type typeToResolve, 
            object? resolutionKey = null)
        {
            resolvingType.CheckTypeDerivation(typeToResolve);
            AddCell(resolvingType.ToKey(resolutionKey!), new ResolvingTypeCell(typeToResolve));
        }


        public override void RegisterSingletonInstance
        (
            Type resolvingType,
            object instance, 
            object? resolutionKey = null)
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
            object? resolutionKey = null)
        {

            resolvingType.CheckTypeDerivation(typeToResolve);

            AddCell
            (
                resolvingType.ToKey(resolutionKey),
                new ResolvingSingletonTypeCell(typeToResolve));
        }

        public void RegisterSingletonFactoryMethod<TResolving>
        (
            Func<TResolving> resolvingFunc,
            object? resolutionKey = null
        )
        {
            Type resolvingType = typeof(TResolving);

            AddCell
            (
                resolvingType.ToKey(resolutionKey),
                new ResolvingFactoryMethodSingletonCell<TResolving>(resolvingFunc));
        }

        public void RegisterFactoryMethod<TResolving>
        (
            Func<TResolving> resolvingFunc,
            object? resolutionKey = null
        )
        {
            Type resolvingType = typeof(TResolving);

            AddCell
            (
                resolvingType.ToKey(resolutionKey),
                new ResolvingFactoryMethodCell<TResolving>(resolvingFunc));
        }

        public override void RegisterSingletonFactoryMethodInfo
        (
            MethodInfo factoryMethodInfo,
            Type? resolvingType = null,
            object? resolutionKey = null)
        {
            resolvingType = factoryMethodInfo.GetAndCheckResolvingType(resolvingType);

            AddCell
            (
                resolvingType.ToKey(resolutionKey),
                new ResolvingMethodInfoSingletonCell(factoryMethodInfo));
        }


        public override void RegisterFactoryMethodInfo
        (
            MethodInfo factoryMethodInfo,
            Type? resolvingType = null,
            object? resolutionKey = null)
        {
            resolvingType = factoryMethodInfo.GetAndCheckResolvingType(resolvingType);

            AddCell
            (
                resolvingType.ToKey(resolutionKey),
                new ResolvingMethodInfoCell(factoryMethodInfo));
        }


        private void ModifyContainerBuilder(Action modificationAction, string errorMessage)
        {
            lock (_cellMap)
            {
                modificationAction();
            }
        }

        public void UnRegister(Type resolvingType, object? resolutionKey)
        {
            FullContainerItemResolvingKey resolvingTypeKey = resolvingType.ToKey(resolutionKey);

            string errorMessage =
                $"IoCy Programming Error: cannot remove key '{resolvingTypeKey}' since configuration has already been completed.";

            ModifyContainerBuilder(() => _cellMap.Remove(resolvingTypeKey), errorMessage);
        }

        public virtual IDependencyInjectionContainer Build()
        {
            return new Container(_cellMap);
        }
    }
}
