using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyIoCContainerTest
{
    public interface IMyInterface<T>
    {
        void Print(T obj);
    }

    public class MyClass<T> : IMyInterface<T>
    {
        public void Print(T obj)
        {
            Console.WriteLine(obj.ToString());
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            IoCContainer iocContainer = new IoCContainer();

            iocContainer.Register(typeof(IMyInterface<>), typeof(MyClass<>));

            iocContainer.GetTargetObj<IMyInterface<int>>();
        }
    }
}
