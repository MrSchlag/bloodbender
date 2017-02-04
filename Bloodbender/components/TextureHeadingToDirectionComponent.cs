﻿using System;
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

        void IComponent.Update(float elapsed)
        {
            float angleLinearVelocityVector = (float)Math.Atan(owner.body.LinearVelocity.Y / owner.body.LinearVelocity.X);
            owner.setRotation(angleLinearVelocityVector + (float)Math.PI / 2);
        }
    }
}
