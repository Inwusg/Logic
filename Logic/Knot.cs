using System;
using System.Collections.Generic;
using System.Text;

namespace Logic
{
    class Knot
    {
        public Knot LeftKnot { get; set; }
        public Knot RightKnot { get; set; }
        public Knot Parent { get; set; }
        public string Name { get; set; }
        public bool Used { get; set; }
        public Knot(string name, Knot parent)
        {
            Used = false;
            Name = name;
            Parent = parent;
            LeftKnot = null;
            RightKnot = null;
        }

        public bool IsOperand()
        {
            if (Name == "+" || Name == "!" || Name == "*") return true;
            return false;
        } 
    }
}
