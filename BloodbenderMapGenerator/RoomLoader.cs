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
                List<Entities> entities = this.loadEntities();
                Vector2 spawnPoint = this.loadSpawnPoint();

                if (walls.Count > 3 && entries.Count >= 1)
                {
                    if (tmxmap.ObjectGroups["player"].Objects[0] != null)
                    {
                        Room room = new Room(tmxmap.TileWidth, tmxmap.Width, tmxmap.Height, walls, entries, entities, spawnPoint);
                        return room;
                    } else
                    {
                        Room room = new Room(tmxmap.TileWidth, tmxmap.Width, tmxmap.Height, walls, entries, entities);
                        return room;
                    }
                    
                } else
                {
                    if (walls.Count < 3)
                    {
                        Debug.WriteLine("Room doesn't have enough walls to make a polygon");
                    } else if (entries.Count < 1)
                    {
                        Debug.WriteLine("Room doesn't have any entries");
                    }
                }
            }
            return null;
        }

        public List<Wall> loadWalls()
        {
            List<Wall> walls = new List<Wall>();
            foreach (TmxObject border_obj in tmxmap.ObjectGroups["wall"].Objects) {
                Collection<TmxObjectPoint> bpoints = border_obj.Points;
                Vector2 bvpoint1 = new Vector2();
                Vector2 bvpoint2 = new Vector2();
                int i = 0;

                foreach (var point in bpoints)
                {
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
                    if (itype != 4)
                    {
                        entryType type = (entryType)itype;
                        Entry entry = new Entry(evpoint1, evpoint2, type);
                        entries.Add(entry);
                    } else
                    {
                        Debug.WriteLine("Type of entry [" + evpoint1.X + "/" + evpoint1.Y + "-" + evpoint2.X + "/" + evpoint2.Y + "] is undefined] => Fix it on Tiled");
                    }
                }
            }
            return entries;
        }

        public List<Entities> loadEntities()
        {
            return new List<Entities>();
        }

        public Vector2 loadSpawnPoint() {
            if (tmxmap.ObjectGroups["player"].Objects[0] != null)
            {
                TmxObject spawnObj = tmxmap.ObjectGroups["player"].Objects[0];
                return new Vector2((float)spawnObj.X, (float)spawnObj.Y);
            } else
            {
                Debug.WriteLine("No Spawn point found");
                return new Vector2();
            }
            
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
            return 4;
        }
    }
}
