﻿namespace TestAllRegisterAndResolveMethods.Interfaces
{
    public interface IOrgGettersOnly
    {
        IPersonGettersOnly Manager { get; }

        ILog Log { get; }
    }
}
