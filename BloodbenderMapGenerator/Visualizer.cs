using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BloodbenderMapGenerator
{
    class Visualizer
    {
        public Visualizer(List<Wall> mapList)
        {
            Debug.WriteLine("yoyoy");
            List<String> lines = new List<String>();
            for (int i = 0; i < 50; i++)
            {
                lines.Add("###########################################################################");
                Debug.WriteLine(lines[0]);
            }
           
        }
    }
}
