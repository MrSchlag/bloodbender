using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace MapGenerator
{
    public class Room
    {
        public int X { get; set; }
        public int Y { get; set; }

        public int tileSize { get; set; }
        public List<Wall> wallList { get; set; }
        public List<Entry> entryList { get; set; }
        public List<Entities> entityList { get; set; }
        public Vector2 spawnPoint { get; set; }

        public Room(int tileSize, int X, int Y, List<Wall> wallList, List<Entry> entryList, List<Entities> entityList, Vector2 spawnPoint)
        {
            this.tileSize = tileSize;
            this.X = X;
            this.Y = Y;
            this.wallList = wallList;
            this.entryList = entryList;
            this.entityList = entityList;
            this.spawnPoint = spawnPoint;
        }

        public Room(int tileSize, int X, int Y, List<Wall> wallList, List<Entry> entryList, List<Entities> entityList)
        {
            this.tileSize = tileSize;
            this.X = X;
            this.Y = Y;
            this.wallList = wallList;
            this.entryList = entryList;
            this.entityList = entityList;
        }
    }
}
