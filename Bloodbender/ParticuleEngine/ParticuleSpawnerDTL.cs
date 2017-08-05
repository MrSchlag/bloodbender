using Bloodbender.ParticuleEngine.Particules;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Bloodbender.ParticuleEngine
{
    abstract class ParticuleSpawnerDTL : ParticuleSpawner
    {
        protected ParticuleDTL particuleToCook;
        public ParticuleSpawnerDTL(Vector2 position) : this(position, 0, null, Vector2.Zero) { }
        public ParticuleSpawnerDTL(Vector2 position, RadianAngle angle) : this(position, angle, null, Vector2.Zero) { }
        public ParticuleSpawnerDTL(Vector2 position, RadianAngle angle, GraphicObj target, Vector2 offSetPosition) : base(position, angle, target, offSetPosition) { }
        public override bool Update(float elapsed)
        {
            if (target != null)
                position = target.position + offSetPosition;

            if (timer >= timeSpawn)
            {
                tryToPopParticule = numberParticuleToPop;
                timer = 0;
            }
            else
                timer += elapsed;

            foreach (ParticuleDTL particule in particules)
            {
                if (!particule.Update(elapsed) && tryToPopParticule >= 1)
                {
                    tryToPopParticule--;
                    particuleToCook = particule;
                    cookParticule();
                }
            }

            while (tryToPopParticule >= 1)
            {
                createParticule();
                cookParticule();
            }

            base.Update(elapsed);

            return true;
        }
    }
}
