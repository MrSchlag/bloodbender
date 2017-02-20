﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarseerPhysics.Common.PolygonManipulation;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common.TextureTools;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics.Common.Decomposition;

namespace Bloodbender
{
    public class MapBound : PhysicObj
    {
        public Vertices mapVertices;

        public float minX;
        public float maxX;
        public float minY;
        public float maxY;

        public MapBound() : base(new Vector2(0, 0))
        {
            mapVertices = new Vertices();
            body.BodyType = BodyType.Static;
            body.Position = new Vector2(0, 0);
            length = 100;
        }

        public void addVertex(Vector2 vertex)
        {
            if (mapVertices.Count == 0)
            {
                minX = vertex.X;
                maxX = vertex.X;
                minY = vertex.Y;
                maxY = vertex.Y;
            }
            if (minX > vertex.X)
                minX = vertex.X;
            if (maxX < vertex.X)
                maxX = vertex.X;
            if (minY > vertex.Y)
                minY = vertex.Y;
            if (maxY < vertex.Y)
                maxY = vertex.Y;
            vertex *= Bloodbender.pixelToMeter;
            mapVertices.Add(vertex);
        }

        public void finiliezMap()
        {
            ChainShape chainShape = new ChainShape(mapVertices, true);
            Fixture chainShapeFix = body.CreateFixture(chainShape);
            chainShapeFix.UserData = new AdditionalFixtureData(this, HitboxType.BOUND);
            addFixtureToCheckedCollision(chainShapeFix);
            createPathFinderNodes();
        }
        public void createPathFinderNodes()
        {
            float pathNodeOffset = (float)Bloodbender.ptr.pathFinder.pathStep;
            int verticePos = 0;

            foreach (Vector2 vertex in mapVertices)
            {
                Vector2 vertexPx = vertex * Bloodbender.meterToPixel;

                /* création des point qui représent les pathnode */
                Vertices cornerSquare1 = new Vertices();
                cornerSquare1.Add(new Vector2(0, 0));
                Vertices cornerSquare2 = new Vertices();
                cornerSquare2.Add(new Vector2(0, 0));

                /* récupération des vertex suivant et précédents pour la création des vecteur */
                Vector2 nextVertex = getNextVertex(verticePos, true) * Bloodbender.meterToPixel;
                Vector2 prevVertex = getNextVertex(verticePos, false) * Bloodbender.meterToPixel;

                Vector2 a = new Vector2(nextVertex.X - vertexPx.X, nextVertex.Y - vertexPx.Y);
                Vector2 b = new Vector2(prevVertex.X - vertexPx.X, prevVertex.Y - vertexPx.Y);

                /* transaltion des point selon le vecteur rescale à pathNodeOffset */
                cornerSquare1.Translate(a * (pathNodeOffset / a.Length()));
                cornerSquare2.Translate(-a * (pathNodeOffset / a.Length()));

                /* rotation des points */
                float vectorAngle = ((float)Math.Atan2(b.Y, b.X) - (float)Math.Atan2(a.Y, a.X)) / 2.0f;
                cornerSquare1.Rotate(vectorAngle);
                cornerSquare2.Rotate(vectorAngle);

                /* repositionnement des points */
                cornerSquare1.Translate(vertexPx);
                cornerSquare2.Translate(vertexPx);

                /* création des PathNode */
                Bloodbender.ptr.pathFinder.addNode(new PathFinderNode(cornerSquare1[0]));
                Bloodbender.ptr.pathFinder.addNode(new PathFinderNode(cornerSquare2[0]));

                verticePos++;
            }
        }

        private Vector2 getNextVertex(int verticePos, bool next)
        {
            if (next == true)
            {
                if (verticePos + 1 == mapVertices.Count)
                    return mapVertices[0];
                return mapVertices[verticePos + 1];
            }
            else
            {
                if (verticePos == 0)
                    return mapVertices[mapVertices.Count - 1];
                return mapVertices[verticePos - 1];
            } 
        }
    }
}
