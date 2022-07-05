using NP.Utilities.BasicInterfaces;

namespace NP.IoCy
{
    internal abstract class ResolvingCell : IResolvingCell
    {
        public abstract ResolvingCellType CellType { get; }

        public abstract object? GetObj(IoCContainer objectComposer);
    }
}
