using System;

namespace NP.IoCy
{
    class ResolvingFactoryMethodCell<T> : IResolvingCell
    {
        public ResolvingCellType CellType => ResolvingCellType.Common;

        public object CellContainerId { get; }

        public bool IfCompositionNotNull { get; set; } = false;

        public Func<T> _factoryMethod;

        public object? GetObj(IoCContainer container, out bool isComposed)
        {
            isComposed = false;
            return _factoryMethod.Invoke();
        }

        public ResolvingFactoryMethodCell(Func<T> factoryMethod, object cellContainerId)
        {
            _factoryMethod = factoryMethod;

            CellContainerId = cellContainerId;
        }

        public override string ToString()
        {
            return "FactoryMethod";
        }

        public IResolvingCell Copy()
        {
            return new ResolvingFactoryMethodCell<T>(_factoryMethod, CellContainerId);
        }
    }
}
