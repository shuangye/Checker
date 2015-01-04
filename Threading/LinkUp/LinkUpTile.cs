using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Threading.LinkUp
{
    public class LinkUpTile: Button
    {
        public LinkUpForm ParentWindow { get; set; }

        public static Point StartPoint { get { return new Point(10, 10); } }
        public static char[] Maps { get; set; }
        public int Space { get { return 10; } }
        public int Side { get { return 50; } }

        public int Index { get; set; }  // zero-based
        public int CoordinateX { get; set; }
        public int CoordinateY { get; set; }
        public int Value { get; set; }
        public int PathLength { get; set; }  // path length to a certain tile

        public LinkUpTile(int index, int x, int y, int value)
        {
            Index = index;
            CoordinateX = x;
            CoordinateY = y;
            Value = value;        
#warning eliminate hard coding            
            this.Visible = -1 != Value;  // is a edge tile?

            this.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // this.Text = String.Format("{0} ({1}) ({2}, {3})", Value, Index, CoordinateX, CoordinateY);
            this.Text = Value.ToString();
            this.Size = new Size(Side, Side);
            this.Location = new Point(StartPoint.X + x * (Side + Space), StartPoint.Y + y * (Side + Space));

            this.Click += (sender, e) =>
            {
                if (null != ParentWindow)
                {
                    ParentWindow.tileClicked(this, e);
                }
            };
        }
    }
}
