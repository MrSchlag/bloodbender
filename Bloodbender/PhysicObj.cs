using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarseerPhysics.Collision;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bloodbender
{
    public enum HitboxType
    {
        BOUND = 0,
        ATTACK = 1
    }

    public class AdditionalFixtureData
    {
        public PhysicObj physicParent;
        public HitboxType type;
        public bool isTouching;
        public List<Fixture> fixInContactList;

        public AdditionalFixtureData(PhysicObj parent, HitboxType type)
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
        //public Body size;
        public float velocity;
        public float lenght;

        public PhysicObj(Body body, Vector2 position) : base(OffSet.BottomCenterHorizontal)
        {
            velocity = 0;
            this.body = body;
            this.body.Position = position * Bloodbender.pixelToMeter;
            this.body.BodyType = BodyType.Dynamic;
            this.body.FixedRotation = true;
            this.body.LinearDamping = 1;
            this.body.AngularDamping = 1;
            this.lenght = 0;
        }

        public PhysicObj(Vector2 position) : base(OffSet.BottomCenterHorizontal)
        {
            velocity = 0;
            body = BodyFactory.CreateBody(Bloodbender.ptr.world);
            body.Position = position * Bloodbender.pixelToMeter;
            body.BodyType = BodyType.Dynamic;
            body.FixedRotation = true;
            body.LinearDamping = 0.02f;
            body.AngularDamping = 1;
        }

       

        public override bool Update(float elapsed)
        {
            position = body.Position * Bloodbender.meterToPixel;

            

            return base.Update(elapsed);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public bool collisionBounds(Fixture f1, Fixture f2, Contact contact)
        {
            //System.Diagnostics.Debug.Print("collision bounds handler");
            if (isCollisionPossibleByHeight(f1, f2) == false)
            {
                return false;
            }
            addFixtureOnCollision(f1, f2);
            return true;
        }

        public void separationBounds(Fixture f1, Fixture f2)
        {
            removeFixtureOnSeparation(f1, f2);
        }

        public bool collisionSensor(Fixture f1, Fixture f2, Contact contact)
        {
            if (isCollisionPossibleByHeight(f1, f2) == false)
            {
                return false;
            }
            addFixtureOnCollision(f1, f2);
            return true;
        }

        public void separationSensor(Fixture f1, Fixture f2)
        {
            removeFixtureOnSeparation(f1, f2);
        }

        /* attribut les handlers de collision en fonction du type de la fixture */
        protected void addFixtureToCheckedCollision(Fixture fix)
        {
            if (fix.IsSensor == true)
            {
                fix.OnCollision += collisionSensor;
                fix.OnSeparation += separationSensor;
            }
            else
            {
                fix.OnCollision += collisionBounds;
                fix.OnSeparation += separationBounds;
            }
        }

        private bool isCollisionPossibleByHeight(Fixture f1, Fixture f2)
        {
            if ((AdditionalFixtureData)f1.UserData == null || (AdditionalFixtureData)f2.UserData == null)
                return true;

            PhysicObj p1 = ((AdditionalFixtureData)f1.UserData).physicParent;
            PhysicObj p2 = ((AdditionalFixtureData)f2.UserData).physicParent;

            float p1Top = (p1.height * Bloodbender.pixelToMeter) + p1.lenght;
            float p1Down = p1.height * Bloodbender.pixelToMeter;
            float p2Top = (p2.height * Bloodbender.pixelToMeter) + p2.lenght;
            float p2Down = p2.height * Bloodbender.pixelToMeter;

            if (p1Top < p2Down || p2Top < p1Down)
                return false;
            return true;
        }

        private void addFixtureOnCollision(Fixture f1, Fixture f2)
        {
            ((AdditionalFixtureData)f1.UserData).isTouching = true;
            ((AdditionalFixtureData)f1.UserData).fixInContactList.Add(f2);
        }

        private void removeFixtureOnSeparation(Fixture f1, Fixture f2)
        {
            AdditionalFixtureData f1data = (AdditionalFixtureData)f1.UserData;
            AdditionalFixtureData f2data = (AdditionalFixtureData)f2.UserData;
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

        public Fixture createRectangleFixture(float width, float height, Vector2 transalationVector, AdditionalFixtureData userData = null)
        {
            //Create rectangles shapes
            Vertices rectangleVertices = PolygonTools.CreateRectangle((width / 2) * Bloodbender.pixelToMeter, (height / 2) * Bloodbender.pixelToMeter);
            PolygonShape rectangleShape = new PolygonShape(rectangleVertices, 1);
            //Transalte rectangles shapes to set there positions
            rectangleShape.Vertices.Translate(transalationVector * Bloodbender.pixelToMeter);
            //Bind body to shpes (create a compound body) and return
            return (body.CreateFixture(rectangleShape, userData));
        }

    }
}
