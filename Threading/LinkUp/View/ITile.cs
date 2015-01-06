using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Threading.LinkUp.View
{
    public interface ITile
    {
        int Index { get; set; }  // zero-based
        int Value { get; set; }
        int CoordinateX { get; set; }  // zero-based
        int CoordinateY { get; set; }  // zero-based        
        int PathLength { get; set; }  // // path length to a certain tile
        bool Visible { get; set; }
    }
}
