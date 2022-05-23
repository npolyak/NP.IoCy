using System.Collections;

namespace NP.IoCy
{
    class ResolvingSingletonCell : ResolvingSingletonCellBase<object>, IResolvingSingletonCell
    {
        public override ResolvingCellType CellType => ResolvingCellType.Singleton;

        public ResolvingSingletonCell(object? obj, object cellContainerId) : base(cellContainerId)
        {
            _obj = obj;
        }

        public IResolvingCell Copy()
        {
            return new ResolvingSingletonCell(_obj, CellContainerId);
        }

        public IList GetAllObjs()
        {
            return new[] { _obj };
        }

        public override string ToString()
        {
            return $"object:{_obj?.ToString()}";
        }
    }
}
