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
    public class Enemy : PhysicObj
    {
        Fixture playerBoundsFix;

        public Enemy(Vector2 position, Player player) : base(position, PathFinderNodeType.CENTER)
        {
            height = 32;

            Animation anim = new Animation(Bloodbender.ptr.Content.Load<Texture2D>("ennemy1"), 8, 0.1f, 32, 0, 0, 0);

            anim.reset();
            addAnimation(anim);

            Bloodbender.ptr.shadowsRendering.addShadow(new Shadow(this));

            velocity = 50;

            playerBoundsFix = createOctogoneFixture(32f, 32f, Vector2.Zero, new AdditionalFixtureData(this, HitboxType.BOUND));
            Radius = 32f;
            //add method to be called on collision, different denpending of fixture
            addFixtureToCheckedCollision(playerBoundsFix);

            IComponent comp = new FollowBehaviorComponent(this, player, 80);
            addComponent(comp);
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
            body.FixtureList[0].IgnoreCollisionWith(proj.body.FixtureList[0]);
            Bloodbender.ptr.listGraphicObj.Add(proj);
        }
    }
}
