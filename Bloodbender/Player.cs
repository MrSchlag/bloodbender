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

        public Player(Vector2 position, uint animNbr = 1) : base(position, animNbr)
        {
            Bloodbender.ptr.camera.TrackingBody = body;

            float pixelToMeter = Bloodbender.ptr.pixelToMeter;

            velocity = 200;

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
            playerBoundsFix.UserData = new HitboxData(this, hitboxType.BOUND);
            playerHitUpFix.UserData = new HitboxData(this, hitboxType.ATTACK);
            playerHitDownFix.UserData = new HitboxData(this, hitboxType.ATTACK);
            playerHitLeftFix.UserData = new HitboxData(this, hitboxType.ATTACK);
            playerHitRightFix.UserData = new HitboxData(this, hitboxType.ATTACK);

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
            int nbrArrowPressed = 0;

            if (Keyboard.GetState().IsKeyDown(Keys.Z)
                || Keyboard.GetState().IsKeyDown(Keys.S)
                || Keyboard.GetState().IsKeyDown(Keys.Q)
                || Keyboard.GetState().IsKeyDown(Keys.D))
                body.LinearVelocity = new Vector2(0, 0);

            if (Keyboard.GetState().IsKeyDown(Keys.Z))
            {
                if (!Keyboard.GetState().IsKeyDown(Keys.S))
                {
                    nbrArrowPressed += 1;                    
                    body.LinearVelocity += new Vector2(0, -velocity * pixelToMeter);
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                nbrArrowPressed += 1;
                body.LinearVelocity += new Vector2(0, velocity * pixelToMeter);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                if (!Keyboard.GetState().IsKeyDown(Keys.Q))
                {
                    nbrArrowPressed += 1;
                    body.LinearVelocity += new Vector2(velocity * pixelToMeter, 0);
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Q))
            {
                nbrArrowPressed += 1;
                body.LinearVelocity += new Vector2(-velocity * pixelToMeter, 0);
            }

            if (nbrArrowPressed >= 2)
                body.LinearVelocity /= 2;

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
