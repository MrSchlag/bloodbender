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
                if (exit.ptA.X == entry.ptA.X && exit.ptB.X == entry.ptB.X)
                    this.straightLinker(exit, entry);
                else
                    this.smoothLinker(exit, entry);
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
            if (entry.ptA.Y < exit.ptA.Y)
                this.smoothTopLinker(exit, entry);
            else if (entry.ptA.Y > exit.ptA.Y)
                this.smoothBotLinker(exit, entry);
            else if (entry.ptA.X < exit.ptA.X)
                this.smoothRightLinker(exit, entry);
            else if (entry.ptA.X > exit.ptA.X)
                this.smoothLeftLinker(exit, entry);
        }

        public void smoothTopLinker(Entry exit, Entry entry)
        {
            float distance = Math.Abs(entry.ptA.Y - exit.ptA.Y);
            float entryLength = Math.Abs(entry.ptA.X - entry.ptB.X);
            float diffPath = (distance - entryLength) / 2;
            if (exit.ptA.X < entry.ptB.X || exit.ptA.X < entry.ptA.X)
            {
                //DROITE
                Vector2 fInVec = exit.ptA;
                Vector2 fOutVec = exit.ptB;
                Vector2 lInVec = new Vector2(exit.ptA.X, exit.ptA.Y - diffPath);
                Vector2 lOutVec = new Vector2(exit.ptB.X, exit.ptB.Y - entryLength - diffPath);
                botWallList.Add(new Wall(fInVec, lInVec));
                topWallList.Add(new Wall(fOutVec, lOutVec));
                fInVec = new Vector2(exit.ptA.X, exit.ptA.Y - diffPath);
                lInVec = new Vector2(entry.ptA.X, exit.ptA.Y - diffPath);
                fOutVec = new Vector2(exit.ptB.X, exit.ptB.Y - entryLength - diffPath);
                lOutVec = new Vector2(entry.ptB.X, exit.ptB.Y - entryLength - diffPath);
                botWallList.Add(new Wall(fInVec, lInVec));
                topWallList.Add(new Wall(fOutVec, lOutVec));
                fInVec = new Vector2(entry.ptA.X, exit.ptA.Y - diffPath);
                lInVec = entry.ptA;
                fOutVec = new Vector2(entry.ptB.X, exit.ptB.Y - entryLength - diffPath);
                lOutVec = entry.ptB;
                botWallList.Add(new Wall(fInVec, lInVec));
                topWallList.Add(new Wall(fOutVec, lOutVec));
            }
            else
            {
                //GAUCHE
                Vector2 fInVec = exit.ptB;
                Vector2 fOutVec = exit.ptA;
                Vector2 lInVec = new Vector2(exit.ptB.X, exit.ptB.Y - diffPath);
                Vector2 lOutVec = new Vector2(exit.ptA.X, exit.ptA.Y - entryLength - diffPath);
                botWallList.Add(new Wall(fInVec, lInVec));
                topWallList.Add(new Wall(fOutVec, lOutVec));
                fInVec = new Vector2(exit.ptB.X, exit.ptB.Y - diffPath);
                lInVec = new Vector2(entry.ptB.X, exit.ptB.Y - diffPath);
                fOutVec = new Vector2(exit.ptA.X, exit.ptA.Y - entryLength - diffPath);
                lOutVec = new Vector2(entry.ptA.X, exit.ptA.Y - entryLength - diffPath);
                botWallList.Add(new Wall(fInVec, lInVec));
                topWallList.Add(new Wall(fOutVec, lOutVec));
                fInVec = new Vector2(entry.ptB.X, exit.ptB.Y - diffPath);
                lInVec = entry.ptB;
                fOutVec = new Vector2(entry.ptA.X, exit.ptA.Y - entryLength - diffPath);
                lOutVec = entry.ptA;
                botWallList.Add(new Wall(fInVec, lInVec));
                topWallList.Add(new Wall(fOutVec, lOutVec));
            }
        }

        public void smoothBotLinker(Entry exit, Entry entry)
        {
            float distance = Math.Abs(entry.ptA.Y - exit.ptA.Y);
            float entryLength = Math.Abs(entry.ptA.X - entry.ptB.X);
            float diffPath = (distance - entryLength) / 2;
            if (exit.ptA.X < entry.ptB.X || exit.ptA.X < entry.ptA.X)
            {
                //DROITE
                Vector2 fInVec = exit.ptA;
                Vector2 fOutVec = exit.ptB;
                Vector2 lInVec = new Vector2(exit.ptA.X, exit.ptA.Y + diffPath);
                Vector2 lOutVec = new Vector2(exit.ptB.X, exit.ptB.Y + entryLength + diffPath);
                botWallList.Add(new Wall(fInVec, lInVec));
                topWallList.Add(new Wall(fOutVec, lOutVec));
                fInVec = new Vector2(exit.ptA.X, exit.ptA.Y + diffPath);
                lInVec = new Vector2(entry.ptA.X, exit.ptA.Y + diffPath);
                fOutVec = new Vector2(exit.ptB.X, exit.ptB.Y + entryLength + diffPath);
                lOutVec = new Vector2(entry.ptB.X, exit.ptB.Y + entryLength + diffPath);
                botWallList.Add(new Wall(fInVec, lInVec));
                topWallList.Add(new Wall(fOutVec, lOutVec));
                fInVec = new Vector2(entry.ptA.X, exit.ptA.Y + diffPath);
                lInVec = entry.ptA;
                fOutVec = new Vector2(entry.ptB.X, exit.ptB.Y + entryLength + diffPath);
                lOutVec = entry.ptB;
                botWallList.Add(new Wall(fInVec, lInVec));
                topWallList.Add(new Wall(fOutVec, lOutVec));
            }
            else
            {
                //GAUCHE
                Vector2 fInVec = exit.ptB;
                Vector2 fOutVec = exit.ptA;
                Vector2 lInVec = new Vector2(exit.ptB.X, exit.ptB.Y + diffPath);
                Vector2 lOutVec = new Vector2(exit.ptA.X, exit.ptA.Y + entryLength + diffPath);
                botWallList.Add(new Wall(fInVec, lInVec));
                topWallList.Add(new Wall(fOutVec, lOutVec));
                fInVec = new Vector2(exit.ptB.X, exit.ptB.Y + diffPath);
                lInVec = new Vector2(entry.ptB.X, exit.ptB.Y + diffPath);
                fOutVec = new Vector2(exit.ptA.X, exit.ptA.Y + entryLength + diffPath);
                lOutVec = new Vector2(entry.ptA.X, exit.ptA.Y + entryLength + diffPath);
                botWallList.Add(new Wall(fInVec, lInVec));
                topWallList.Add(new Wall(fOutVec, lOutVec));
                fInVec = new Vector2(entry.ptB.X, exit.ptB.Y + diffPath);
                lInVec = entry.ptB;
                fOutVec = new Vector2(entry.ptA.X, exit.ptA.Y + entryLength + diffPath);
                lOutVec = entry.ptA;
                botWallList.Add(new Wall(fInVec, lInVec));
                topWallList.Add(new Wall(fOutVec, lOutVec));
            }
        }

        public void smoothLeftLinker(Entry exit, Entry entry)
        {
            float distance = Math.Abs(entry.ptA.X - exit.ptA.X);
            float entryLength = Math.Abs(entry.ptA.Y - entry.ptB.Y);
            float diffPath = (distance - entryLength) / 2;
        }

        public void smoothRightLinker(Entry exit, Entry entry)
        {
            float distance = Math.Abs(entry.ptA.X - exit.ptA.X);
            float entryLength = Math.Abs(entry.ptA.Y - entry.ptB.Y);
            float diffPath = (distance - entryLength) / 2;
        }
    }
}
