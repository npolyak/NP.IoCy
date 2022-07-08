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

using System;

namespace NP.IoCy
{
    internal class ResolvingSingletonTypeCell : ResolvingCell
    {
        Type _resolvingType;

        public override ResolvingCellType CellType => ResolvingCellType.Singleton;

        private object? _obj;

        public override object? GetObj(IoCContainer objectComposer)
        {
            if (_obj == null)
            {
                // create object
                _obj = objectComposer.CreateAndComposeObjFromType(_resolvingType);
            }

            return _obj;
        }

        public ResolvingSingletonTypeCell(Type resolvingType)
        {
            _resolvingType = resolvingType;
        }
    }

    internal class ResolvingTypeCell : ResolvingCell
    {
        Type _resolvingType;

        public override ResolvingCellType CellType => ResolvingCellType.Common;

        public override object? GetObj(IoCContainer objectComposer)
        {
            return objectComposer.CreateAndComposeObjFromType(_resolvingType);
        }

        public ResolvingTypeCell(Type resolvingType)
        {
            _resolvingType = resolvingType;
        }
    }
}
