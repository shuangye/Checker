// 使用预处理宏可避免在程序中作过多的逻辑判断、从而提高性能，但失去了运行时的灵活性
#undef SAME_VALUE_CLEAR  // clear if the two tiles have the same value

/// 怎样根据两个格子的 Value 判断它们能否消除？
/// 两种玩法：
/// A) 两个格子的 Value 相同时可消除。适于两个格子的图案相同时消除的玩法
/// B) 两个格子的 Value 满足一定规律时可消除（如相加为0）。适于两个格子的图案不同时消除的玩法，如猫格子吃掉老鼠格子，然后二者消除。
/// 本程序中，A 玩法中所有格子的 Value 均为正值；B 玩法中，捕食者的 Value 为正值，猎物的 Value 为负值。


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Threading.LinkUp.View;
using Threading.Helpers;

namespace Threading.LinkUp.Model
{
    public class LinkUpModel
    {
        #region Properties
                
        public static bool SameValueClear { get; private set; } 
        public int VisibleDimensionX { get; private set; }
        public int VisibleDimensionY { get; private set; }
        public int TotalDimensionX { get { return VisibleDimensionX + 2; } }
        public int TotalDimensionY { get { return VisibleDimensionY + 2; } }
        public int MaxTotalTileCount { get { return TotalDimensionX * TotalDimensionY; } }
        public int MaxVisibleTileCount { get { return VisibleDimensionX * VisibleDimensionY; } }  // the outer tiles are invisible                
        public int TextureKind { get; private set; }
        public Tile[] Tiles { get; set; }

        #endregion Properties


        /// <summary>
        /// Construct a model.
        /// Every texture shall appear even times.
        /// </summary>
        /// <param name="visibleDimensionX">The visible dimension X</param>
        /// <param name="visibleDimensionY">The visible dimension Y</param>
        /// <param name="textureKind">How many kind of textures</param>
        public LinkUpModel(int visibleDimensionX, int visibleDimensionY, int textureKind, bool sameValueClear)
        {
            VisibleDimensionX = visibleDimensionX;
            VisibleDimensionY = visibleDimensionY;
            TextureKind = textureKind;
            SameValueClear = sameValueClear;

            if (VisibleDimensionX < 2 || VisibleDimensionY < 2 ||              // at least 4 tiles
                TextureKind <= 0 || TextureKind > MaxVisibleTileCount / 2 ||   // valid range [1, MaxVisibleTileCount / 2]
                MaxVisibleTileCount % 2 != 0)                                  // ensure every texture appearing even times
                throw new ArgumentException();

            // 求 MaxVisibleTileCount 的所有质数因子

            if (SameValueClear ? CommonDef.Textures.Length < TextureKind : CommonDef.PredatorTextures.Length < TextureKind || CommonDef.PreyTextures.Length < TextureKind)
                throw new NotImplementedException("Currently these are only 6 pairs of textures. Please select lower levels.");

            Tiles = new Tile[TotalDimensionX * TotalDimensionY];
        }

        /// <summary>
        /// Setup the initial state
        /// </summary>
        public void SetupInitTiles()
        {
            int middleVisible = MaxVisibleTileCount / 2;             // split visible tiles to 2 parts
            int[] halfTextureValues = new int[middleVisible];        // for visible tiles
            Random random = new Random(Environment.TickCount);
            for (int i = 0; i < middleVisible; ++i)
                halfTextureValues[i] = random.Next(1, TextureKind + 1);  // [1, TextureKind]            

#if SAME_VALUE_CLEAR

            for (int index = 0, textureIndex = 0; index < MaxTotalTileCount; ++index)
            {
                int x = index % TotalDimensionX;
                int y = index / TotalDimensionX;
                
                // randomize just before starting the 2nd part
                if (textureIndex == middleVisible)
                    // visibleTextureValues = visibleTextureValues.OrderBy(_ => random.Next(0, TextureKind)).ToArray();
                    halfTextureValues = halfTextureValues.Shuffle<int>();

                // create an instance of generic type                
                Tiles[index] = (T)Activator.CreateInstance(typeof(T),
                    new object[] { index, x, y, isFringeTile(x, y)? FringeTileValue : halfTextureValues[textureIndex++ % middleVisible] });
            }

#else

            // var temp = (int[]) halfTextureValues.Clone();
            // var wholeTextureValues = temp.Concat(halfTextureValues.Negative()).ToArray().Shuffle();
            var wholeTextureValues = new int[MaxVisibleTileCount];
            halfTextureValues.CopyTo(wholeTextureValues, 0);
            if (!SameValueClear)
                halfTextureValues = halfTextureValues.Negative();
            halfTextureValues.CopyTo(wholeTextureValues, middleVisible);
            wholeTextureValues.Shuffle();
            for (int index = 0, textureIndex = 0; index < MaxTotalTileCount; ++index)
            {
                int x = index % TotalDimensionX;
                int y = index / TotalDimensionX;
#if false
                // don't try to create UI elements in model.
                Tiles[index] = (T)Activator.CreateInstance(typeof(T),
                    new object[] { index, x, y, isFringeTile(x, y) ? CommonDef.FringeTileValue : wholeTextureValues[textureIndex++] });
                // textureIndex won't be out of range because of shortcut logical evaluation
#endif
                // create a model entity instead:
                Tiles[index] = new Tile(index, x, y, isFringeTile(x, y) ? CommonDef.FringeTileValue : wholeTextureValues[textureIndex++]);
            }

#endif // SAME_VALUE_CLEAR
        }
        
        public Tile TileAtIndex(int index)
        {
            // Tile result = null;
            // if (index >= 0 && index < MaxTotalTileCount)
            //     result = Tiles[index];
            // return result;

            return Tiles.Where(x => x.Index == index).FirstOrDefault();
        }

        public Tile TileAtCoordinate(int x, int y)
        {
            return TileAtIndex(y * TotalDimensionX + x);
        }


        #region Logics

        public void Refresh()
        {
            Random random = new Random(Environment.TickCount);
            var visibleTiles = Tiles.Where(x => x.Visible).ToArray();
            for (int i = visibleTiles.Length - 1; i > 0; --i)
            {
                int j = random.Next() % (i + 1);
                // change value only
                var value = visibleTiles[i].Value;
                visibleTiles[i].Value = visibleTiles[j].Value;
                visibleTiles[j].Value = value;
            }
        }

        public List<Tile> FindClearableTiles(out List<int> turns)
        {
            turns = null;
            var tiles = Tiles.Where(x => x.Visible);
            foreach (var tileA in tiles)
                foreach (var tileB in tiles)
                    if (CanClear(tileA, tileB, out turns))
                        return new List<Tile> { tileA, tileB };
            return null;
        }

        /// <summary>
        /// Determine if tileA and tileB can be cleared
        /// </summary>
        /// <param name="tileA"></param>
        /// <param name="tileB"></param>
        /// <returns></returns>
        public bool CanClear(Tile tileA, Tile tileB)
        {
            if (null == tileA || null == tileB ||
                !tileA.Visible || !tileB.Visible ||              // blank tiles
                tileA.Index == tileB.Index ||                    // the same tile
                !isValueEligible(tileA.Value, tileB.Value))      // texture not match 
                return false;

            return DirectTiles(tileA).Where(x => x.Index == tileB.Index && x.Visible).Count() > 0 ||
                OneTurnTiles(tileA).Where(x => x.Index == tileB.Index && x.Visible).Count() > 0 ||
                TwoTurnTiles(tileA).Where(x => x.Index == tileB.Index && x.Visible).Count() > 0;
        }

        public bool CanClear(Tile tileA, Tile tileB, out List<int> turns)
        {
            turns = new List<int>();  // path from tileA to tileB (contains turns only, tileA and tileB not included)

            if (null == tileA || null == tileB ||
                !tileA.Visible || !tileB.Visible ||              // blank tiles
                tileA.Index == tileB.Index ||                    // the same tile
                !isValueEligible(tileA.Value, tileB.Value))      // texture not match              
                return false;

            // tileA and tileB can be directly connected
            if (DirectTiles(tileA).Where(x => x.Index == tileB.Index && x.Visible == true).Count() > 0)
                return true;

            // can be connected via one turn
            foreach (var tile in DirectTiles(tileA).Where(x => x.Visible == false))
            {
                if (DirectTiles(tile).Where(x => x.Index == tileB.Index && x.Visible == true).Count() > 0)
                {
                    turns.Add(tile.Index);
                    return true;
                }
            }

            // can be connected via two turns
            foreach (var tile1 in DirectTiles(tileA).Where(x => x.Visible == false))
            {
                foreach (var tile2 in DirectTiles(tile1).Where(x => x.Visible == false))
                    if (DirectTiles(tile2).Where(x => x.Index == tileB.Index && x.Visible == true).Count() > 0)
                    {
                        turns.Add(tile1.Index);
                        turns.Add(tile2.Index);
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
        List<Tile> DirectTiles(Tile targetTile)
        {
            List<Tile> result = new List<Tile>();
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

        List<Tile> OneTurnTiles(Tile targetTile)
        {
            List<Tile> result = new List<Tile>();
            foreach (var item in DirectTiles(targetTile).Where(x => !x.Visible))
                result.AddRange(DirectTiles(item));
            return result;
        }

        List<Tile> TwoTurnTiles(Tile targetTile)
        {
            List<Tile> result = new List<Tile>();
            foreach (var item in OneTurnTiles(targetTile).Where(x => !x.Visible))
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
        private List<Tile> DirectTilesOnOneDirection(Tile targetTile, int deltaX, int deltaY)
        {
            List<Tile> result = new List<Tile>();
            for (int x = targetTile.CoordinateX + deltaX, y = targetTile.CoordinateY + deltaY;
                x >= 0 && x < TotalDimensionX && y >= 0 && y < TotalDimensionY;
                x += deltaX, y += deltaY)
            {
                Tile tile = TileAtCoordinate(x, y);
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

        private bool isFringeTile(int x, int y)
        {
            return x * y == 0 || x + 1 == TotalDimensionX || y + 1 == TotalDimensionY;
        }

        private bool isValueEligible(int tileAValue, int tileBValue)
        {
            if (SameValueClear)
                return tileAValue == tileBValue;
            else
                return tileAValue + tileBValue == 0;
        }

        #endregion Logics


        #region Obsolete Methods

        [Obsolete]
        private List<Tile> Horizontal(Tile targetTile, int delta)
        {
            List<Tile> result = new List<Tile>();
            for (int i = targetTile.CoordinateX + delta; i >= 0 && i < TotalDimensionX; i += delta)
            {
                Tile tile = TileAtCoordinate(i, targetTile.CoordinateY);
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
        private List<Tile> Vertical(Tile targetTile, int delta)
        {
            List<Tile> result = new List<Tile>();
            for (int i = targetTile.CoordinateY + delta; i >= 0 && i < TotalDimensionY; i += delta)
            {
                Tile tile = TileAtCoordinate(targetTile.CoordinateX, i);
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

        #endregion Obsolete Methods
    }
}
