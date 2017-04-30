using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledSharp;

namespace BloodbenderMapGenerator
{
    public class RoomLoader
    {
        TmxMap tmxmap;

        public Room load(string path) {
            this.tmxmap = new TmxMap(path);

            if (tmxmap != null)
            {
                List<Wall> walls = this.loadWalls();
                List<Entry> entries = this.loadEntries();
                Vector2 spawnPoint = this.loadSpawnPoint();


                if (walls.Count > 2 && entries.Count > 1)
                {
                    Room room = new Room(tmxmap.TileWidth, tmxmap.Width, tmxmap.Height, walls, entries, spawnPoint);
                    return room;
                }
            }
            return null;
        }

        public List<Wall> loadWalls()
        {
            TmxObject border_obj = tmxmap.ObjectGroups["wall"].Objects[0];
            Collection<TmxObjectPoint> bpoints = border_obj.Points;

            Vector2 bvpoint1 = new Vector2();
            Vector2 bvpoint2 = new Vector2();
            List<Wall> walls = new List<Wall>();

            int i = 0;
            foreach (var point in bpoints)
            {
                Debug.WriteLine(point.X + " " + point.Y);
                if (i % 2 == 0)
                    bvpoint2 = new Vector2((float)(border_obj.X + point.X), (float)(border_obj.Y + point.Y));
                else
                    bvpoint1 = new Vector2((float)(border_obj.X + point.X), (float)(border_obj.Y + point.Y));
                if (i > 0 && !bvpoint1.Equals(null) && !bvpoint2.Equals(null))
                {
                    Wall wall;
                    if (i % 2 == 0)
                        wall = new Wall(bvpoint1, bvpoint2);
                    else
                        wall = new Wall(bvpoint2, bvpoint1);

                    walls.Add(wall);
                }
                i++;
            }
            return walls;
        }

        public List<Entry> loadEntries() {
            TmxList<TmxObject> elist = tmxmap.ObjectGroups["entry"].Objects;
            List<Entry> entries = new List<Entry>();

            foreach (var entry_obj in elist)
            {
                if (!entry_obj.Points[0].Equals(null) && !entry_obj.Points[1].Equals(null))
                {
                    Vector2 evpoint1 = new Vector2((float)(entry_obj.X + entry_obj.Points[0].X), (float)(entry_obj.Y + entry_obj.Points[0].Y));
                    Vector2 evpoint2 = new Vector2((float)(entry_obj.X + entry_obj.Points[1].X), (float)(entry_obj.Y + entry_obj.Points[1].Y));
                    int itype = this.findType(entry_obj.Type);
                    
                    if (itype != -1)
                    {
                        entryType type = (entryType)itype;
                        Entry entry = new Entry(evpoint1, evpoint2, type);
                        entries.Add(entry);
                    }
                }
            }
            return entries;
        }

        public Vector2 loadSpawnPoint() {
            TmxObject spawnObj = tmxmap.ObjectGroups["player"].Objects[0];
            return new Vector2((float)spawnObj.X, (float)spawnObj.Y);
        }

        public int findType(String type) {
            if (type == "top")
                return 0;
            else if (type == "bot")
                return 1;
            else if (type == "left")
                return 2;
            else if (type == "right")
                return 3;
            else if (type == "topleftdiag")
                return 4;
            else if (type == "toprightdiag")
                return 5;
            else if (type == "botleftdiag")
                return 6;
            else if (type == "botrightdiag")
                return 7;
            return -1;
        }
    }
}
