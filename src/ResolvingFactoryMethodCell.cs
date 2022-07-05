using System;

namespace NP.IoCy
{
    internal class ResolvingFactoryMethodSingletonCell<T> : ResolvingCell
    {
        public override ResolvingCellType CellType => ResolvingCellType.Singleton;

        private object _obj;

        Func<T> _func;

        public override object? GetObj(IoCContainer objectComposer)
        {
            return _obj;
        }

        public ResolvingFactoryMethodSingletonCell(Func<T> func)
        {
            _func = func;

            _obj = _func()!;
        }
    }

    internal class ResolvingFactoryMethodCell<T> : ResolvingCell
    {
        public override ResolvingCellType CellType => ResolvingCellType.Singleton;

        Func<T> _func;

        public override object? GetObj(IoCContainer objectComposer)
        {
            return _func();
        }

        public ResolvingFactoryMethodCell(Func<T> func)
        {
            _func = func;
        }
    }
}
