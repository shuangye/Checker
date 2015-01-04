using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Threading.LinkUp
{
    public class LinkUpModel
    {
        public int dimensionX { get { return 14; } }
        public int dimensionY { get { return 10; } }
        public int maxTileCount { get { return dimensionX * dimensionY; } }
        public LinkUpTile[] Tiles { get; set; }

        public LinkUpModel()
        {
            Tiles = new LinkUpTile[dimensionX * dimensionY];
        }

        LinkUpTile TileAtIndex(int index)
        {
            LinkUpTile result = null;
            if (index >= 0 && index < maxTileCount)
            {
                result = Tiles[index];
            }
            return result;
        }

        LinkUpTile TileAtCoordinate(int x, int y)
        {
            return TileAtIndex(y * dimensionX + x);
        }

        public bool CanClear(LinkUpTile tileA, LinkUpTile tileB)
        {
            if (null == tileA || null == tileB || tileA.Index == tileB.Index || tileA.Value != tileB.Value)
                return false;

            foreach (var item in DirectTiles(tileA).Where(x => x.Visible == true))
            {
                if (item.Index == tileB.Index)
                    return true;
            }

            foreach (var item in OneTurnTiles(tileA).Where(x => x.Visible == true))
            {
                if (item.Index == tileB.Index)
                    return true;
            }

            foreach (var item in TwoTurnTiles(tileA).Where(x => x.Visible == true))
            {
                if (item.Index == tileB.Index)
                    return true;
            }

            return false;
        }

        // blank and non-blank
        List<LinkUpTile> DirectTiles(LinkUpTile targetTile)
        {
            List<LinkUpTile> result = new List<LinkUpTile>();
            result.AddRange(Horizontal(targetTile, -1));  // left
            result.AddRange(Horizontal(targetTile, 1));  // right
            result.AddRange(Vertical(targetTile, -1));  // up
            result.AddRange(Vertical(targetTile, 1));  // bottom
            return result;
        }

        List<LinkUpTile> OneTurnTiles(LinkUpTile targetTile)
        {
            List<LinkUpTile> result = new List<LinkUpTile>();

            foreach (var item in DirectTiles(targetTile).Where(x => x.Visible == false))
            {
                result.AddRange(DirectTiles(item));
            }

            return result;
        }

        List<LinkUpTile> TwoTurnTiles(LinkUpTile targetTile)
        {
            List<LinkUpTile> result = new List<LinkUpTile>();

            foreach (var item in OneTurnTiles(targetTile).Where(x => x.Visible == false))
            {
                result.AddRange(DirectTiles(item));
            }

            return result;
        }


        // refactor to one method via index, instead of coordinate

        private List<LinkUpTile> Horizontal(LinkUpTile targetTile, int delta)
        {
            List<LinkUpTile> result = new List<LinkUpTile>();
            for (int i = targetTile.CoordinateX + delta; i >= 0 && i < dimensionX; i += delta)
            {
                LinkUpTile tile = TileAtCoordinate(i, targetTile.CoordinateY);
                if (null != tile)
                {
                    tile.PathLength = Math.Abs(i - targetTile.CoordinateX);
                    result.Add(tile);
                    if (tile.Visible)
                        break;  // find the first non-blank tile
                }
            }
            return result;
        }

        private List<LinkUpTile> Vertical(LinkUpTile targetTile, int delta)
        {
            List<LinkUpTile> result = new List<LinkUpTile>();
            for (int i = targetTile.CoordinateY + delta; i >= 0 && i < dimensionY; i += delta)
            {
                LinkUpTile tile = TileAtCoordinate(targetTile.CoordinateX, i);
                if (null != tile)
                {
                    tile.PathLength = Math.Abs(i - targetTile.CoordinateY);
                    result.Add(tile);
                    if (tile.Visible)
                        break;  // find only the first tile
                }
            }
            return result;
        }

    }
}
