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
    public class FollowBehaviorComponent : IComponent
    {
        PhysicObj owner;
        PhysicObj target;

        float escapeZoneRadius;

        float pathRequestRate = 0.1f;
        float pathRequestRateCounter = 0f;

        public FollowBehaviorComponent(PhysicObj obj, PhysicObj target, float escapeZoneRadius)
        {
            this.owner = obj;
            this.target = target;
            this.escapeZoneRadius = escapeZoneRadius;
        }

        void IComponent.Update(float elapsed)
        {
            pathRequestRateCounter += elapsed;
            if (pathRequestRateCounter > pathRequestRate)
            {
                pathRequestRateCounter = 0f;
                //Console.WriteLine("[FollowBehaviorComponent] asking path");
                PathFinderNode nextNode = Bloodbender.ptr.pathFinder.pathRequest(owner, owner.getPosNode(), target.getPosNode());

                Vector2 posToNode = nextNode.position - owner.body.Position;
                Vector2 posToTarget = target.body.Position - owner.body.Position;
                if (posToTarget.Length() * Bloodbender.meterToPixel > escapeZoneRadius)
                {
                    posToNode.Normalize();
                    posToNode *= owner.velocity * Bloodbender.pixelToMeter;
                    owner.body.LinearVelocity = posToNode;
                }
                else
                {
                    owner.body.LinearVelocity = Vector2.Zero;
                }
            }
        }

        public void changeTarget(PhysicObj target)
        {
            this.target = target;
        }
    }
}
