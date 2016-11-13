using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarseerPhysics.Collision;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bloodbender
{
    public enum hitboxType
    {
        BOUND = 0,
        ATTACK = 1
    }

    public class HitboxData
    {
        public PhysicObj physicParent;
        public hitboxType type;
        public bool isTouching;
        public List<Fixture> fixInContactList;

        public HitboxData(PhysicObj parent, hitboxType type)
        {
            physicParent = parent;
            this.type = type;
            isTouching = false;
            fixInContactList = new List<Fixture>();
        }
    }

    public class PhysicObj : GraphicObj
    {
        public Body body;
        public Body size;
        public float velocity;

        public PhysicObj(Body body, Vector2 position, uint animNbr = 1) : base(animNbr)
        {
            velocity = 0;
            this.body = body;
            this.body.Position = position * Bloodbender.ptr.pixelToMeter;
            this.body.BodyType = BodyType.Dynamic;
            this.body.FixedRotation = true;
            this.body.LinearDamping = 1;
            this.body.AngularDamping = 1;
        }

        public PhysicObj(Vector2 position, uint animNbr = 1) : base(animNbr)
        {
            velocity = 0;
            body = BodyFactory.CreateBody(Bloodbender.ptr.world);
            body.Position = position * Bloodbender.ptr.pixelToMeter;
            body.BodyType = BodyType.Dynamic;
            body.FixedRotation = true;
            body.LinearDamping = 0.02f;
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

        public bool collisionSensor(Fixture f1, Fixture f2, Contact contact)
        {
            ((HitboxData)f1.UserData).isTouching = true;
            ((HitboxData)f1.UserData).fixInContactList.Add(f2);
            return true;
        }

        public void separationSensor(Fixture f1, Fixture f2)
        {
            HitboxData f1data = (HitboxData)f1.UserData;
            HitboxData f2data = (HitboxData)f2.UserData;
            Fixture fixToRemove = null;

            if (f1data == null || f2data == null)
                return;

            foreach (Fixture fixSearched in f1data.fixInContactList)
            {
                if (fixSearched == f2)
                {
                    fixToRemove = fixSearched;
                    break;
                }
            }

            f1data.fixInContactList.Remove(fixToRemove);
            if (f1data.fixInContactList.Count == 0)
                f1data.isTouching = false;
        }

        public void setLinearDamping(float damping)
        {
            body.LinearDamping = damping;
        }

        public void isRotationFixed(bool state)
        {
            body.FixedRotation = state;
        }

        public void setBodyType(BodyType type)
        {
            body.BodyType = type;
        }
    }
}
