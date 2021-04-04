using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceStubTest
{
    static class Programm
    {
        static void Main(string[] args)
        {
            var entity = new EntityStub();
            var iEntity = (IEntity)entity;

            Console.WriteLine(entity);
        }
    }
}
