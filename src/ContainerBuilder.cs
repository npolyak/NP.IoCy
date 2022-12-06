using NP.DependencyInjection.Attributes;
using NP.DependencyInjection.Interfaces;
using NP.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace NP.IoCy
{
    public class ContainerBuilder : IContainerBuilder
    {
        private Dictionary<ContainerItemResolvingKey, IResolvingCell> _cellMap =
            new Dictionary<ContainerItemResolvingKey, IResolvingCell>();

        void CheckTypeDerivation(Type resolvingType, Type typeToResolve)
        {
            if (!resolvingType.IsAssignableFrom(typeToResolve))
            {
                throw new Exception($"Type to resolve '{typeToResolve.FullName}' does not derive from the resolving type: '{resolvingType.FullName}'");
            }
        }


        private IResolvingCell AddCell(ContainerItemResolvingKey typeToResolveKey, IResolvingCell resolvingCell)
        {
            lock (_cellMap)
            {
                _cellMap[typeToResolveKey] = resolvingCell;

                return resolvingCell;
            }
        }


        public void RegisterType(Type resolvingType, Type typeToResolve, object? resolutionKey = null)
        {
            CheckTypeDerivation(resolvingType, typeToResolve);
            AddCell(resolvingType.ToKey(resolutionKey!), new ResolvingTypeCell(typeToResolve));
        }

        public void RegisterType<TResolving, TToResolve>(object? resolutionKey = null)
            where TToResolve : TResolving
        {
            RegisterType(typeof(TResolving), typeof(TToResolve), resolutionKey);
        }


        public void RegisterSingletonInstance
        (
            Type resolvingType,
            object instance, 
            object? resolutionKey = null)
        {
            CheckTypeDerivation(resolvingType, instance.GetType());

            AddCell
            (
                resolvingType.ToKey(resolutionKey),
                new ResolvingObjSingletonCell(instance));
        }

        private void RegisterSingletonType
        (
            Type resolvingType,
            Type typeToResolve,
            object? resolutionKey = null)
        {

            CheckTypeDerivation(resolvingType, typeToResolve);

            AddCell
            (
                resolvingType.ToKey(resolutionKey),
                new ResolvingSingletonTypeCell(typeToResolve));
        }

        public void RegisterSingletonInstance<TResolving>(object instance, object? resolutionKey = null)
        {
            RegisterSingletonInstance(typeof(TResolving), instance, resolutionKey);
        }

        public void RegisterSingletonType<TResolving, TToResolve>(object? resolutionKey = null)
            where TToResolve : TResolving
        {
            RegisterSingletonType(typeof(TResolving), typeof(TToResolve), resolutionKey);
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

        public void RegisterSingletonFactoryMethodInfo<TResolving>
        (
            MethodInfo factoryMethodInfo,
            object? resolutionKey = null)
        {
            RegisterSingletonFactoryMethodInfo(factoryMethodInfo, typeof(TResolving), resolutionKey);
        }


        public void RegisterSingletonFactoryMethodInfo
        (
            MethodInfo factoryMethodInfo,
            Type? resolvingType = null,
            object? resolutionKey = null)
        {
            Type typeToResolve = factoryMethodInfo.ReturnType;

            if (resolvingType == null)
            {
                resolvingType = factoryMethodInfo.ReturnType;
            }
            else
            {
                CheckTypeDerivation(resolvingType, typeToResolve);
            }

            AddCell
            (
                resolvingType.ToKey(resolutionKey),
                new ResolvingMethodInfoSingletonCell(factoryMethodInfo));
        }


        public void RegisterFactoryMethodInfo
        (
            MethodInfo factoryMethodInfo,
            Type? resolvingType = null,
            object? resolutionKey = null)
        {
            Type typeToResolve = factoryMethodInfo.ReturnType;

            if (resolvingType == null)
            {
                resolvingType = factoryMethodInfo.ReturnType;
            }
            else
            {
                CheckTypeDerivation(resolvingType, typeToResolve);
            }

            AddCell
            (
                resolvingType.ToKey(resolutionKey),
                new ResolvingMethodInfoCell(factoryMethodInfo));
        }

        public void RegisterFactoryMethodInfo<TResolving>
        (
            MethodInfo factoryMethodInfo,
            object? resolutionKey = null)
        {
            RegisterFactoryMethodInfo(factoryMethodInfo, typeof(TResolving), resolutionKey);
        }


        public void RegisterAttributedType(Type attributedType)
        {
            RegisterTypeAttribute implementsAttribute =
                   attributedType.GetCustomAttribute<RegisterTypeAttribute>()!;

            if (implementsAttribute != null)
            {
                if (implementsAttribute.ResolvingType == null)
                {
                    implementsAttribute.ResolvingType =
                        attributedType.GetBaseTypeOrFirstInterface() ?? throw new Exception($"IoCy Programming Error: Type {attributedType.FullName} has an 'Implements' attribute, but does not have any base type and does not implement any interfaces");
                }

                Type resolvingType = implementsAttribute.ResolvingType;
                object? resolutionKeyObj = implementsAttribute.ResolutionKey;
                bool isSingleton = implementsAttribute.IsSingleton;

                if (isSingleton)
                {
                    this.RegisterSingletonType(resolvingType, attributedType, resolutionKeyObj);
                }
                else
                {
                    this.RegisterType(resolvingType, attributedType, resolutionKeyObj);
                }
            }
            else
            {
                HasRegisterMethodsAttribute? hasRegisterMethodAttribute =
                    attributedType.GetCustomAttribute<HasRegisterMethodsAttribute>();

                if (hasRegisterMethodAttribute != null)
                {
                    foreach (var methodInfo in attributedType.GetMethods().Where(methodInfo => methodInfo.IsStatic))
                    {
                        RegisterMethodAttribute factoryMethodAttribute = methodInfo.GetAttr<RegisterMethodAttribute>();

                        if (factoryMethodAttribute != null)
                        {
                            Type resolvingType = factoryMethodAttribute.ResolvingType ?? methodInfo.ReturnType;
                            object? partKeyObj = factoryMethodAttribute.ResolutionKey;
                            bool isSingleton = factoryMethodAttribute.IsSingleton;

                            if (isSingleton)
                            {
                                this.RegisterSingletonFactoryMethodInfo(methodInfo, resolvingType, partKeyObj);
                            }
                            else
                            {
                                this.RegisterFactoryMethodInfo(methodInfo, resolvingType, partKeyObj);
                            }
                        }
                    }
                }
            }
        }

        public void RegisterAssembly(Assembly assembly)
        {
            foreach (Type resolvingType in assembly.GetExportedTypes())
            {
                RegisterAttributedType(resolvingType);
            }
        }

        public void RegisterDynamicAssemblyByFullPath(string assemblyPath)
        {
            if (!File.Exists(assemblyPath))
                throw new Exception($"There is no assembly at path '{assemblyPath}'");

            string absoluteAssemblyPath = Path.GetFullPath(assemblyPath);

            //Assembly.ReflectionOnlyLoadFrom(absoluteAssemblyPath);

            Assembly loadedAssembly = Assembly.LoadFile(absoluteAssemblyPath);

            RegisterAssembly(loadedAssembly);
        }


        public void RegisterPluginsFromFolder
        (
            string assemblyFolderPath,
            Regex? matchingFileName = null
        )
        {
            if (!Directory.Exists(assemblyFolderPath))
                throw new Exception($"There is no folder at path '{assemblyFolderPath}'");

            foreach (string filePath in Directory.GetFiles(assemblyFolderPath))
            {
                if (!filePath.ToLower().EndsWith(".dll"))
                    continue;

                if (matchingFileName?.IsMatch(filePath) != false)
                {
                    string absoluteAssemblyPath = Path.GetFullPath(filePath);

                    Assembly assembly = Assembly.LoadFile(absoluteAssemblyPath);

                    RegisterAssembly(assembly);
                }
            }
        }

        // loads and registers assemblies that match the rejex 
        // from all direct sub-folders of folder specified
        // by baseFolderPath argument.
        public void RegisterPluginsFromSubFolders
        (
            string baseFolderPath,
            Regex? matchingFileName = null)
        {
            foreach (string folderPath in Directory.GetDirectories(baseFolderPath))
            {
                if (folderPath == "." || folderPath.StartsWith(".."))
                {
                    continue;
                }
                RegisterPluginsFromFolder(folderPath, matchingFileName);
            }
        }
        private static Assembly ResolveReferencedAssembly(object sender, ResolveEventArgs args)
        {
            AssemblyName? name =
                args.RequestingAssembly
                    ?.GetReferencedAssemblies()
                    .FirstOrDefault(a => a.FullName == args.Name);

            return name?.FindOrLoadAssembly()!;
        }


        protected virtual void SetAssemblyResolver()
        {
            AppDomain.CurrentDomain.AssemblyResolve += ResolveReferencedAssembly!;
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
            ContainerItemResolvingKey resolvingTypeKey = resolvingType.ToKey(resolutionKey);

            string errorMessage =
                $"IoCy Programming Error: cannot remove key '{resolvingTypeKey}' since configuration has already been completed.";

            ModifyContainerBuilder(() => _cellMap.Remove(resolvingTypeKey), errorMessage);
        }

        public ContainerBuilder()
        {
            SetAssemblyResolver();
        }

        public virtual IDependencyInjectionContainer Build()
        {
            return new Container(_cellMap);
        }
    }
}
