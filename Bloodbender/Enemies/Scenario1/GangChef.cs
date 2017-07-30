using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bloodbender.Enemies.Scenario1
{
    class GangChef : Enemy
    {
        public GangChef(Vector2 position, PhysicObj target) : base(position, target)
        {
            height = 0;

            Animation anim = new Animation(Bloodbender.ptr.Content.Load<Texture2D>("carre"), 32, 32);
            addAnimation(anim);

            Bloodbender.ptr.shadowsRendering.addShadow(new Shadow(this));

            canAttack = false;
        }

        public override bool Update(float elapsed)
        {
            Console.WriteLine(angleWith(target) * position);

            return base.Update(elapsed);
        }

        public Vector2 givePositionToMinion(float offSet)
        {
            Vector2 newPosition = new Vector2(0,0);

            return newPosition;
        }
    }
}
