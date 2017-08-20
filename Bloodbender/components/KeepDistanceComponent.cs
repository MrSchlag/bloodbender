using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Bloodbender.components
{
    public class KeepDistanceComponent : IComponent
    {
        private PhysicObj _owner;
        private PhysicObj _target;
        private PhysicObj _guarded;
        private float _distance;

        public KeepDistanceComponent(float distance, PhysicObj target, PhysicObj owner, PhysicObj guarded)
        {
            _owner = owner;
            _distance = distance * Bloodbender.pixelToMeter;
            _target = target;
            _guarded = guarded;
        }
        
        public bool Update(float elapsed)
        {
            if ((_guarded.body.Position - _target.body.Position).Length() < _distance)
            {
                var closestEscapePoint = FindClosestEscapePoint();
                _owner.body.Position = closestEscapePoint;
                _owner.position = closestEscapePoint * Bloodbender.meterToPixel;
                return false;
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
                vec = vectorToRotate + _guarded.body.Position;

                if (!TreePlanter.IsPointOutside(vec.X, vec.Y) && !IsPointBetweenRooms(vec))
                    pointsAround.Add(vec);
            }

            return FarthestPointFromTarget(pointsAround);
        }

        private Vector2 FarthestPointFromTarget(List<Vector2> pointsAround)
        {
            Vector2 closest = _guarded.body.Position;
            float closestDistance = -1f;

            foreach (var vec in pointsAround)
            {
                var distance = (vec - _target.body.Position).LengthSquared();
                if (closestDistance == -1f || distance > closestDistance)
                {
                    closest = vec;
                    closestDistance = distance;
                }
            }

            return closest;
        }

        private bool IsPointBetweenRooms(Vector2 point)
        {
            var pt = point * Bloodbender.meterToPixel;
            var rooms = Bloodbender.ptr.mapFactory.mGen.rooms;
            
            foreach (var room in rooms)
            {
                if (pt.Y > room.minY && pt.Y < room.maxY)
                    return false;
            }

            return true;
        }
      
        public void Remove()
        {
               
        }
    }
}
