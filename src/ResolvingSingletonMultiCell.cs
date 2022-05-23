using System;
using System.Collections;
using System.Collections.Generic;

namespace NP.IoCy
{
    class ResolvingSingletonMultiCell : ResolvingSingletonCellBase<IList>, IResolvingSingletonCell
    {
        public override ResolvingCellType CellType => ResolvingCellType.Multi;

        Type _itemType;
        public ResolvingSingletonMultiCell(Type itemType, object cellContainerId) : base(cellContainerId)
        {
            _itemType = itemType;
            _obj = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(_itemType))!;
        }

        public override string ToString()
        {
            return $"objects:{string.Join(",", _obj)}";
        }

        public void Add(object item)
        {
            _obj!.Add(item);
        }

        object IResolvingCellBase<object>.GetObj(IoCContainer container, out bool isComposed)
        {
            return GetObj(container, out isComposed)!;
        }

        public IList GetAllObjs()
        {
            return _obj!;
        }

        public IResolvingCell Copy()
        {
            ResolvingSingletonMultiCell result = new ResolvingSingletonMultiCell(_itemType, CellContainerId);

            foreach (object item in _obj!)
            {
                result.Add(item);
            }

            return result;
        }
    }

}
