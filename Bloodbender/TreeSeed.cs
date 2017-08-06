using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Bloodbender
{
    public class TreeSeed : PhysicObj
    {
        private float _stopTime;
        private float _stopTimer = -1;
        private float _minStopTime;
        private Vector2 _linearVelocity;
        private static Random rnd = new Random();

        public TreeSeed(Vector2 position, Vector2 direction, int minStopTime) : base(position * Bloodbender.meterToPixel, PathFinderNodeType.OTHER)
        {
            Fixture fix = FixtureFactory.AttachCircle(0.05f, 1, body);
            fix.UserData = new AdditionalFixtureData(this, HitboxType.BOUND);
            fix.OnCollision += CollisionSensor;
            _minStopTime = minStopTime;

            Vector2 posToCenter = direction;
            posToCenter.Normalize();
            posToCenter *= 50;
            _linearVelocity = posToCenter;

            body.LinearDamping = 0;
            body.Restitution = 1;

            Bloodbender.ptr.listGraphicObj.Add(this);
        }

        public override bool Update(float elapsed)
        {
            if (_stopTimer != -1 && _stopTimer > _stopTime)
            {
                body.LinearVelocity = Vector2.Zero;
                AddTree();
                _stopTimer = -1;
            }
            _stopTimer += elapsed;
            return true;
        }

        private void AddTree()
        {
            var tree = new GraphicObj(OffSet.BottomCenterHorizontal);
            tree.position = body.Position * Bloodbender.meterToPixel;
            //tree.position.Y += -128;
            //tree.position.X += -64;
            tree.addAnimation(new Animation(Bloodbender.ptr.Content.Load<Texture2D>("tree1")));
            Bloodbender.ptr.listGraphicObj.Add(tree);
        }

        public bool CollisionSensor(Fixture f1, Fixture f2, Contact contact)
        {
            if (_minStopTime == 0 && rnd.Next(1, 12) == 1)
                body.LinearVelocity = Vector2.Zero;
            else
            {
                _stopTime = rnd.Next((int)_minStopTime, 200 + (int)_minStopTime) / 1000f;
                _stopTimer = 0;
            }
            return true;
        }

        public void Run()
        {
            body.LinearVelocity = new Vector2(_linearVelocity.X + rnd.Next(-30, 30), _linearVelocity.Y + rnd.Next(-30, 30));
        }
    }
}
