using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodbender.ParticuleEngine.Particules
{
    class GreenCercle : Particule
    {
        public GreenCercle(Vector2 position, float speed) : base(position, speed)
        {
            Texture2D texture = Bloodbender.ptr.Content.Load<Texture2D>("cercle_vert");
            addAnimation(new Animation(texture));
        }
    }
}
