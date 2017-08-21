using Bloodbender.Projectiles;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using Microsoft.Xna.Framework;

namespace Bloodbender.Enemies.Scenario3
{
    public class BadBat : Bat
    {
        public BadBat(Vector2 position, PhysicObj target) : base(position, target)
        {
            velocity = 70;
            canBeHitByPlayer = false;
            canBeHitByProjectile = true;
            canGenerateProjectile = false;
            canAttack = true;
            fixture.OnCollision += Collision;
        }

        public object Partner { get; private set; }

        private bool Collision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            AdditionalFixtureData additionalFixtureData = (AdditionalFixtureData)fixtureB.UserData;
            if (additionalFixtureData != null)
            {
                if (additionalFixtureData.physicParent is Blood)
                {
                    lifePoints = 0;
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
