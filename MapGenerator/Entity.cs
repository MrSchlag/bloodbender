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
        public string type { get; set; }
        public int chiefId { get; set; }
        public int numberMinion { get; set; }

        public Entity(Vector2 position, string type)
        {
            this.position = position;
            this.type = type;
        }

        public Entity(Vector2 position, string type, int chiefId, int numberMinion)
        {
            this.position = position;
            this.type = type;
            this.chiefId = chiefId;
            this.numberMinion = numberMinion;
        }
    }
}
