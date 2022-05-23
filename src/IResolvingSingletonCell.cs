using System.Collections;

namespace NP.IoCy
{
    interface IResolvingSingletonCell : IResolvingCell
    {
        IList GetAllObjs();
    }
}
