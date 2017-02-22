using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarseerPhysics.Common.PolygonManipulation;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common.TextureTools;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bloodbender
{
    public class Enemy : PhysicObj
    {
        Fixture playerBoundsFix;

        public Enemy(Vector2 position, Player player) : base(position, PathFinderNodeType.CENTER)
        {
            Bloodbender.ptr.shadowsRendering.addShadow(new Shadow(this));

            velocity = 100;

            Fixture playerBoundsFix = createOctogoneFixture(32f, 32f, Vector2.Zero, new AdditionalFixtureData(this, HitboxType.BOUND));

            //add method to be called on collision, different denpending of fixture
            addFixtureToCheckedCollision(playerBoundsFix);

            IComponent comp = new FollowBehaviorComponent(this, player, 40);
            addComponent(comp);
        }

        public override bool Update(float elapsed)
        {
            return base.Update(elapsed);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
