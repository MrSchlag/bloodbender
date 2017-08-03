using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Bloodbender.ParticuleEngine.Particules
{
    class SnowParticule : Particule
    {
        public Vector2 referencePosition;
        public Vector2 intermediatePosition;
        public RadianAngle angle;
        public float distanceMax;
        public SnowParticule() : this(Vector2.Zero, 0, 0, 0) { }
        public SnowParticule(Vector2 position, float speed, float distanceMax, RadianAngle angle) : base(position, speed)
        {
            Animation anim = new Animation(Bloodbender.ptr.Content.Load<Texture2D>("blood3"), 16, 16);
            addAnimation(anim);

            this.distanceMax = distanceMax;
            this.angle = angle;
            referencePosition = position;
            intermediatePosition = Vector2.Zero;
        }

        public override bool Update(float elapsed)
        {
            intermediatePosition.X += speed * elapsed;

            if (intermediatePosition.X >= distanceMax)
            {
                inWait = true;
                return false;
            }

            position = intermediatePosition + referencePosition;

            //Console.WriteLine(position);

            position = new Vector2( (float)(referencePosition.X + Math.Cos(angle) * (position.X - referencePosition.X) - Math.Sin(angle) * (position.Y - referencePosition.Y)),
                                    (float)(referencePosition.Y + Math.Sin(angle) * (position.X - referencePosition.X) + Math.Cos(angle) * (position.Y - referencePosition.Y))
                                    );



            return true;
        }
    }
}
