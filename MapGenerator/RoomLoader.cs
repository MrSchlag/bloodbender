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
                List<Entity> entities = new List<Entity>();
                Vector2 spawnPoint = new Vector2();
                if (tmxmap.ObjectGroups.Contains("wall") && tmxmap.ObjectGroups["wall"].Objects.Count > 0)
                    walls = this.loadWalls();
                if (tmxmap.ObjectGroups.Contains("entry") && tmxmap.ObjectGroups["entry"].Objects.Count > 0)
                    entries = this.loadEntries();
                if (tmxmap.ObjectGroups.Contains("entity") && tmxmap.ObjectGroups["entity"].Objects.Count > 0)
                    entities = this.loadEntities();
                if (tmxmap.ObjectGroups.Contains("player") && tmxmap.ObjectGroups["player"].Objects.Count == 1)
                    
                    
                    spawnPoint = this.loadSpawnPoint();
                if (walls.Count > 3 && entries.Count >= 1)
                {
                    if (tmxmap.ObjectGroups.Contains("player") && tmxmap.ObjectGroups["player"].Objects.Count > 0)
                        return new Room(tmxmap.TileWidth, tmxmap.Width, tmxmap.Height, walls, entries, entities, spawnPoint);
                    else
                        return new Room(tmxmap.TileWidth, tmxmap.Width, tmxmap.Height, walls, entries, entities);
                } else {
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
                this.tmxmap = null;
                this.tmxmap = new TmxMap(path);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Invalid room file path");
            }
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
                        Entry entry;
                        if (evpoint1.X > evpoint2.X || evpoint1.Y > evpoint2.Y)
                            entry = new Entry(evpoint1, evpoint2, type);
                        else
                            entry = new Entry(evpoint2, evpoint1, type);
                        entries.Add(entry);
                    } else
                    {
                        Debug.WriteLine("Type of entry [" + evpoint1.X + "/" + evpoint1.Y + "-" + evpoint2.X + "/" + evpoint2.Y + "] is undefined] => Fix it on Tiled");
                    }
                }
            }
            return entries;
        }
        
        public List<Entity> loadEntities()
        {
            TmxList<TmxObject> elist = tmxmap.ObjectGroups["entity"].Objects;
            List<Entity> entities = new List<Entity>();

            foreach (var entity_obj in elist)
            {
                string type = "bat";
                int chiefId = 0;
                int numberMinion = 0;
                Vector2 entposition = new Vector2((float)entity_obj.X, (float)entity_obj.Y);
                if (entity_obj.Properties.Count > 0 && entity_obj.Properties["type"] != null)
                {
                    type = entity_obj.Properties["type"];
                    if (type == "chief")
                    {
                        chiefId = Int32.Parse(entity_obj.Properties["id"]);
                        numberMinion = Int32.Parse(entity_obj.Properties["numberMinion"]);
                    }
                }
                if (type == "chief")
                    entities.Add(new Entity(entposition, type, chiefId, numberMinion));
                else
                    entities.Add(new Entity(entposition, type));
            }
            return entities;
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
