using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bloodbender
{
    public class Player : PhysicObj
    {
        public Player(Vector2 position, uint animNbr = 1) : base(BodyFactory.CreateRectangle(Bloodbender.ptr.world,
            32 * Bloodbender.ptr.pixelToMeter, 32 * Bloodbender.ptr.pixelToMeter, 1), position, animNbr)
        {

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
    }
}
