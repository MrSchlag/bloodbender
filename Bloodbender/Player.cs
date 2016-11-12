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
    public class Player : PhysicObj
    {
        enum hitboxType {
            BOUND = 0,
            ATTACK = 1
        }

        public Player(Vector2 position, uint animNbr = 1) : base(position, animNbr)
        {
            float pixelToMeter = Bloodbender.ptr.pixelToMeter;

            //Create rectangles shapes
            Vertices rectangleVertices = PolygonTools.CreateRectangle(16 * pixelToMeter, 16 * pixelToMeter);
            
            PolygonShape playerBounds = new PolygonShape(rectangleVertices, 1);
            PolygonShape playerHitUp = new PolygonShape(rectangleVertices, 1);
            PolygonShape playerHitDown = new PolygonShape(rectangleVertices, 1);
            PolygonShape playerHitLeft = new PolygonShape(rectangleVertices, 1);
            PolygonShape playerHitRight = new PolygonShape(rectangleVertices, 1);

            //Transalte rectangles shapes to set there positions
            playerHitUp.Vertices.Translate(new Vector2(0, -32 * pixelToMeter));
            playerHitDown.Vertices.Translate(new Vector2(0, 32 * pixelToMeter));
            playerHitLeft.Vertices.Translate(new Vector2(-32 * pixelToMeter, 0));
            playerHitRight.Vertices.Translate(new Vector2(32 * pixelToMeter, 0));

            //Bind body to shpes (create a compound body)
            Fixture playerBoundsFix = body.CreateFixture(playerBounds);
            Fixture playerHitUpFix = body.CreateFixture(playerHitUp);
            Fixture playerHitDownFix = body.CreateFixture(playerHitDown);
            Fixture playerHitLeftFix = body.CreateFixture(playerHitLeft);
            Fixture playerHitRightFix = body.CreateFixture(playerHitRight);

            //set the UserData fixture's members with HitboxData object (contain parent and uint)
            playerBoundsFix.UserData = new HitboxData(this, (uint)hitboxType.BOUND);
            playerHitUpFix.UserData = new HitboxData(this, (uint)hitboxType.ATTACK);
            playerHitDownFix.UserData = new HitboxData(this, (uint)hitboxType.ATTACK);
            playerHitLeftFix.UserData = new HitboxData(this, (uint)hitboxType.ATTACK);
            playerHitRightFix.UserData = new HitboxData(this, (uint)hitboxType.ATTACK);

            //add method to be called on collision, different denpending of fixture
            playerBoundsFix.OnCollision += collisionOnBounds;
            playerHitUpFix.OnCollision += collisionOnAttackBox;
            playerHitDownFix.OnCollision += collisionOnAttackBox;
            playerHitLeftFix.OnCollision += collisionOnAttackBox;
            playerHitRightFix.OnCollision += collisionOnAttackBox;

        }

        public override bool Update(float elapsed)
        {
            float pixelToMeter = Bloodbender.ptr.pixelToMeter;
            if (Keyboard.GetState().IsKeyDown(Keys.Z))
            {
                body.LinearVelocity = new Vector2(0, -100 * pixelToMeter);
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                body.LinearVelocity = new Vector2(0, 100 * pixelToMeter);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                body.LinearVelocity = new Vector2(100 * pixelToMeter, 0);
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Q))
            {
                body.LinearVelocity = new Vector2(-100 * pixelToMeter, 0);
            }

            return base.Update(elapsed);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public bool collisionOnBounds(Fixture f1, Fixture f2, Contact test)
        {
            return true;
        }

        public bool collisionOnAttackBox(Fixture f1, Fixture f2, Contact test)
        {
            System.Diagnostics.Debug.WriteLine("collision with an attack hitbox"); //TO REMOVE
            return false;
        }
    }
}
