using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Windows.Forms;
using Threading.Helpers;
using Threading.LinkUp.Model;
using Threading.LinkUp.View;

namespace Threading.LinkUp
{
    public partial class LinkUpForm : Form
    {
        public int VisibleDimensionX { get { return 14; } }
        public int VisibleDimensionY { get { return 8; } }
        public int TotalDimensionX { get { return VisibleDimensionX + 2; } }
        public int TotalDimensionY { get { return VisibleDimensionY + 2; } }
        public int Difficulty { get { return trackBarDifficulty.Value; } }
        public int TilePadding { get { return 10; } }
        public ButtonTile[] Tiles { get; set; }
        private Size UsableAreaSize
        {
            get
            {
                return new Size(this.ClientSize.Width - TilePadding * 2,
                    this.ClientSize.Height - TilePadding * 2 - this.menuStrip1.Height - this.trackBarDifficulty.Height);
            }
        }
        public int TileSide { get { return Math.Min(UsableAreaSize.Width / (VisibleDimensionX + 1) - TilePadding, UsableAreaSize.Height / (VisibleDimensionY + 1) - TilePadding); } }
        public Point TileStartPoint { get { return new Point(-TileSide / 2, -TileSide / 2); } }  // relative to the parent container.        
        public GameMode GameMode { get; set; }
        public LinkUpModel Model { get; set; }
        public ButtonTile PrevTile { get; set; }
        public ButtonTile CurrentTile { get; set; }
        public Pen Pen { get { return new Pen(Color.Green, 3); } }
        public Graphics ContextGraphics { get { return tilesPanel.CreateGraphics(); } }        
        public int Score { get; set; }

        public LinkUpForm()
        {
            InitializeComponent();

            this.Load += (_sender, _e) =>
            {
#if !DEBUG
                lblDebug.Visible = false;
                lblStatus.Visible = false;
                this.tilesPanel.BorderStyle = BorderStyle.None;
#endif
                trackBarDifficulty.Maximum = VisibleDimensionX * VisibleDimensionY / 2;
                trackBarDifficulty.Value = trackBarDifficulty.Maximum / 2;

                this.SizeChanged += (sender, e) => { ArrangeTilesSize(); };
                this.trackBarDifficulty.MouseCaptureChanged += (sender, e) => { StartOver(); };
                restartToolStripMenuItem.Click += (sender, e) => { StartOver(); };

                GameMode = GameMode.Classical;  // default
                StartOver();
            };
        }


        #region Event Handling

        public void tileClicked(ButtonTile sender, EventArgs e)
        {
            List<int> turns;
            PrevTile = CurrentTile;
            CurrentTile = sender;
            this.lblStatus.Text = String.Format("State: Prev {0}, Current {1}", null == PrevTile ? "none" : PrevTile.Index.ToString(), CurrentTile.Index);

            if (null != PrevTile && null != CurrentTile)
            {
                if (Model.CanClear(Model.TileAtIndex(PrevTile.Index), Model.TileAtIndex(CurrentTile.Index), out turns))
                    ClearTwoTiles(PrevTile, CurrentTile, turns);
            }
        }

        private void btnHint_Click(object sender, EventArgs e)
        {
            List<int> turns;
            var tiles = Model.FindClearableTiles(out turns);
            if (null != tiles)
            {
                DrawCues(TileAtIndex(tiles[0].Index), TileAtIndex(tiles[1].Index), turns);                

                System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer { Interval = 500 };
                timer.Tick += (_sender, _e) =>
                {
                    if (null != ContextGraphics)
                        ContextGraphics.Clear(this.BackColor);
                    timer.Stop();
                };
                timer.Start();
            }
            else if (Model.Tiles.Where(x => x.Visible).Count() > 0)  // dead lock
            {
                if (System.Windows.Forms.DialogResult.Yes ==
                    MessageBox.Show("No tiles can be cleared. Do you want to refresh tiles?", "Dead Lock",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
                    btnRefresh.PerformClick();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            Model.Refresh();
            // foreach (ButtonTile tile in tilesPanel.Controls)
            // {
            //     if (tile is ButtonTile && tile.Visible)
            //         tile.Refresh();  // to update the Text property because it is computed from the Value property
            // }
            ArrangeTilesSize();
        }

#warning consider moving this function to a worker thread
        /// <summary>
        /// 这种实现方式，会使线程一直处于忙碌状态，UI 得不到更新。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [Obsolete]
        private void _btnAutoPlay_Click(object sender, EventArgs e)
        {
            List<int> turns;
            var tiles = Model.FindClearableTiles(out turns);
            while (null != tiles)
            {
                ClearTwoTiles(TileAtIndex(tiles[0].Index), TileAtIndex(tiles[1].Index), turns);
                tiles = Model.FindClearableTiles(out turns);
            }

            // dead lock
            if (Model.Tiles.Where(x => x.Visible).Count() > 0)
                if (System.Windows.Forms.DialogResult.Yes ==
                    MessageBox.Show("No tiles can be cleared. Do you want to refresh tiles?", "Dead Lock",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
                    btnRefresh.PerformClick();
        }

        /// <summary>
        /// 这种实现方式，使线程在 timer 的 Interval 期间得以休息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAutoPlay_Click(object sender, EventArgs e)
        {
            btnAutoPlay.BackColor = Color.Red;
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer { Interval = 1000 };
            timer.Tick += (_sender, _e) =>
            {
                List<int> turns;
                btnAutoPlay.Visible = !btnAutoPlay.Visible;
                var tiles = Model.FindClearableTiles(out turns);
                if (null != tiles)
                    ClearTwoTiles(TileAtIndex(tiles[0].Index), TileAtIndex(tiles[1].Index), turns);
                else  // all tiles are cleared, or dead lock
                {
                    timer.Stop();
                    timer.Dispose();
                    btnAutoPlay.BackColor = btnRefresh.BackColor;
                    btnAutoPlay.Visible = true;

                    if (Model.Tiles.Where(x => x.Visible).Count() > 0)  // dead lock
                        if (System.Windows.Forms.DialogResult.Yes ==
                            MessageBox.Show("No tiles can be cleared. Do you want to refresh tiles?", "Dead Lock",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
                            btnRefresh.PerformClick();
                }
            };
            timer.Start();
        }

        #endregion Event Handling


        #region Refactored Methods

        private ButtonTile TileAtIndex(int index)
        {
            return Tiles.Where(x => x.Index == index).FirstOrDefault();
        }

        private ButtonTile TileAtCoordinate(int x, int y)
        {
            return TileAtIndex(y * TotalDimensionX * x);
        }

#warning every init accompanies creating a larget data structure: Model.
        private void StartOver()
        {
            try
            {
                Model = new LinkUpModel(this.VisibleDimensionX, this.VisibleDimensionY, Difficulty, GameMode == GameMode.Classical);
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Invalid dimension.", "Link Up", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (NotImplementedException ex)
            {
                MessageBox.Show(ex.Message, "Link Up", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Model.SetupInitTiles();
            Tiles = new ButtonTile[TotalDimensionX * TotalDimensionY];  // memory leak :) GC
            tilesPanel.Controls.Clear();
            foreach (var modelTile in Model.Tiles)
            {
                // create UI elements basing on the model entity info
                ButtonTile uiTile = new ButtonTile(modelTile.Index, modelTile.CoordinateX, modelTile.CoordinateY, modelTile.Value, modelTile.Visible);
                uiTile.ParentWindow = this;
                Tiles[uiTile.Index] = uiTile;
                tilesPanel.Controls.Add(uiTile);
            }
            ArrangeTilesSize();

            Score = 0;
        }

        private void DrawCues(ButtonTile startTile, ButtonTile endTile, List<int> turns)
        {
            if (null == startTile || null == endTile || null == turns || null == Pen || null == ContextGraphics)
                return;

            List<ButtonTile> turnTiles = new List<ButtonTile>();
            foreach (var turn in turns)
                turnTiles.Add(TileAtIndex(turn));
            switch (turnTiles.Count)
            {
                case 0:
                    ContextGraphics.DrawLine(Pen, startTile.Center, endTile.Center);
                    break;
                case 1:
                    ContextGraphics.DrawLine(Pen, startTile.Center, turnTiles[0].Center);
                    ContextGraphics.DrawLine(Pen, turnTiles[0].Center, endTile.Center);
                    break;
                case 2:
                    ContextGraphics.DrawLine(Pen, startTile.Center, turnTiles[0].Center);
                    ContextGraphics.DrawLine(Pen, turnTiles[0].Center, turnTiles[1].Center);
                    ContextGraphics.DrawLine(Pen, turnTiles[1].Center, endTile.Center);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Premise: the two tiles has been verified can be cleared
        /// </summary>
        /// <param name="startTile"></param>
        /// <param name="endTile"></param>
        /// <param name="turns"></param>
        private void ClearTwoTiles(ButtonTile startTile, ButtonTile endTile, List<int> turns)
        {
            DrawCues(startTile, endTile, turns);

            // consider moving this timer to global to avoid creating a timer object every time.            
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer { Interval = 250 };
            timer.Tick += (sender, e) =>
            {
                if (null != ContextGraphics)
                    ContextGraphics.Clear(this.BackColor);

                startTile.Visible = false;
                endTile.Visible = false;
                // do NOT remove tiles from the Controls set, because they blank tiles
                // act as ways to non-blank tiles
                // this.Controls.Remove(PrevTile);
                // this.Controls.Remove(CurrentTile);

                // Update model
                Model.TileAtIndex(startTile.Index).Visible = startTile.Visible;
                Model.TileAtIndex(endTile.Index).Visible = endTile.Visible;

                startTile = null;
                endTile = null;

                timer.Stop();
            };
            timer.Start();

            this.lblScore.Text = String.Format("Score: {0}", Score += Difficulty);  // score with weight
#if DEBUG
            this.lblStatus.Text = String.Format("State: Prev {0}, Current {1}", null == PrevTile ?
                "none" : PrevTile.Index.ToString(), null == CurrentTile ? "none" : CurrentTile.Index.ToString());
            this.lblDebug.Text = String.Format("Debug Info: visible count {0}", Model.Tiles.Where(x => x.Visible).Count());
#endif
        }

        private void ArrangeTilesSize()
        {
            tilesPanel.Size = new Size((VisibleDimensionX + 1) * (TileSide + TilePadding) + TilePadding, (VisibleDimensionY + 1) * (TileSide + TilePadding) + TilePadding);
            tilesPanel.Location = new Point((UsableAreaSize.Width - tilesPanel.Size.Width) / 2 + TilePadding,
                (UsableAreaSize.Height - tilesPanel.Size.Height) / 2 + this.menuStrip1.Height + this.trackBarDifficulty.Height + TilePadding);
            tilesPanel.Anchor = AnchorStyles.None;

            foreach (ButtonTile tile in tilesPanel.Controls)
            {
                if (tile is ButtonTile)
                {
                    tile.Size = new Size(TileSide, TileSide);
                    tile.Location = new Point(TileStartPoint.X + tile.CoordinateX * (TileSide + TilePadding),
                        TileStartPoint.Y + tile.CoordinateY * (TileSide + TilePadding));
                    tile.Font = new Font("微软雅黑", TileSide / 2, GraphicsUnit.Pixel);
                    tile.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

                    // Model value may be updated, so retrieve this info from model
                    tile.Value = Model.TileAtIndex(tile.Index).Value;

                    if (!LinkUpModel.SameValueClear && tile.Visible)
                    {
                        // this.Image = (Image)CommonDef.PredatorPreyTextures.SingleOrDefault(item => item.Key == this.Value).Value;
#warning try caching mechanism to lower memory footprint
                        String fileName = (String)CommonDef.PredatorPreyTextures.SingleOrDefault(item => item.Key == tile.Value).Value;
                        try
                        {
                            tile.Image = new Bitmap(Image.FromFile(Path.Combine(@".\Resources", fileName)), tile.Size);
                        }
                        catch
                        {
                            tile.Text = fileName;
                        }
                    }

                    tile.Refresh();
                }
            }

            // The code above created many new objects and also incurred many garbages.
            // So force the garbage collector to collect garbages.
            GC.Collect();
        }

        #endregion Refactored Methods

        #region Menu Items

        private void statisticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<int> values = new List<int>();
            foreach (var item in Model.Tiles)
                if (!values.Contains(item.Value))
                    values.Add(item.Value);

            String layoutSummary = String.Empty;
            // foreach (var item in Model.Tiles.Distinct(new TileEqualityComparer()))
            foreach (var item in values.OrderBy(x => x))
                layoutSummary += String.Format("{0}: {1}\t\t", item.ToString("D2"), Model.Tiles.Count(x => x.Value == item).ToString("D2"));
            MessageBox.Show(String.Format("Total tiles: {0}, non-blank tiles: {1}.",
                Model.Tiles.Count(), Model.Tiles.Count(x => x.Value != CommonDef.FringeTileValue))
                + Environment.NewLine + layoutSummary,
                "Link Up", MessageBoxButtons.OK);
        }

        private void predatorModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            predatorModeToolStripMenuItem.Checked = true;
            classicalModeToolStripMenuItem.Checked = false;
            GameMode = GameMode.PredatorPrey;

            int availableCount = CommonDef.PredatorPreyTextures.Count / 2;
            if (trackBarDifficulty.Value > availableCount)
                trackBarDifficulty.Value = availableCount;

            StartOver();
        }

        private void classicalModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            classicalModeToolStripMenuItem.Checked = true;
            predatorModeToolStripMenuItem.Checked = false;
            GameMode = GameMode.Classical;
            StartOver();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new About()).Show();
        }

        #endregion Menu Items
    }
}
