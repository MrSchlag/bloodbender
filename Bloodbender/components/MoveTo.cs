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
        int currentPoint = -1, count = 0;
        Vector2 point1 = Vector2.Zero, point2 = Vector2.Zero;
        public MoveTo(GraphicObj obj, List<PathFinderNode> listVect, float speed = 200.0f)
        {
            target = obj;
            points = listVect;
            this.speed = speed;
        }

        public bool Update(float elapsed)
        {
            if (points.Count != count)
            {
                distance = 0;
                currentPoint = 0;
                count = points.Count;
            }

            if (distance <= 0)
            {
                currentPoint++;
                if (currentPoint == points.Count)
                {
                    currentPoint = 0;
                    target.position = points[0].position * Bloodbender.meterToPixel; // mettre une condition pour boucler ou non
                }
                point1 = target.position;
                point2 = points[currentPoint].position * Bloodbender.meterToPixel;
                distance = Math.Sqrt(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2));
                
                
            }

            float step = speed * elapsed;

            distance -= step;

            //result.X = (float)(distance * Math.Cos(0));
            //result.Y = (float)(distance * Math.Sin(0));//inutile

            //Console.WriteLine(point1 + " " + point2);



            float deltaY = point1.Y - point2.Y;
            float deltaX = point1.X - point2.X;

            float angle = (float)(Math.Atan2(deltaY, deltaX));


            Vector2 stepVec = new Vector2((float)(step * -Math.Cos(angle)), (float)(step * -Math.Sin(angle)));

            target.position += stepVec;

            

                // passe au second point
            

            return true;
        }
    }
}
