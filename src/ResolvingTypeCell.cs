// (c) Nick Polyak 2018 - http://awebpros.com/
// License: MIT License (https://opensource.org/licenses/MIT)
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author of this software if something goes wrong. 
// 
// Also, please, mention this software in any documentation for the 
// products that use it.

using NP.IoC.CommonImplementations;
using System;

namespace NP.IoCy
{
    internal class ResolvingSingletonTypeCell : ResolvingCell
    {
        Type _typeToResolve;

        public override ResolvingCellType CellType => ResolvingCellType.Singleton;

        private object? _obj;

        public override object? GetObj(Container objectComposer)
        {
            if (_obj == null)
            {
                // create object
                _obj = objectComposer.CreateAndComposeObjFromType(_typeToResolve);
            }

            return _obj;
        }

        public ResolvingSingletonTypeCell(Type typeToResolve)
        {
            _typeToResolve = typeToResolve;
        }
    }

    internal class ResolvingTypeCell : ResolvingCell
    {
        Type _typeToResolve;

        public override ResolvingCellType CellType => ResolvingCellType.Common;

        public override object? GetObj(Container objectComposer)
        {
            return objectComposer.CreateAndComposeObjFromType(_typeToResolve);
        }

        public ResolvingTypeCell(Type typeToResolve)
        {
            _typeToResolve = typeToResolve;
        }
    }
}
