using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodbender.ParticuleEngine.Particules
{
    class SnowMarkParticule : ParticuleTTL
    {
        Animation anim;
        public SnowMarkParticule() : this(Vector2.Zero, 0, 0, 0) { }
        public SnowMarkParticule(Vector2 position, float speed, float distanceMax, RadianAngle angle) : base(position, speed, distanceMax, angle)
        {
            offSet = OffSet.BottomCenterHorizontal;
            anim = new Animation(Bloodbender.ptr.Content.Load<Texture2D>("snowMark"));
            addAnimation(anim);
            anim.forceDepth(0);
        }

        public override bool Update(float elapsed)
        {
            //intermediatePosition.X += speed * elapsed;

            color = new Color(color, (0.80f * (lifeTime - timer)) / lifeTime);

            return base.Update(elapsed);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
