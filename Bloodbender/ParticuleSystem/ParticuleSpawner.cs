using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodbender
{
    public class ParticuleSpawner : GraphicObj
    {
        public List<Particule> particules = null;

        //
        int currentParticule = 0;
        float margin, length, timer = 0.0f;
        //
        public ParticuleSpawner(int nb, Vector2 position)
        {
            particules = new List<Particule>();

            this.position = position;

            for (int i = 0; i < nb; ++i)
            {
                GreenCercle part = new GreenCercle(); //
                part.position = position;
                particules.Add(part);
            }

            //
            length = 1f;
            margin = length / nb;
            //
        }

        public override bool Update(float elapsed)
        {
            //
            timer += elapsed;

            Particule part = particules[currentParticule];

            part.position = position;

            if (timer >= (margin * (currentParticule + 1)))
            {
                ++currentParticule;
                part.reset();

            }

            if (timer >= length)
            {
                currentParticule = 0;
                timer = 0.0f;
            }

            

            //

            foreach (Particule particule in particules)
            {
                particule.getAnimation(0).color = Color.White * (particule.timer / length);

                if (!particule.Update(elapsed))
                {
                    particule.reset();
                }
            }

            base.Update(elapsed);

            return true;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (Particule particule in particules)
            {
                particule.Draw(spriteBatch);
            }
        }
    }
}
