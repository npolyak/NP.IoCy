﻿// (c) Nick Polyak 2018 - http://awebpros.com/
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
using System.Threading;

namespace NP.IoCy
{
    internal class ResolvingTypeCell : ResolvingCell
    {
        Type _typeToResolve;

        protected override object? CreateObject(IObjComposer objComposer)
        {
            return objComposer.CreateAndComposeObjFromType(_typeToResolve);
        }

        public ResolvingTypeCell(bool isSingleton, Type typeToResolve)
            :
            base(isSingleton)
        {
            _typeToResolve = typeToResolve;
        }
    }
}
