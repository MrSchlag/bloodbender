using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapGenerator
{
    public enum entryType
    {
        top,
        bot,
        left,
        right,
        undefined
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

        public entryType findOppositeEntryType()
        {
            if (type == entryType.top)
                return entryType.bot;
            else if (type == entryType.bot)
                return entryType.top;
            if (type == entryType.left)
                return entryType.right;
            else if (type == entryType.right)
                return entryType.left;
            else
                return entryType.undefined;
        }
    }
}
