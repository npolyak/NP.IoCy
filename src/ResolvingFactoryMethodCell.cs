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
    internal class ResolvingFactoryMethodSingletonCell<TResolving> : ResolvingCell
    {
        public override ResolvingCellType CellType => ResolvingCellType.Singleton;

        private object _obj;

        Func<TResolving> _func;

        public override object? GetObj(Container objectComposer)
        {
            return _obj;
        }

        public ResolvingFactoryMethodSingletonCell(Func<TResolving> func)
        {
            _func = func;

            _obj = _func()!;
        }
    }

    internal class ResolvingFactoryMethodCell<TResolving> : ResolvingCell
    {
        public override ResolvingCellType CellType => ResolvingCellType.Common;

        Func<TResolving> _func;

        public override object? GetObj(Container objectComposer)
        {
            return _func();
        }

        public ResolvingFactoryMethodCell(Func<TResolving> func)
        {
            _func = func;
        }
    }
}
