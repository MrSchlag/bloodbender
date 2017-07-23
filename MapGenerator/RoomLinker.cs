using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapGenerator
{
    public class RoomLinker
    {
        public List<Wall> botWallList { get; set; }
        public List<Wall> topWallList { get; set; }

        public RoomLinker(Entry exit, Entry entry)
        {
            botWallList = new List<Wall>();
            topWallList = new List<Wall>();
            if (exit.type == entryType.top || exit.type == entryType.bot)
            {
                // Debug.WriteLine("exit  {0} / {1} - {2} / {3}", exit.ptA.X, exit.ptA.Y, exit.ptB.X, exit.ptB.Y);
                // Debug.WriteLine("entry {0} / {1} - {2} / {3}", entry.ptA.X, entry.ptA.Y, entry.ptB.X, entry.ptB.Y);
                if (exit.ptA.X == entry.ptA.X && exit.ptB.X == entry.ptB.X)
                {
                    Vector2 initVecA = new Vector2(exit.ptA.X, exit.ptA.Y);
                    Vector2 middleVecA = new Vector2(exit.ptA.X, entry.ptA.Y - ((entry.ptA.Y - exit.ptA.Y) / 2));
                    Vector2 lastVecA = entry.ptA;
                    // Debug.WriteLine("A INIT VEC {0}/{1}", initVecA.X, initVecA.Y);
                    // Debug.WriteLine("A MIDDLE VEC {0}/{1}", middleVecA.X, middleVecA.Y);
                    // Debug.WriteLine("A LAST VEC {0}/{1}", lastVecA.X, lastVecA.Y);
                    botWallList.Add(new Wall(initVecA, middleVecA));
                    botWallList.Add(new Wall(middleVecA, lastVecA));
                    Vector2 initVecB = new Vector2(exit.ptB.X, exit.ptB.Y);
                    Vector2 middleVecB = new Vector2(exit.ptB.X, entry.ptB.Y - ((entry.ptB.Y - exit.ptB.Y) / 2));
                    Vector2 lastVecB = entry.ptB;
                    // Debug.WriteLine("B INIT VEC {0}/{1}", initVecB.X, initVecB.Y);
                    // Debug.WriteLine("B MIDDLE VEC {0}/{1}", middleVecB.X, middleVecB.Y);
                    // Debug.WriteLine("B LAST VEC {0}/{1}", lastVecB.X, lastVecB.Y);
                    topWallList.Add(new Wall(initVecB, middleVecB));
                    topWallList.Add(new Wall(middleVecB, lastVecB));
                }
                else
                {

                }
            }
        }
    }
}
