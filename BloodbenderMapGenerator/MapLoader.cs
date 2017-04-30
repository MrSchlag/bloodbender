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
    public class MapLoader
    {
        public MapLoader(){
            Debug.WriteLine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            RoomLoader rloader = new RoomLoader();
            Room room1 = rloader.load("../../map/sans-titre.tmx");
            Room room2 = rloader.load("../../map/sans-titre.tmx");

            foreach (var wall in room2.wallList)
            {
                wall.ptA = Vector2.Transform(wall.ptA, Matrix.CreateTranslation(room1.X * room1.tileSize, 0, 0));
                wall.ptB = Vector2.Transform(wall.ptB, Matrix.CreateTranslation(room1.X * room1.tileSize, 0, 0));
            }
            foreach (var entry in room2.entryList)
            {
                entry.ptA = Vector2.Transform(entry.ptA, Matrix.CreateTranslation(room1.X * room1.tileSize, 0, 0));
                entry.ptB = Vector2.Transform(entry.ptB, Matrix.CreateTranslation(room1.X * room1.tileSize, 0, 0));
            }

            //this.findPotentialPaths(room1, room2);
            this.choosePath();
            this.createPath();
            
            this.translateRoom();
        }


        public void   translateRoom()
        {
        }
        public void findPotentialPaths(ref Room room1, ref Room room2)
        {
            foreach (var entry1 in room1.entryList)
            {
                foreach (var entry2 in room2.entryList)
                {
                    if (entry1.type == entryType.top && entry2.type == entryType.bot)
                    {
                        Debug.WriteLine("top to bot");
                        //Debug.WriteLine("{0}/{1}-{2}/{3}", entry1.ptA, entry1.ptB, entry2.ptA, entry2.ptB);

                    }
                    if (entry1.type == entryType.bot && entry2.type == entryType.top)
                    {
                        Debug.WriteLine("bot to top");
                    }
                    if (entry1.type == entryType.left && entry2.type == entryType.right)
                    {
                        Debug.WriteLine("left to right");
                    }
                    if (entry1.type == entryType.right && entry2.type == entryType.left)
                    {
                        Debug.WriteLine("right to left");
                    }
                    if (entry1.type == entryType.topleftdiag && entry2.type == entryType.botrightdiag)
                    {
                        Debug.WriteLine("topleftdiag to botrightdiag");
                    }
                    if (entry1.type == entryType.botrightdiag && entry2.type == entryType.topleftdiag)
                    {
                        Debug.WriteLine("botrightdiag to topleftdiag");
                    }
                    if (entry1.type == entryType.toprightdiag && entry2.type == entryType.botleftdiag)
                    {
                        Debug.WriteLine("toprightdiag to botleftdiag");
                    }
                    if (entry1.type == entryType.botleftdiag && entry2.type == entryType.toprightdiag)
                    {
                        Debug.WriteLine("botrightdiag to toprightdiag");
                    }
                }
            }
        }

        public void choosePath() { }
        public void createPath() { }
    }
}
