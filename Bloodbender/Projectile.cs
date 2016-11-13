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

    public class Projectile : PhysicObj
    {
        public Projectile(Vector2 position, float angle, float speed, uint animNbr = 1) : base(position, animNbr)
        {
            float pixelToMeter = Bloodbender.ptr.pixelToMeter;
            body = BodyFactory.CreateRectangle(Bloodbender.ptr.world, 10 * pixelToMeter, 10 * pixelToMeter, 1);
            body.BodyType = BodyType.Dynamic;
            body.Position = position * pixelToMeter;
            body.LinearVelocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            body.LinearVelocity *= speed * pixelToMeter;
            body.FixedRotation = true;
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
