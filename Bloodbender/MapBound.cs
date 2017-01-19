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

namespace Bloodbender
{
    public class MapBound : PhysicObj
    {
        Vertices mapVertices;
        public MapBound() : base(new Vector2(0, 0))
        {
            mapVertices = new Vertices();
            body.BodyType = BodyType.Static;
            body.Position = new Vector2(0, 0);
            lenght = 100;
        }

        public void addVertex(Vector2 vertex)
        {
            vertex *= Bloodbender.pixelToMeter;
            mapVertices.Add(vertex);
        }

        public void finiliezMap()
        {
            ChainShape shape = new ChainShape(mapVertices, true);
            Fixture shapeFix = body.CreateFixture(shape);
            shapeFix.UserData = new AdditionalFixtureData(this, HitboxType.BOUND);
            addFixtureToCheckedCollision(shapeFix);
        }
    }
}
