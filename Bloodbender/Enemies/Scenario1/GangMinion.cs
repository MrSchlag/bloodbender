using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using Bloodbender.Projectiles;

namespace Bloodbender.Enemies.Scenario1
{
    class GangMinion : Enemy
    {
        GangChef chef;
        FollowBehaviorComponent followBehavior;

        bool goneMad = false;

        static float refTimeRdnRun = 1;
        float timerRdnRun = refTimeRdnRun;

        public GangMinion(Vector2 position, GangChef chef, PhysicObj target) : base(position, target)
        {
            height = 0;
            lifePoints = 2;

            Animation anim = new Animation(Bloodbender.ptr.Content.Load<Texture2D>("GangMinion/goblinrun"), 5, 0.1f, 0, 0, 0, 0);
            anim.reset();
            addAnimation(anim);

            Animation attackAnimation = new Animation(Bloodbender.ptr.Content.Load<Texture2D>("GangMinion/goblinattack"), 7, 0.1f, 0, 0, 0, 0);
            attackAnimation.isLooping = false;
            addAnimation(attackAnimation);

            Animation scaredAnimation = new Animation(Bloodbender.ptr.Content.Load<Texture2D>("GangMinion/goblinrunscream"), 5, 0.1f, 0, 0, 0, 0);
            //scaredAnimation.isLooping = false;
            addAnimation(scaredAnimation);

            Bloodbender.ptr.shadowsRendering.addShadow(new Shadow(this));

            scale = new Vector2(1.35f, 1.35f);

            this.chef = chef;

            velocity = 35;
            fixture = createOctogoneFixture(40f, 40f, Vector2.Zero, new AdditionalFixtureData(this, HitboxType.BOUND));
            Radius = 50f;
            //add method to be called on collision, different denpending of fixture
            addFixtureToCheckedCollision(fixture);

            fixture.OnCollision += Collision;

            followBehavior = new FollowBehaviorComponent(this, chef.node, 3);
            addComponent(followBehavior);

            distanceAttackWithTarget = 250;

            canAttack = true;
            canGenerateProjectile = false;
            canBeHitByPlayer = false;
            canBeHitByProjectile = false;

            bloodSpawner.scaleRef = 1.25f;
        }

        private bool Collision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            AdditionalFixtureData additionalFixtureData = (AdditionalFixtureData)fixtureB.UserData;
            if (additionalFixtureData != null)
            {
                if (additionalFixtureData.physicParent is LanceGobelin)
                    return false;
            }
            
            //else if
            return true;

        }

        protected override void Attack()
        {
            Projectile proj = new LanceGobelin(position, 10,angleWith(chef.target), 400f);
            Bloodbender.ptr.listGraphicObj.Add(proj);

            base.Attack();
        }

        public override bool Update(float elapsed)
        {
            /*
            if (body.LinearVelocity.X > 0)
                spriteEffect = SpriteEffects.None;
            else if (body.LinearVelocity.X < 0)
                spriteEffect = SpriteEffects.FlipHorizontally;
                */

            if (goneMad)
            {
                if (timerRdnRun > refTimeRdnRun)
                {
                    timerRdnRun = 0;
                    float runSpeedX = Bloodbender.ptr.rdn.Next(0, 50) * Bloodbender.pixelToMeter;
                    float runSpeedY = Bloodbender.ptr.rdn.Next(0, 50) * Bloodbender.pixelToMeter;

                    if (Bloodbender.ptr.rdn.Next(0, 2) == 0)
                        runSpeedX *= -1;
                    if (Bloodbender.ptr.rdn.Next(0, 2) == 0)
                        runSpeedY *= -1;
                    body.LinearVelocity = new Vector2(runSpeedX, runSpeedY);
                    if (runSpeedY > 0)
                        spriteEffect = SpriteEffects.None;
                    else if (runSpeedY < 0)
                        spriteEffect = SpriteEffects.FlipHorizontally;
                }
                else
                    timerRdnRun += elapsed;
                return base.Update(elapsed);
            }

            if (!chef.shouldDie)
            {
                RadianAngle tmpangle = angleWith(chef.target);
                tmpangle -= (float)Math.PI / 2;

                if (tmpangle < 0)
                    spriteEffect = SpriteEffects.None;
                else if (tmpangle > 0)
                    spriteEffect = SpriteEffects.FlipHorizontally;
            }
            else
            {
                followBehavior.paused = true;
                canAttack = false;
                runAnimation(2);
                goneMad = true;
                runDefaultAnim = false;
                canBeHitByPlayer = true;
            }

            return base.Update(elapsed);
        }
    }
}
