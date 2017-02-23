using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodbender
{
    public class ParticuleSystem
    {
        public List<ParticuleSpawner> particuleSpawners = null;
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
    }
}
