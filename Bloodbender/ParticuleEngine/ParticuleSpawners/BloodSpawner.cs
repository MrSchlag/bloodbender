using Bloodbender.ParticuleEngine.Particules;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodbender.ParticuleEngine.ParticuleSpawners
{
    public class BloodSpawner : ParticuleSpawnerTTL
    {
        public float scaleRef = 1;
        public BloodSpawner(Vector2 position) : this(position, 0, null, Vector2.Zero) { }
        public BloodSpawner(Vector2 position, RadianAngle angle) : this(position, angle, null, Vector2.Zero) { }
        public BloodSpawner(Vector2 position, RadianAngle angle, GraphicObj target, Vector2 offSetPosition) : base(position, angle, target, offSetPosition)
        {
            particuleFollowSpawner = false;

        }

        public override bool Update(float elapsed)
        {
            
            diffPosition = position;

            if (target != null)
                position = target.position + offSetPosition;

            diffPosition -= position;
            /*
            if (timer >= timeSpawn)
            {
                tryToPopParticule = numberParticuleToPop;
                timer = 0;
            }
            else
                timer += elapsed;
             */

            if (canSpawn)
            {
                tryToPopParticule = numberParticuleToPop;
            }

            bool result = base.Update(elapsed);

            canSpawn = false;
            numberParticuleToPop = 0;

            return result;
        }
        protected override void createParticule()
        {
            particuleToCook = new BloodParticule(); //TEMPLATE??
            particules.Add(particuleToCook);
        }

        protected override void cookParticule()
        {
            particuleToCook.inWait = false;
            particuleToCook.getAnimation(0).reset();
            //particuleToCook.speed = Bloodbender.ptr.rdn.Next(100, 301);
            particuleToCook.speed = 0;
            particuleToCook.lifeTime = 0.4f;
            //particuleToCook.angle = angle + (Bloodbender.ptr.rdn.Next(-2000, 2001) / 10000.0f);
            //particuleToCook.angle = -(float)(Math.PI);

            particuleToCook.position = position;
            particuleToCook.height = 35;
            particuleToCook.position.Y += 1;
            particuleToCook.position.X += Bloodbender.ptr.rdn.Next(-10, 11);

            particuleToCook.setRotation((Bloodbender.ptr.rdn.Next(-8000, 8000) / 10000.0f));
            //particuleToCook.position.X += Bloodbender.ptr.rdn.Next(-550, 551);
            //particuleToCook.position = RadianAngle.rotate(position, particuleToCook.position, (float)(angle + (Math.PI / 2)));

            particuleToCook.referencePosition = particuleToCook.position;
            particuleToCook.intermediatePosition = Vector2.Zero;

            particuleToCook.scale = new Vector2(scaleRef, scaleRef);

            //particuleToCook.spriteEffect = Bloodbender.ptr.player.spriteEffect;

            //float s = (Bloodbender.ptr.rdn.Next(2000, 4001) / 10000.0f);

            //particuleToCook.scale = new Vector2(s, s);
        }
    }
}
