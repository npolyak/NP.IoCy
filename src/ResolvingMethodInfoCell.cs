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
using NP.Utilities;
using System.Reflection;

namespace NP.IoCy
{
    internal class ResolvingMethodInfoSingletonCell : ResolvingCell
    {
        public override ResolvingCellType CellType => ResolvingCellType.Singleton;

        private MethodInfo _factoryMethodInfo;

        private object? _obj;
        
        public override object? GetObj(IoCContainer objectComposer)
        {
            if (_obj == null)
            {
                // create object
                _obj = objectComposer.CreateAndComposeObjFromMethod(_factoryMethodInfo);
            }

            return _obj;
        }


        public ResolvingMethodInfoSingletonCell(MethodInfo factoryMethodInfo)
        {
            _factoryMethodInfo = factoryMethodInfo;

            if (!_factoryMethodInfo.IsStatic)
            {
                $"Cannot use instance Method {factoryMethodInfo.Name.Sq()} for Object Creation".ThrowProgError();
            }

            if (_factoryMethodInfo.ReturnType == null)
            {
                $"Cannot use Void Method {factoryMethodInfo.Name.Sq()} for Object Creation".ThrowProgError();
            }
        }
    }

    internal class ResolvingMethodInfoCell : ResolvingCell
    {
        public override ResolvingCellType CellType => ResolvingCellType.Singleton;

        private MethodInfo _factoryMethodInfo;

        private object? _obj;

        public override object? GetObj(IoCContainer objectComposer)
        {
            return objectComposer.CreateAndComposeObjFromMethod(_factoryMethodInfo);
        }


        public ResolvingMethodInfoCell(MethodInfo factoryMethodInfo)
        {
            _factoryMethodInfo = factoryMethodInfo;

            if (!_factoryMethodInfo.IsStatic)
            {
                $"Cannot use instance Method {factoryMethodInfo.Name.Sq()} for Object Creation".ThrowProgError();
            }

            if (_factoryMethodInfo.ReturnType == null)
            {
                $"Cannot use Void Method {factoryMethodInfo.Name.Sq()} for Object Creation".ThrowProgError();
            }
        }
    }
}
