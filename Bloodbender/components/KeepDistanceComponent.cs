using Microsoft.Xna.Framework;
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
            {
                var closestEscapePoint = FindClosestEscapePoint();
                _owner.body.Position = closestEscapePoint;
                _owner.position = closestEscapePoint * Bloodbender.meterToPixel;
                return false;
            }
            else
            {

            }
            return true;
        }

        private Vector2 FindClosestEscapePoint()
        {
            var vectorToRotate = new Vector2(_distance, 0);
            List<Vector2> pointsAround = new List<Vector2>();
            Vector2 vec;
            int step = 10;
            for (int i = 0; i <= 360; i += step)
            {
                vectorToRotate = vectorToRotate.Rotate(i * (float)Math.PI / 180f);
                vec = vectorToRotate + _target.body.Position;

                if (!TreePlanter.IsPointOutside(vec.X, vec.Y))
                    pointsAround.Add(vec);
            }

            return ClosestPointInList(pointsAround);
        }

        private Vector2 ClosestPointInList(List<Vector2> pointsAround)
        {
            Vector2 closest = _owner.body.Position;
            float closestDistance = -1f;

            foreach (var vec in pointsAround)
            {
                var distance = (vec - _owner.body.Position).LengthSquared();
                if (closestDistance == -1f || distance < closestDistance)
                {
                    closest = vec;
                    closestDistance = distance;
                }
             }

            return closest;
        }

        public void Remove()
        {
               
        }
    }
}
