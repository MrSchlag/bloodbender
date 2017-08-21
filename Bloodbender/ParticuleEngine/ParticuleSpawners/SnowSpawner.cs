﻿using Bloodbender.ParticuleEngine.Particules;
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
            particuleFollowSpawner = true;

            timeSpawn = 0.02f;
            this.angle = 2.25f;
            numberParticuleToPop = 5;
        }

        public override bool Update(float elapsed)
        {
            diffPosition = position;

            if (target != null)
                position = target.position + offSetPosition;

            diffPosition -= position;

            if (timer >= timeSpawn)
            {
                tryToPopParticule = numberParticuleToPop;
                timer = 0;
            }
            else
                timer += elapsed;

            return base.Update(elapsed);
        }
        protected override void createParticule()
        {
            particuleToCook = new SnowParticule(); //TEMPLATE??
            particules.Add(particuleToCook);
        }

        protected override void cookParticule()
        {
            particuleToCook.inWait = false;
            particuleToCook.speed = Bloodbender.ptr.rdn.Next(100, 301);
            particuleToCook.distanceMax = 1400;
            particuleToCook.angle = angle + (Bloodbender.ptr.rdn.Next(-2000, 2001) / 10000.0f);

            particuleToCook.position = position;
            particuleToCook.position.X += Bloodbender.ptr.rdn.Next(-650, 651);
            particuleToCook.position = RadianAngle.rotate(position, particuleToCook.position, (float)(angle + (Math.PI / 2)));

            particuleToCook.referencePosition = particuleToCook.position;
            particuleToCook.intermediatePosition = Vector2.Zero;

            float s = (Bloodbender.ptr.rdn.Next(2000, 4001) / 10000.0f);

            particuleToCook.scale = new Vector2(s, s);
        }
    }
}
