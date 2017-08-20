using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodbender.ParticuleEngine.Particules
{
    class BloodParticule : ParticuleTTL
    {
        Animation anim;
        public BloodParticule() : this(Vector2.Zero, 0, 0, 0, 1) { }
        public BloodParticule(Vector2 position, float speed, float lifeTime, RadianAngle angle, float scaleIN) : base(position, speed, lifeTime, angle)
        {
            offSet = OffSet.BottomCenterHorizontal;
            anim = new Animation(Bloodbender.ptr.Content.Load<Texture2D>("blood1hit"), 6, 0.06f, 32, 0, 0, 0);
            anim.isLooping = false;
            anim.reset();
            addAnimation(anim);
            scale = new Vector2(scaleIN, scaleIN);
        }

        public override bool Update(float elapsed)
        {
            //intermediatePosition.X += speed * elapsed;

            //color = new Color(color, (0.6f * (lifeTime - timer)) / lifeTime);

            return base.Update(elapsed);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
