using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sched
{
    class Recreation
    {
        public string Name { get; set; }
        public int Energy { get; set; }

        public Recreation(string name, int energy)
        {
            Name = name.Trim().Replace(' ', '_');
            Energy = Math.Abs(energy);
        }

        public override string ToString()
        {
            return "recr " + Name  + " " + Convert.ToString(Energy);
        }
    }
}
