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

        bool IComponent.Update(float elapsed)
        {
            incTimer += elapsed;
            if (incTimer > frequency)
            {
                GenerateProjectile();
                incTimer = 0.0f;
            }

            return true;
        }

        void GenerateProjectile()
        {
            float precisionOffset = rnd.Next(-300, 301) / 1000.0f;

            Projectile proj = new Projectile(owner.position, 3, shootAngle + precisionOffset, 60f);
            int bloodRand = rnd.Next(0, 3);
            if (bloodRand == 0)
                proj.addAnimation(new Animation(Bloodbender.ptr.blood1));
            else if (bloodRand == 1)
                proj.addAnimation(new Animation(Bloodbender.ptr.blood2));
            else
                proj.addAnimation(new Animation(Bloodbender.ptr.blood3));

            proj.setRotation(shootAngle + precisionOffset + (float)Math.PI / 2.0f);

            Bloodbender.ptr.listGraphicObj.Add(proj);
        }

        public void Remove()
        {
        }
    }
}
