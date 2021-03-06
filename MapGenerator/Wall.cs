﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapGenerator
{
    public class Wall
    {
        public Vector2 ptA { get; set; }
        public Vector2 ptB { get; set; }
        public int objIndex { get; set; }

        public Wall(Vector2 ptA, Vector2 ptB, int objIndex)
        {
            this.ptA = ptA;
            this.ptB = ptB;
            this.objIndex = objIndex;
        }

        public Wall(Vector2 ptA, Vector2 ptB)
        {
            this.ptA = ptA;
            this.ptB = ptB;
            this.objIndex = objIndex;
        }
    }
}
