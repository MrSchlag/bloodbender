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
            // this.straightLinker(exit, entry);
            // this.smoothLinker(exit, entry);
            if (exit.type == entryType.top || exit.type == entryType.bot)
            {
                if (exit.ptA.X == entry.ptA.X && exit.ptB.X == entry.ptB.X)
                    this.straightLinker(exit, entry);
                else
                {
                    this.smoothLinker(exit, entry);
                }
            }
        }

        public void straightLinker(Entry exit, Entry entry)
        {
            Vector2 fVecA = entry.ptA;
            Vector2 mVecA = new Vector2(exit.ptA.X, entry.ptA.Y - ((entry.ptA.Y - exit.ptA.Y) / 2));
            Vector2 lVecA = new Vector2(exit.ptA.X, exit.ptA.Y);
            botWallList.Add(new Wall(fVecA, mVecA));
            botWallList.Add(new Wall(mVecA, lVecA));
            Vector2 fVecB = entry.ptB;
            Vector2 mVecB = new Vector2(exit.ptB.X, entry.ptB.Y - ((entry.ptB.Y - exit.ptB.Y) / 2));
            Vector2 lVecB = new Vector2(exit.ptB.X, exit.ptB.Y);
            topWallList.Add(new Wall(fVecB, mVecB));
            topWallList.Add(new Wall(mVecB, lVecB));
        }

        public void smoothLinker(Entry exit, Entry entry)
        {
            float distance = Math.Abs(entry.ptA.Y - exit.ptA.Y);
            float entryLength = Math.Abs(entry.ptA.X - entry.ptB.X);
            float diffPath = (distance - entryLength) / 2;
            Debug.WriteLine(distance + " " + entryLength + " " + diffPath);
            Debug.WriteLine(entry.ptA.Y + " " + exit.ptA.Y);
            //if (exit.ptA.X > exit.ptB.X)
            //{
                Vector2 fVecA = exit.ptA;
                //to be removed
                Debug.WriteLine(exit.ptA.Y + " " + (exit.ptA.Y - diffPath));
                Vector2 mVecA = new Vector2(exit.ptA.X, exit.ptA.Y - diffPath);
                Vector2 lVecA = new Vector2(exit.ptA.X, exit.ptA.Y - diffPath - 20);
                botWallList.Add(new Wall(fVecA, mVecA));
                botWallList.Add(new Wall(mVecA, lVecA));
                Vector2 fVecB = exit.ptB;
                //to be removed
                Vector2 mVecB = new Vector2(exit.ptB.X, exit.ptB.Y - entryLength - diffPath);
                Vector2 lVecB = new Vector2(exit.ptB.X, exit.ptB.Y - entryLength - diffPath - 20);
                topWallList.Add(new Wall(fVecB, mVecB));
                topWallList.Add(new Wall(mVecB, lVecB));
            //} else
            //{

            //}
            
        }
    }
}
