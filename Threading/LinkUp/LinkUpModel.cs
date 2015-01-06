using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Threading.LinkUp.View;

namespace Threading.LinkUp
{
    public class LinkUpModel<T> where T : class, ITile
    {
        #region Properties

        public static int EdgeTileValue { get { return -1; } }
        public int DimensionX { get; private set; }
        public int DimensionY { get; private set; }
        public int Pieces { get; private set; }        
        public int MaxTotalTileCount { get { return DimensionX * DimensionY; } }
        public int MaxVisibleTileCount { get { return (DimensionX - 2) * (DimensionY - 2); } }  // the outer tiles are invisible        
        public int TextureKind { get { return MaxVisibleTileCount / Pieces; } }
        public static char[] TextureMap
        {
            get
            {
                return new char[] { '๑', '◎', '○', '◇', '☆', '♤', '♧', '☺', '♥', '☀',
                '¤', '⊙', '♀', '♂', '♪', '♫', '∮', '◑', '☢', '☪',
                '☃', '☂', '❁', '❀', '♨', '㊣', '☎', '☏' };
            }
        }
        public T[] Tiles { get; set; }

        #endregion Properties


        /// <summary>
        /// Construct a model.
        /// Every texture shall appear even times.
        /// </summary>
        /// <param name="dimensionX"></param>
        /// <param name="dimensionY"></param>
        /// <param name="pieces"></param>
        public LinkUpModel(int dimensionX, int dimensionY, int pieces)
        {
            if (dimensionX <= 0 || dimensionY <= 0 || pieces <= 0 || dimensionX * dimensionY % 2 != 0)
                throw new ArgumentException();

            DimensionX = dimensionX;
            DimensionY = dimensionY;
            Pieces = pieces;

            if (MaxVisibleTileCount % pieces != 0)
                throw new ArgumentException();

            Tiles = new T[DimensionX * DimensionY];
        }

        /// <summary>
        /// Setup the initial state
        /// </summary>
        public void SetupInitTiles()
        {
            int[] textureMap = new int[TextureKind];  // textures for one part 这种方式中图案只在该部分随机，而不是全局随机
            Random random = new Random(Environment.TickCount);
            for (int i = 0; i < TextureKind; ++i)
            {
                textureMap[i] = random.Next(0, TextureKind);
            }

            for (int index = 0, texture = 0; index < MaxTotalTileCount; ++index)
            {
                int x = index % DimensionX;
                int y = index / DimensionX;
                int value;

                // randomize the texture map just before starting another piece
                if (texture > 0 && texture % TextureKind == 0)
                    textureMap = textureMap.OrderBy(_ => random.Next(0, TextureKind)).ToArray();
                    // textureMap = textureMap.Shuffle<int>();

                if (isEdgeTile(x, y))
                    value = EdgeTileValue;
                else
                {
                    value = textureMap[texture % TextureKind];
                    ++texture;
                }

                // create an instance of generic type                
                Tiles[index] = (T)Activator.CreateInstance(typeof(T), new object[] { index, x, y, value });
            }
        }

        public T TileAtIndex(int index)
        {
            T result = null;
            if (index >= 0 && index < MaxTotalTileCount)
            {
                result = Tiles[index];
            }
            return result;
        }

        public T TileAtCoordinate(int x, int y)
        {
            return TileAtIndex(y * DimensionX + x);
        }


        #region Logics

        /// <summary>
        /// Determine if tileA and tileB can be cleared
        /// </summary>
        /// <param name="tileA"></param>
        /// <param name="tileB"></param>
        /// <returns></returns>
        public bool CanClear(T tileA, T tileB)
        {
            if (null == tileA || null == tileB || tileA.Index == tileB.Index || tileA.Value != tileB.Value)
                return false;

            return DirectTiles(tileA).Where(x => x.Index == tileB.Index && x.Visible == true).Count() > 0 ||
                OneTurnTiles(tileA).Where(x => x.Index == tileB.Index && x.Visible == true).Count() > 0 ||
                TwoTurnTiles(tileA).Where(x => x.Index == tileB.Index && x.Visible == true).Count() > 0;
        }

        public bool CanClear(T tileA, T tileB, out List<int> path)
        {
            path = new List<int>();  // path from tileA to tileB (contains turns only, tileA and tileB not included)

            if (null == tileA || null == tileB || tileA.Index == tileB.Index || tileA.Value != tileB.Value)
                return false;

            // tileA and tileB can be directly connected
            if (DirectTiles(tileA).Where(x => x.Index == tileB.Index && x.Visible == true).Count() > 0)
                return true;

            // can be connected via one turn
            foreach (var tile in DirectTiles(tileA).Where(x => x.Visible == false))
            {
                if (DirectTiles(tile).Where(x => x.Index == tileB.Index && x.Visible == true).Count() > 0)
                {
                    path.Add(tile.Index);
                    return true;
                }
            }

            // can be connected via two turns
            foreach (var tile1 in DirectTiles(tileA).Where(x => x.Visible == false))
            {
                foreach (var tile2 in DirectTiles(tile1).Where(x => x.Visible == false))
                    if (DirectTiles(tile2).Where(x => x.Index == tileB.Index && x.Visible == true).Count() > 0)
                    {                    
                        path.Add(tile1.Index);
                        path.Add(tile2.Index);
                        return true;
                    }
            }                        

            return false;
        }

        /// <summary>
        /// Find tiles that can be directly connected to a tile on 4 directions,
        /// including blank tiles (invisible) and non-blank tiles (visible)
        /// </summary>
        /// <param name="targetTile"></param>
        /// <returns></returns>
        List<T> DirectTiles(T targetTile)
        {
            List<T> result = new List<T>();
            if (null != targetTile)
            {
                // result.AddRange(Horizontal(targetTile, -1));  // left
                // result.AddRange(Horizontal(targetTile, 1));  // right
                // result.AddRange(Vertical(targetTile, -1));  // up
                // result.AddRange(Vertical(targetTile, 1));  // bottom
                result.AddRange(DirectTilesOnOneDirection(targetTile, -1, 0)); // left
                result.AddRange(DirectTilesOnOneDirection(targetTile, 1, 0)); // right
                result.AddRange(DirectTilesOnOneDirection(targetTile, 0, -1)); // up
                result.AddRange(DirectTilesOnOneDirection(targetTile, 0, 1)); // bottom
            }
            return result;
        }

        List<T> OneTurnTiles(T targetTile)
        {
            List<T> result = new List<T>();
            foreach (var item in DirectTiles(targetTile).Where(x => x.Visible == false))            
                result.AddRange(DirectTiles(item));            
            return result;
        }

        List<T> TwoTurnTiles(T targetTile)
        {
            List<T> result = new List<T>();
            foreach (var item in OneTurnTiles(targetTile).Where(x => x.Visible == false))            
                result.AddRange(DirectTiles(item));
            return result;
        }


        /// <summary>
        /// Find tiles that can be directly connected to target tile on one direction,
        /// including blank tiles (invisible) and non-blank tiles (visible)
        /// </summary>
        /// <param name="targetTile"></param>
        /// <param name="deltaX"></param>
        /// <param name="deltaY"></param>
        /// <returns></returns>
        private List<T> DirectTilesOnOneDirection(T targetTile, int deltaX, int deltaY)
        {
            List<T> result = new List<T>();
            for (int x = targetTile.CoordinateX + deltaX, y = targetTile.CoordinateY + deltaY;
                x >= 0 && x < DimensionX && y >= 0 && y < DimensionY;
                x += deltaX, y += deltaY)
            {
                T tile = TileAtCoordinate(x, y);
                if (null != tile)
                {
                    tile.PathLength = Math.Abs(x - targetTile.CoordinateX) + Math.Abs(y - targetTile.CoordinateY);
                    result.Add(tile);
                    if (tile.Visible)
                        break;  // reached the first non-blank tile
                }
            }
            return result;
        }

        private bool isEdgeTile(int x, int y)
        {
            return x * y == 0 || x + 1 == DimensionX || y + 1 == DimensionY;
        }

        #endregion Logics


        #region obsolete methods

        [Obsolete]
        private List<T> Horizontal(T targetTile, int delta)
        {
            List<T> result = new List<T>();
            for (int i = targetTile.CoordinateX + delta; i >= 0 && i < DimensionX; i += delta)
            {
                T tile = TileAtCoordinate(i, targetTile.CoordinateY);
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

        [Obsolete]
        private List<T> Vertical(T targetTile, int delta)
        {
            List<T> result = new List<T>();
            for (int i = targetTile.CoordinateY + delta; i >= 0 && i < DimensionY; i += delta)
            {
                T tile = TileAtCoordinate(targetTile.CoordinateX, i);
                if (null != tile)
                {
                    tile.PathLength = Math.Abs(i - targetTile.CoordinateY);
                    result.Add(tile);
                    if (tile.Visible)
                        break;  // reached only the first tile
                }
            }
            return result;
        }

        #endregion obsolete methods
    }
}
