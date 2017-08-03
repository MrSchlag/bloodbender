using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodbender
{
    class RadianAngle
    {
        public float value;

        public RadianAngle(float value) { this.value = value; }

        public static implicit operator float(RadianAngle radianAngle)
        {
            return radianAngle.value;
        }

        public static implicit operator RadianAngle(float f)
        {
            return new RadianAngle(f);
        }

        public static RadianAngle operator+ (RadianAngle a, RadianAngle b)
        {
            RadianAngle result = new RadianAngle(0);

            result.value = (float)((a.value + b.value + Math.PI) % (2 * Math.PI) - Math.PI);

            return result;
        }

        public static RadianAngle operator- (RadianAngle a, RadianAngle b)
        {
            RadianAngle result = new RadianAngle(0);

            result.value = (float)((a.value - b.value + Math.PI) % (2 * Math.PI) - Math.PI);

            return result;
        }
    }
}
