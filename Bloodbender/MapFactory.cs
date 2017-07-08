using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using MapGenerator;
using Microsoft.Xna.Framework;

namespace Bloodbender
{
    public class MapFactory
    {
        MapGeneration mGen = new MapGeneration();

        public void newMap()
        {
            mGen.newMap();
            foreach (Room room in mGen.rooms)
            {
                int objIndex = 0;
                MapBound mapBound = new MapBound();
                foreach (Wall wall in room.wallList)
                {
                    // Debug.WriteLine("{0} => {1}/{2} {3}/{4}", wall.objIndex, wall.ptA.X, wall.ptA.Y, wall.ptB.X, wall.ptB.Y);
                    if (wall.objIndex != objIndex)
                    {
                        mapBound.finilizeMap();
                        if (wall.objIndex != objIndex)
                            mapBound = new MapBound();
                        objIndex++;
                    }
                    mapBound.addVertex(wall.ptA, wall.ptB);
                }
                mapBound.finilizeMap();
            }
        }
    }
}
