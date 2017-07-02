using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;

namespace BloodbenderMapGenerator
{
    class Visualizer
    {
        public void visualizeMap(List<Wall> wallList)
        {
            List<StringBuilder> lines = init(wallList);
            foreach(Wall wall in wallList)
            {
                Debug.WriteLine(wall.ptA.X / 32 + "/" + wall.ptA.Y / 32 + " " + wall.ptB.X / 32 + "/" + wall.ptB.Y / 32);
                if (wall.ptA.X == wall.ptB.X)
                {
                    int i = (int)wall.ptA.Y / 32;
                    int j = (int)wall.ptB.Y / 32;
                    if (i <= j)
                    {
                        while (i <= j)
                        {

                            lines[i][(int)wall.ptA.X / 32] = '-';
                            i++;
                        }
                    }
                    else
                    {
                        while (i >= j)
                        {
                            lines[i][(int)wall.ptA.X / 32] = '-';
                            i--;
                        }
                    }

                }
                if (wall.ptA.Y == wall.ptB.Y)
                {
                    int i = (int)wall.ptA.X / 32;
                    int j = (int)wall.ptB.X / 32;
                    Debug.WriteLine("choosed " + i + " " + j);
                    if (i <= j) {
                        while (i <= j)
                        {
                            lines[(int)wall.ptA.Y / 32][i] = '-';
                            i++;
                        }
                    } else {
                        while (i > j)
                        {
                            lines[(int)wall.ptA.Y / 32][i] = '-';
                            i--;
                        }
                    }
                }
            }
            using (StreamWriter writer = new StreamWriter("../../map/visualizer.txt"))
            {
                foreach (StringBuilder line in lines)
                {
                    Debug.WriteLine(line.ToString());
                    writer.WriteLine(line.ToString());
                }
                
            }

        }

        public List<StringBuilder> init(List<Wall> wallList)
        {
            List<StringBuilder> lines = new List<StringBuilder>();
            String line = "";
            float maxX = 0;
            float maxY = 0;
            foreach (Wall wall in wallList)
            {
                if (wall.ptA.X > maxX)
                {
                    maxX = wall.ptA.X;
                }
                if (wall.ptB.X > maxX)
                {
                    maxX = wall.ptB.X;
                }
                if (wall.ptA.Y > maxY)
                {
                    maxY = wall.ptA.Y;
                }
                if (wall.ptB.Y > maxY)
                {
                    maxY = wall.ptB.Y;
                }
            }
            if (maxX > 0)
            {
                maxX = (maxX / 32) + 3;
            }
            if (maxY > 0)
            {
                maxY = (maxY / 32) + 3;
            }
            Debug.WriteLine(maxX + " " + maxY); 
            for (int x = 0; x <= maxX; x++)
            {
                line += "X";
            }
            for (int y = 0; y <= maxY; y++)
            {
                lines.Add(new StringBuilder(line));
            }
            return lines;
        }
    }
}
