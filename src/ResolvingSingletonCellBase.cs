namespace NP.IoCy
{
    abstract class ResolvingSingletonCellBase<T> : IResolvingCellBase<T>
    {
        public abstract ResolvingCellType CellType { get; }

        public object CellContainerId { get; }

        protected T? _obj;

        public bool IfCompositionNotNull { get; set; } = false;

        // the object is composed on the configuration completion stage,
        // so, isComposed is set to true. 
        public T? GetObj(IoCContainer container, out bool isComposed)
        {
            isComposed = true;
            return _obj;
        }

        public ResolvingSingletonCellBase(object cellContainerId)
        {
            CellContainerId = cellContainerId;
        }
    }

}
