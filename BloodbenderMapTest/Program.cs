using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using BloodbenderMapGenerator;

namespace BloodbenderMapTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Debug.WriteLine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            Debug.WriteLine("zobi");
            MapLoader mLoader = new MapLoader();
        }
    }
}
