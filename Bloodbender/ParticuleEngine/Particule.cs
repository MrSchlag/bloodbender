using Microsoft.Xna.Framework;
using System;

namespace Bloodbender.ParticuleEngine
{
    public abstract class Particule : GraphicObj
    {
        public Particule() : base(OffSet.Center)
        {

        }

        public abstract void reset();
    }
}
