using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodbender.ParticuleEngine.Particules
{
    class DashParticule : ParticuleTTL
    {
        Animation anim;
        public DashParticule() : this(Vector2.Zero, 0, 0, 0) { }
        public DashParticule(Vector2 position, float speed, float distanceMax, RadianAngle angle) : base(position, speed, distanceMax, angle)
        {
            offSet = OffSet.BottomCenterHorizontal;
            anim = new Animation(Bloodbender.ptr.Content.Load<Texture2D>("Soldat/course"), 64, 64, 0, 0);
            addAnimation(anim);
        }

        public override bool Update(float elapsed)
        {
            //intermediatePosition.X += speed * elapsed;

            color = new Color(color, (0.30f * (lifeTime - timer)) / lifeTime);

            return base.Update(elapsed);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
