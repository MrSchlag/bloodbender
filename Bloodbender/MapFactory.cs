using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using MapGenerator;
using Microsoft.Xna.Framework;
using Bloodbender.Enemies.Scenario3;
using Bloodbender.Enemies.Scenario1;

namespace Bloodbender
{
    public class MapFactory
    {
        public MapGeneration mGen = new MapGeneration();
        public List<KeyValuePair<Room, MapBound>> MapBoundDict;


        public float minX { get; set; }
        public float minY { get; set; }
        public float maxX { get; set; }
        public float maxY { get; set; }

        public Player player { get; set; }

        public void newMap(List<GraphicObj> listGraphicObj)
        {
            MapBoundDict = new List<KeyValuePair<Room, MapBound>>();
            minX = 0;
            minY = 0;
            maxX = 0;
            maxY = 0;
            player = null;
            mGen.newMap();
            this.loadRoomWalls();
            //this.loadRoomLinkers();
            this.loadPlayer(listGraphicObj);
            this.loadEntities(listGraphicObj);
        }

        public void loadRoomWalls()
        {
            int i = 0;
            foreach (Room room in mGen.rooms)
            {
                int indexWall = 0;
                int objIndex = 0;
                MapBound mapBound = new MapBound();
                
                MapBoundDict.Add(new KeyValuePair<Room, MapBound>(room, mapBound));
                foreach (Wall wall in room.wallList)
                {
                    i++;
                    Debug.WriteLine("{0}/{1} - {2}/{3} => {4} {5}", wall.ptA.X, wall.ptA.Y, wall.ptB.X, wall.ptB.Y, objIndex, i);
                    this.updateMinMaxMap(wall);
                    this.updateMinMaxRoom(room, wall, indexWall == 0);
                    if (wall.objIndex != objIndex)
                    {
                        mapBound.finilizeMap();
                        if (wall.objIndex != objIndex)
                        {
                            mapBound = new MapBound();
                            MapBoundDict.Add(new KeyValuePair<Room, MapBound>(room, mapBound));
                        }
                            objIndex++;
                    }
                    mapBound.addVertex(wall.ptA, wall.ptB);
                    indexWall++;
                }
                mapBound.finilizeMap();
                // Debug.WriteLine("ROOM {0}/{1} - {2}/{3}", room.minX, room.minY, room.maxX, room.maxY);
            }
            // Debug.WriteLine("MAP {0}/{1} - {2}/{3}", minX, minY, maxX, maxY);
        }

        public void loadRoomLinkers()
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

        public void loadPlayer(List<GraphicObj> listGraphicObj)
        {
            this.player = new Player(mGen.rooms[0].spawnPoint);
            player.setLinearDamping(10);
            listGraphicObj.Add(player);
            Bloodbender.ptr.player = player;
        }

        public void loadEntities(List<GraphicObj> listGraphicObj)
        {
            foreach (Room room in mGen.rooms)
            {
                foreach (Entity entity in room.entityList)
                {
                    //Debug.WriteLine("{0} {1} {2}", entity.type, entity.chiefId, entity.numberMinion);
                    if (entity.type == "totem")
                    {
                        listGraphicObj.Add(new Totem(entity.position));
                    } else if (entity.type == "chief") {
                        listGraphicObj.Add(new GangChef(entity.numberMinion, entity.position, player));
                    } else {
                        listGraphicObj.Add(new Bat(entity.position, player));
                    } 
                }
            }
        }

        public void updateMinMaxMap(Wall wall)
        {
            if (wall.ptA.X > maxX)
                maxX = wall.ptA.X;
            if (wall.ptA.X < minX)
                minX = wall.ptA.X;
            if (wall.ptB.X > maxX)
                maxX = wall.ptB.X;
            if (wall.ptB.X < minX)
                minX = wall.ptB.X;
            if (wall.ptA.Y > maxY)
                maxY = wall.ptA.Y;
            if (wall.ptA.Y < minY)
                minY = wall.ptA.Y;
            if (wall.ptB.Y > maxY)
                maxY = wall.ptB.Y;
            if (wall.ptB.Y < minY)
                minY = wall.ptB.Y;
        }

        public void updateMinMaxRoom(Room room, Wall wall, Boolean firstUpdate)
        {
            if (firstUpdate)
            {
                room.maxX = wall.ptA.X;
                room.minX = wall.ptA.X;
                room.maxX = wall.ptB.X;
                room.minX = wall.ptB.X;
                room.maxY = wall.ptA.Y;
                room.minY = wall.ptA.Y;
                room.maxY = wall.ptB.Y;
                room.minY = wall.ptB.Y;
            } else {
                if (wall.ptA.X > room.maxX)
                    room.maxX = wall.ptA.X;
                if (wall.ptA.X < room.minX)
                    room.minX = wall.ptA.X;
                if (wall.ptB.X > room.maxX)
                    room.maxX = wall.ptB.X;
                if (wall.ptB.X < room.minX)
                    room.minX = wall.ptB.X;
                if (wall.ptA.Y > room.maxY)
                    room.maxY = wall.ptA.Y;
                if (wall.ptA.Y < room.minY)
                    room.minY = wall.ptA.Y;
                if (wall.ptB.Y > room.maxY)
                    room.maxY = wall.ptB.Y;
                if (wall.ptB.Y < room.minY)
                    room.minY = wall.ptB.Y;
            }
        }

        public void CreateMeshForRoom(Room room)
        {
            foreach (var pairRoomBound in MapBoundDict)
            {
                if (pairRoomBound.Key == room)
                {
                    pairRoomBound.Value.createPathFinderNodes();
                }
            }
        }
    }
}
