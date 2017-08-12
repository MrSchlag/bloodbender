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
using Bloodbender.ParticuleEngine.ParticuleSpawners;

namespace Bloodbender
{
    public class Player : PhysicObj
    {
        private Fixture playerHitSensorFix;
        private float attackSensorAngle;
        private float dashSpeed = 450f;
        private float dashDuration = 0.25f;
        private float dashTime = 0f;
        private bool isInDash = false;
        private float dashRestDuration = 1f;
        private float dashRestTime = 0f;
        private bool isInDashRest;
        private bool isBlocked;
        private float timerBlocked = 0f;

        private static float normalDashSpeed = 450f;
        private static float normalDashDuration = 0.25f;
        private static float normalDashRestDuration = 1f;
        private static float attackDashSpeed = 350f;
        private static float attackDashDuration = 0.02f;
        private static float attackDashRestDuration = 0f;

        private Vector2 dashLinearVelocity = Vector2.Zero;

        DashSpawner dashSpawner;
        SnowMarkSpawner snowMarkSpawner;

        public Player(Vector2 position) : base(position, PathFinderNodeType.CENTER)
        {
            Bloodbender.ptr.shadowsRendering.addShadow(new Shadow(this));
            Bloodbender.ptr.camera.TrackingBody = body;

            velocity = 200;
            attackSensorAngle = 0f;
            Radius = 32f;

            Fixture playerBoundsFix = createOctogoneFixture(32f, 32f, Vector2.Zero, new AdditionalFixtureData(this, HitboxType.BOUND));

            playerHitSensorFix = createRectangleFixture(70.0f, 60.0f, new Vector2(30f, 0), new AdditionalFixtureData(this, HitboxType.ATTACK));

            //set wether the fixture is a sensor or not (sensor: no response, no contact point)
            playerHitSensorFix.IsSensor = true;

            //add method to be called on collision, different denpending of fixture
            addFixtureToCheckedCollision(playerHitSensorFix); 
            addFixtureToCheckedCollision(playerBoundsFix);

            //height = 10;

            Texture2D texture1 = Bloodbender.ptr.Content.Load<Texture2D>("Soldat/soldat-bas");
            Texture2D texture2 = Bloodbender.ptr.Content.Load<Texture2D>("Soldat/course");
            Texture2D textureAttack1 = Bloodbender.ptr.Content.Load<Texture2D>("Soldat/attack1");

            addAnimation(new Animation(texture1));
            addAnimation(new Animation(texture2, 8, 0.06f, 64, 0, 0, 0));

            Animation attack1 = new Animation(textureAttack1, 9, 0.04f, 128, 0, 0, 0);
            attack1.isLooping = false;
            attack1.offSet = new Vector2(0,32);
            addAnimation(attack1);

            dashSpawner = new DashSpawner(new Vector2(0, 0), 0, this, new Vector2(0, 0));
            Bloodbender.ptr.particuleSystem.addParticuleSpawner(dashSpawner);
            dashSpawner.canSpawn = false;

            snowMarkSpawner = new SnowMarkSpawner(new Vector2(0, 0), 0, this, new Vector2(0, 0));
            Bloodbender.ptr.particuleSystem.addParticuleSpawner(snowMarkSpawner);
        }

        public override bool Update(float elapsed)
        {

            ContinueDash(elapsed);

            if (isBlocked)
            {
                if (timerBlocked != 0f)
                {
                    if (timerBlocked < 0f)
                        isBlocked = false;

                    timerBlocked -= elapsed;
                    return base.Update(elapsed);
                }
            }

            isSensorColliding();
            playerHitSensorFixMousePosRotation();
            checkSensorInteractions();

            InputSwitch(elapsed);



            height = MathHelper.Clamp(height, 0.0f, 10000.0f);

            Vector2 blAbs = MathUtils.Abs(body.LinearVelocity);

            if (blAbs.X + blAbs.Y >= 6.25f)
                snowMarkSpawner.canSpawn = true;
            else
                snowMarkSpawner.canSpawn = false;

            return base.Update(elapsed);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        private void InputSwitch(float elapsed)
        {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

            if (isInDash)
                return;

            if (getAnimation(2).isRunning)
                return;

            spriteEffect = SpriteEffects.None;

            if (gamePadState.IsConnected)
            {
                body.LinearVelocity = new Vector2(gamePadState.ThumbSticks.Left.X * velocity * Bloodbender.pixelToMeter, -gamePadState.ThumbSticks.Left.Y * velocity * Bloodbender.pixelToMeter);

                if (gamePadState.ThumbSticks.Left.X > 0)
                {
                    spriteEffect = SpriteEffects.None;
                }
                else if (gamePadState.ThumbSticks.Left.X < 0)
                {
                    spriteEffect = SpriteEffects.FlipHorizontally;
                }

                if (gamePadState.Triggers.Left == 1)
                {
                    dashSpeed = normalDashSpeed;
                    dashDuration = normalDashDuration;
                    dashRestDuration = normalDashRestDuration;
                    StartDash(body.LinearVelocity);
                }

                if (gamePadState.Triggers.Right == 1)
                {
                    playerHitSensorFixMousePosRotation();

                    dashSpeed = attackDashSpeed;
                    dashDuration = attackDashDuration;
                    dashRestDuration = attackDashRestDuration;
                    runAnimation(2);
                    StartDash(new Vector2((float)Math.Cos(angleWithMouse()), (float)Math.Sin(angleWithMouse())), false);
                    block(0.04f * 9);
                    return;
                }


                if (gamePadState.ThumbSticks.Left.X != 0 || gamePadState.ThumbSticks.Left.Y != 0)
                {
                    runAnimation(1);
                    return;
                }
            }

            int nbrArrowPressed = 0;
            if (Keyboard.GetState().IsKeyDown(Keys.Z)
                || Keyboard.GetState().IsKeyDown(Keys.S)
                || Keyboard.GetState().IsKeyDown(Keys.Q)
                || Keyboard.GetState().IsKeyDown(Keys.D))
            {
                body.LinearVelocity = new Vector2(0, 0);
                runAnimation(1);
            }
            else
                runAnimation(0);

            if (Keyboard.GetState().IsKeyDown(Keys.Z))
            {
                if (!Keyboard.GetState().IsKeyDown(Keys.S))
                {
                    nbrArrowPressed += 1;
                    body.LinearVelocity += new Vector2(0, -velocity * Bloodbender.pixelToMeter);
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                nbrArrowPressed += 1;
                body.LinearVelocity += new Vector2(0, velocity * Bloodbender.pixelToMeter);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                spriteEffect = SpriteEffects.None;


                if (!Keyboard.GetState().IsKeyDown(Keys.Q))
                {
                    nbrArrowPressed += 1;
                    body.LinearVelocity += new Vector2(velocity * Bloodbender.pixelToMeter, 0);
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Q))
            {
                spriteEffect = SpriteEffects.FlipHorizontally;

                nbrArrowPressed += 1;
                body.LinearVelocity += new Vector2(-velocity * Bloodbender.pixelToMeter, 0);
            }
            if (nbrArrowPressed >= 2)
                body.LinearVelocity /= 2;

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                dashSpeed = normalDashSpeed;
                dashDuration = normalDashDuration;
                dashRestDuration = normalDashRestDuration;
                StartDash(body.LinearVelocity);
            }

            if (Bloodbender.ptr.inputHelper.IsNewMouseButtonPress(MouseButtons.LeftButton))
            {
                playerHitSensorFixMousePosRotation();

                dashSpeed = attackDashSpeed;
                dashDuration = attackDashDuration;
                dashRestDuration = attackDashRestDuration;
                runAnimation(2);
                StartDash(new Vector2((float)Math.Cos(angleWithMouse()), (float)Math.Sin(angleWithMouse())), false);
                block(0.04f * 9);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                height += 50 * elapsed;
            else if (Keyboard.GetState().IsKeyDown(Keys.Down))
                height -= 50 * elapsed;
        }

        private void StartDash(Vector2 direction, bool checkDashReset = true)
        {
            if (checkDashReset)
                if (isInDashRest)
                    return;
            block(dashDuration);
            if (direction == Vector2.Zero)
                return;
            dashSpawner.canSpawn = true;
            direction.Normalize();
            body.LinearVelocity = direction * dashSpeed * Bloodbender.pixelToMeter;
            dashLinearVelocity = body.LinearVelocity;

            isInDash = true;
            dashTime = 0f;
            dashRestTime = 0f;
        }

        private void ContinueDash(float elapsed)
        {
            if (isInDash == true)
            {
                if (dashTime < dashDuration)
                {
                    dashTime += elapsed;
                    body.LinearVelocity = dashLinearVelocity;
                }
                else
                {
                    dashSpawner.canSpawn = false;
                    dashTime = 0f;
                    isInDash = false;
                    isInDashRest = true;
                }
            }
            else if (isInDashRest)
            {
                dashRestTime += elapsed;
                if (dashRestTime > dashRestDuration)
                    isInDashRest = false;
            }
        }

        private void playerHitSensorFixMousePosRotation()
        {
            float mouseAngle = angleWithMouse();
            float pi = (float)Math.PI;
            float piBy4 = 0.78539816339f;
            float threePiBy4 = 2.35619449019f;
            float newAttackSensorAngle = 0;

            if (mouseAngle > -piBy4 && mouseAngle < piBy4) // droite
            {
                newAttackSensorAngle = 0;
                spriteEffect = SpriteEffects.None;
            }
            /*
            else if (mouseAngle > piBy4 && mouseAngle < threePiBy4) // bas
            {
                newAttackSensorAngle = pi / 2f;
            }
            */
            //else if ((mouseAngle > threePiBy4 && mouseAngle < pi) || (mouseAngle > -pi && mouse@Angle < -threePiBy4)) // gauche
            else if (mouseAngle < pi && mouseAngle > pi / 2 || mouseAngle < -pi / 2 && mouseAngle > -pi)
            {
                newAttackSensorAngle = pi;
                spriteEffect = SpriteEffects.FlipHorizontally;
            }
            /*
            else if (mouseAngle > -threePiBy4 && mouseAngle < -piBy4) // haut
            {
                newAttackSensorAngle = -pi / 2f;
            }
            */

            if (newAttackSensorAngle != attackSensorAngle)
            {
                ((PolygonShape)playerHitSensorFix.Shape).Vertices.Rotate(newAttackSensorAngle - attackSensorAngle);
                attackSensorAngle = newAttackSensorAngle;
                body.Awake = true;
            }
        }

        private void checkSensorInteractions()
        {
            //if (!Keyboard.GetState().IsKeyDown(Keys.Space))
            if (!Bloodbender.ptr.inputHelper.IsNewMouseButtonPress(MouseButtons.LeftButton))
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
                    if (fixInContactData.physicParent is Enemy)
                        ((Enemy)fixInContactData.physicParent).takeHit(angleWithMouse());
                }
            }
        }

        private void block(float timer = 0f)
        {
            timerBlocked = timer;
            isBlocked = true;
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
