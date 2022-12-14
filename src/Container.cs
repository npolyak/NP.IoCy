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
using NP.Utilities;
using System.Collections.Generic;

namespace NP.IoCy
{
    public class Container : AbstractContainer, IDependencyInjectionContainer
    {
        Dictionary<FullContainerItemResolvingKey, IResolvingCell> _cellMap =
            new Dictionary<FullContainerItemResolvingKey, IResolvingCell>();

        internal Container (Dictionary<FullContainerItemResolvingKey, IResolvingCell> cellMap)
        {
            _cellMap.AddAll(cellMap);

            ComposeAllSingletonObjects();
        }

        private IResolvingCell? GetResolvingCellCurrentContainer(FullContainerItemResolvingKey resolvingKey)
        {
            if (_cellMap.TryGetValue(resolvingKey, out IResolvingCell? resolvingCell))
            {
                return resolvingCell;
            }

            return null;
        }

        private IResolvingCell GetResolvingCell(FullContainerItemResolvingKey resolvingKey)
        {
            IResolvingCell resolvingCell = GetResolvingCellCurrentContainer(resolvingKey)!;

            return resolvingCell;
        }

        protected override object ResolveKey(FullContainerItemResolvingKey fullResolvingKey)
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
    }
}
