using Bloodbender.ParticuleEngine.Particules;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Bloodbender.ParticuleEngine
{
    class SnowSpawner : ParticuleSpawner
    {
        bool tryToPopParticule;
        float timer;
        float timeSpawn;

        public SnowSpawner(Vector2 position) : this(position, null, Vector2.Zero) { }
        public SnowSpawner(Vector2 position, GraphicObj target, Vector2 offSetPosition) : base(position, target, offSetPosition)
        {
            timeSpawn = 2f;
        }
        public override bool Update(float elapsed)
        {
            if (target != null)
                position = target.position + offSetPosition;

            if (timer >= timeSpawn)
            {
                tryToPopParticule = true;
                timer = 0;
            }
            else
                timer += elapsed;

            foreach (SnowParticule particule in particules)
            {
                if (!particule.Update(elapsed) && tryToPopParticule)
                {
                    tryToPopParticule = false;
                    cookParticule(particule);

                }
            }

            if (tryToPopParticule)
            {
                tryToPopParticule = false;
                SnowParticule newParticule = new SnowParticule();
                particules.Add(newParticule);
                cookParticule(newParticule);
            }

            Console.WriteLine(particules.Count);

            base.Update(elapsed);

            return true;
        }

        private void cookParticule(SnowParticule particule)
        {
            particule.inWait = false;
            particule.speed = 100;
            particule.distanceMax = 50;
            particule.angle = (float)(5 * Math.PI / 6);
            particule.position = position;
            particule.referencePosition = position;
            particule.intermediatePosition = Vector2.Zero;
        }
    }
}
