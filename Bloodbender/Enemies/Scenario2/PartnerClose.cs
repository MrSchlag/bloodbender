using System;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Bloodbender.Projectiles;

namespace Bloodbender.Enemies.Scenario2
{
    public class PartnerClose : Enemy
    {
        public PartnerFar Partner { get; set; }

        public PartnerClose(Vector2 position, PhysicObj target) : base(position, target)
        {
            height = 0;
            
            Animation anim = new Animation(Bloodbender.ptr.Content.Load<Texture2D>("Pair/coursePair1"), 7, 0.1f, 0, 0, 0, 0);
            anim.reset();
            addAnimation(anim);

            Animation attackAnimation = new Animation(Bloodbender.ptr.Content.Load<Texture2D>("Pair/attackPair1"), 9, 0.075f, 0, 0, 0, 0);
            attackAnimation.isLooping = false;
            addAnimation(attackAnimation);

            Bloodbender.ptr.shadowsRendering.addShadow(new Shadow(this));

            fixture = createOctogoneFixture(50f, 50f, Vector2.Zero, new AdditionalFixtureData(this, HitboxType.BOUND));
            Radius = 50f;
            velocity = 30;

            fixture.OnCollision += Collision;

            addFixtureToCheckedCollision(fixture);

            IComponent comp = new FollowBehaviorComponent(this, target, 40f);
            addComponent(comp);

            distanceAttackWithTarget = 50;
            lifePoints = 2;
            canAttack = true;
            canGenerateProjectile = false;
            canBeHitByPlayer = false;
            canBeHitByProjectile = true;
        }

        public override bool Update(float elapsed)
        {
            return base.Update(elapsed);
        }

        private bool Collision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            AdditionalFixtureData additionalFixtureData = (AdditionalFixtureData)fixtureB.UserData;
            if (additionalFixtureData != null)
            {
                if (additionalFixtureData.physicParent is Blood)
                {
                    lifePoints -= 1;
                    bloodSpawner.numberParticuleToPop += 1;
                    bloodSpawner.canSpawn = true;
                }
                else if (additionalFixtureData.physicParent is LanceGobelin)
                    return false;
            }
            return true;

        }
    }
}
