using NP.Utilities.BasicInterfaces;

namespace NP.IoCy
{
    enum ResolvingCellType
    {
        Common,
        Singleton
    }

    internal interface IResolvingCell 
    {
        ResolvingCellType CellType { get; }

        object? GetObj(IoCContainer objectComposer);
    }
}
