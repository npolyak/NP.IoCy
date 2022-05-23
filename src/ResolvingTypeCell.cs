using System;

namespace NP.IoCy
{
    class ResolvingTypeCell : IResolvingCell
    {
        Type _type;

        public ResolvingCellType CellType => ResolvingCellType.Common;

        public object CellContainerId { get; }

        public bool IfCompositionNotNull { get; set; } = false;

        public object GetObj(IoCContainer container, out bool isComposed)
        {
            isComposed = false;
            return container.ConstructObject(_type);
        }

        public ResolvingTypeCell(Type type, object cellContainerId)
        {
            _type = type;
            CellContainerId = cellContainerId;
        }

        public override string ToString()
        {
            return $"Type:{_type.Name}";
        }

        public IResolvingCell Copy()
        {
            return new ResolvingTypeCell(_type, CellContainerId);
        }
    }
}
