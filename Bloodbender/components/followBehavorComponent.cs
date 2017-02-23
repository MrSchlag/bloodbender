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

        float pathRequestRate = 0.2f;
        float pathRequestRateCounter = 0f;

        public FollowBehaviorComponent(PhysicObj obj, PhysicObj target, float escapeZoneRadius)
        {
            this.owner = obj;
            this.target = target;
            this.escapeZoneRadius = escapeZoneRadius;
        }

        bool IComponent.Update(float elapsed)
        {
            pathRequestRateCounter += elapsed;
            if (pathRequestRateCounter > pathRequestRate)
            {
                pathRequestRateCounter = 0f;
                //Console.WriteLine("[FollowBehaviorComponent] asking path");
                PathFinderNode nextNode = Bloodbender.ptr.pathFinder.pathRequest(owner, owner.getPosNode(), target.getPosNode());
                Vector2 newNodePosCorrected = correctNodePositionForBodyWidth(nextNode);

                Vector2 posToNode = nextNode.position - owner.body.Position;
                //Vector2 posToNode = newNodePosCorrected - owner.body.Position;
                //Console.WriteLine("posToNode : " + posToNode);
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
                //Console.WriteLine("pos : " + owner.body.Position);
            }

            return true;
        }

        private Vector2 correctNodePositionForBodyWidth(PathFinderNode node)
        {
            if (node.offset == Vector2.Zero)
                return node.position;

            Vector2 centerToVertexOffset = node.offset;

            centerToVertexOffset *= new Vector2(maxLenghtCentroidVertex() / centerToVertexOffset.Length());

            //Console.WriteLine("[followComponenet] corrected : " + (node.position + centerToVertexOffset));
            //Console.WriteLine("[followComponenet] initial  : " + node.position);

            return node.position + centerToVertexOffset;
        }

        private float maxLenghtCentroidVertex()
        {
            float maxLenght = 0f;
            float lenght;
            Vertices vertices = null;
            
            foreach (Fixture fixBounds in owner.body.FixtureList)
            {
                if (((AdditionalFixtureData)fixBounds.UserData).type == HitboxType.BOUND)
                {
                    vertices = ((PolygonShape)fixBounds.Shape).Vertices;
                    break;
                }
            }

            if (vertices == null)
                return 0f;

            foreach (Vector2 vertex in vertices)
            {
                lenght = (vertex - vertices.GetCentroid()).Length();
                maxLenght = lenght > maxLenght ? lenght : maxLenght;
            }

            return maxLenght;
        }

        public void changeTarget(PhysicObj target)
        {
            this.target = target;
        }
    }
}
