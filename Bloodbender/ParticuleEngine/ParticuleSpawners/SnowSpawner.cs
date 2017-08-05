using Bloodbender.ParticuleEngine.Particules;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodbender.ParticuleEngine.ParticuleSpawners
{
    class SnowSpawner : ParticuleSpawnerDTL
    {
        public SnowSpawner(Vector2 position) : this(position, 0, null, Vector2.Zero) { }
        public SnowSpawner(Vector2 position, RadianAngle angle) : this(position, angle, null, Vector2.Zero) { }
        public SnowSpawner(Vector2 position, RadianAngle angle, GraphicObj target, Vector2 offSetPosition) : base(position, angle, target, offSetPosition)
        {
            timeSpawn = 0.2f;
            this.angle = 2;
        }
        protected override void createParticule()
        {
            tryToPopParticule--;
            particuleToCook = new SnowParticule(); //TEMPLATE??
            particules.Add(particuleToCook);
        }

        protected override void cookParticule()
        {
            particuleToCook.inWait = false;
            particuleToCook.speed = 100;
            particuleToCook.distanceMax = 100;
            particuleToCook.angle = angle;
            particuleToCook.position = position;
            particuleToCook.referencePosition = position;
            particuleToCook.intermediatePosition = Vector2.Zero;
        }
    }
}
