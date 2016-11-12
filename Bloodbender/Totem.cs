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
    public class Totem : PhysicObj
    {
        enum hitboxType
        {
            BOUND = 0
        }
        public Totem(Vector2 position, uint animNbr = 1) : base(position, animNbr)
        {
            float pixelToMeter = Bloodbender.ptr.pixelToMeter;
            Vertices rectangleVertices = PolygonTools.CreateRectangle(50 * pixelToMeter, 50 * pixelToMeter);
            PolygonShape totemBounds = new PolygonShape(rectangleVertices, 1);
            Fixture totemBoundsFix = body.CreateFixture(totemBounds, new HitboxData(this, (uint)hitboxType.BOUND));
            body.BodyType = BodyType.Static;
        }

        public override bool Update(float elapsed)
        {
            return base.Update(elapsed);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
