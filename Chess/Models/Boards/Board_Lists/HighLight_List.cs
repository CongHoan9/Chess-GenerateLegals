using System;
using System.Collections.Generic;
using System.Text;

namespace Chess
{
    public class HighLight_List(Func<Position, bool> isinside) : Reusable_List<Arrow> 
    {
        public Func<Position, bool> IsInSide => isinside;
    }
}
