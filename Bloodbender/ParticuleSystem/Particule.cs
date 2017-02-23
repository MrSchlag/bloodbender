using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodbender
{
    public class Particule : GraphicObj
    {
        public float timer = 0.0f;
        public float lifeTime = 0.0f;
        public Particule() : base(OffSet.Center)
        {

        }

        public override bool Update(float elapsed)
        {
            base.Update(elapsed);

            timer -= elapsed;

            if (timer <= 0.0f)
                return false;

            return true;
        }

        public void reset()
        {
            timer = lifeTime;
            getAnimation(0).color = Color.White;
        }
    }
}
