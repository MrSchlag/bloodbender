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
        String spawnRoomFileTop;
        String spawnRoomFileBot;
        String spawnRoomFileLeft;
        String spawnRoomFileRight;
        List<String> roomFilesWithTopBotEntries;
        List<String> roomFilesWithLeftRightEntries;
        String endRoomFileTop;
        String endRoomFileBot;
        String endRoomFileLeft;
        String endRoomFileRight;
        RoomLoader rloader;

        public Random rand { get; set; }
        public int numberOfRooms { get; set; }
        public List<Room> rooms { get; set; }
        public List<RoomLinker> roomLinkers { get; set; }

        public MapGeneration()
        {
            spawnRoomFileTop = "./Maps/roomSpawnTop.tmx";
            spawnRoomFileBot = "./Maps/roomSpawnBot.tmx";
            spawnRoomFileLeft = "./Maps/roomSpawnLeft.tmx";
            spawnRoomFileRight = "./Maps/roomSpawnRight.tmx";
            roomFilesWithTopBotEntries = new List<String>(new string[]
            {
                "./Maps/room1.tmx",
                "./Maps/room2.tmx",
                "./Maps/room3.tmx",
                "./Maps/room4.tmx",
                "./Maps/room5.tmx",
            });
            roomFilesWithLeftRightEntries = new List<String>(new string[]
            {
            });
            endRoomFileTop = "./Maps/roomEndTop.tmx";
            endRoomFileBot = "./Maps/roomEndBot.tmx";
            endRoomFileLeft = "./Maps/roomEndLeft.tmx";
            endRoomFileRight = "./Maps/roomEndRight.tmx";
        }

        public void newMap()
        {
            int seed = Guid.NewGuid().GetHashCode();
            rand = new Random(seed);
            rloader = new RoomLoader();
            //numberOfRooms = rand.Next(3, 6);
            //MINIMUM 3
            numberOfRooms = 8;
            rooms = new List<Room>();
            roomLinkers = new List<RoomLinker>(); 
            this.addRoomToMap(selectSpawn(), 0, 0);
            for (int i = 0; i < numberOfRooms - 2; i++)
                this.addRandomRoom();
            this.addEndRoom();
            this.saveMap(seed);
            // this.visualizeMap();
        }

        public Room selectSpawn()
        {
            Room spawn;
            //int i = rand.Next(0, 4);
            //if (i == 0)
            //    spawn = rloader.load(spawnRoomFileTop);
            //else if (i == 1)
            //    spawn = rloader.load(spawnRoomFileBot);
            //else if (i == 2)
            //    spawn = rloader.load(spawnRoomFileTop);
            //else
            //    spawn = rloader.load(spawnRoomFileBot);
            spawn = rloader.load(spawnRoomFileTop);
            spawn.exitSelected = spawn.entryList[0];
            return spawn;
        }

        public void addEndRoom()
        {
            Room end = this.selectEnd();
            Room lastRoom = rooms[rooms.Count - 1];
            if (rooms.Count > 1)
                lastRoom.selectRandomExit(rand);
            positionAndAddNewRoom(end, rooms[rooms.Count - 1]);
        }

        public Room selectEnd()
        {
            if (rooms.Count > 1)
                rooms[rooms.Count - 1].selectRandomExit(rand);
            Room end;
            Debug.WriteLine(rooms[rooms.Count - 1].exitSelected.type);
            if (rooms[rooms.Count - 1].exitSelected.type == entryType.top)
                end = rloader.load(endRoomFileTop);
            else //if (rooms[rooms.Count - 1].exitSelected.type == entryType.bot)
                end = rloader.load(endRoomFileBot);
            //else if (rooms[rooms.Count - 1].exitSelected.type == entryType.left)
            //    end = rloader.load(endRoomFileLeft);
            //else
            //    end = rloader.load(endRoomFileRight);
            //end = rloader.load(endRoomFileLeft);
            end.entrySelected = end.entryList[0];
            return end;
        }

        public void addRandomRoom()
        {
            Room lastRoom = rooms[rooms.Count - 1];
            int entryIndex = rand.Next(0, lastRoom.entryList.Count);
            entryType opEntryType = entryType.undefined;
            if (rooms.Count > 1)
                lastRoom.selectRandomExit(rand);
            opEntryType = lastRoom.exitSelected.findOppositeEntryType();
            Room newRoom = findRandomRoomWithEntryType(opEntryType);
            if (newRoom == null)
                return;
            this.positionAndAddNewRoom(newRoom, lastRoom);
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
                // Debug.WriteLine(roomsFilesSelected[roomIndex]);
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

        public void positionAndAddNewRoom(Room newRoom, Room lastRoom)
        {
            float translateY = 0;
            float translateX = 0;
            float randomTranslate; 
            int i = 0;
            if (lastRoom.exitSelected.type == entryType.top || lastRoom.exitSelected.type == entryType.bot)
            {
                randomTranslate = rand.Next(-40, 40);
                translateX = lastRoom.exitSelected.ptA.X + (randomTranslate * lastRoom.tileSize) - newRoom.entrySelected.ptA.X;
                foreach (Room room in rooms)
                {
                    if (i > 0)
                        translateY += ((room.Y + 5) * room.tileSize);
                    translateY += 5 * room.tileSize;
                    i++;
                }
                translateY += (newRoom.Y) * newRoom.tileSize;
                if (lastRoom.exitSelected.type == entryType.top)
                   this.addRoomToMap(newRoom, translateX, -translateY, lastRoom.exitSelected);
                else if (lastRoom.exitSelected.type == entryType.bot)
                {
                    if (numberOfRooms != 2 && rooms.Count < numberOfRooms - 1)
                        translateY -= (rooms[0].Y + 8) * rooms[0].tileSize;
                    this.addRoomToMap(newRoom, translateX, translateY, lastRoom.exitSelected);
                }  
            } else if (lastRoom.exitSelected.type == entryType.left || lastRoom.exitSelected.type == entryType.right)
            {
                randomTranslate = rand.Next(-20, 20);
                translateY = lastRoom.exitSelected.ptA.Y + (randomTranslate * lastRoom.tileSize) - newRoom.entrySelected.ptA.Y;
                foreach (Room room in rooms)
                {
                    if (i > 0)
                        translateX += ((room.X + 5) * room.tileSize);
                    translateX += 5 * room.tileSize;
                    i++;
                }
                translateX += (newRoom.X) * newRoom.tileSize;
                if (lastRoom.exitSelected.type == entryType.right)
                    this.addRoomToMap(newRoom, translateX, translateY, lastRoom.exitSelected);
                else
                {
                    if (numberOfRooms != 2 && rooms.Count < numberOfRooms - 1)
                        translateX -= (rooms[0].X + 8) * rooms[0].tileSize;
                    this.addRoomToMap(newRoom, -translateX, translateY, lastRoom.exitSelected);
                }
                
                    
            }
        }

        public void addRoomToMap(Room room, float xTrans, float yTrans, Entry exit = null)
        {
            if (room != null)
            {
                if (xTrans != 0  || yTrans != 0)
                {
                    // Debug.WriteLine("translation {0}/{1}", xTrans, yTrans);
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
                    foreach (Entity entity in room.entityList)
                    {
                        entity.position = Vector2.Transform(entity.position, Matrix.CreateTranslation(xTrans, yTrans, 0));
                    }
                    if (room.spawnPoint.X != 0 && room.spawnPoint.Y != 0)
                        room.spawnPoint = Vector2.Transform(room.spawnPoint, Matrix.CreateTranslation(xTrans, yTrans, 0));
                }
                rooms.Add(room);
                if (exit != null)
                {
                    RoomLinker roomLinker = new RoomLinker(exit, room.entrySelected);
                    roomLinkers.Add(roomLinker);
                }
            }
        }

        public void saveMap(int seed)
        {
            string path = "./Maps/save.txt";
            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("// Map seeds savings");
                    sw.Close();
                }
            }
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(seed.ToString());
                sw.Close();
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
