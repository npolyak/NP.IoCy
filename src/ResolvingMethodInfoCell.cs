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
