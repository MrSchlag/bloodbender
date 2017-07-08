using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;

namespace MapGenerator
{
    class Visualizer
    {
        float maxX = 0;
        float minX = 0;
        float maxY = 0;
        float minY = 0;

        public void visualizeMap(List<Wall> wallList)
        {
            List<StringBuilder> lines = init(wallList);
            foreach (Wall wall in wallList)
            {
                if (wall.ptA.X == wall.ptB.X)
                {
                    int i = (int)(wall.ptA.Y / 32 + Math.Abs(minY));
                    int j = (int)(wall.ptB.Y / 32 + Math.Abs(minY));
                    if (i <= j)
                    {
                        while (i <= j)
                        {
                            lines[i][(int)(wall.ptA.X / 32 + Math.Abs(minX))] = '-';
                            i++;
                        }
                    }
                    else
                    {
                        while (i >= j)
                        {
                            lines[i][(int)(wall.ptA.X / 32 + Math.Abs(minX))] = '-';
                            i--;
                        }
                    }

                }
                if (wall.ptA.Y == wall.ptB.Y)
                {
                    int i = (int)(wall.ptA.X / 32 + Math.Abs(minX));
                    int j = (int)(wall.ptB.X / 32 + Math.Abs(minX));
                    if (i <= j)
                    {
                        while (i <= j)
                        {
                            lines[(int)(wall.ptA.Y / 32 + Math.Abs(minY))][i] = '-';
                            i++;
                        }
                    }
                    else
                    {
                        while (i > j)
                        {
                            // Debug.WriteLine(i + " " + j);
                            lines[(int)(wall.ptA.Y / 32 + Math.Abs(minY))][i] = '-';
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
            maxX = 0;
            minX = 0;
            maxY = 0;
            minY = 0;
            foreach (Wall wall in wallList)
            {
                if (wall.ptA.X > maxX)
                    maxX = wall.ptA.X;
                if (wall.ptB.X > maxX)
                    maxX = wall.ptB.X;
                if (wall.ptA.Y > maxY)
                    maxY = wall.ptA.Y;
                if (wall.ptB.Y > maxY)
                    maxY = wall.ptB.Y;
                if (wall.ptA.X < minX)
                    minX = wall.ptA.X;
                if (wall.ptB.X < minX)
                    minX = wall.ptB.X;
                if (wall.ptA.Y < minY)
                    minY = wall.ptA.Y;
                if (wall.ptB.Y < minY)
                    minY = wall.ptB.Y;
            }
            maxX = (maxX / 32) + 3;
            maxY = (maxY / 32) + 3;
            minX = (minX / 32) - 3;
            minY = (minY / 32) - 3;
            Debug.WriteLine("X {0}|{1}", minX, maxX);
            Debug.WriteLine("Y {0}|{1}", minY, maxY);
            for (int x = (int)minX; x <= maxX; x++)
                line += "X";
            Debug.WriteLine(line.Length);
            for (int y = (int)minY; y <= maxY; y++)
                lines.Add(new StringBuilder(line));
            Debug.WriteLine(lines.Count);
            return lines;
        }
    }
}
