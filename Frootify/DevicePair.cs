using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frootify
{
    public class DevicePair
    {
        public int Index { get; set; }
        public string Name { get; set; }

        public DevicePair(int index, string name)
        {
            Index = index;
            Name = name;
        }
    }
}
