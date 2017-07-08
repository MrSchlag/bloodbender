using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using MapGenerator;

namespace MapGeneratorTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Debug.WriteLine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            MapGeneration mGen = new MapGeneration();
            mGen.newMap();

            foreach (Room room in mGen.rooms)
            {
                int objIndex = 0;
                foreach (Wall wall in room.wallList)
                {
                    Debug.WriteLine("{0} => {1}/{2} {3}/{4}", wall.objIndex, wall.ptA.X, wall.ptA.Y, wall.ptB.X, wall.ptB.Y);
                    if (wall.objIndex == objIndex)
                    {

                    } else if (wall.objIndex != objIndex)
                    {
                        objIndex++;
                    }
                }
            }
        }
    }
}
