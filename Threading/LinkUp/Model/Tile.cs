using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Threading.LinkUp.Model
{
    // public interface ITile
    /// <summary>
    /// Tile should be a model entity. It should not define any UI related things.
    /// So change it from an interface, which defines the common interface of UI tiles,
    /// to a class, which defines a model entity.
    /// </summary>
    public class Tile
    {
        public int Index { get; set; }  // zero-based
        public int Value { get; set; }
        public int CoordinateX { get; set; }  // zero-based
        public int CoordinateY { get; set; }  // zero-based        
        public int PathLength { get; set; }  // // path length to a certain tile
        public bool Visible { get; set; }

        public Tile(int index, int coordinateX, int coordinateY, int value)
        {
            Index = index;
            CoordinateX = coordinateX;
            CoordinateY = coordinateY;
            Value = value;
            Visible = Value != CommonDef.FringeTileValue;
            PathLength = int.MaxValue;
        }
    }
}
