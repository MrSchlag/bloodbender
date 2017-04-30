using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodbenderMapGenerator
{
    public enum entryType
    {
        top,
        bot,
        left,
        right,
        topleftdiag,
        toprightdiag,
        botleftdiag,
        botrightdiag
    }
    public class Entry
    {
        public Vector2 ptA { get; set; }
        public Vector2 ptB { get; set; }
        public entryType type { get; set; }
        public Entry(Vector2 ptA, Vector2 ptB, entryType type)
        {
            this.ptA = ptA;
            this.ptB = ptB;
            this.type = type;
        }
    }
}
