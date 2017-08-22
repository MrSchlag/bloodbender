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
        String spawnRoomFileTop = "./Maps/roomSpawnTop.tmx";
        String spawnRoomFileBot = "./Maps/roomSpawnBot.tmx";
        String spawnRoomFileLeft = "./Maps/roomSpawnLeft.tmx";
        String spawnRoomFileRight = "./Maps/roomSpawnRight.tmx";
        List<String> roomFilesWithTopBotEntries = new List<String>(new string[]
        {
            //"./Maps/room1.tmx",
            //"./Maps/room2.tmx",
            "./Maps/room3.tmx",
            //"./Maps/room4.tmx",
            //"./Maps/room5.tmx",
            //"./Maps/room6.tmx",
        });
        List<String> roomFilesWithLeftRightEntries = new List<String>(new string[]
        {
        });
        String endRoomFileTop = "./Maps/roomEndTop.tmx";
        String endRoomFileBot = "./Maps/roomEndBot.tmx";
        String endRoomFileLeft = "./Maps/roomEndLeft.tmx";
        String endRoomFileRight = "./Maps/roomEndRight.tmx";
        String saveFile = "./Maps/save.txt";

        RoomLoader rloader;
        public Random rand { get; set; }
        public int numberOfRooms { get; set; }
        public List<Room> rooms { get; set; }
        public List<RoomLinker> roomLinkers { get; set; }
        public List<int> savedSeeds { get; set; }

        public MapGeneration()
        {
            importSeedsFromSaveFile();
        }

        public void newMap()
        {
            int seed = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            savedSeeds.Add(seed);
            rand = new Random(seed);
            rloader = new RoomLoader();
            //numberOfRooms = rand.Next(3, 6);
            //MINIMUM 3
            numberOfRooms = 3;
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
            //0 to 4 to rand between all type of spawn
            int i = rand.Next(0, 2);
            if (i == 0)
                spawn = rloader.load(spawnRoomFileTop);
            else if (i == 1)
                spawn = rloader.load(spawnRoomFileBot);
            else if (i == 2)
                spawn = rloader.load(spawnRoomFileLeft);
            else
                spawn = rloader.load(spawnRoomFileRight);
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
            if (rooms[rooms.Count - 1].exitSelected.type == entryType.top)
                end = rloader.load(endRoomFileTop);
            else if (rooms[rooms.Count - 1].exitSelected.type == entryType.bot)
                end = rloader.load(endRoomFileBot);
            else if (rooms[rooms.Count - 1].exitSelected.type == entryType.left)
                end = rloader.load(endRoomFileLeft);
            else
                end = rloader.load(endRoomFileRight);
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
                if (rand.Next(0, 2) == 0)
                    randomTranslate = rand.Next(-30, -5);
                else
                    randomTranslate = rand.Next(5, 30);
                translateX = lastRoom.exitSelected.ptA.X + (randomTranslate * lastRoom.tileSize) - newRoom.entrySelected.ptA.X;
                foreach (Room room in rooms)
                {
                    if (i > 0)
                        translateY += ((room.Y + 7) * room.tileSize);
                    translateY += 7 * room.tileSize;
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
                if (rand.Next(0, 2) == 0)
                    randomTranslate = rand.Next(-20, -5);
                else
                    randomTranslate = rand.Next(5, 20);
                translateY = lastRoom.exitSelected.ptA.Y + (randomTranslate * lastRoom.tileSize) - newRoom.entrySelected.ptA.Y;
                foreach (Room room in rooms)
                {
                    if (i > 0)
                        translateX += ((room.X + 7) * room.tileSize);
                    translateX += 7 * room.tileSize;
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
            if (!File.Exists(saveFile))
            {
                using (StreamWriter sw = File.CreateText(saveFile))
                {
                    sw.WriteLine("// Map seeds savings");
                    sw.Close();
                }
            }
            using (StreamWriter sw = File.AppendText(saveFile))
            {
                sw.WriteLine(seed.ToString());
                sw.Close();
            }
        }

        public void importSeedsFromSaveFile()
        {
            if (File.Exists(saveFile))
            {
                var lines = File.ReadLines(saveFile);
                int i = lines.Count() - 1;
                savedSeeds = new List<int>();
                while (i > 0 && savedSeeds.Count() <= 7)
                {
                    savedSeeds.Add(Int32.Parse(lines.ElementAt(i)));
                    i--;
                }
            }
        }

        public void visualizeMap()
        { 
            List<Wall> wallList = new List<Wall>();
            foreach (Room room in rooms)
            {
                foreach (var wall in room.wallList)
                    wallList.Add(wall);
            }
            Visualizer visualizer = new Visualizer();
            visualizer.visualizeMap(wallList);
        }
    }
}
