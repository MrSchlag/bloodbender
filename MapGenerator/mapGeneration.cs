using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MapGenerator
{
    public class MapGeneration
    {
        List<String> spawnRoomFilesWithTopBotEntries;
        List<String> spawnRoomFilesWithLeftRightEntries;
        List<String> roomFilesWithTopBotEntries;
        List<String> roomFilesWithLeftRightEntries;
        List<String> endRoomFilesWithTopBotEntries;
        List<String> endRoomFilesWithLeftRightEntries;
        RoomLoader rloader;

        public Random rand { get; set; }
        public int numberOfRooms { get; set; }
        public List<Room> rooms { get; set; }
        public List<RoomLinker> roomLinkers { get; set; }

        public MapGeneration()
        {
            // spawn rooms with entry top or bot
            spawnRoomFilesWithTopBotEntries = new List<String>(new string[]
            {
                "../../../../map/roomSpawn1.tmx",
            });
            // spawn rooms with entry left or right
            spawnRoomFilesWithLeftRightEntries = new List<String>(new string[]
            {
            });
            // rooms that goes from bot to top (or opposite)
            roomFilesWithTopBotEntries = new List<String>(new string[]
            {
                "../../../../map/room1.tmx",
            });
            // rooms that goes from top to bot (or opposite)
            roomFilesWithLeftRightEntries = new List<String>(new string[]
            {
            });
            // end rooms with entry top or bot
            endRoomFilesWithTopBotEntries = new List<String>(new string[]
            {
            });
            // end rooms with entry left or right
            endRoomFilesWithLeftRightEntries = new List<String>(new string[]
            {
            });
        }

        public void newMap()
        {
            rand = new Random();
            rloader = new RoomLoader();
            numberOfRooms = rand.Next(4, 10);
            rooms = new List<Room>();
            roomLinkers = new List<RoomLinker>(); 
            this.addRoomToList(selectRandomSpawn(), 0, 0);
            for (int i = 2; i <= 2 /*numberOfRooms*/; i++)
            {
                this.addRandomRoom();
            }
            //this.addRoomToList(room1, 0, 0);
            //this.addRoomToList(room2, 0, (room2.Y + 3) * room2.tileSize);
            // this.visualizeMap();
        }

        public Room selectRandomSpawn()
        {
            int spawnIndex = rand.Next(0, spawnRoomFilesWithTopBotEntries.Count);
            // Debug.WriteLine("SPAWN INDEX" + spawnIndex + " " + spawnRoomFilesWithTopBotEntries.Count);
            Room spawn = rloader.load(spawnRoomFilesWithTopBotEntries[spawnIndex]);
            return spawn;
        }

        public void addRandomRoom()
        {
            // Debug.WriteLine("ZOBI");
            Room lastRoom = rooms[rooms.Count - 1];
            int entryIndex = rand.Next(0, lastRoom.entryList.Count);
            // Debug.WriteLine("ROOM INDEX " + entryIndex + " " + lastRoom.entryList.Count);
            entryType opEntryType = lastRoom.entryList[entryIndex].findOppositeEntryType();
            if (opEntryType == entryType.undefined)
                return;
            // Debug.WriteLine(lastRoom.entryList[entryIndex].type + " " + opEntryType);
            Room newRoom = this.findRandomRoomWithEntryType(opEntryType);
            if (newRoom == null)
                return;
            this.positionNewRoom(lastRoom.entryList[entryIndex], newRoom, lastRoom);
        }

        public Room findRandomRoomWithEntryType(entryType type)
        {
            List<String> roomsFilesSelected = null;
            if (type == entryType.top || type == entryType.bot)
                roomsFilesSelected = roomFilesWithTopBotEntries;
            else if (type == entryType.left || type == entryType.right)
                roomsFilesSelected = roomFilesWithLeftRightEntries;
            if (roomsFilesSelected != null)
            {
                int roomIndex = rand.Next(0, roomsFilesSelected.Count);
                Room room = rloader.load(roomsFilesSelected[roomIndex]);
                if (room == null)
                    return null;
                Entry entry = room.findRandomEntryByType(rand, type);
                if (entry == null)
                    return null;
                room.entrySelected = entry;
                return room;
            }
            return null;
        }

        public void positionNewRoom(Entry entry, Room newRoom, Room lastRoom)
        {
            // Debug.WriteLine(entry.ptA.X + "/" + entry.ptA.Y + " - " + entry.ptB.X + "/" + entry.ptB.Y);
            // Debug.WriteLine(newRoom.entrySelected.ptA.X + " " + newRoom.entrySelected.ptA.Y);

            float randomTranslate = rand.Next(0, 4);
            float translateValue = 0;
            if (entry.type == entryType.top || entry.type == entryType.bot)
            {
                // Debug.WriteLine("{0} - {1}", newRoom.entrySelected.ptA.X, entry.ptA.X);
                translateValue = entry.ptA.X - newRoom.entrySelected.ptA.X;
                // this.addRoomToList(newRoom, 0, (lastRoom.Y) * lastRoom.tileSize);

                Debug.WriteLine("translateValue X " + translateValue);
                if (entry.type == entryType.top)
                   this.addRoomToList(newRoom, translateValue, -((lastRoom.Y) * lastRoom.tileSize));
                else if (entry.type == entryType.bot)
                   this.addRoomToList(newRoom, translateValue, ((lastRoom.Y) * lastRoom.tileSize));
            }
            else if (entry.type == entryType.left || entry.type == entryType.right)
            {
                // Debug.WriteLine("yo");
                translateValue = entry.ptA.Y - newRoom.entrySelected.ptA.Y;
                this.addRoomToList(newRoom, (lastRoom.X) * lastRoom.tileSize, 0);
                Debug.WriteLine("translateValue Y " + translateValue);
                if (entry.type == entryType.left)
                    this.addRoomToList(newRoom, -(lastRoom.X) * lastRoom.tileSize, translateValue);
                else if (entry.type == entryType.right)
                    this.addRoomToList(newRoom, (lastRoom.X) * lastRoom.tileSize, translateValue);
            }
        }

        public void addRoomToList(Room room, float xTrans, float yTrans)
        {
            // Debug.WriteLine("madafuc");
            if (room != null)
            {
                if (xTrans != 0  || yTrans != 0)
                {
                    Debug.WriteLine("{0}/{1}", xTrans, yTrans);
                    foreach(Wall wall in room.wallList)
                    {
                        wall.ptA = Vector2.Transform(wall.ptA, Matrix.CreateTranslation(xTrans, yTrans, 0));
                        wall.ptB = Vector2.Transform(wall.ptB, Matrix.CreateTranslation(xTrans, yTrans, 0));
                    }
                    foreach (Entry entry in room.entryList)
                    {
                        entry.ptA = Vector2.Transform(entry.ptA, Matrix.CreateTranslation(xTrans, yTrans, 0));
                        entry.ptB = Vector2.Transform(entry.ptB, Matrix.CreateTranslation(xTrans, yTrans, 0));
                    }
                    foreach (Entities entity in room.entityList)
                    {
                        entity.ptA = Vector2.Transform(entity.ptA, Matrix.CreateTranslation(xTrans, yTrans, 0));
                        entity.ptB = Vector2.Transform(entity.ptB, Matrix.CreateTranslation(xTrans, yTrans, 0));
                    }
                    if (room.spawnPoint.X != 0 && room.spawnPoint.Y != 0)
                        room.spawnPoint = Vector2.Transform(room.spawnPoint, Matrix.CreateTranslation(xTrans, yTrans, 0));
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
