using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using Bloodbender.Enemies.Scenario1;

namespace Bloodbender.Projectiles
{
    class LanceGobelin : Projectile
    {
        public LanceGobelin(Vector2 position, float radius, float angle, float speed) : base(position, radius, angle, speed)
        {
            offSet = OffSet.Center;

            Animation anim = new Animation(Bloodbender.ptr.Content.Load<Texture2D>("GangMinion/spear"));
            anim.reset();
            addAnimation(anim);

            body.FixtureList[0].OnCollision += Collision;
        }

        private bool Collision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            AdditionalFixtureData additionalFixtureData = (AdditionalFixtureData)fixtureB.UserData;
            if (additionalFixtureData != null)
            {
                shouldDie = true;
                if (additionalFixtureData.physicParent is Projectile)
                {
                    shouldDie = false;
                }
                else if (additionalFixtureData.physicParent is GangChef)
                {
                    shouldDie = false;
                }
                else if (additionalFixtureData.physicParent is GangMinion)
                {
                    shouldDie = false;
                }
                else if (additionalFixtureData.type == HitboxType.ATTACK)
                {
                    shouldDie = false;
                }
            }
            return true;
        }
    }
}
