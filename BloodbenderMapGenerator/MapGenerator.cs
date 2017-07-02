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
        public void newMap()
        {
            RoomLoader rloader = new RoomLoader();
            Room room1 = rloader.load("../../map/room1.tmx");
            Room room2 = rloader.load("../../map/room1.tmx");
            Room room3 = rloader.load("../../map/room1.tmx");
            Room room4 = rloader.load("../../map/room1.tmx");
            foreach (var wall in room2.wallList)
            {
                wall.ptA = Vector2.Transform(wall.ptA, Matrix.CreateTranslation(0, (room2.Y + 3) * room2.tileSize, 0));
                wall.ptB = Vector2.Transform(wall.ptB, Matrix.CreateTranslation(0, (room2.Y + 3) * room2.tileSize, 0));
            }
            foreach (var wall in room3.wallList)
            {
                wall.ptA = Vector2.Transform(wall.ptA, Matrix.CreateTranslation((room3.X + 3) * room3.tileSize, 0, 0));
                wall.ptB = Vector2.Transform(wall.ptB, Matrix.CreateTranslation((room3.X + 3) * room3.tileSize, 0, 0));
            }
            foreach (var wall in room4.wallList)
            {
                wall.ptA = Vector2.Transform(wall.ptA, Matrix.CreateTranslation((room4.X + 3) * room4.tileSize, (room4.Y + 3) * room4.tileSize, 0));
                wall.ptB = Vector2.Transform(wall.ptB, Matrix.CreateTranslation((room4.X + 3) * room4.tileSize, (room4.Y + 3) * room4.tileSize, 0));
            }
            List<Wall> wallList = new List<Wall>();

            foreach (var wall in room1.wallList)
            {
                wallList.Add(wall);
            }

            foreach (var wall in room2.wallList)
            {
                wallList.Add(wall);
            }
            foreach (var wall in room3.wallList)
            {
                wallList.Add(wall);
            }

            foreach (var wall in room4.wallList)
            {
                wallList.Add(wall);
            }
            visualizeMap(wallList);
        }

        public void visualizeMap(List<Wall> wallList)
        {
            Visualizer visualizer = new Visualizer();
            visualizer.visualizeMap(wallList);
        }
        
    }
}
