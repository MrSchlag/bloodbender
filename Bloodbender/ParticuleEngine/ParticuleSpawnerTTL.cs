using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodbender.ParticuleEngine
{
    abstract class ParticuleSpawnerTTL : ParticuleSpawner
    {
        protected ParticuleTTL particuleToCook;
        protected Vector2 diffPosition;
        protected bool particuleFollowSpawner = false;
        public ParticuleSpawnerTTL(Vector2 position) : this(position, 0, null, Vector2.Zero) { }
        public ParticuleSpawnerTTL(Vector2 position, RadianAngle angle) : this(position, angle, null, Vector2.Zero) { }
        public ParticuleSpawnerTTL(Vector2 position, RadianAngle angle, GraphicObj target, Vector2 offSetPosition) : base(position, angle, target, offSetPosition) { }
        public override bool Update(float elapsed)
        {
            foreach (ParticuleTTL particule in particules)
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
