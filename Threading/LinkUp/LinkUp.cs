using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Threading.LinkUp
{
    public partial class LinkUpForm : Form
    {        
        public LinkUpModel Model { get; set; }
        public LinkUpTile PrevTile { get; set; }
        public LinkUpTile CurrentTile { get; set; }
        public int Score { get; set; }

        public LinkUpForm()
        {
            Model = new LinkUpModel();
            InitializeComponent();

            this.Load += (sender, e) =>
            {
                setupTiles();
            };
        }


        private void setupTiles()
        {
            const int divides = 6;
            const int edgeTileValue = -1;
            Score = 0;
            int kinds = Model.maxTileCount / divides;
            int[] randValues = new int[kinds];
            Random random = new Random(Environment.TickCount);
            for (int i = 0; i < randValues.Length; ++i) 
            {
                randValues[i] = random.Next(0, kinds);
            }

            for (int pass = 0; pass < divides; ++pass)
            {
                randValues = randValues.OrderBy(x => random.Next()).ToArray();
                for (int i = 0; i < kinds; ++i)
                {
                    int index = pass * kinds + i;
                    int x = index % Model.dimensionX;
                    int y = index / Model.dimensionX;
                    LinkUpTile tile = new LinkUpTile(index, x, y, isEdgeTile(x, y) ? edgeTileValue : randValues[i]);
                    tile.ParentWindow = this;
                    Model.Tiles[index] = tile;
                    this.Controls.Add(tile);
                }
            }
        }

        private bool isEdgeTile(int x, int y)
        {
            return x * y == 0 || x + 1 == Model.dimensionX || y + 1 == Model.dimensionY;
        }

        public void tileClicked(LinkUpTile sender, EventArgs e)
        {            
            PrevTile = CurrentTile;
            CurrentTile = sender;
            this.lblStatus.Text = String.Format("Prev: {0}, Current: {1}", null == PrevTile ? "null" : PrevTile.Index.ToString(), CurrentTile.Index);

            if (null == PrevTile || null == CurrentTile || !PrevTile.Visible || !CurrentTile.Visible)
                return;

            if (Model.CanClear(PrevTile, CurrentTile))
            {
                PrevTile.Visible = false;
                CurrentTile.Visible = false;
                PrevTile = null;
                CurrentTile = null;
                ++Score;
                this.lblScore.Text = String.Format("Score: {0}", Score);
            }
        }
    }
}
