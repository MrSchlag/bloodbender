using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bloodbender.Enemies.Scenario3
{
    public class Bat : Enemy
    {
        public Bat(Vector2 position, PhysicObj target) : base(position, target)
        {
            height = 0;

            Animation anim = new Animation(Bloodbender.ptr.Content.Load<Texture2D>("Ennemy1/ennemy1"), 8, 0.1f, 32, 0, 0, 0);
            anim.reset();
            addAnimation(anim);

            Animation attackAnimation = new Animation(Bloodbender.ptr.Content.Load<Texture2D>("Ennemy1/ennemy1attack"), 6, 0.1f, 32, 0, 0, 0);
            attackAnimation.isLooping = false;
            addAnimation(attackAnimation);

            Bloodbender.ptr.shadowsRendering.addShadow(new Shadow(this));

            velocity = 50;

            fixture = createOctogoneFixture(16f, 16f, Vector2.Zero, new AdditionalFixtureData(this, HitboxType.BOUND));
            Radius = 32f;
            //add method to be called on collision, different denpending of fixture
            addFixtureToCheckedCollision(fixture);

            IComponent comp = new FollowBehaviorComponent(this, target, 32);
            addComponent(comp);

            cacAttack = true;
        }

        public override bool Update(float elapsed)
        {
            return base.Update(elapsed);
        }
    }
}
