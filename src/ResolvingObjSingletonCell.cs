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

namespace NP.IoCy
{
    class ResolvingObjSingletonCell : ResolvingCell
    {
        private bool _isComposed = false;

        public override ResolvingCellType CellType => ResolvingCellType.Singleton;

        private object _obj;

        public override object? GetObj(Container objectComposer)
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
