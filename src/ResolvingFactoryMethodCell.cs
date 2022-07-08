// (c) Nick Polyak 2022 - http://awebpros.com/
// License: MIT License (https://opensource.org/licenses/MIT)
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author of this software if something goes wrong. 
// 
// Also, please, mention this software in any documentation for the 
// products that use it.

using NP.Utilities.BasicInterfaces;
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
