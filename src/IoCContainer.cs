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
    static class TypeToResolveKeyUtils
    {
        public static ContainerItemResolvingKey ToKey(this Type typeToResolve, object? resolutionKey, bool isMulti = false)
        {
            ContainerItemResolvingKey typeToResolveKey = new ContainerItemResolvingKey(typeToResolve, resolutionKey, isMulti);

            return typeToResolveKey;
        }
    }

    public class IoCContainer : IObjectComposer
    {
        public static int CurrentContainerId { get; private set; }

        static IoCContainer()
        {
            //AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += CurrentDomain_ReflectionOnlyAssemblyResolve;
        }

        private static Assembly CurrentDomain_ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        {
            return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.FullName == args.Name)!;
        }

        public object ContainerId { get; private set; }

        private IoCContainer? ParentContainer { get; set; }

        Dictionary<ContainerItemResolvingKey, IResolvingCell> _typeMap =
            new Dictionary<ContainerItemResolvingKey, IResolvingCell>();

        public bool ConfigurationCompleted { get; private set; } = false;

        private bool _isProtected;

        public event Action? ConfigurationCompletedEvent = null;

        private Func<Type, object>? DefaultResolver { get; }

        public IoCContainer
        (
            bool isProtected = true, 
            Func<Type, object> defaultResolver = null!,
            object? containerId = null
        )
        {
            _isProtected = isProtected;
            DefaultResolver = defaultResolver;

            CurrentContainerId++;
            ContainerId = containerId ?? CurrentContainerId;
        }

        private IResolvingCell? GetResolvingCellCurrentContainer(ContainerItemResolvingKey typeToResolveKey)
        {
            if (_typeMap.TryGetValue(typeToResolveKey, out IResolvingCell? resolvingCell))
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

        private bool IsOwnContainerId(object containerId)
        {
            return containerId.ObjEquals(ContainerId);
        }

        IResolvingCell AddCellUnprotected(ContainerItemResolvingKey typeToResolveKey, IResolvingCell resolvingCell)
        {
            if (_typeMap.TryGetValue(typeToResolveKey, out IResolvingCell? cell))
            {
                if (IsOwnContainerId(cell.CellContainerId))
                {
                    throw new Exception($"key {typeToResolveKey.ToString()} is already mapped to '{cell.ToString()}' cell.");
                }
                else
                {

                }
            }

            _typeMap[typeToResolveKey] = resolvingCell;

            return resolvingCell;
        }

        IResolvingCell AddCellProtected(ContainerItemResolvingKey typeToResolveKey, IResolvingCell resolvingCell)
        {
            if (ConfigurationCompleted)
            {
                throw new Exception($"cannot add key '{typeToResolveKey.ToString()}' mapped to '{resolvingCell.ToString()}' cell since configuration has already been completed.");
            }

            lock (_typeMap)
            {
                return AddCellUnprotected(typeToResolveKey, resolvingCell);
            }
        }

        ResolvingSingletonMultiCell AddOrGetMultiCellBase(ContainerItemResolvingKey typeToResolveKey, bool isProtected)
        {
            if (_typeMap.TryGetValue(typeToResolveKey, out IResolvingCell? cell))
            {
                if (cell is ResolvingSingletonMultiCell multiCell)
                {
                    return multiCell;
                }
                else
                {
                    throw new Exception($"IoCy Programming Error: key {typeToResolveKey.ToStr()} is already mapped to a not-multi cell");
                }
            }
            else
            {
                if (isProtected && ConfigurationCompleted)
                {
                    throw new Exception($"cannot add key '{typeToResolveKey.ToString()}'  multicell since configuration has already been completed.");
                }

                ResolvingSingletonMultiCell multiCell = new ResolvingSingletonMultiCell(typeToResolveKey.TypeToResolve, ContainerId);
                _typeMap.Add(typeToResolveKey, multiCell);

                return multiCell;
            }
        }

        ResolvingSingletonMultiCell AddOrGetMultiCellUnprotected(ContainerItemResolvingKey typeToResolveKey)
        {
            return AddOrGetMultiCellBase(typeToResolveKey, false);
        }

        ResolvingSingletonMultiCell AddOrGetMultiCellProtected(ContainerItemResolvingKey typeToResolveKey)
        {
            lock (_typeMap)
            {
                return AddOrGetMultiCellBase(typeToResolveKey, true);
            }
        }

        private IResolvingCell AddCell(ContainerItemResolvingKey typeToResolveKey, IResolvingCell resolvingCell)
        {
            if (_isProtected)
            {
                return AddCellProtected(typeToResolveKey, resolvingCell);
            }
            else
            {
                return AddCellUnprotected(typeToResolveKey, resolvingCell);
            }
        }

        private (object, bool) ResolveCurrentObj(ContainerItemResolvingKey typeToResolveKey, out bool isComposed)
        {
            IResolvingCell resolvingCell = GetResolvingCell(typeToResolveKey);

            if (resolvingCell == null)
            {
                isComposed = true;
                return (DefaultResolver?.Invoke(typeToResolveKey.TypeToResolve)!, false);
            }

            return (resolvingCell.GetObj(this, out isComposed)!, resolvingCell.IfCompositionNotNull);
        }

        private (object, bool) ResolveCurrentObj(Type typeToResolve, out bool isComposed, object? resolutionKey = null)
        {
            return ResolveCurrentObj(typeToResolve.ToKey(resolutionKey), out isComposed);
        }

        public void MapType(Type typeToResolve, Type resolvingType, object? resolutionKey = null)
        {
            CheckTypeDerivation(typeToResolve, resolvingType);
            AddCell(typeToResolve.ToKey(resolutionKey!), new ResolvingTypeCell(resolvingType, ContainerId));
        }

        void AddObjToMultiCellUnprotected(ResolvingSingletonMultiCell multiCell, object resolvingObj)
        {
            multiCell.Add(resolvingObj);
        }

        void AddObjToMultiCellProtected(ResolvingSingletonMultiCell multiCell, object resolvingObj)
        {
            lock (multiCell)
            {
                AddObjToMultiCellUnprotected(multiCell, resolvingObj);
            }
        }

        public void MapMultiPartObj(Type typeToResolve, object resolvingObj, object? resolutionKey = null)
        {
            ContainerItemResolvingKey typeToResolveKey = typeToResolve.ToKey(resolutionKey);

            if (_isProtected && ConfigurationCompleted)
            {
                throw new Exception($"Cannot add another object to a multicell with key '{typeToResolveKey.ToStr()}' after Configuration has been Completed");
            }

            CheckTypeDerivation(typeToResolve, resolvingObj.GetType());

            ResolvingSingletonMultiCell? multiCell = null;

            if(_isProtected)
            {
                multiCell = AddOrGetMultiCellProtected(typeToResolveKey);
            }
            else
            {
                multiCell = AddOrGetMultiCellUnprotected(typeToResolveKey);
            }

            if (!_isProtected)
            {
                ComposeObject(resolvingObj, multiCell.IfCompositionNotNull);
            }

            if (_isProtected)
            {
                AddObjToMultiCellProtected(multiCell, resolvingObj);
            }
            else
            {
                AddObjToMultiCellUnprotected(multiCell, resolvingObj);
            }
        }

        public void MapMultiPartObj<TToResolve>(object resolvingObj, object? resolutionKey = null)
        {
            MapMultiPartObj(typeof(TToResolve), resolvingObj, resolutionKey);
        }

        public void MapMultiType(Type typeToResolve, Type resolvingType, object? resolutionKey = null)
        {
            object resolvingObj = ConstructObject(resolvingType);

            MapMultiPartObj(typeToResolve, resolvingObj, resolutionKey);
        }

        public void MapMultiType<TToResolve, TResolving>(object? resolutionKey = null)
        {
            MapMultiType(typeof(TToResolve), typeof(TResolving), resolutionKey);
        }


        public void Map<TToResolve, TResolving>(object? resolutionKey = null)
            where TResolving : TToResolve
        {
            MapType(typeof(TToResolve), typeof(TResolving), resolutionKey);
        }


        private void MapSingletonObjImpl
        (
            Type typeToResolve, 
            object resolvingObj, 
            object? resolutionKey = null,
            bool ifCompositionNotNull = false)
        {
            CheckTypeDerivation(typeToResolve, resolvingObj.GetType());

            if (!_isProtected)
            {
                ComposeObject(resolvingObj, ifCompositionNotNull);
            }

            AddCell
            (
                typeToResolve.ToKey(resolutionKey), 
                new ResolvingSingletonCell(resolvingObj, ContainerId) { IfCompositionNotNull = ifCompositionNotNull });
        }

        private void MapSingletonTypeImpl
        (
            Type typeToResolve, 
            Type resolvingObjType, 
            object? resolutionKey = null,
            bool ifCompositionNotNull = false)
        {
            object resolvingObj = ConstructObject(resolvingObjType);

            MapSingletonObjImpl(typeToResolve, resolvingObj, resolutionKey, ifCompositionNotNull);
        }

        public void MapSingleton<TToResolve, TResolving>
        (
            TResolving resolvingObj, 
            object? resolutionKey = null, 
            bool ifCompositionNotNull = false)
            where TResolving : TToResolve
        {
            MapSingletonObjImpl(typeof(TToResolve), resolvingObj!, resolutionKey, ifCompositionNotNull);
        }

        public void MapSingleton<TToResolve, TResolving>
        (
            object? resolutionKey = null, 
            bool ifCompositionNotNull = false)
            where TResolving : TToResolve
        {
            MapSingletonTypeImpl(typeof(TToResolve), typeof(TResolving), resolutionKey, ifCompositionNotNull);
        }

        public void MapFactory<TResolving>(Type typeToResolve, Func<TResolving> resolvingFactory, object? resolutionKey = null)
        {
            CheckTypeDerivation(typeToResolve, typeof(TResolving));
            AddCell
            (
                typeToResolve.ToKey(resolutionKey), 
                new ResolvingFactoryMethodCell<TResolving>(resolvingFactory, ContainerId));
        }

        public void MapFactory<TToResolve, TResolving>(Func<TResolving> resolvingFactory, object? resolutionKey = null)
            where TResolving : TToResolve
        {
            MapFactory(typeof(TToResolve), resolvingFactory, resolutionKey);
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

            if (!partAttr.IsMulti)
            {
                return realPropOrParamType?.ToKey(partAttr.PartKey);
            }
            else
            {
                return realPropOrParamType?.GetGenericArguments().First().ToKey(partAttr.PartKey, true);
            }
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
            if (!key.IsMulti)
            {
                return Resolve(key);
            }
            else
            {
                return MultiResolve(key);
            }
        }

        public void ComposeObject(object obj, bool ifCompositionNotNull = false)
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

                if ((!ifCompositionNotNull) || (subObj != null))
                {
                    propInfo.SetMethod.Invoke(obj, new[] { subObj });
                }
            }
        }

        internal IEnumerable<object?> GetConstructorParamValues(ConstructorInfo constructorInfo)
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

            return Activator.CreateInstance(type, GetConstructorParamValues(constructorInfo).ToArray())!;
        }

        private object Resolve(ContainerItemResolvingKey typeToResolveKey)
        {
            bool isComposed;
            (object resolvingObj, bool ifCompositionNotNull) =
                ResolveCurrentObj
                (
                    typeToResolveKey,
                    out isComposed);

            if (!isComposed)
            {
                ComposeObject(resolvingObj, ifCompositionNotNull);
            }

            return resolvingObj;
        }

        private object ResolveImpl(ContainerItemResolvingKey typeToResolveKey)
        {
            if (_isProtected && (!ConfigurationCompleted))
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

        private IEnumerable? MultiResolve(ContainerItemResolvingKey typeToResolveKey)
        {
            IEnumerable? result = ResolveImpl(typeToResolveKey) as IEnumerable;

            return result;
        }

        public IEnumerable<TToResolve> MultiResolve<TToResolve>(object? resolutionKey = null)
        {
            IEnumerable<TToResolve> result =
                MultiResolve(typeof(TToResolve).ToKey(resolutionKey))?.Cast<TToResolve>()!;

            if (result == null)
                result = Enumerable.Empty<TToResolve>();

            return result;
        }

        public IEnumerable<TResult> MultiResolve<TToResolve, TResult>(object? resolutionKey = null)
            where TResult : TToResolve
        {
            return MultiResolve<TToResolve>().Where(item => item is TResult).Cast<TResult>();
        }

        private void ComposeAllSingletonObjects()
        {
            foreach(IResolvingCell resolvingCell in this._typeMap.Values)
            {
                IResolvingSingletonCell? singletonCell =
                    resolvingCell as IResolvingSingletonCell;

                if (singletonCell != null)
                {
                    foreach(object obj in singletonCell.GetAllObjs())
                    {
                        ComposeObject(obj, singletonCell.IfCompositionNotNull);
                    }
                }
            }
        }

        // before calling this method you cannot resolve any objects
        // from the container. 
        // after calling this method, you cannot modify 
        // the container any more.
        public void CompleteConfiguration()
        {
            if (!_isProtected)
            {
                throw new Exception($"Should not call CompletedConfiguration on unprotected container");
            }

            ComposeAllSingletonObjects();

            ConfigurationCompleted = true;

            ConfigurationCompletedEvent?.Invoke();
        }

        private void ModifyContainer(Action modificationAction, string errorMessage)
        {
            if (_isProtected)
            {
                if (ConfigurationCompleted)
                {
                    throw new Exception(errorMessage);
                }
                else
                {
                    lock (_typeMap)
                    {
                        modificationAction();
                    }
                }
            }
            else
            {
                modificationAction();
            }
        }

        public void Remove(Type typeToResolve, object resolutionKey)
        {
            ContainerItemResolvingKey typeToResolveKey = typeToResolve.ToKey(resolutionKey);

            string errorMessage =
                $"IoCy Programming Error: cannot remove key '{typeToResolveKey.ToString()}' since configuration has already been completed.";

            ModifyContainer(() => _typeMap.Remove(typeToResolveKey), errorMessage);
        }

        private void MergeInImpl(IoCContainer anotherIoCContainer)
        {
            foreach (KeyValuePair<ContainerItemResolvingKey, IResolvingCell> kvp in anotherIoCContainer._typeMap)
            {
                ContainerItemResolvingKey typeToResolveKey = kvp.Key;

                IResolvingCell valCopy = kvp.Value.Copy();

                AddCellProtected(typeToResolveKey, valCopy);
            }
        }

        public void MergeIn(IoCContainer anotherIoCContainer)
        {
            string errorMessage =
                  "IoCy Programming Error: cannot modify the container since configuration has already been completed.";

            ModifyContainer(() => MergeInImpl(anotherIoCContainer), errorMessage);
        }

        public IoCContainer CreateChild(bool isProtected = true)
        {
            IoCContainer container = new IoCContainer(isProtected);

            container.ParentContainer = this;

            container.MergeIn(this);

            return container;
        }

        public void InjectType(Type resolvingType)
        {
            ImplementsAttribute implementsAttribute =
                   resolvingType.GetCustomAttribute<ImplementsAttribute>()!;

            if (implementsAttribute == null)
                return;

            if (implementsAttribute.TypeToResolve == null)
            {
                implementsAttribute.TypeToResolve =
                    resolvingType.GetBaseTypeOrFirstInterface() ?? throw new Exception($"IoCy Programming Error: Type {resolvingType.FullName} has an 'Implements' attribute, but does not have any base type and does not implement any interfaces");
            }

            Type typeToResolve = implementsAttribute.TypeToResolve;
            object partKeyObj = implementsAttribute.PartKey;
            bool isSingleton = implementsAttribute.IsSingleton;

            if (!implementsAttribute.IsMulti)
            {
                if (isSingleton)
                {
                    this.MapSingletonTypeImpl(typeToResolve, resolvingType, partKeyObj);
                }
                else
                {
                    this.MapType(typeToResolve, resolvingType, partKeyObj);
                }
            }
            else
            {
                this.MapMultiType(typeToResolve, resolvingType, partKeyObj);
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
