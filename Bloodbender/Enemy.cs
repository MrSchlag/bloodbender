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
        PhysicObj target;
        float attackRate = 1.5f;
        float timerAttack = 0;

        public Enemy(Vector2 position, Player player) : base(position, PathFinderNodeType.CENTER)
        {
            height = 32;

            Animation anim = new Animation(Bloodbender.ptr.Content.Load<Texture2D>("Ennemy1/ennemy1"), 8, 0.1f, 32, 0, 0, 0);
            anim.reset();
            addAnimation(anim);

            Animation attackAnimation = new Animation(Bloodbender.ptr.Content.Load<Texture2D>("Ennemy1/ennemy1attack"), 6, 0.1f, 32, 0, 0, 0);
            attackAnimation.isLooping = false;
            addAnimation(attackAnimation);

            Bloodbender.ptr.shadowsRendering.addShadow(new Shadow(this));

            velocity = 50;

            playerBoundsFix = createOctogoneFixture(32f, 32f, Vector2.Zero, new AdditionalFixtureData(this, HitboxType.BOUND));
            Radius = 32f;
            //add method to be called on collision, different denpending of fixture
            addFixtureToCheckedCollision(playerBoundsFix);

            IComponent comp = new FollowBehaviorComponent(this, player, 32);
            addComponent(comp);

            target = player;
        }

        public override bool Update(float elapsed)
        {
            if (timerAttack > 0)
                timerAttack -= elapsed;

            if (distanceWith(target.position) < 40)
            {
                if (timerAttack <= 0)
                {
                    runAnimation(1);
                    timerAttack = attackRate;
                }
                else
                {
                    if (!getAnimation(1).isRunning)
                        runAnimation(0);
                }
            }
            else
                runAnimation(0);

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
