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
        public Totem(Vector2 position) : base(position)
        {
            Bloodbender.ptr.shadowsRendering.addShadow(new Shadow(this));

            float pixelToMeter = Bloodbender.pixelToMeter;
            Vertices rectangleVertices = PolygonTools.CreateRectangle(50 * pixelToMeter, 50 * pixelToMeter);
            PolygonShape totemBounds = new PolygonShape(rectangleVertices, 1);
            Fixture totemBoundsFix = body.CreateFixture(totemBounds, new AdditionalFixtureData(this, HitboxType.BOUND));
            addFixtureToCheckedCollision(totemBoundsFix);
            body.BodyType = BodyType.Static;
            height = 00;
        }

        public override bool Update(float elapsed)
        {
            return base.Update(elapsed);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
        
        public void generateProjectile(float angle)
        {
            //System.Diagnostics.Debug.WriteLine("Totem touched by playerattacksensor");
            Projectile proj = new Projectile(body.Position * Bloodbender.meterToPixel, angle, 400f);
            proj.addAnimation(new Animation(Bloodbender.ptr.bouleRouge));
            body.FixtureList[0].IgnoreCollisionWith(proj.body.FixtureList[0]);
            Bloodbender.ptr.listGraphicObj.Add(proj);
        }
    }
}
