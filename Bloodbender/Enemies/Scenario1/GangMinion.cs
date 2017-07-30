using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bloodbender.Enemies.Scenario1
{
    class GangMinion : Enemy
    {
        public GangMinion(Vector2 position, GangChef chef, PhysicObj target) : base(position, target)
            {
            height = 0;

            Animation anim = new Animation(Bloodbender.ptr.Content.Load<Texture2D>("carre2"), 32, 32);
            
            addAnimation(anim);

            Bloodbender.ptr.shadowsRendering.addShadow(new Shadow(this));

            scale = new Vector2(0.5f, 0.5f);
        }

        public override bool Update(float elapsed)
        {

            return base.Update(elapsed);
        }
    }
}
