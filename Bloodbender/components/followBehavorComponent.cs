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
        PhysicObj target;

        float escapeZoneRadius;

        float pathRequestRate = 1f;
        float pathRequestRateCounter = 0f;

        public FollowBehaviorComponent(PhysicObj obj, PhysicObj target, float escapeZoneRadius)
        {
            this.owner = obj;
            this.target = target;
        }

        void IComponent.Update(float elapsed)
        {
            pathRequestRateCounter += elapsed;
            if (pathRequestRateCounter > pathRequestRate)
            {
                Console.WriteLine("[FollowBehaviorComponent] asking path");
                Bloodbender.ptr.pathFinder.pathRequest(owner, owner.getPosNode(), target.getPosNode());
                pathRequestRateCounter = 0f;
            }
        }

        public void changeTarget(PhysicObj target)
        {
            this.target = target;
        }
    }
}
