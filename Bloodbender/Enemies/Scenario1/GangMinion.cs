using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bloodbender.Enemies.Scenario1
{
    class GangMinion : Enemy
    {
        public GangMinion(Vector2 position, PhysicObj target) : base(position, target)
            { }
    }
}
