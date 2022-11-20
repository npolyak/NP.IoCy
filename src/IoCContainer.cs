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
using NP.Utilities.Attributes;
using NP.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using NP.Utilities.BasicInterfaces;

namespace NP.IoCy
{
    public class IoCContainer : IObjectComposer
    {
        public static int CurrentContainerId { get; private set; }

        static IoCContainer()
        {
        }

        private static Assembly ResolveReferencedAssembly(object sender, ResolveEventArgs args)
        {
            AssemblyName? name =
                args.RequestingAssembly
                    ?.GetReferencedAssemblies()
                    .FirstOrDefault(a => a.FullName == args.Name);

            return name?.FindOrLoadAssembly()!;
        }

        private IoCContainer? ParentContainer { get; set; }

        Dictionary<ContainerItemResolvingKey, IResolvingCell> _cellMap =
            new Dictionary<ContainerItemResolvingKey, IResolvingCell>();

        public bool ConfigurationCompleted { get; private set; } = false;

        public event Action? ConfigurationCompletedEvent = null;

        public IoCContainer ()
        {
        }

        protected virtual void SetAssemblyResolver()
        {
            AppDomain.CurrentDomain.AssemblyResolve += ResolveReferencedAssembly!;
        }

        private IResolvingCell? GetResolvingCellCurrentContainer(ContainerItemResolvingKey typeToResolveKey)
        {
            if (_cellMap.TryGetValue(typeToResolveKey, out IResolvingCell? resolvingCell))
            {
                return resolvingCell;
            }

            return null;
        }

        private IResolvingCell GetResolvingCell(ContainerItemResolvingKey typeToResolveKey)
        {
            IResolvingCell resolvingCell = GetResolvingCellCurrentContainer(typeToResolveKey)!;

            return resolvingCell;
        }

        void CheckTypeDerivation(Type typeToResolve, Type resolvingType)
        {
            if (!typeToResolve.IsAssignableFrom(resolvingType))
            {
                throw new Exception($"Resolving type '{resolvingType.FullName}' does not derive from type to resolve '{typeToResolve.FullName}'");
            }
        }

        IResolvingCell AddCell(ContainerItemResolvingKey typeToResolveKey, IResolvingCell resolvingCell)
        {
            if (ConfigurationCompleted)
            {
                throw new Exception($"cannot add key '{typeToResolveKey.ToString()}' mapped to '{resolvingCell.ToString()}' cell since configuration has already been completed.");
            }

            lock (_cellMap)
            {
                _cellMap[typeToResolveKey] = resolvingCell;

                return resolvingCell;
            }
        }

        private object ResolveCurrentObj(ContainerItemResolvingKey typeToResolveKey)
        {
            IResolvingCell resolvingCell = GetResolvingCell(typeToResolveKey);

            return resolvingCell?.GetObj(this)!;
        }

        private object ResolveCurrentObj(Type typeToResolve, object? resolutionKey = null)
        {
            return ResolveCurrentObj(typeToResolve.ToKey(resolutionKey));
        }

        public void MapType(Type typeToResolve, Type resolvingType, object? resolutionKey = null)
        {
            CheckTypeDerivation(typeToResolve, resolvingType);
            AddCell(typeToResolve.ToKey(resolutionKey!), new ResolvingTypeCell(resolvingType));
        }

        public void Map<TToResolve, TResolving>(object? resolutionKey = null)
            where TResolving : TToResolve
        {
            MapType(typeof(TToResolve), typeof(TResolving), resolutionKey);
        }


        private void MapSingletonObj
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

        private void MapSingletonType
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

        public void MapSingleton<TToResolve>(object resolvingOjb, object? resolutionKey = null)
        {
            MapSingletonObj(typeof(TToResolve), resolvingOjb, resolutionKey);
        }

        public void MapSingleton<TToResolve, TResolving>(object? resolutionKey = null)
        {
            MapSingletonType(typeof(TToResolve), typeof(TResolving), resolutionKey);
        }

        public void MapSingletonFactoryMethod<TToResolve, TResolving>
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

        public void MapFactoryMethod<TToResolve, TResolving>
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


        public void MapSingletonFactoryMethodInfo
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

        public void MapSingletonFactoryMethodInfo<TToResolve>
        (
            MethodInfo factoryMethodInfo,
            object? resolutionKey = null)
        {
            MapSingletonFactoryMethodInfo(typeof(TToResolve), factoryMethodInfo, resolutionKey);
        }



        public void MapFactoryMethodInfo
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

        public void MapFactoryMethodInfo<TToResolve>
        (
            MethodInfo factoryMethodInfo,
            object? resolutionKey = null)
        {
            MapFactoryMethodInfo(typeof(TToResolve), factoryMethodInfo, resolutionKey);
        }

        private ContainerItemResolvingKey? GetTypeToResolveKey
        (
            ICustomAttributeProvider propOrParam,
            Type propOrParamType,
            bool returnNullIfNoPartAttr = true)
        {
            PartAttribute partAttr =
                propOrParam.GetAttr<PartAttribute>();

            if (partAttr == null)
            {
                if (returnNullIfNoPartAttr)
                {
                    return null;
                }
                else
                {
                    partAttr = new PartAttribute(propOrParamType);
                }
            }

            if (propOrParamType != null && partAttr.TypeToResolve != null)
            {
                if (!propOrParamType.IsAssignableFrom(partAttr.TypeToResolve))
                {
                    throw new ProgrammingError($"Actual type of a part should be a super-type of the type to resolve");
                }
            }

            Type? realPropOrParamType = partAttr.TypeToResolve ?? propOrParamType;

            return realPropOrParamType?.ToKey(partAttr.PartKey);
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

        private object Resolve(ContainerItemResolvingKey typeToResolveKey)
        {
            object resolvingObj = ResolveCurrentObj(typeToResolveKey);

            return resolvingObj;
        }

        private object ResolveImpl(ContainerItemResolvingKey typeToResolveKey)
        {
            if (!ConfigurationCompleted)
            {
                throw new Exception($"IoCy Programming Error: Cannot call Resolve method since the container is protected and configuration is not completed. TypeToResolveKey is {typeToResolveKey.ToString()}");
            }

            object result = Resolve(typeToResolveKey);

            return result;
        }

        public object Resolve(Type typeToResolve, object? resolutionKey = null)
        {
            ContainerItemResolvingKey typeToResolveKey = typeToResolve.ToKey(resolutionKey);

            return ResolveImpl(typeToResolveKey);
        }

        private object ResolveImpl<TToResolve>(object? resolutionKey)
        {
            Type typeToResolve = typeof(TToResolve);

            return Resolve(typeToResolve, resolutionKey);
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

        // before calling this method you cannot resolve any objects
        // from the container. 
        // after calling this method, you cannot modify 
        // the container any more.
        public void CompleteConfiguration()
        {
            ComposeAllSingletonObjects();

            ConfigurationCompleted = true;

            ConfigurationCompletedEvent?.Invoke();
        }

        private void ModifyContainer(Action modificationAction, string errorMessage)
        {
            if (ConfigurationCompleted)
            {
                throw new Exception(errorMessage);
            }
            else
            {
                lock (_cellMap)
                {
                    modificationAction();
                }
            }
        }

        public void Remove(Type typeToResolve, object resolutionKey)
        {
            ContainerItemResolvingKey typeToResolveKey = typeToResolve.ToKey(resolutionKey);

            string errorMessage =
                $"IoCy Programming Error: cannot remove key '{typeToResolveKey.ToString()}' since configuration has already been completed.";

            ModifyContainer(() => _cellMap.Remove(typeToResolveKey), errorMessage);
        }

        //private void MergeInImpl(IoCContainer anotherIoCContainer)
        //{
        //    foreach (KeyValuePair<ContainerItemResolvingKey, IResolvingCell> kvp in anotherIoCContainer._cellMap)
        //    {
        //        ContainerItemResolvingKey typeToResolveKey = kvp.Key;

        //        IResolvingCell valCopy = kvp.Value.Copy();

        //        AddCell(typeToResolveKey, valCopy);
        //    }
        //}

        //public void MergeIn(IoCContainer anotherIoCContainer)
        //{
        //    string errorMessage =
        //          "IoCy Programming Error: cannot modify the container since configuration has already been completed.";

        //    ModifyContainer(() => MergeInImpl(anotherIoCContainer), errorMessage);
        //}


        public void InjectType(Type resolvingType)
        {
            ImplementsAttribute implementsAttribute =
                   resolvingType.GetCustomAttribute<ImplementsAttribute>()!;

            if (implementsAttribute != null)
            {
                if (implementsAttribute.TypeToResolve == null)
                {
                    implementsAttribute.TypeToResolve =
                        resolvingType.GetBaseTypeOrFirstInterface() ?? throw new Exception($"IoCy Programming Error: Type {resolvingType.FullName} has an 'Implements' attribute, but does not have any base type and does not implement any interfaces");
                }

                Type typeToResolve = implementsAttribute.TypeToResolve;
                object partKeyObj = implementsAttribute.PartKey;
                bool isSingleton = implementsAttribute.IsSingleton;

                if (isSingleton)
                {
                    this.MapSingletonType(typeToResolve, resolvingType, partKeyObj);
                }
                else
                {
                    this.MapType(typeToResolve, resolvingType, partKeyObj);
                }
            }
            else
            {
                HasFactoryMethodsAttribute? hasFactoryMethodAttribute = 
                    resolvingType.GetCustomAttribute<HasFactoryMethodsAttribute>();

                if (hasFactoryMethodAttribute != null)
                {
                    foreach(var methodInfo in resolvingType.GetMethods().Where(methodInfo => methodInfo.IsStatic))
                    {
                        FactoryMethodAttribute factoryMethodAttribute = methodInfo.GetAttr<FactoryMethodAttribute>();

                        if (factoryMethodAttribute != null)
                        {
                            Type typeToResolve = factoryMethodAttribute.TypeToResolve ?? methodInfo.ReturnType;
                            object partKeyObj = factoryMethodAttribute.PartKey;
                            bool isSingleton = factoryMethodAttribute.IsSingleton;

                            if (isSingleton)
                            {
                                this.MapSingletonFactoryMethodInfo(typeToResolve, methodInfo, partKeyObj);
                            }
                            else
                            {
                                this.MapFactoryMethodInfo(typeToResolve, methodInfo, partKeyObj);
                            }
                        }    
                    }
                }
            }
        }

        public void InjectAssembly(Assembly assembly)
        {
            foreach(Type resolvingType in assembly.GetExportedTypes())
            {
               InjectType(resolvingType);
            }
        }

        public void InjectDynamicAssemblyByFullPath(string assemblyPath)
        {
            if (!File.Exists(assemblyPath))
                throw new Exception($"There is no assembly at path '{assemblyPath}'");

            string absoluteAssemblyPath = Path.GetFullPath(assemblyPath);

            //Assembly.ReflectionOnlyLoadFrom(absoluteAssemblyPath);

            Assembly loadedAssembly = Assembly.LoadFile(absoluteAssemblyPath);

            InjectAssembly(loadedAssembly);
        }


        public void InjectPluginsFromFolder
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

                    InjectAssembly(assembly);
                }
            }
        }

        // loads and injects assemblies that match the rejex 
        // from all direct sub-folders of folder specified
        // by baseFolderPath argument.
        public void InjectPluginsFromSubFolders
        (
            string baseFolderPath, 
            Regex? matchingFileName = null)
        {
            foreach(string folderPath in Directory.GetDirectories(baseFolderPath))
            {
                if (folderPath == "." || folderPath.StartsWith("..")) 
                {
                    continue;
                }
                InjectPluginsFromFolder(folderPath, matchingFileName);
            }
        }
    }
}
