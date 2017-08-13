using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodbender.components
{
    public class KeepDistanceComponent : IComponent
    {
        private PhysicObj _owner;
        private PhysicObj _target;
        private float _distance;

        public KeepDistanceComponent(float distance, PhysicObj target, PhysicObj owner)
        {
            _owner = owner;
            _distance = distance * Bloodbender.pixelToMeter;
            _target = target;
        }
        
        public bool Update(float elapsed)
        {
            if ((_target.body.Position - _owner.body.Position).Length() < _distance)
                return false;
            return true;
        }

        public void Remove()
        {
            
        }
    }
}
