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
        GangChef chef;
        public GangMinion(Vector2 position, GangChef chef, PhysicObj target) : base(position, target)
        {
            height = 0;

            Animation anim = new Animation(Bloodbender.ptr.Content.Load<Texture2D>("carre2"), 32, 32);
            addAnimation(anim);

            Bloodbender.ptr.shadowsRendering.addShadow(new Shadow(this));

            scale = new Vector2(0.5f, 0.5f);

            this.chef = chef;

            velocity = 50;
            playerBoundsFix = createOctogoneFixture(16f, 16f, Vector2.Zero, new AdditionalFixtureData(this, HitboxType.BOUND));
            Radius = 0f;
            //add method to be called on collision, different denpending of fixture
            addFixtureToCheckedCollision(playerBoundsFix);

            IComponent comp = new FollowBehaviorComponent(this, chef.node, 3);
            addComponent(comp);

            canAttack = false;
        }

        public override bool Update(float elapsed)
        {
            return base.Update(elapsed);
        }
    }
}
