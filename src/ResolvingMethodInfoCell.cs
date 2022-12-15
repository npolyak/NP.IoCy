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

using System.Reflection;
using NP.IoC.CommonImplementations;

namespace NP.IoCy
{
    internal class ResolvingMethodInfoSingletonCell : ResolvingCell
    {
        public override ResolvingCellType CellType => ResolvingCellType.Singleton;

        private MethodBase _factoryMethod;

        private object? _obj;
        
        public override object? GetObj(Container objectComposer)
        {
            if (_obj == null)
            {
                // create object
                _obj = objectComposer.CreateAndComposeObjFromMethod(_factoryMethod);
            }

            return _obj;
        }


        public ResolvingMethodInfoSingletonCell(MethodBase factoryMethod)
        {
            _factoryMethod = factoryMethod;

            if (!_factoryMethod.IsStatic)
            {
                $"Cannot use instance Method {factoryMethod.Name.Sq()} for Object Creation".ThrowProgError();
            }

            if (factoryMethod is MethodInfo factoryMethodInfo && factoryMethodInfo.ReturnType == null)
            {
                $"Cannot use Void Method {factoryMethod.Name.Sq()} for Object Creation".ThrowProgError();
            }
        }
    }

    internal class ResolvingMethodInfoCell : ResolvingCell
    {
        public override ResolvingCellType CellType => ResolvingCellType.Transient;

        private MethodBase _factoryMethodInfo;

        private object? _obj;

        public override object? GetObj(Container objectComposer)
        {
            return objectComposer.CreateAndComposeObjFromMethod(_factoryMethodInfo);
        }


        public ResolvingMethodInfoCell(MethodBase factoryMethod)
        {
            _factoryMethodInfo = factoryMethod;

            if (!_factoryMethodInfo.IsStatic)
            {
                $"Cannot use instance Method {factoryMethod.Name.Sq()} for Object Creation".ThrowProgError();
            }

            if (factoryMethod is MethodInfo factoryMethodInfo && factoryMethodInfo.ReturnType == null)
            {
                $"Cannot use Void Method {factoryMethod.Name.Sq()} for Object Creation".ThrowProgError();
            }
        }
    }
}
