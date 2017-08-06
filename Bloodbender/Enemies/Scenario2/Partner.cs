using Microsoft.Xna.Framework;

namespace Bloodbender.Enemies.Scenario2
{
    public class Partner : Enemy
    {
        public Partner(Vector2 position, PhysicObj target) : base(position, target)
        {
            height = 0;
        }
    }
}
