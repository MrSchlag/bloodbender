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
            timeSpawn = 0.02f;
            this.angle = 2;
            numberParticuleToPop = 3;
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
            particuleToCook.speed = 200;
            particuleToCook.distanceMax = 300;
            particuleToCook.angle = angle;

            particuleToCook.position = position;
            particuleToCook.position.X += Bloodbender.ptr.rdn.Next(-150, 151);
            particuleToCook.position = RadianAngle.rotate(position, particuleToCook.position, (float)(angle + (Math.PI / 2)));

            particuleToCook.referencePosition = particuleToCook.position;
            particuleToCook.intermediatePosition = Vector2.Zero;
        }
    }
}
