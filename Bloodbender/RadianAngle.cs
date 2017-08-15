using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodbender
{
    public class RadianAngle
    {
        public float value;

        public RadianAngle(float value) { this.value = value; }

        public static Vector2 rotate(Vector2 origin, Vector2 point, RadianAngle angle)
        {
            return new Vector2( (float)(origin.X + Math.Cos(angle) * (point.X - origin.X) - Math.Sin(angle) * (point.Y - origin.Y)),
                                (float)(origin.Y + Math.Sin(angle) * (point.X - origin.X) + Math.Cos(angle) * (point.Y - origin.Y)) );
        }

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

            result.value = (float)((a.value - b.value - Math.PI) % (2 * Math.PI) + Math.PI);

            return result;
        }
    }
}
