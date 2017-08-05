using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Bloodbender.ParticuleEngine.Particules
{
    public abstract class ParticuleDTL : Particule
    {
        public Vector2 referencePosition;
        public Vector2 intermediatePosition;
        public RadianAngle angle;
        public float distanceMax;
        public ParticuleDTL() : this(Vector2.Zero, 0, 0, 0) { }
        public ParticuleDTL(Vector2 position, float speed, float distanceMax, RadianAngle angle) : base(position, speed)
        {
            this.distanceMax = distanceMax;
            this.angle = angle;
            referencePosition = position;
            intermediatePosition = Vector2.Zero;
        }

        public override bool Update(float elapsed)
        {
            if (intermediatePosition.X >= distanceMax)
            {
                inWait = true;
                return false;
            }

            position = intermediatePosition + referencePosition;

            position = RadianAngle.rotate(referencePosition, position, angle);
            
            return true;
        }
    }
}
