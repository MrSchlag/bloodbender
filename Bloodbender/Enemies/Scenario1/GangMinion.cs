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
        public GangMinion(Vector2 position, GangChef chef, PhysicObj target) : base(position, target)
        {
            height = 0;

            Animation anim = new Animation(Bloodbender.ptr.Content.Load<Texture2D>("GangMinion/goblinrun"), 5, 0.1f, 0, 0, 0, 0);
            anim.reset();
            addAnimation(anim);

            Animation attackAnimation = new Animation(Bloodbender.ptr.Content.Load<Texture2D>("GangMinion/goblinattack"), 7, 0.1f, 0, 0, 0, 0);
            attackAnimation.isLooping = false;
            addAnimation(attackAnimation);

            Bloodbender.ptr.shadowsRendering.addShadow(new Shadow(this));

            scale = new Vector2(1.35f, 1.35f);

            this.chef = chef;

            velocity = 35;
            fixture = createOctogoneFixture(40f, 40f, Vector2.Zero, new AdditionalFixtureData(this, HitboxType.BOUND));
            Radius = 50f;
            //add method to be called on collision, different denpending of fixture
            addFixtureToCheckedCollision(fixture);

            fixture.OnCollision += Collision;

            IComponent comp = new FollowBehaviorComponent(this, chef.node, 3);
            addComponent(comp);

            distanceAttackWithTarget = 250;

            canAttack = true;
            canGenerateProjectile = false;
            canBeHitByPlayer = false;
            canBeHitByProjectile = false;
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

            
            RadianAngle tmpangle = angleWith(chef.target);
            tmpangle -= (float)Math.PI / 2;

            if (tmpangle < 0)
                spriteEffect = SpriteEffects.None;
            else if (tmpangle > 0)
                spriteEffect = SpriteEffects.FlipHorizontally;

            if (chef.shouldDie)
            {
                //go mad
            }

            return base.Update(elapsed);
        }
    }
}
