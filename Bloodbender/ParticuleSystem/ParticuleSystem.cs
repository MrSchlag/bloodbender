using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodbender
{
    public class ParticuleSystem
    {
        List<ParticuleSpawner> particuleSpawners = null;
        public ParticuleSystem()
        {

        }

        public bool Update(float elapsed)
        {

            return true;
        }
    }
}
