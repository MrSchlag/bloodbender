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

        public Player(Vector2 position) : base(position)
        {
            Bloodbender.ptr.shadowsRendering.addShadow(new Shadow(this));
            Bloodbender.ptr.camera.TrackingBody = body;

            velocity = 200;
            attackSensorAngle = 0f;
            
            playerBoundsFix = createRectangleFixture(32.0f, 32.0f, Vector2.Zero, new AdditionalFixtureData(this, HitboxType.BOUND));
            playerHitSensorFix = createRectangleFixture(32.0f, 32.0f, new Vector2(32.0f, 0), new AdditionalFixtureData(this, HitboxType.ATTACK));

            //set wether the fixture is a sensor or not (sensor: no response, no contact point)
            playerHitSensorFix.IsSensor = true;

            //add method to be called on collision, different denpending of fixture
            addFixtureToCheckedCollision(playerHitSensorFix); 
            addFixtureToCheckedCollision(playerBoundsFix);
            height = 0;

            IComponent comp = new GenerateProjectileComponent(this);
            addComponent(comp);
        }

        public override bool Update(float elapsed)
        {
            float pixelToMeter = Bloodbender.pixelToMeter;
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
            AdditionalFixtureData fixInContactData;
            AdditionalFixtureData sensorData = (AdditionalFixtureData)playerHitSensorFix.UserData;
            if (sensorData.isTouching == true)
            {
                foreach (Fixture fixInContact in sensorData.fixInContactList)
                {
                    if (fixInContact.UserData == null)
                        continue;
                    fixInContactData = (AdditionalFixtureData)fixInContact.UserData;
                    if (fixInContactData.physicParent is Totem)
                        ((Totem)fixInContactData.physicParent).generateProjectile(angleWithMouse());
                }
            }
        }

        private void isSensorColliding()
        {
            if (((AdditionalFixtureData)playerHitSensorFix.UserData).isTouching == true)
            {
                //System.Diagnostics.Debug.WriteLine("attackSensor colliding");
            }
        }
    }
}
