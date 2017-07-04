using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BloodbenderMapGenerator
{
    public class MapGenerator
    {
        List<String> spawnRoomFiles;
        List<String> roomFiles;
        RoomLoader rloader;

        public Random rand { get; set; }
        public int numberOfRooms { get; set; }
        public List<Room> rooms { get; set; }
        public List<RoomLinker> roomLinkers { get; set; }

        public MapGenerator()
        {
            spawnRoomFiles = new List<String>(new string[]
            {
                "../../map/roomSpawn1.tmx",
            });
            roomFiles = new List<String>(new string[]
            {
                "../../map/room1.tmx",
            });
            rand = new Random();
            rloader = new RoomLoader();
            numberOfRooms = rand.Next(4, 10);
            rooms = new List<Room>();
            roomLinkers = new List<RoomLinker>();
        }

        public void newMap()
        {
            rooms.Add(selectRandomSpawn());
            for (int i = 2; i <= 3/*numberOfRooms*/; i++)
            {
                selectRandomRoom();
            }
            //addRoom(room1, 0, 0);
            //addRoom(room2, 0, (room2.Y + 3) * room2.tileSize);
            visualizeMap();
        }

        public Room selectRandomSpawn()
        {
            int spawnIndex = rand.Next(0, spawnRoomFiles.Count);
            Debug.WriteLine("SPAWN INDEX" + spawnIndex + " " + spawnRoomFiles.Count);
            Room spawn = rloader.load(spawnRoomFiles[spawnIndex]);
            return spawn;
        }

        public void selectRandomRoom()
        {
            Room lastRoom = rooms[rooms.Count - 1];
            int entryIndex = rand.Next(0, lastRoom.entryList.Count);
            Debug.WriteLine("ROOM INDEX " + entryIndex + " " + lastRoom.entryList.Count);
            int oppositEntryIndex = lastRoom.entryList[entryIndex].findOppositeEntryType();
        }

        public void addRoom(Room room, int xTrans, int yTrans)
        {
            if (room != null)
            {
                if (xTrans != 0  || yTrans != 0)
                {
                    foreach(Wall wall in room.wallList)
                    {
                        wall.ptA = Vector2.Transform(wall.ptA, Matrix.CreateTranslation(xTrans, yTrans, 0));
                        wall.ptB = Vector2.Transform(wall.ptB, Matrix.CreateTranslation(xTrans, yTrans, 0));
                    }
                }
                rooms.Add(room);
            }
        }

        public void visualizeMap()
        {
            List<Wall> wallList = new List<Wall>();
            foreach (Room room in rooms)
            {
                foreach (var wall in room.wallList)
                {
                    wallList.Add(wall);
                }
            }
            Visualizer visualizer = new Visualizer();
            visualizer.visualizeMap(wallList);
        }
        
    }
}
