using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodbender
{
    public class GenerateProjectileComponent : IComponent
    {
        GraphicObj owner;
        float incTimer = 0.0f;
        float frequency = 1.0f;
        float shootAngle = (float)Math.PI / 2.0f;

        public GenerateProjectileComponent(GraphicObj obj)
        {
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
            Projectile proj = new Projectile(owner.position, shootAngle, 400f);
            proj.addAnimation(new Animation(Bloodbender.ptr.bouleRouge));
            Bloodbender.ptr.listGraphicObj.Add(proj);
        }
    }
}
