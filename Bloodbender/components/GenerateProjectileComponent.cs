using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodbender
{
    public class GenerateProjectileComponent : IComponent
    {
        PhysicObj owner;

        Random rnd;
        float incTimer = 0.0f;
        float frequency = 0.05f;
        float shootAngle = (float)Math.PI / 2.0f;

        public GenerateProjectileComponent(PhysicObj obj)
        {
            rnd = new Random();
            owner = obj;
        }

        void IComponent.Update(float elapsed)
        {
            incTimer += elapsed;
            if (incTimer > frequency)
            {
                GenerateProjectile();
                incTimer = 0.0f;
            }
        }

        void GenerateProjectile()
        {
            float precisionOffset = rnd.Next(-500, 501) / 1000.0f;

            Projectile proj = new Projectile(owner.position, shootAngle + precisionOffset, 400f);
            int bloodRand = rnd.Next(0, 3);
            if (bloodRand == 0)
                proj.addAnimation(new Animation(Bloodbender.ptr.blood1));
            else if (bloodRand == 1)
                proj.addAnimation(new Animation(Bloodbender.ptr.blood2));
            else
                proj.addAnimation(new Animation(Bloodbender.ptr.blood3));

            float direction = (float)Math.Atan2(owner.body.LinearVelocity.X, owner.body.LinearVelocity.Y);
            proj.setRotation(direction);

            Bloodbender.ptr.listGraphicObj.Add(proj);
        }
    }
}
