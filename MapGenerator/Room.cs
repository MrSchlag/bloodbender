using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        public List<Entity> entityList { get; set; }
        public Vector2 spawnPoint { get; set; }
        public Entry entrySelected { get; set; }
        public Entry exitSelected { get; set; }

        public float minX { get; set; }
        public float minY { get; set; }
        public float maxX { get; set; }
        public float maxY { get; set; }

        public Room(int tileSize, int X, int Y, List<Wall> wallList, List<Entry> entryList, List<Entity> entityList, Vector2 spawnPoint)
        {
            this.tileSize = tileSize;
            this.X = X;
            this.Y = Y;
            this.wallList = wallList;
            this.entryList = entryList;
            this.entityList = entityList;
            this.spawnPoint = spawnPoint;
        }
        
        public Room(int tileSize, int X, int Y, List<Wall> wallList, List<Entry> entryList, List<Entity> entityList)
        {
            this.tileSize = tileSize;
            this.X = X;
            this.Y = Y;
            this.wallList = wallList;
            this.entryList = entryList;
            this.entityList = entityList;
        }

        public Entry findRandomEntryByType(Random rand, entryType type)
        {
            List<Entry> entriesFound = new List<Entry>();
            foreach (Entry entry in entryList)
            {
                if (entry.type == type)
                {
                    // Debug.WriteLine("TYPE TO FIND {0} - INDEX FOUND/TYPE {0}/{1}", type, entryList.IndexOf(entry), entry.type); ;
                    entriesFound.Add(entry);
                }  
            }
            if (entriesFound.Count == 0)
                return null;
            int entryIndex = rand.Next(0, entriesFound.Count);
            return entriesFound[entryIndex];
        }

        public void selectRandomExit(Random rand)
        {
            List<Entry> exitsFound = new List<Entry>();
            foreach (Entry entry in entryList)
            {
                if (entry.type == this.entrySelected.findOppositeEntryType())
                    exitsFound.Add(entry);
            }
            int exitIndex = rand.Next(0, exitsFound.Count);
            this.exitSelected = this.entryList[exitIndex];
        }
    }
}
