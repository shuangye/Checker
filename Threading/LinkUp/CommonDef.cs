using System;
using System.Collections.Generic;
using Threading.Helpers;
using Threading.Properties;

namespace Threading.LinkUp
{
    public enum GameMode
    {
        Classical, PredatorPrey
    }

    /// <summary>
    /// Common resources definitions
    /// </summary>
    public static class CommonDef
    {
        public static int FringeTileValue { get { return 0; } }
        public static char[] PredatorTextures = new char[] { '猫', '狗', '狼', '牛', '鸡', '猴' };
        public static char[] PreyTextures = new char[] { '鼠', '骨', '羊', '草', '米', '蕉' };
        public static List<KeyValuePair<int, object>> PredatorPreyTextures
        {
            get
            {
                return new List<KeyValuePair<int, object>> {
                new KeyValuePair<int, object>(0, null),
                // high memory usage and poor performance when loading from resx files
                // new KeyValuePair<int, object>(1, Resources.Cat),
                // new KeyValuePair<int, object>(-1, Resources.Mouse),
                // new KeyValuePair<int, object> (2, Resources.Dog),
                // new KeyValuePair<int, object> (-2, Resources.Bone),
                // new KeyValuePair<int, object> (3, Resources.Wolf),
                // new KeyValuePair<int, object> (-3, Resources.Goat),
                // new KeyValuePair<int, object> (4, Resources.Ox),
                // new KeyValuePair<int, object> (-4, Resources.Grass),
                // new KeyValuePair<int, object> (5, Resources.Chicken),
                // new KeyValuePair<int, object> (-5, Resources.Rice),
                // new KeyValuePair<int, object> (6, Resources.Monkey),
                // new KeyValuePair<int, object> (-6, Resources.Banana)
                new KeyValuePair<int, object>(1, "Cat.jpg"),
                new KeyValuePair<int, object>(-1, "Mouse.jpg"),
                new KeyValuePair<int, object>(2, "Dog.jpg"),
                new KeyValuePair<int, object>(-2, "Bone.jpg"),
                new KeyValuePair<int, object>(3, "Wolf.jpg"),
                new KeyValuePair<int, object>(-3, "Goat.jpg"),
                new KeyValuePair<int, object>(4, "Ox.jpg"),
                new KeyValuePair<int, object>(-4, "Grass.jpg"),
                new KeyValuePair<int, object>(5, "Chicken.jpg"),
                new KeyValuePair<int, object>(-5, "Rice.jpg"),
                new KeyValuePair<int, object>(6, "Monkey.jpg"),
                new KeyValuePair<int, object>(-6, "Banana.jpg"),
                };
            }
        }

        public static char[] Textures
        {
            get
            {
                // return HunterTextures.Concat(PreyTextures).ToArray().Shuffle();
                return new char[] { '๑', '◎', '○', '◇', '☆', '♤', '♧', '☺', '♥', '☀',
                '¤', '⊙', '♀', '♂', '∮', '◕', '◐', '◑', '☢', '☪',
                '☃', '☂', '❁', '❀', '♨', '㊣', '☎', '☏', '△', '▽',
                '▉', '□', '⊱', '⊰', '⋌', '⌒', '®', '¢', '卍', '卐',
                '※', '∷', '¶', '♯', '$', 'Ψ', '§', '♫', '♪', '◈',
                '▣', '☄', '☣', '❂', '➹', '❦', '❧', '❀', 'ஐ', 'ღ',
                '☠', '✄', '✎'}; //.Shuffle(); ;
            }
        }
    }
}