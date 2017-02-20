using System;
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
        }
    }
}
