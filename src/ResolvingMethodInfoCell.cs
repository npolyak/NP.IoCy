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
    internal class ResolvingMethodInfoCell : ResolvingCell
    {
        private MethodBase _factoryMethodInfo;

        protected override object? CreateObject(IObjComposer objComposer)
        {
            return 
                objComposer.CreateAndComposeObjFromMethod(_factoryMethodInfo);
        }

        public ResolvingMethodInfoCell(bool isSingleton, MethodBase factoryMethod)
            :
            base(isSingleton)
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
