using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Bloodbender.ParticuleEngine
{
    public class ParticuleSpawner : GraphicObj
    {
        public List<Particule> particules = null;

        public ParticuleSpawner(Vector2 position)
        {
            particules = new List<Particule>();

            this.position = position;
        }

        public override bool Update(float elapsed)
        {
            foreach (Particule particule in particules)
            {
                if (!particule.Update(elapsed))
                {
                    particule.reset();
                }
            }

            base.Update(elapsed);

            return true;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (Particule particule in particules)
            {
                particule.Draw(spriteBatch);
            }
        }
    }
}
