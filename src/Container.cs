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

using NP.DependencyInjection.Interfaces;
using NP.IoC.CommonImplementations;
using System;
using System.Collections.Generic;

namespace NP.IoCy
{
    public class Container<TKey> : AbstractContainer<TKey>, IDependencyInjectionContainer<TKey>
    {
        Dictionary<FullContainerItemResolvingKey<TKey>, IResolvingCell> _cellMap =
            new Dictionary<FullContainerItemResolvingKey<TKey>, IResolvingCell>();

        internal Container (IDictionary<FullContainerItemResolvingKey<TKey>, IResolvingCell> cellMap)
        {
            _cellMap.AddAll(cellMap);

            ComposeAllSingletonObjects();
        }

        private IResolvingCell? GetResolvingCellCurrentContainer
        (
            FullContainerItemResolvingKey<TKey> resolvingKey)
        {
            if (_cellMap.TryGetValue(resolvingKey, out IResolvingCell? resolvingCell))
            {
                return resolvingCell;
            }

            return null;
        }

        private IResolvingCell GetResolvingCell(FullContainerItemResolvingKey<TKey> resolvingKey)
        {
            IResolvingCell resolvingCell = GetResolvingCellCurrentContainer(resolvingKey)!;

            return resolvingCell;
        }

        protected override object ResolveKey(FullContainerItemResolvingKey<TKey> fullResolvingKey)
        {
            IResolvingCell resolvingCell = GetResolvingCell(fullResolvingKey);

            return resolvingCell?.GetObj(this)!;
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

        public IEnumerable<TResolving> ResolveMultiCell<TResolving>
        (
            TKey resolutionKey = default)
        {
            object result = ResolveKey(typeof(TResolving).ToKey(resolutionKey));

            return (IEnumerable<TResolving>)result;
        }
    }

    public class Container : Container<object?>
    {
        internal Container(IDictionary<FullContainerItemResolvingKey<object?>, IResolvingCell> cellMap) 
        : 
        base(cellMap)
        {
            
        }
    }
}
