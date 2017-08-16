using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bloodbender.Enemies.Scenario2
{
    public class Partner : Enemy
    {
        bool _master;

        /// <summary>
        /// Add a partner in the game
        /// </summary>
        /// <param name="position"></param>
        /// <param name="target"></param>
        /// <param name="partner">null if fist of two partners</param>
        public Partner(Vector2 position, PhysicObj target, Partner partner) : base(position, target)
        {
            height = 0;

            Animation anim = new Animation(Bloodbender.ptr.Content.Load<Texture2D>("carre"));
            addAnimation(anim);

            Bloodbender.ptr.shadowsRendering.addShadow(new Shadow(this));

            fixture = createOctogoneFixture(50f, 50f, Vector2.Zero, new AdditionalFixtureData(this, HitboxType.BOUND));
            Radius = 50f;
            velocity = 50;


            addFixtureToCheckedCollision(fixture);

            //IComponent comp = new FollowBehaviorComponent(this, target, 100f);
            //addComponent(comp);
            //IComponent comp = new

            if (partner == null)
                _master = true;
        }
    }
}
