﻿using System;
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
            this.loadRoomLinkers();
            this.loadRoomWalls();
            this.loadPlayer();
            this.loadEntities();
        }

        public void loadRoomLinkers()
        {
            foreach (Room room in mGen.rooms)
            {
                int objIndex = 0;
                MapBound mapBound = new MapBound();
                foreach (Wall wall in room.wallList)
                {
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

        public void loadRoomWalls()
        {
            foreach (RoomLinker roomLinker in mGen.roomLinkers)
            {
                MapBound botMapBound = new MapBound();
                MapBound topMapBound = new MapBound();
                if (roomLinker.botWallList.Count > 0)
                {
                    foreach (Wall wall in roomLinker.botWallList)
                    {
                        // Debug.WriteLine("OH {0}/{1} - {2}/{3}", wall.ptA.X, wall.ptA.Y, wall.ptB.X, wall.ptB.Y);
                        botMapBound.addVertex(wall.ptA, wall.ptB);
                    }
                    botMapBound.finilizeMap();
                }

                if (roomLinker.topWallList.Count > 0)
                {
                    foreach (Wall wall in roomLinker.topWallList)
                    {
                        // Debug.WriteLine("AH {0}/{1} - {2}/{3}", wall.ptA.X, wall.ptA.Y, wall.ptB.X, wall.ptB.Y);
                        topMapBound.addVertex(wall.ptA, wall.ptB);
                    }
                    topMapBound.finilizeMap();
                }
            }
        }

        public void loadPlayer()
        {

        }

        public void loadEntities()
        {

        }
    }
}
