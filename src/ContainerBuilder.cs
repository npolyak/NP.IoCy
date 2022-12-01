using NP.IoC.Attributes;
using NP.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace NP.IoCy
{
    public class ContainerBuilder
    {
        private Dictionary<ContainerItemResolvingKey, IResolvingCell> _cellMap =
            new Dictionary<ContainerItemResolvingKey, IResolvingCell>();

        void CheckTypeDerivation(Type typeToResolve, Type resolvingType)
        {
            if (!typeToResolve.IsAssignableFrom(resolvingType))
            {
                throw new Exception($"Resolving type '{resolvingType.FullName}' does not derive from type to resolve '{typeToResolve.FullName}'");
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


        public void RegisterType(Type typeToResolve, Type resolvingType, object? resolutionKey = null)
        {
            CheckTypeDerivation(typeToResolve, resolvingType);
            AddCell(typeToResolve.ToKey(resolutionKey!), new ResolvingTypeCell(resolvingType));
        }

        public void Register<TToResolve, TResolving>(object? resolutionKey = null)
            where TResolving : TToResolve
        {
            RegisterType(typeof(TToResolve), typeof(TResolving), resolutionKey);
        }


        private void RegisterSingletonObj
        (
            Type typeToResolve,
            object resolvingObj,
            object? resolutionKey = null)
        {
            CheckTypeDerivation(typeToResolve, resolvingObj.GetType());

            AddCell
            (
                typeToResolve.ToKey(resolutionKey),
                new ResolvingObjSingletonCell(resolvingObj));
        }

        private void RegisterSingletonType
        (
            Type typeToResolve,
            Type resolvingObjType,
            object? resolutionKey = null)
        {

            CheckTypeDerivation(typeToResolve, resolvingObjType);

            AddCell
            (
                typeToResolve.ToKey(resolutionKey),
                new ResolvingSingletonTypeCell(resolvingObjType));
        }



        public void RegisterSingleton<TToResolve>(object resolvingOjb, object? resolutionKey = null)
        {
            RegisterSingletonObj(typeof(TToResolve), resolvingOjb, resolutionKey);
        }

        public void RegisterSingleton<TToResolve, TResolving>(object? resolutionKey = null)
        {
            RegisterSingletonType(typeof(TToResolve), typeof(TResolving), resolutionKey);
        }

        public void RegisterSingletonFactoryMethod<TToResolve, TResolving>
        (
            Func<TResolving> resolvingFunc,
            object? resolutionKey = null)
        {
            Type typeToResolve = typeof(TToResolve);
            Type resolvingType = typeof(TResolving);

            CheckTypeDerivation(typeToResolve, resolvingType);

            AddCell
            (
                typeToResolve.ToKey(resolutionKey),
                new ResolvingSingletonTypeCell(resolvingType));
        }

        public void RegisterFactoryMethod<TToResolve, TResolving>
        (
            Func<TResolving> resolvingFunc,
            object? resolutionKey = null)
        {
            Type typeToResolve = typeof(TToResolve);
            Type resolvingType = typeof(TResolving);

            CheckTypeDerivation(typeToResolve, resolvingType);

            AddCell
            (
                typeToResolve.ToKey(resolutionKey),
                new ResolvingTypeCell(resolvingType));
        }


        public void RegisterSingletonFactoryMethodInfo
        (
            Type typeToResolve,
            MethodInfo factoryMethodInfo,
            object? resolutionKey = null)
        {
            Type resolvingType = factoryMethodInfo.ReturnType;

            CheckTypeDerivation(typeToResolve, resolvingType);

            AddCell
            (
                typeToResolve.ToKey(resolutionKey),
                new ResolvingMethodInfoSingletonCell(factoryMethodInfo));
        }

        public void RegisterSingletonFactoryMethodInfo<TToResolve>
        (
            MethodInfo factoryMethodInfo,
            object? resolutionKey = null)
        {
            RegisterSingletonFactoryMethodInfo(typeof(TToResolve), factoryMethodInfo, resolutionKey);
        }



        public void RegisterFactoryMethodInfo
        (
            Type typeToResolve,
            MethodInfo factoryMethodInfo,
            object? resolutionKey = null)
        {
            Type resolvingType = factoryMethodInfo.ReturnType;

            CheckTypeDerivation(typeToResolve, resolvingType);

            AddCell
            (
                typeToResolve.ToKey(resolutionKey),
                new ResolvingMethodInfoCell(factoryMethodInfo));
        }

        public void RegisterFactoryMethodInfo<TToResolve>
        (
            MethodInfo factoryMethodInfo,
            object? resolutionKey = null)
        {
            RegisterFactoryMethodInfo(typeof(TToResolve), factoryMethodInfo, resolutionKey);
        }


        public void RegisterAttributedType(Type resolvingType)
        {
            RegisterTypeAttribute implementsAttribute =
                   resolvingType.GetCustomAttribute<RegisterTypeAttribute>()!;

            if (implementsAttribute != null)
            {
                if (implementsAttribute.TypeToResolveBy == null)
                {
                    implementsAttribute.TypeToResolveBy =
                        resolvingType.GetBaseTypeOrFirstInterface() ?? throw new Exception($"IoCy Programming Error: Type {resolvingType.FullName} has an 'Implements' attribute, but does not have any base type and does not implement any interfaces");
                }

                Type typeToResolve = implementsAttribute.TypeToResolveBy;
                object? resolutionKeyObj = implementsAttribute.ResolutionKey;
                bool isSingleton = implementsAttribute.IsSingleton;

                if (isSingleton)
                {
                    this.RegisterSingletonType(typeToResolve, resolvingType, resolutionKeyObj);
                }
                else
                {
                    this.RegisterType(typeToResolve, resolvingType, resolutionKeyObj);
                }
            }
            else
            {
                HasRegisterMethodsAttribute? hasRegisterMethodAttribute =
                    resolvingType.GetCustomAttribute<HasRegisterMethodsAttribute>();

                if (hasRegisterMethodAttribute != null)
                {
                    foreach (var methodInfo in resolvingType.GetMethods().Where(methodInfo => methodInfo.IsStatic))
                    {
                        RegisterMethodAttribute factoryMethodAttribute = methodInfo.GetAttr<RegisterMethodAttribute>();

                        if (factoryMethodAttribute != null)
                        {
                            Type typeToResolve = factoryMethodAttribute.TypeToResolveBy ?? methodInfo.ReturnType;
                            object? partKeyObj = factoryMethodAttribute.ResolutionKey;
                            bool isSingleton = factoryMethodAttribute.IsSingleton;

                            if (isSingleton)
                            {
                                this.RegisterSingletonFactoryMethodInfo(typeToResolve, methodInfo, partKeyObj);
                            }
                            else
                            {
                                this.RegisterFactoryMethodInfo(typeToResolve, methodInfo, partKeyObj);
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

        public void Remove(Type typeToResolve, object resolutionKey)
        {
            ContainerItemResolvingKey typeToResolveKey = typeToResolve.ToKey(resolutionKey);

            string errorMessage =
                $"IoCy Programming Error: cannot remove key '{typeToResolveKey.ToString()}' since configuration has already been completed.";

            ModifyContainerBuilder(() => _cellMap.Remove(typeToResolveKey), errorMessage);
        }

        public ContainerBuilder()
        {
            SetAssemblyResolver();
        }

        public virtual Container Build()
        {
            return new Container(_cellMap);
        }
    }
}
