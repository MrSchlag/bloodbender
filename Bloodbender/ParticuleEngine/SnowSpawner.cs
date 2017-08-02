using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Bloodbender.ParticuleEngine
{
    class SnowSpawner : ParticuleSpawner
    {
        public SnowSpawner(Vector2 position) : base(position)
        {

        }
        public override bool Update(float elapsed)
        {


            base.Update(elapsed);

            return true;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}
