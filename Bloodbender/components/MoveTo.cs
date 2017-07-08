using Bloodbender.PathFinding;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodbender
{
    public class MoveTo : IComponent
    {
        GraphicObj target;
        List<PathFinderNode> points;
        float speed = 200.0f;
        double distance = 0;
        int currentPoint = 0, count = 0;
        Vector2 point1 = Vector2.Zero, point2 = Vector2.Zero;
        public MoveTo(GraphicObj obj, List<PathFinderNode> listVect, float speed = 200.0f)
        {
            target = obj;
            points = listVect;
            this.speed = speed;
        }

        public bool Update(float elapsed)
        {
            float step = speed * elapsed;

            

            distance -= step;

            

            if (distance <= 0)
            {
                currentPoint++;
                if (currentPoint == points.Count)
                {
                    //target.shouldDie = true;
                    //return false;

                    currentPoint = 0;
                    target.position = points[0].position * Bloodbender.meterToPixel; // mettre une condition pour boucler ou non
                    return true;
                }
            }

            if (points.Count != count)
            {
                distance = 0;
                currentPoint = 0;
                count = points.Count;
                target.position = points[0].position * Bloodbender.meterToPixel;
            }



            point1 = target.position;
            point2 = points[currentPoint].position * Bloodbender.meterToPixel;
            distance = Math.Sqrt(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2));


            float deltaY = point1.Y - point2.Y;
            float deltaX = point1.X - point2.X;

            float angle = (float)(Math.Atan2(deltaY, deltaX));


            Vector2 stepVec = new Vector2((float)(step * -Math.Cos(angle)), (float)(step * -Math.Sin(angle)));

            
            if (distance - step > 0)
            target.position += stepVec;

                // passe au second point
            

            return true;
        }
    }
}
