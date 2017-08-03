using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Bloodbender.ParticuleEngine
{
    public class ParticuleSpawner : GraphicObj
    {
        public List<Particule> particules = null;
        public GraphicObj target = null;
        public Vector2 offSetPosition;

        public ParticuleSpawner(Vector2 position) : this(position, null, Vector2.Zero) { }
        public ParticuleSpawner(Vector2 position, GraphicObj target, Vector2 offSetPosition)
        {
            particules = new List<Particule>();

            this.position = position;
            this.target = target;
            this.offSetPosition = offSetPosition;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (Particule particule in particules)
            {
                if (!particule.inWait)
                    particule.Draw(spriteBatch);
            }
        }
    }
}
