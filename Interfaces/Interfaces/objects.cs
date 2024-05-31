using interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace interfaces
{
    internal class spada : IPickup
    {
        public void Pickup()
        {
            Console.WriteLine("SPADA!");
        }
    }

    internal class FUCILE : IPickup
    {
        public void Pickup()
        {
            Console.WriteLine("FUCILE!");
        }
    }


    internal class veleno : IPickup
    {
        public void Pickup()
        {
             Console.WriteLine("VELENO!");
        }
    }

    internal class orbomagico : IPickup
    {
        public void Pickup()
        {
            Console.WriteLine("ORBO!");
        }
    }
}
