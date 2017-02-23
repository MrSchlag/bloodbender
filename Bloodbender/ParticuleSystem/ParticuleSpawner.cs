using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodbender
{
    public class ParticuleSpawner
    {
        public Vector2 position = Vector2.Zero;
        public List<Particule> particules = null;

        //
        int currentParticule = 0;
        float margin, length, timer = 0.0f;
        //
        public ParticuleSpawner(int nb)
        {
            particules = new List<Particule>();

            for (int i = 0; i < nb; ++i)
            {
                particules.Add(new GreenCercle());
            }

            //
            length = 1.0f;
            margin = length / nb;
            //
        }

        public bool Update(float elapsed)
        {
            //
            position = Bloodbender.ptr.camera.ConvertScreenToWorld(Bloodbender.ptr.getMousePosition());

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

            return true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Particule particule in particules)
            {
                particule.Draw(spriteBatch);
            }
        }
    }
}
