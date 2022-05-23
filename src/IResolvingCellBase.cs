namespace NP.IoCy
{
    enum ResolvingCellType
    {
        Common,
        Singleton,
        Multi
    }

    interface IResolvingCellBase<T>
    {
        ResolvingCellType CellType { get; }

        object CellContainerId { get; }

        bool IfCompositionNotNull { get; set; }
        T? GetObj(IoCContainer container, out bool isComposed);
    }
}
