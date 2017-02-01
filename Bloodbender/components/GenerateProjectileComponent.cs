using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodbender
{
    public class GenerateProjectileComponent : IPhysicComponent
    {
        PhysicObj physicOwner;
        float incTimer = 0.0f;
        float frequency = 1.0f;
        float shootAngle = (float)Math.PI / 2.0f;

        void IPhysicComponent.Initialize(PhysicObj obj)
        {
            physicOwner = obj;    
        }

        void IPhysicComponent.Update(float elapsed)
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
            Projectile proj = new Projectile(physicOwner.body.Position * Bloodbender.meterToPixel, shootAngle, 400f);
            proj.addAnimation(new Animation(Bloodbender.ptr.bouleRouge));
            Bloodbender.ptr.listGraphicObj.Add(proj);
        }
    }
}
