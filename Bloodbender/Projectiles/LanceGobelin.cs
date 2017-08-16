using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodbender.Projectiles
{
    class LanceGobelin : Projectile
    {
        public LanceGobelin(Vector2 position, float radius, float angle, float speed) : base(position, radius, angle, speed)
        {
            offSet = OffSet.Center;

            Animation anim = new Animation(Bloodbender.ptr.Content.Load<Texture2D>("GangMinion/spear"));
            anim.reset();
            addAnimation(anim);
        }
    }
}
