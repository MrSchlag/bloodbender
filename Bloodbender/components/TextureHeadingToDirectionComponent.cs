using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodbender
{
    public class TextureHeadingToDirectionComponent : IComponent
    {
        PhysicObj owner;

        public TextureHeadingToDirectionComponent(PhysicObj obj)
        {
            owner = obj;
        }

        public void Remove()
        {
        }

        bool IComponent.Update(float elapsed)
        {
            //float angleLinearVelocityVector = (float)Math.Atan(owner.body.LinearVelocity.Y / owner.body.LinearVelocity.X);
            //owner.setRotation(angleLinearVelocityVector + (float)Math.PI / 2);

            //float angleLinearVelocityVector = (float)(Math.Atan(owner.body.LinearVelocity.Y / owner.body.LinearVelocity.X) * 180 / Math.PI + 90);

            float angleLinearVelocityVector = (float)(Math.Atan2(owner.body.LinearVelocity.Y, owner.body.LinearVelocity.X) - Math.Atan2(1, 0) + Math.PI / 2);
            owner.setRotation(angleLinearVelocityVector);

            return true;
        }
    }
}
