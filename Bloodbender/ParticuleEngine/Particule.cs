using Microsoft.Xna.Framework;
using System;

namespace Bloodbender.ParticuleEngine
{
    public abstract class Particule : GraphicObj
    {
        public float speed;
        public bool inWait;
        public Particule(Vector2 position, float speed) : base(OffSet.Center) { this.position = position; this.speed = speed; }
    }
}
