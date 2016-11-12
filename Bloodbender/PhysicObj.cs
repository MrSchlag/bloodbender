using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bloodbender
{
    public class HitboxData
    {
        public PhysicObj physicParent;
        public uint type;

        public HitboxData(PhysicObj parent, uint type)
        {
            physicParent = parent;
            this.type = type;
        }
    }
    
    public class PhysicObj : GraphicObj
    {
        public Body body;
        public Body size;

        public PhysicObj(Body body, Vector2 position, uint animNbr = 1) : base(animNbr)
        {
            this.body = body;
            this.body.Position = position * Bloodbender.ptr.pixelToMeter;
            this.body.BodyType = BodyType.Dynamic;
            this.body.FixedRotation = true;
            this.body.LinearDamping = 1;
            this.body.AngularDamping = 1;
        }

        public PhysicObj(Vector2 position, uint animNbr = 1) : base(animNbr)
        {
            body = BodyFactory.CreateBody(Bloodbender.ptr.world);
            body.Position = position * Bloodbender.ptr.pixelToMeter;
            body.BodyType = BodyType.Dynamic;
            body.FixedRotation = true;
            body.LinearDamping = 1;
            body.AngularDamping = 1;
        }

        public override bool Update(float elapsed)
        {
            return base.Update(elapsed);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            position = body.Position * Bloodbender.ptr.meterToPixel;
            rotation = body.Rotation;

            base.Draw(spriteBatch);
        }

        public void setLinearDamping(float damping)
        {
            body.LinearDamping = damping;
        }

        public void canRotate(bool state)
        {
            body.FixedRotation = state;
        }

        public void setBodyType(BodyType type)
        {
            body.BodyType = type;
        }
    }
}
