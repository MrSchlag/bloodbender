using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodbender
{
    class MathAssistant
    {
        public static float sumAngle(float angleA, float angleB)
        {
            return (float)((angleA + angleB + Math.PI) % (2 * Math.PI) - Math.PI);
        }
    }
}
