using Microsoft.VisualBasic;
using NP.DependencyInjection.Interfaces;
using NP.IoC.CommonImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;

namespace NP.IoCy
{
    public class ContainerBuilder<TKey> : AbstractContainerBuilder<TKey>, IContainerBuilderWithMultiCells<TKey>
    {
        public bool AllowOverrides { get; }

        private Dictionary<TKey, object?> ResolutionKeys { get; } =
            new Dictionary<TKey, object?>();

        internal IDictionary<FullContainerItemResolvingKey<TKey>, IResolvingCell> _cellMap =
            new Dictionary<FullContainerItemResolvingKey<TKey>, IResolvingCell>();

        private IResolvingCell? GetCurrentCell(FullContainerItemResolvingKey<TKey> key)
        {
            if (_cellMap.TryGetValue(key, out var currentCell))
            {
                return currentCell;
            }

            return null;
        }

        private IResolvingCell? GetCurrentCell(Type resolvingType, TKey resolutionKey = default)
        {
            return GetCurrentCell(resolvingType.ToKey<TKey>(resolutionKey));
        }

        private void AddCell
        (
            FullContainerItemResolvingKey<TKey> typeToResolveKey,
            IResolvingCell resolvingCell)
        {
            if (resolvingCell is ResolvingMultiObjCell)
            {
                throw new ProgrammingError($"MultiCells cannot be handled by this method (key = '{typeToResolveKey.ToStr()}'");
            }

            lock (_cellMap)
            {
                IResolvingCell? currentCell = GetCurrentCell(typeToResolveKey);
                if (currentCell != null)
                {
                    // found an existing (ResolvingType, ResolutionKey) combination
                    if (currentCell is ResolvingMultiObjCell multiObjCell)
                    {
                        throw new 
                            ProgrammingError($"This method cannot add a cell to a MultiCell (key = '{typeToResolveKey.ToStr()}'");
                        //if (!isMulti)
                        //{
                        //    throw new ProgrammingError($"Cannot add to a multicell for within this workflow. The key is '{typeToResolveKey.ToStr()}'");
                        //}

                        //addedCell = true;
                        //multiObjCell.AddCell(resolvingCell, resolvingType);
                    }
                    else if (!AllowOverrides)
                    {
                        throw new Exception($"ERROR: Trying to override already existing key '{typeToResolveKey.ToStr()}'. Unregister the old key first!");
                    }
                    else if (resolvingCell is ResolvingMultiObjCell)
                    {
                        throw new Exception($"ERROR: Trying to override a normal cell with a multicell for key '{typeToResolveKey.ToStr()}'");
                    }
                }
                else // did NOT find an existing (ResolvingType, ResolutionKey) combination
                {
                    // if the resolution key is not null (default) check that such key has not been already used. 

                    TKey resolutionKey = typeToResolveKey.KeyObject;

                    if (!resolutionKey.ObjEquals(default(TKey)))
                    {
                        if (ResolutionKeys.ContainsKey(resolutionKey!))
                        {
                            throw new Exception($"Non default ResolutionKey {resolutionKey} has already been used");
                        }
                        else
                        {
                            ResolutionKeys.Add(resolutionKey!, null);
                        }
                    }
                }

                _cellMap[typeToResolveKey] = resolvingCell;
            }
        }

        public ContainerBuilder(bool allowOverrides = false)
        {
            AllowOverrides = allowOverrides;
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


        private ResolvingMultiObjCell AddEmptyMultiCell(Type cellType, TKey resolutionKey = default)
        {
            ResolvingMultiObjCell multiCellToAdd = new ResolvingMultiObjCell(cellType);

            Type resolvingType = multiCellToAdd.ResolvingType;

            FullContainerItemResolvingKey<TKey> fullResolvingKey = resolvingType.ToKey(resolutionKey);

            var multiCellToReturn = multiCellToAdd;

            lock (_cellMap)
            {
                IResolvingCell? currentCell = GetCurrentCell(resolvingType, resolutionKey);

                if (currentCell == null) // new multi cell
                {
                    _cellMap[fullResolvingKey] = multiCellToAdd;
                    ResolutionKeys.Add(resolutionKey!, null);
                }
                else if (currentCell is ResolvingMultiObjCell currentMultiCell)
                {
                    multiCellToReturn = currentMultiCell;
                }
                else
                {
                    throw new ProgrammingError($"current cell already exists for key '{resolutionKey}' and it is not a MULTIcell");
                }
            }

            return multiCellToReturn;
        }

        public void RegisterMultiCell
        (
            Type cellType,
            TKey resolutionKey = default
        )
        {
            AddEmptyMultiCell(cellType, resolutionKey);
        }

        public void RegisterMultiCell<TCell>(TKey resolutionKey = default)
        {
            AddEmptyMultiCell(typeof(TCell), resolutionKey);
        }

        public void RegisterMultiCellObjInstance
        (
            Type cellType,  
            object instance, 
            TKey resolutionKey = default)
        {
            var multiCell = 
                AddEmptyMultiCell(cellType, resolutionKey);

            multiCell.AddCell(new ResolvingObjSingletonCell(instance), instance.GetType());
        }

        public void RegisterMultiCellObjInstance<TCell>(object instance, TKey resolutionKey = default)
        {
            RegisterMultiCellObjInstance(typeof(TCell), instance, resolutionKey);
        }

        public void RegisterMultiCellType
        (
            Type cellType,
            Type typeToResolve,
            TKey resolutionKey = default)
        {
            var multiCell = AddEmptyMultiCell(cellType, resolutionKey);

            multiCell.AddCell(new ResolvingTypeCell(true, typeToResolve), typeToResolve);
        }

        public void RegisterMultiCellType<TCell, TToResolve>(TKey resolutionKey = default)
        {
            RegisterMultiCellType(typeof(TCell), typeof(TToResolve), resolutionKey);
        }

        protected override void RegisterAttributedMultiCellType
        (
            Type cellType,
            Type typeToResolve,
            TKey resolutionKey = default)
        {
            RegisterMultiCellType(cellType, typeToResolve, resolutionKey);
        }

        public void RegisterMultiCellFactoryMethod<TCell>(Func<TCell> resolvingFunc, TKey resolutionKey = default)
        {
            var multiCell = AddEmptyMultiCell(typeof(TCell), resolutionKey);

            multiCell.AddCell(new ResolvingFactoryMethodCell<TCell>(true, resolvingFunc), typeof(TCell));
        }


        public void RegisterMultiCellFactoryMethod<TCell>(Func<IEnumerable<TCell>> resolvingFunc, TKey resolutionKey = default)
        {
            var multiCell = AddEmptyMultiCell(typeof(TCell), resolutionKey);

            multiCell.AddCell(new ResolvingFactoryMethodCell<IEnumerable<TCell>>(true, resolvingFunc), typeof(IEnumerable<TCell>));
        }

        public override void RegisterMultiCellFactoryMethodInfo
        (
            MethodBase factoryMethodInfo, 
            Type cellType, 
            TKey resolutionKey = default)
        {
            var multiCell = AddEmptyMultiCell(cellType, resolutionKey);

            var resolvingType = factoryMethodInfo.GetMethodType();

            multiCell.AddCell(new ResolvingMethodInfoCell(true, factoryMethodInfo), resolvingType);
        }

        public void RegisterMultiCellFactoryMethodInfo<TCell>
        (
            MethodBase factoryMethodInfo, 
            TKey resolutionKey = default)
        {
            RegisterFactoryMethodInfo(factoryMethodInfo, typeof(TCell), resolutionKey);
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
