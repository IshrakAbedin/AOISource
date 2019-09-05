using System;

namespace Sched
{
    class Work
    {
        public string Name { get; set;}
        public int Energy { get; set; }
        public int Duration { get; set; }
        public int Priority { get; set; }

        public Work(string name, int energy, int duration, int priority)
        {
            Name = name.Trim().Replace(' ', '_');
            Energy = Math.Abs(energy);
            Duration = Math.Abs(duration);
            Priority = Math.Abs(priority);
            if (Priority > 9) Priority = 9;
        }

        public override string ToString()
        {
            return "work " + Name + " " + Convert.ToString(Energy) + " " + Convert.ToString(Duration) + " " + Convert.ToString(Priority);
        }
    }
}
