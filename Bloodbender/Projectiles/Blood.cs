using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

namespace Bloodbender.Projectiles
{
    class Blood : Projectile
    {
        public Blood(Vector2 position, float radius, float angle, float speed) : base(position, radius, angle, speed)
        {
            offSet = OffSet.Center;

            Animation anim = new Animation(Bloodbender.ptr.Content.Load<Texture2D>("blood"), 4, 0.05f, 32, 0, 0, 0);
            anim.reset();
            addAnimation(anim);

            body.FixtureList[0].OnCollision += Collision;
        }

        private bool Collision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            AdditionalFixtureData additionalFixtureData = (AdditionalFixtureData)fixtureB.UserData;
            if (additionalFixtureData != null)
            {
                if (shouldDie)
                    return true;
                shouldDie = true;

                if (additionalFixtureData.type == HitboxType.ATTACK)
                {
                    shouldDie = false;
                }
                else if (additionalFixtureData.physicParent is Projectile)
                {
                    shouldDie = false;
                }
                else if (additionalFixtureData.physicParent is Totem)
                {
                    shouldDie = false;
                }

            }
            return true;
        }
    }
}
