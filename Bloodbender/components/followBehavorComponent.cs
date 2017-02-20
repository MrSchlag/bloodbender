using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodbender
{
    public class FollowBehaviorComponent : IComponent
    {
        PhysicObj owner;
        GraphicObj followed;

        float escapeZoneRadius;

        public FollowBehaviorComponent(PhysicObj obj, GraphicObj followed, float escapeZoneRadius)
        {

        }

        void IComponent.Update(float elapsed)
        {

        }
    }
}
