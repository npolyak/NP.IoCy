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

using NP.IoC.CommonImplementations;
using System;
using System.Collections.Generic;

namespace NP.IoCy
{
    internal abstract class ResolvingCell : IResolvingCell
    {
        internal protected bool IsSingleton { get; }
        public ResolvingCellKind CellKind =>
            IsSingleton ? ResolvingCellKind.Singleton : ResolvingCellKind.Transient;

        public ResolvingCell(bool isSingleton)
        {
            IsSingleton = isSingleton;
        }

        public object? TheObj { get; private set; }

        private bool _hasCreated = false;

        public virtual object? GetObj(IObjComposer objectComposer)
        {
            if ((!IsSingleton) || (!_hasCreated))
            {
                // create object
                TheObj = CreateObject(objectComposer);

                _hasCreated = true;
            }

            return TheObj;
        }

        protected abstract object? CreateObject(IObjComposer objComposer);
    }
}
