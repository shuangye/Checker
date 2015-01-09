using System;
using System.Drawing;
using System.Windows.Forms;

namespace Threading.LinkUp.View
{
    public class ButtonTile : Button, ITile
    {
        public int Index { get; set; }
        public int Value { get; set; }
        public int CoordinateX { get; set; }
        public int CoordinateY { get; set; }
        public int PathLength { get; set; }
        public Point Center { get { return new Point(this.Location.X + Width / 2, this.Location.Y + Height / 2); } }
        // the Visible property is implemented in Button?
        
        public LinkUpForm ParentWindow { get; set; }

        public override string Text
        {
            // Making the Text property be computed from the Value property allows Text property changes at any time 
            // when the Value property changes, instead of chaning only at constructing time.
            get
            {
                // return Value.ToString();
                if (LinkUpModel<ButtonTile>.SameValueClear)
                    return CommonDef.Textures[Value - 1].ToString();
                else
                    // return Value > 0 ? CommonDef.PredatorTextures[Value - 1].ToString() : CommonDef.PreyTextures[-Value - 1].ToString();
                    return String.Empty;
            }            
        }

                
        public ButtonTile(int index, int x, int y, int value)
        {
            Index = index;
            Value = value;
            CoordinateX = x;
            CoordinateY = y;                       
            this.Visible =  CommonDef.FringeTileValue != Value;  // is a fringe tile?            
            
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
