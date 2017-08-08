using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapGenerator
{
    public class Entity
    {
        public Vector2 position { get; set; }

        public Entity(Vector2 position)
        {
            this.position = position;
        }
    }
}
