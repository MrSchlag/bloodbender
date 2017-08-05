using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodbender.ParticuleEngine.Particules
{
    class SnowParticule : ParticuleDTL
    {
        public SnowParticule() : this(Vector2.Zero, 0, 0, 0) { }
        public SnowParticule(Vector2 position, float speed, float distanceMax, RadianAngle angle) : base(position, speed, distanceMax, angle)
        {
            Animation anim = new Animation(Bloodbender.ptr.Content.Load<Texture2D>("blood3"), 16, 16);
            addAnimation(anim);
        }

        public override bool Update(float elapsed)
        {
            intermediatePosition.X += speed * elapsed;

            return base.Update(elapsed);
        }
    }
}
