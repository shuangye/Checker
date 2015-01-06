using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Threading.LinkUp.View
{
    public class ButtonTile: Button, ITile
    {
        public int Index { get; set; }
        public int Value { get; set; }
        public int CoordinateX { get; set; }
        public int CoordinateY { get; set; }
        public int PathLength { get; set; }
        // the Visible property is implemented in Button?

        // All tiles should have the same size, so make these properties static
        public static new int Padding { get { return 10; } }  // the "new" keyword hides the property of same name from parent
        public static int Side { get { return 50; } }        
        public static Font TextFont { get { return new Font("微软雅黑", 20, FontStyle.Regular); } }
        public static Point StartPoint { get { return new Point(10, 10); } }        
        public LinkUpForm ParentWindow { get; set; }
        

        public ButtonTile(int index, int x, int y, int value)
        {
            Index = index;
            Value = value;  
            CoordinateX = x;
            CoordinateY = y;                 
 
            this.Visible = LinkUpModel<ButtonTile>.EdgeTileValue != Value;  // is a edge tile?                        
            if (this.Visible)
            {
                this.Font = TextFont;
                this.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                this.Text = LinkUpModel<ButtonTile>.TextureMap[Value].ToString();
                // this.Text = Value.ToString();
            }            

            this.Size = new Size(Side, Side);
            this.Location = new Point(StartPoint.X + x * (Side + Padding), StartPoint.Y + y * (Side + Padding));

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
