using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Bloodbender.ParticuleEngine
{
    public class ParticuleSystem
    {
        private List<ParticuleSpawner> particuleSpawners = null;
        public ParticuleSystem()
        {
            particuleSpawners = new List<ParticuleSpawner>();
        }

        public bool Update(float elapsed)
        {
            foreach (ParticuleSpawner particuleSpawner in particuleSpawners)
            {
                particuleSpawner.Update(elapsed);
            }

            particuleSpawners.RemoveAll(item => item.shouldDie == true);

            return true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (ParticuleSpawner particuleSpawner in particuleSpawners)
            {
                particuleSpawner.Draw(spriteBatch);
            }
        }

        public void addParticuleSpawner(ParticuleSpawner particuleSpawner)
        {
            particuleSpawners.Add(particuleSpawner);
        }

        public void removeParticuleSpawner(ParticuleSpawner particuleSpawner)
        {
            particuleSpawners.Remove(particuleSpawner);
        }

        public void removeAtParticuleSpawner(int index)
        {
            particuleSpawners.RemoveAt(index);
        }
    }
}
