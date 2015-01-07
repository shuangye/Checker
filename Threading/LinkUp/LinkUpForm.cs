using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Threading.Helpers;
using Threading.LinkUp.View;

namespace Threading.LinkUp
{
    public partial class LinkUpForm : Form
    {
        public LinkUpModel<ButtonTile> Model { get; set; }
        public ButtonTile PrevTile { get; set; }
        public ButtonTile CurrentTile { get; set; }
        public Pen Pen { get { return new Pen(Color.Green, 5); } }
        public Graphics FormGraphics { get { return this.CreateGraphics(); } }
        public int Score { get; set; }

        public LinkUpForm()
        {
            InitializeComponent();

            this.Load += (sender, e) =>
            {
                try
                {
                    Model = new LinkUpModel<ButtonTile>(14, 8, 28);
                    Model.SetupInitTiles();
                }
                catch (ArgumentException)
                {
                    MessageBox.Show("Invalid dimension.", "Link Up", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
                catch (NotImplementedException ex)
                {
                    MessageBox.Show(ex.Message, "Link Up", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }

#if DEBUG
                String layoutSummary = String.Empty;                
                foreach (var item in Model.Tiles)                
                    layoutSummary += String.Format("{0}: {1}\t\t\t", item.Value.ToString("D2"), Model.Tiles.Count(x => x.Value == item.Value).ToString("D2"));
                MessageBox.Show(Environment.NewLine + Model.Tiles.Count().ToString() + Model.Tiles.Count(x => x.Value != -1).ToString() + Environment.NewLine + layoutSummary,
                    "Link Up", MessageBoxButtons.OK);
#endif

                foreach (var tile in Model.Tiles)
                {
                    tile.ParentWindow = this;
                    this.Controls.Add(tile);
                }

                Score = 0;
            };
        }


        #region Event Handling

        public void tileClicked(ButtonTile sender, EventArgs e)
        {
            List<int> path;
            PrevTile = CurrentTile;
            CurrentTile = sender;
            this.lblStatus.Text = String.Format("Prev: {0}, Current: {1}", null == PrevTile ? "null" : PrevTile.Index.ToString(), CurrentTile.Index);                        

            if (Model.CanClear(PrevTile, CurrentTile, out path))
            {
                if (null != Pen && null != FormGraphics)
                {
                    int offset = ButtonTile.Side / 2;
                    switch (path.Count)
                    {
                        case 0:
                            FormGraphics.DrawLine(Pen, PrevTile.Location.X + offset, PrevTile.Location.Y + offset,
                                CurrentTile.Location.X + offset, CurrentTile.Location.Y + offset);
                            break;
                        case 1:
                            ButtonTile turn = Model.TileAtIndex(path[0]);
                            FormGraphics.DrawLine(Pen, PrevTile.Location.X + offset, PrevTile.Location.Y + offset,
                                turn.Location.X + offset, turn.Location.Y + offset);
                            FormGraphics.DrawLine(Pen, turn.Location.X + offset, turn.Location.Y + offset,
                                CurrentTile.Location.X + offset, CurrentTile.Location.Y + offset);
                            break;
                        case 2:
                            ButtonTile turn1 = Model.TileAtIndex(path[0]);
                            ButtonTile turn2 = Model.TileAtIndex(path[1]);
                            FormGraphics.DrawLine(Pen, PrevTile.Location.X + offset, PrevTile.Location.Y + offset, turn1.Location.X + offset, turn1.Location.Y + offset);
                            FormGraphics.DrawLine(Pen, turn1.Location.X + offset, turn1.Location.Y + offset, turn2.Location.X + offset, turn2.Location.Y + offset);
                            FormGraphics.DrawLine(Pen, turn2.Location.X + offset, turn2.Location.Y + offset, CurrentTile.Location.X + offset, CurrentTile.Location.Y + offset);
                            break;
                        default:
                            break;
                    }
                    Thread.Sleep(300);
                    FormGraphics.Clear(this.BackColor);
                }
                
                PrevTile.Visible = false;
                CurrentTile.Visible = false;
                // this.Controls.Remove(PrevTile);
                // this.Controls.Remove(CurrentTile);

                PrevTile = null;
                CurrentTile = null;
                ++Score;
                this.lblScore.Text = String.Format("Score: {0}", Score);
            }
        }
                
        private void btnHint_Click(object sender, EventArgs e)
        {
            var tiles = Model.Tiles.Where(x => x.Visible == true);
            foreach (var tileA in tiles)
                foreach (var tileB in tiles.Where(x => x.Index != tileA.Index))
                    if (Model.CanClear(tileA, tileB))
                    {
                        // MessageBox.Show(String.Format("{0} - {1}", tileA.Index, tileB.Index));
                        tileA.Visible = tileB.Visible = false;                        
                        Thread.Sleep(300);
                        tileA.Visible = tileB.Visible = true;
                        return;
                    }
        }

        private void btnAutoPlay_Click(object sender, EventArgs e)
        {
            while (Model.Tiles.Where(x => x.Visible == true).Count() >= 2)
            {
                var tiles = Model.Tiles.Where(x => x.Visible == true);
                foreach (var tileA in tiles)
                    foreach (var tileB in tiles.Where(x => x.Index != tileA.Index))
                    {
                        tileA.PerformClick();
                        tileB.PerformClick();
                    }
            }
        }

        #endregion Event Handling


        #region Obsolete Methods

        [Obsolete]
        private void setupTiles()
        {
            Score = 0;            
            int textureKind = Model.TextureKind;
            int[] textureMap = new int[textureKind];  // textures for one part 这种方式中图案只在该部分随机，而不是全局随机

            Random random = new Random(Environment.TickCount);
            for (int i = 0; i < textureKind; ++i)
            {
                textureMap[i] = random.Next(0, textureKind);
            }

            for (int index = 0, texture = 0; index < Model.MaxTotalTileCount; ++index)
            {
                int x = index % Model.TotalDimensionX;
                int y = index / Model.TotalDimensionX;
                int value;

                if (texture > 0 && texture % textureKind == 0)
                    // textureMap = textureMap.OrderBy(_ => random.Next(0, textureKind)).ToArray();
                    textureMap = textureMap.Shuffle<int>();

                if (isFringeTile(x, y))
                    value = -1;
                else
                {
                    value = textureMap[texture % textureKind];
                    ++texture;
                }

                ButtonTile tile = new ButtonTile(index, x, y, value);
                tile.ParentWindow = this;
                Model.Tiles[index] = tile;
                this.Controls.Add(tile);
            }

#if DEBUG
            String counter = String.Empty;
            foreach (var item in textureMap)
            {
                counter += String.Format("{0}: {1}" + Environment.NewLine, item, Model.Tiles.Count(x => x.Value == item));
            }
            MessageBox.Show(Model.Tiles.Count().ToString() + Model.Tiles.Count(x => x.Value != -1).ToString() + Environment.NewLine + counter);
#endif

            //    for (int pass = 0; pass < partCount; ++pass)
            //    {
            //        textureMaps = textureMaps.OrderBy(x => random.Next()).ToArray();
            //        int texture = 0;
            //        for (int i = 0; i < part; ++i)
            //        {
            //            int index = pass * part + i;
            //            int x = index % Model.dimensionX;
            //            int y = index / Model.dimensionX;
            //            int value;
            //            if (isFringeTile(x, y))
            //                value = fringeTileValue;
            //            else
            //            {
            //                value = textureMaps[texture];
            //                ++texture;
            //            }
            //
            //            LinkUpTile tile = new LinkUpTile(index, x, y, value);
            //            tile.ParentWindow = this;
            //            Model.Tiles[index] = tile;
            //            this.Controls.Add(tile);
            //        }
            //    }
        }

        [Obsolete]
        private bool isFringeTile(int x, int y)
        {
            return x * y == 0 || x + 1 == Model.TotalDimensionX || y + 1 == Model.TotalDimensionY;
        }

        #endregion Obsolete Methods
    }
}
