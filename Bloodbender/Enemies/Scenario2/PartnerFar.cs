using Bloodbender.components;
using Bloodbender.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bloodbender.Enemies.Scenario2
{
    public class PartnerFar : Enemy
    {
        public PartnerClose Partner { get; set; }
        private PhysicObj _node;
        private readonly float _initialVelocity = 50;

        public PartnerFar(Vector2 position, PhysicObj target) : base(position, target)
        {
            height = 0;
            
            Animation anim = new Animation(Bloodbender.ptr.Content.Load<Texture2D>("Pair/coursePair2"), 7, 0.1f, 0, 0, 0, 0);
            anim.reset();
            addAnimation(anim);

            Animation attackAnimation = new Animation(Bloodbender.ptr.Content.Load<Texture2D>("Pair/attackPair2"), 7, 0.075f, 0, 0, 0, 0);
            attackAnimation.isLooping = false;
            addAnimation(attackAnimation);

            Bloodbender.ptr.shadowsRendering.addShadow(new Shadow(this));

            fixture = createOctogoneFixture(50f, 50f, Vector2.Zero, new AdditionalFixtureData(this, HitboxType.BOUND));
            Radius = 50f;
            velocity = _initialVelocity;

            addFixtureToCheckedCollision(fixture);

            _node = new PhysicObj(new Vector2(position.X, position.Y), PathFinderNodeType.CENTER);
            _node.createOctogoneFixture(10, 10, Vector2.Zero, new AdditionalFixtureData(_node, HitboxType.BOUND)).IsSensor = true;
            _node.Radius = 10.0f;

            IComponent comp = new KeepDistanceComponent(300, Bloodbender.ptr.player, _node, this);
            _node.addComponent(comp);

            IComponent comp2 = new FollowBehaviorComponent(this, _node, 0);
            addComponent(comp2);

            distanceAttackWithTarget = 400;

            canAttack = true;
            canGenerateProjectile = true;
            canBeHitByPlayer = false;
            canBeHitByProjectile = false;
        }

        public override bool Update(float elapsed)
        {
            _node.Update(elapsed);


            return base.Update(elapsed);
        }

        protected override void Attack()
        {
            Projectile proj = new LanceGobelin(position, 10, angleWith(target), 400f);
            Bloodbender.ptr.listGraphicObj.Add(proj);

            base.Attack();
        }

    }
}
