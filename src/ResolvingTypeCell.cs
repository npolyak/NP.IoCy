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
