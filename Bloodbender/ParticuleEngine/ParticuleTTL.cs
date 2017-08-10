using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodbender.ParticuleEngine
{
    public abstract class ParticuleTTL : Particule
    {
        public Vector2 referencePosition;
        public Vector2 intermediatePosition;
        public RadianAngle angle;
        public float lifeTime;
        protected float timer;
        public ParticuleTTL() : this(Vector2.Zero, 0, 0, 0) { }
        public ParticuleTTL(Vector2 position, float speed, float lifeTime, RadianAngle angle) : base(position, speed)
        {
            this.lifeTime = lifeTime;
            this.angle = angle;
            referencePosition = position;
            intermediatePosition = Vector2.Zero;
        }

        public override bool Update(float elapsed)
        {
            if (inWait == true)
                return false;

            if (timer >= lifeTime)
            {
                timer = 0;
                inWait = true;
                return false;
            }
            else
                timer += elapsed;

            position = intermediatePosition + referencePosition;

            position = RadianAngle.rotate(referencePosition, position, angle);

            return true;
        }
    }
}
