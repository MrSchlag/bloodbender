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

namespace MapGenerator
{
    public class RoomLoader
    {
        TmxMap tmxmap;

        public Room load(string path) {

            loadFile(path);
            if (tmxmap != null)
            {
                List<Wall> walls = new List<Wall>();
                List<Entry> entries = new List<Entry>();
                List<Entities> entities = new List<Entities>();
                Vector2 spawnPoint = new Vector2();

                if (this.checkWallExist())
                    walls = this.loadWalls();
                if (this.checkEntryExist())
                    entries = this.loadEntries();
                if (this.checkEntityExist())
                    entities = this.loadEntities();
                if (this.checkSpawnExist())
                    spawnPoint = this.loadSpawnPoint();
                if (walls.Count > 3 && entries.Count >= 1)
                {
                    try
                    {
                        if (tmxmap.ObjectGroups["player"].Objects[0] != null)
                            return new Room(tmxmap.TileWidth, tmxmap.Width, tmxmap.Height, walls, entries, entities, spawnPoint);
                    }
                    catch (Exception e)
                    {
                        return new Room(tmxmap.TileWidth, tmxmap.Width, tmxmap.Height, walls, entries, entities);
                    }
                    
                } else
                {
                    if (walls.Count < 3)
                        Debug.WriteLine("Room doesn't have enough walls to make a polygon");
                    else if (entries.Count < 1)
                        Debug.WriteLine("Room doesn't have any entries");
                }
            }
            return null;
        }

        public void loadFile(String path)
        {
            try
            {
                this.tmxmap = new TmxMap(path);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Invalid room file path");
            }
        }

        public bool checkWallExist()
        {
            try
            {
                if (tmxmap.ObjectGroups["wall"].Objects != null)
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public bool checkEntryExist()
        {
            try
            {
                if (tmxmap.ObjectGroups["entry"].Objects != null)
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public bool checkEntityExist()
        {
            try
            {
                if (tmxmap.ObjectGroups["entity"].Objects != null)
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public bool checkSpawnExist()
        {
            try
            {
                if (tmxmap.ObjectGroups["player"].Objects != null)
                    return true;
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public List<Wall> loadWalls()
        {
            List<Wall> walls = new List<Wall>();
            int objIndex = 0;
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
                            wall = new Wall(bvpoint1, bvpoint2, objIndex);
                        else
                            wall = new Wall(bvpoint2, bvpoint1, objIndex);
                        walls.Add(wall);
                    }
                    i++;
                }
                objIndex++;
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
                    int itype = this.findEntryType(entry_obj.Type);
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
            TmxObject spawnObj = tmxmap.ObjectGroups["player"].Objects[0];
            return new Vector2((float)spawnObj.X, (float)spawnObj.Y);
        }

        public int findEntryType(String type) {
            if (type == "top")
                return (int)entryType.top;
            else if (type == "bot")
                return (int)entryType.bot;
            else if (type == "left")
                return (int)entryType.left;
            else if (type == "right")
                return (int)entryType.right;
            return (int)entryType.undefined;
        }
    }
}
