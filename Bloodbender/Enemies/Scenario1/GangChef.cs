using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using Bloodbender.Projectiles;

namespace Bloodbender.Enemies.Scenario1
{
    class GangChef : Enemy
    {
        float currentAngleWithTarget = 0;
        float distanceMinions = 180;
        public PhysicObj node;

        public GangChef(int numberMinion, Vector2 position, PhysicObj target) : base(position, target)
        {
            height = 0;

            Animation anim = new Animation(Bloodbender.ptr.Content.Load<Texture2D>("GangChef/chefstand"));
            addAnimation(anim);

            Animation attackAnimation = new Animation(Bloodbender.ptr.Content.Load<Texture2D>("GangChef/chefwork"), 7, 0.1f, 0, 0, 0, 0);
            attackAnimation.isLooping = false;
            addAnimation(attackAnimation);

            attackRate = 3.5f;

            Bloodbender.ptr.shadowsRendering.addShadow(new Shadow(this));

            scale = new Vector2(1.5f, 1.5f);

            fixture = createOctogoneFixture(64f, 64f, Vector2.Zero, new AdditionalFixtureData(this, HitboxType.BOUND));
            Radius = 0f;
            body.BodyType = BodyType.Static;
            //add method to be called on collision, different denpending of fixture
            addFixtureToCheckedCollision(fixture);

            fixture.OnCollision += Collision;

            node = new PhysicObj(Vector2.Zero, PathFinderNodeType.CENTER);
            node.createOctogoneFixture(10, 10, Vector2.Zero).IsSensor = true;
            node.Radius = 0.0f;

            canAttack = true;
            canGenerateProjectile = false;
            canBeHitByPlayer = false;
            canBeHitByProjectile = true;

            for (int i = 0; i < numberMinion; ++i)
                Bloodbender.ptr.listGraphicObj.Add(new GangMinion(new Vector2(position.X += Bloodbender.ptr.rdn.Next(-50, 51), position.Y += Bloodbender.ptr.rdn.Next(-50, 51)), this, target));
        }

        private bool Collision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (((AdditionalFixtureData)fixtureB.UserData).physicParent is LanceGobelin)
                return false;
            //else if
            return true;

        }

        public override bool Update(float elapsed)
        {
            currentAngleWithTarget = angleWith(target);

            node.Update(elapsed);
            node.body.Position = givePosition(0) * Bloodbender.pixelToMeter;

            distanceAttackWithTarget = (float)distanceWith(target.position) + 10;

            RadianAngle tmpangle = currentAngleWithTarget ;
            tmpangle -= (float)Math.PI / 2;

            if (tmpangle < 0)
                spriteEffect = SpriteEffects.None;
            else if (tmpangle > 0)
                spriteEffect = SpriteEffects.FlipHorizontally;

            return base.Update(elapsed);
        }

        public Vector2 givePosition(float offSet)
        {
            //use offset
            return new Vector2((float)(position.X + Math.Cos(currentAngleWithTarget) * distanceMinions), (float)(position.Y + Math.Sin(currentAngleWithTarget) * distanceMinions));
        }
    }
}
