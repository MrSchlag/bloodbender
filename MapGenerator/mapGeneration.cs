﻿using Microsoft.Xna.Framework;
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
                "./Maps/roomSpawn1.tmx",
            });
            // spawn rooms with entry left or right
            spawnRoomFilesWithLeftRightEntries = new List<String>(new string[]
            {
            });
            // rooms that goes from bot to top (or opposite)
            roomFilesWithTopBotEntries = new List<String>(new string[]
            {
                "./Maps/room1.tmx",
                "./Maps/room2.tmx",
                "./Maps/room3.tmx",
                "./Maps/room4.tmx",
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
            int seed = Guid.NewGuid().GetHashCode();
            rand = new Random(seed);
            rloader = new RoomLoader();
            numberOfRooms = rand.Next(3, 5);
            // numberOfRooms = 3;
            rooms = new List<Room>();
            roomLinkers = new List<RoomLinker>(); 
            this.addRoomToMap(selectRandomSpawn(), 0, 0);
            for (int i = 2; i <= numberOfRooms; i++)
            {
                this.addRandomRoom();
            }
            this.saveMap(seed);
            // this.visualizeMap();
        }

        public Room selectRandomSpawn()
        {
            int spawnIndex = rand.Next(0, spawnRoomFilesWithTopBotEntries.Count);
            // Debug.WriteLine("SPAWN INDEX" + spawnIndex + " " + spawnRoomFilesWithTopBotEntries.Count);
            Room spawn = rloader.load(spawnRoomFilesWithTopBotEntries[spawnIndex]);
            spawn.exitSelected = spawn.entryList[0];
            return spawn;
        }

        public void addRandomRoom()
        {
            Room lastRoom = rooms[rooms.Count - 1];
            int entryIndex = rand.Next(0, lastRoom.entryList.Count);
            // Debug.WriteLine("ROOM INDEX " + entryIndex + " " + lastRoom.entryList.Count);
            entryType opEntryType = entryType.undefined;
            if (this.rooms.Count > 1)
                lastRoom.selectRandomExit(rand);
            opEntryType = lastRoom.exitSelected.findOppositeEntryType();
            Room newRoom = this.findRandomRoomWithEntryType(opEntryType);
            if (newRoom == null)
                return;
            this.positionNewRoom(newRoom, lastRoom);
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
                // Debug.WriteLine("ROOM SELECTED PATH " + roomsFilesSelected[roomIndex]);
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

        public void positionNewRoom(Room newRoom, Room lastRoom)
        {
            // Debug.WriteLine(entry.ptA.X + "/" + entry.ptA.Y + " - " + entry.ptB.X + "/" + entry.ptB.Y);
            // Debug.WriteLine(newRoom.entrySelected.ptA.X + " " + newRoom.entrySelected.ptA.Y);

            float randomTranslate = rand.Next(-40, 40);
            // float randomTranslate = 0;
            int i = 0;
            if (lastRoom.exitSelected.type == entryType.top || newRoom.entrySelected.type == entryType.bot)
            {
                float translateY = 0;
                float translateX = 0;
                translateX = lastRoom.exitSelected.ptA.X + (randomTranslate * lastRoom.tileSize) - newRoom.entrySelected.ptA.X;
                foreach (Room room in this.rooms)
                {
                    if (i > 0)
                        translateY += ((room.Y + 5) * room.tileSize);
                    translateY += 5 * room.tileSize;
                    i++;
                }
                translateY += (newRoom.Y) * newRoom.tileSize;
                // Debug.WriteLine((newRoom.Y) * newRoom.tileSize + " " + (newRoom.Y + 10) * newRoom.tileSize, translateY);
                // Debug.WriteLine("EXIT TYPE " + lastRoom.exitSelected.type);
                if (lastRoom.exitSelected.type == entryType.top)
                   this.addRoomToMap(newRoom, translateX, -translateY, lastRoom.exitSelected);
                else if (lastRoom.exitSelected.type == entryType.bot)
                   this.addRoomToMap(newRoom, translateX, translateY, lastRoom.exitSelected);
            }
            else if (lastRoom.exitSelected.type == entryType.left || lastRoom.exitSelected.type == entryType.right)
            {
                float translateY = 0;
                float translateX = 0;
                translateY = lastRoom.exitSelected.ptA.Y - newRoom.entrySelected.ptA.Y;
                foreach (Room room in this.rooms)
                {
                    if (i > 0)
                        translateX += room.X * room.tileSize;
                    i++;
                }
                if (lastRoom.exitSelected.type == entryType.left)
                    this.addRoomToMap(newRoom, -translateX, translateY, lastRoom.exitSelected);
                else if (lastRoom.exitSelected.type == entryType.right)
                    this.addRoomToMap(newRoom, translateX, translateY, lastRoom.exitSelected);
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
                    foreach (Entities entity in room.entityList)
                    {
                        entity.ptA = Vector2.Transform(entity.ptA, Matrix.CreateTranslation(xTrans, yTrans, 0));
                        entity.ptB = Vector2.Transform(entity.ptB, Matrix.CreateTranslation(xTrans, yTrans, 0));
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
