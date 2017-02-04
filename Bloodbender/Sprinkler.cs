using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarseerPhysics.Common.PolygonManipulation;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common.TextureTools;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bloodbender
{
    public class Sprinkler : PhysicObj
    {
        public Sprinkler(Vector2 position) : base(position)
        {
            DirectionalBlastComponent blastComp = new DirectionalBlastComponent(this, 300,
                -0.349066f - (float)Math.PI / 2, 16, 3, new Vector2(50, 0), 2);
            addComponent(blastComp);
        }

        public override bool Update(float elapsed)
        {
            return base.Update(elapsed);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}