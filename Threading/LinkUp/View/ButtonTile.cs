﻿using System;
using System.Drawing;
using System.Windows.Forms;
using Threading.LinkUp.Model;

namespace Threading.LinkUp.View
{
    public class ButtonTile : Button
    {
        public int Index { get; set; }
        public int Value { get; set; }
        public int CoordinateX { get; set; }
        public int CoordinateY { get; set; }
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
                if (this.Visible && LinkUpModel.SameValueClear)
                    return CommonDef.Textures[Value - 1].ToString();
                else
                    // return Value > 0 ? CommonDef.PredatorTextures[Value - 1].ToString() : CommonDef.PreyTextures[-Value - 1].ToString();
                    return String.Empty;
            }            
        }

                
        public ButtonTile(int index, int x, int y, int value, bool visible)
        {
            Index = index;
            Value = value;
            CoordinateX = x;
            CoordinateY = y;
            this.Visible = visible;            
            
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
