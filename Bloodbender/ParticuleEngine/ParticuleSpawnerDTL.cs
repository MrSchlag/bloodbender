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
        protected Vector2 diffPosition;
        protected bool particuleFollowSpawner = false;
        public ParticuleSpawnerDTL(Vector2 position) : this(position, 0, null, Vector2.Zero) { }
        public ParticuleSpawnerDTL(Vector2 position, RadianAngle angle) : this(position, angle, null, Vector2.Zero) { }
        public ParticuleSpawnerDTL(Vector2 position, RadianAngle angle, GraphicObj target, Vector2 offSetPosition) : base(position, angle, target, offSetPosition) { }
        public override bool Update(float elapsed)
        {
            foreach (ParticuleDTL particule in particules)
            {
                if (particuleFollowSpawner)
                    particule.referencePosition -= diffPosition;

                if (!particule.Update(elapsed) && tryToPopParticule >= 1 && canSpawn)
                {
                    tryToPopParticule--;
                    particuleToCook = particule;
                    cookParticule();
                }
            }

            while (tryToPopParticule >= 1 && canSpawn)
            {
                tryToPopParticule--;
                createParticule();
                cookParticule();
            }

            base.Update(elapsed);

            return true;
        }
    }
}
