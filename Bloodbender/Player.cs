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
        Fixture playerBoundsFix;
        Fixture playerHitSensorFix;
        float attackSensorAngle; 

        public Player(Vector2 position, uint animNbr = 1) : base(position, animNbr)
        {
            Bloodbender.ptr.camera.TrackingBody = body;

            float pixelToMeter = Bloodbender.ptr.pixelToMeter;

            velocity = 200;
            attackSensorAngle = 0f;

            //Create rectangles shapes
            Vertices rectangleVertices = PolygonTools.CreateRectangle(16 * pixelToMeter, 16 * pixelToMeter);

            PolygonShape playerBounds = new PolygonShape(rectangleVertices, 1);
            PolygonShape playerHitSensor = new PolygonShape(rectangleVertices, 1);

            //Transalte rectangles shapes to set there positions
            playerHitSensor.Vertices.Translate(new Vector2(32 * pixelToMeter, 0));

            //Bind body to shpes (create a compound body)
            playerBoundsFix = body.CreateFixture(playerBounds);
            playerHitSensorFix = body.CreateFixture(playerHitSensor);

            //set the UserData fixture's members with HitboxData object (contain parent and uint)
            playerBoundsFix.UserData = new HitboxData(this, hitboxType.BOUND);
            playerHitSensorFix.UserData = new HitboxData(this, hitboxType.ATTACK);

            //set wether the fixture is a sensor or not (sensor: no response, no contact point)
            playerHitSensorFix.IsSensor = true;

            //add method to be called on collision, different denpending of fixture
            playerHitSensorFix.OnCollision += collisionSensor;
            playerHitSensorFix.OnSeparation += separationSensor;

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

            isSensorColliding();
            playerHitSensorFixMousePosRotation();
            checkSensorInteractions();

            return base.Update(elapsed);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        private void playerHitSensorFixMousePosRotation()
        {
            float mouseAngle = angleWithMouse();
            float pi = (float)Math.PI;
            float piBy4 = 0.78539816339f;
            float threePiBy4 = 2.35619449019f;
            float newAttackSensorAngle = 0;

            if (mouseAngle > -piBy4 && mouseAngle < piBy4) //trouver les angles corespondants avec des fraction de pi
                newAttackSensorAngle = 0;
            else if (mouseAngle > piBy4 && mouseAngle < threePiBy4)
                newAttackSensorAngle = pi / 2f;
            else if ((mouseAngle > threePiBy4 && mouseAngle < pi) || (mouseAngle > -pi && mouseAngle < -threePiBy4))
                newAttackSensorAngle = pi;
            else if (mouseAngle > -threePiBy4 && mouseAngle < -piBy4)
                newAttackSensorAngle = -pi / 2f;
            if (newAttackSensorAngle != attackSensorAngle)
            {
                ((PolygonShape)playerHitSensorFix.Shape).Vertices.Rotate(newAttackSensorAngle - attackSensorAngle);
                attackSensorAngle = newAttackSensorAngle;
                body.Awake = true;
            }

        }

        private void checkSensorInteractions()
        {
            if (!Keyboard.GetState().IsKeyDown(Keys.Space))
                return;
            HitboxData contactData = null;
            HitboxData sensorData = (HitboxData)playerHitSensorFix.UserData;
            if (sensorData.isTouching == true)
            {
                foreach (Contact contact in sensorData.contactList)
                {
                    contactData = (HitboxData)contact.FixtureB.UserData;
                    if (contactData == null)
                        continue;
                    if (contactData.physicParent is Totem)
                        ((Totem)contactData.physicParent).generateProjectile(angleWithMouse());
                }
            }
        }

        private void isSensorColliding()
        {
            if (((HitboxData)playerHitSensorFix.UserData).isTouching == true)
            {
                //System.Diagnostics.Debug.WriteLine("attackSensor colliding");
            }
        }
    }
}
