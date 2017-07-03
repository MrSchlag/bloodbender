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

        List<Room> rooms = new List<Room>();
        List<RoomLinker> roomLinkers = new List<RoomLinker>();

        public void newMap()
        {
            RoomLoader rloader = new RoomLoader();
            Room room1 = rloader.load("../../map/room1.tmx");
            Room room2 = rloader.load("../../map/room1.tmx");
            Room room3 = rloader.load("../../map/room1.tmx");
            Room room4 = rloader.load("../../map/room1.tmx");
            addRoom(room1, 0, 0);
            addRoom(room2, 0, (room2.Y + 3) * room2.tileSize);
            addRoom(room3, (room3.X + 3) * room3.tileSize, 0);
            addRoom(room4, (room4.X + 3) * room4.tileSize, (room4.Y + 3) * room4.tileSize);
            List<Wall> wallList = new List<Wall>();

            foreach(Room room in rooms)
            {
                foreach (var wall in room.wallList)
                {
                    wallList.Add(wall);
                }
            }
            visualizeMap(wallList);
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
        public void visualizeMap(List<Wall> wallList)
        {
            Visualizer visualizer = new Visualizer();
            visualizer.visualizeMap(wallList);
        }
        
    }
}
