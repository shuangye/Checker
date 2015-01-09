using System.Collections;
using System.Collections.Generic;
using Threading.LinkUp.View;

namespace Threading.Helpers
{
    public class TileEqualityComparer : IEqualityComparer<ITile>
    {
        public bool Equals(ITile tileA, ITile tileB)
        {
            return tileA.Value == tileB.Value;            
        }

        public int GetHashCode(ITile tile)
        {
            return tile.Index.GetHashCode();
        }
    }
}
