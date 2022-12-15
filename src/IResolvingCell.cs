﻿// (c) Nick Polyak 2022 - http://awebpros.com/
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
    enum ResolvingCellType
    {
        Transient,
        Singleton
    }

    internal interface IResolvingCell 
    {
        ResolvingCellType CellType { get; }

        object? GetObj(Container objectComposer);
    }
}
