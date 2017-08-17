﻿using Microsoft.Xna.Framework;
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
            float distance = Math.Abs(entry.ptA.Y - exit.ptA.Y);
            float entryLength = Math.Abs(entry.ptA.X - entry.ptB.X);
            float diffPath = (distance - entryLength) / 2;
            this.smoothLinkerFirstStep(exit, entry, diffPath, entryLength);
            this.smoothLinkerMidStep(exit, entry, diffPath, entryLength);
            this.smoothLinkerLastStep(exit, entry, diffPath, entryLength);
        }

        public void smoothLinkerFirstStep(Entry exit, Entry entry, float diffPath, float entryLength)
        {
            //DE BAS EN HAUT
            if (entry.ptA.Y < exit.ptA.Y)
            {
                if (exit.ptA.X < entry.ptB.X || exit.ptA.X < entry.ptA.X)
                {
                    //DROITE
                    Vector2 fInVec = exit.ptA;
                    Vector2 fOutVec = exit.ptB;
                    Vector2 lInVec = new Vector2(exit.ptA.X, exit.ptA.Y - diffPath);
                    Vector2 lOutVec = new Vector2(exit.ptB.X, exit.ptB.Y - entryLength - diffPath);
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
                }
            //DE HAUT EN BAS
            } else if (entry.ptA.Y > exit.ptA.Y) {
                if (exit.ptA.X < entry.ptB.X || exit.ptA.X < entry.ptA.X)
                {
                    //DROITE
                    Vector2 fInVec = exit.ptA;
                    Vector2 fOutVec = exit.ptB;
                    Vector2 lInVec = new Vector2(exit.ptA.X, exit.ptA.Y + diffPath);
                    Vector2 lOutVec = new Vector2(exit.ptB.X, exit.ptB.Y + entryLength + diffPath);
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
                }
            }
        }

        public void smoothLinkerMidStep(Entry exit, Entry entry, float diffPath, float entryLength)
        {
            if (entry.ptA.Y < exit.ptA.Y)
            {
                if (exit.ptA.X < entry.ptB.X || exit.ptA.X < entry.ptA.X)
                {
                    Vector2 fInVec = new Vector2(exit.ptA.X, exit.ptA.Y - diffPath);
                    Vector2 lInVec = new Vector2(entry.ptA.X, exit.ptA.Y - diffPath);
                    Vector2 fOutVec = new Vector2(exit.ptB.X, exit.ptB.Y - entryLength - diffPath);
                    Vector2 lOutVec = new Vector2(entry.ptB.X, exit.ptB.Y - entryLength - diffPath);
                    botWallList.Add(new Wall(fInVec, lInVec));
                    topWallList.Add(new Wall(fOutVec, lOutVec));
                }
                else
                {
                    Vector2 fInVec = new Vector2(exit.ptB.X, exit.ptB.Y - diffPath);
                    Vector2 lInVec = new Vector2(entry.ptB.X, exit.ptB.Y - diffPath);
                    Vector2 fOutVec = new Vector2(exit.ptA.X, exit.ptA.Y - entryLength - diffPath);
                    Vector2 lOutVec = new Vector2(entry.ptA.X, exit.ptA.Y - entryLength - diffPath);
                    botWallList.Add(new Wall(fInVec, lInVec));
                    topWallList.Add(new Wall(fOutVec, lOutVec));
                }
            } else if (entry.ptA.Y > exit.ptA.Y) {
                if (exit.ptA.X < entry.ptB.X || exit.ptA.X < entry.ptA.X)
                {
                    Vector2 fInVec = new Vector2(exit.ptA.X, exit.ptA.Y + diffPath);
                    Vector2 lInVec = new Vector2(entry.ptA.X, exit.ptA.Y + diffPath);
                    Vector2 fOutVec = new Vector2(exit.ptB.X, exit.ptB.Y + entryLength + diffPath);
                    Vector2 lOutVec = new Vector2(entry.ptB.X, exit.ptB.Y + entryLength + diffPath);
                    botWallList.Add(new Wall(fInVec, lInVec));
                    topWallList.Add(new Wall(fOutVec, lOutVec));
                }
                else
                {
                    Vector2 fInVec = new Vector2(exit.ptB.X, exit.ptB.Y + diffPath);
                    Vector2 lInVec = new Vector2(entry.ptB.X, exit.ptB.Y + diffPath);
                    Vector2 fOutVec = new Vector2(exit.ptA.X, exit.ptA.Y + entryLength + diffPath);
                    Vector2 lOutVec = new Vector2(entry.ptA.X, exit.ptA.Y + entryLength + diffPath);
                    botWallList.Add(new Wall(fInVec, lInVec));
                    topWallList.Add(new Wall(fOutVec, lOutVec));
                }
            }
        }

        public void smoothLinkerLastStep(Entry exit, Entry entry, float diffPath, float entryLength)
        {
            if (entry.ptA.Y < exit.ptA.Y)
            {
                if (exit.ptA.X < entry.ptB.X || exit.ptA.X < entry.ptA.X)
                {
                    Vector2 fInVec = new Vector2(entry.ptA.X, exit.ptA.Y - diffPath);
                    Vector2 lInVec = entry.ptA;
                    Vector2 fOutVec = new Vector2(entry.ptB.X, exit.ptB.Y - entryLength - diffPath);
                    Vector2 lOutVec = entry.ptB;
                    botWallList.Add(new Wall(fInVec, lInVec));
                    topWallList.Add(new Wall(fOutVec, lOutVec));
                }
                else
                {
                    Vector2 fInVec = new Vector2(entry.ptB.X, exit.ptB.Y - diffPath);
                    Vector2 lInVec = entry.ptB;
                    Vector2 fOutVec = new Vector2(entry.ptA.X, exit.ptA.Y - entryLength - diffPath);
                    Vector2 lOutVec = entry.ptA;
                    botWallList.Add(new Wall(fInVec, lInVec));
                    topWallList.Add(new Wall(fOutVec, lOutVec));
                }
            } else if (entry.ptA.Y > exit.ptA.Y) {
                if (exit.ptA.X < entry.ptB.X || exit.ptA.X < entry.ptA.X)
                {
                    Vector2 fInVec = new Vector2(entry.ptA.X, exit.ptA.Y + diffPath);
                    Vector2 lInVec = entry.ptA;
                    Vector2 fOutVec = new Vector2(entry.ptB.X, exit.ptB.Y + entryLength + diffPath);
                    Vector2 lOutVec = entry.ptB;
                    botWallList.Add(new Wall(fInVec, lInVec));
                    topWallList.Add(new Wall(fOutVec, lOutVec));
                }
                else
                {
                    Vector2 fInVec = new Vector2(entry.ptB.X, exit.ptB.Y + diffPath);
                    Vector2 lInVec = entry.ptB;
                    Vector2 fOutVec = new Vector2(entry.ptA.X, exit.ptA.Y + entryLength + diffPath);
                    Vector2 lOutVec = entry.ptA;
                    botWallList.Add(new Wall(fInVec, lInVec));
                    topWallList.Add(new Wall(fOutVec, lOutVec));
                }
            }
        }
    }
}
