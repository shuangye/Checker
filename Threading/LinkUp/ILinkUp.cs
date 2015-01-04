using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Threading.LinkUp
{
    public class LinkUpInterface
    {
        public char[] Maps { get; set; }

        public LinkUpInterface(int textureKinds)
        {
            if (textureKinds > 0)
            {
                Maps = new char[textureKinds];
                Maps[0] = ' ';
            }
        }
    }
}
