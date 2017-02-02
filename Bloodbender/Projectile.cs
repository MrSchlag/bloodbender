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
        public Projectile(Vector2 position, float angle, float speed) : base(position)
        {
            //Bloodbender.ptr.shadowsRendering.addShadow(new Shadow(this));

            float pixelToMeter = Bloodbender.pixelToMeter;
            
            body = BodyFactory.CreateRectangle(Bloodbender.ptr.world, 10 * pixelToMeter, 10 * pixelToMeter, 1);
            //body = BodyFactory.CreateCircle(Bloodbender.ptr.world, 5 * pixelToMeter, 1);
            body.BodyType = BodyType.Dynamic;
            body.Position = position * pixelToMeter;
            body.LinearVelocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            body.LinearVelocity *= speed * pixelToMeter;
            body.FixedRotation = false;
            body.FixtureList[0].CollisionGroup = -1;
            body.Restitution = 0.01f;
            body.FixtureList[0].UserData = new AdditionalFixtureData(this, HitboxType.BOUND);
            addFixtureToCheckedCollision(body.FixtureList[0]);
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
