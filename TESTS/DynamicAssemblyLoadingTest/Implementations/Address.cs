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

using NP.Utilities.Attributes;
using DynamicAssemblyLoadingTest.Interfaces;

namespace DynamicAssemblyLoadingTest.Implementations
{
    [Implements]
    public class Address : IAddress
    {
        public string City { get; set; }

        public string ZipCode { get; set; }
    }
}
