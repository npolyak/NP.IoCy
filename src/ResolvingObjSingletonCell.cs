using NP.Utilities.BasicInterfaces;

namespace NP.IoCy
{
    class ResolvingObjSingletonCell : ResolvingCell
    {
        private bool _isComposed = false;

        public override ResolvingCellType CellType => ResolvingCellType.Singleton;

        private object _obj;

        public override object? GetObj(IoCContainer objectComposer)
        {
            if (!_isComposed)
            {
                objectComposer.ComposeObject(_obj);
                _isComposed = true;
            }

            return _obj;
        }

        public ResolvingObjSingletonCell(object obj)
        {
            _obj = obj;
        }
    }
}
