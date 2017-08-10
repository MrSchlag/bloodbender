using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Bloodbender.ParticuleEngine
{
    abstract public class ParticuleSpawner : GraphicObj
    {
        public List<Particule> particules = null;
        public GraphicObj target = null;
        public Vector2 offSetPosition;
        public RadianAngle angle;

        public bool followCamera = false;

        protected int numberParticuleToPop = 1;
        protected int tryToPopParticule = 0;
        protected float timer;
        protected float timeSpawn = 1f;
        public bool canSpawn = true;

        public ParticuleSpawner(Vector2 position) : this(position, 0, null, Vector2.Zero) { }
        public ParticuleSpawner(Vector2 position, RadianAngle angle) : this(position, angle, null, Vector2.Zero) { }
        public ParticuleSpawner(Vector2 position, RadianAngle angle, GraphicObj target, Vector2 offSetPosition)
        {
            particules = new List<Particule>();

            this.position = position;
            this.target = target;
            this.offSetPosition = offSetPosition;
            this.angle = angle;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (Particule particule in particules)
            {
                if (!particule.inWait)
                    particule.Draw(spriteBatch);
            }
        }

        protected abstract void createParticule();

        protected abstract void cookParticule();//should be in each particule
    }
}
