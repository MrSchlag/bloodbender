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
        float currentAngleWithTarget = 0;
        float distanceMinions = 80;
        public PhysicObj node;
        public GangChef(Vector2 position, PhysicObj target) : base(position, target)
        {
            height = 0;

            Animation anim = new Animation(Bloodbender.ptr.Content.Load<Texture2D>("carre"), 32, 32);
            addAnimation(anim);

            Bloodbender.ptr.shadowsRendering.addShadow(new Shadow(this));

            node = new PhysicObj(Vector2.Zero, PathFinderNodeType.CENTER);
            node.createOctogoneFixture(10, 10, Vector2.Zero).IsSensor = true;
            node.Radius = 0.0f;

            canAttack = false;
        }

        public override bool Update(float elapsed)
        {
            currentAngleWithTarget = angleWith(target);

            node.Update(elapsed);
            node.body.Position = givePosition(0) * Bloodbender.pixelToMeter;

            return base.Update(elapsed);
        }

        public Vector2 givePosition(float offSet)
        {
            //use offset
            return new Vector2((float)(position.X + Math.Cos(currentAngleWithTarget) * distanceMinions), (float)(position.Y + Math.Sin(currentAngleWithTarget) * distanceMinions));
        }
    }
}
