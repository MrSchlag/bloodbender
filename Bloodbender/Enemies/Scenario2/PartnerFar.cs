using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bloodbender.Enemies.Scenario2
{
    public class PartnerFar : Enemy
    {
        public PartnerClose Partner { get; set; }

        public PartnerFar(Vector2 position, PhysicObj target) : base(position, target)
        {
            height = 0;
            
            Animation anim = new Animation(Bloodbender.ptr.Content.Load<Texture2D>("bouleRouge"));
            addAnimation(anim);

            Bloodbender.ptr.shadowsRendering.addShadow(new Shadow(this));

            fixture = createOctogoneFixture(50f, 50f, Vector2.Zero, new AdditionalFixtureData(this, HitboxType.BOUND));
            Radius = 50f;
            velocity = 50;

            addFixtureToCheckedCollision(fixture);

            //IComponent comp = new FollowBehaviorComponent(this, target, 100f);
            //addComponent(comp);
            //IComponent comp = new
        }

        public override bool Update(float elapsed)
        {
            return base.Update(elapsed);
        }
    }
}
