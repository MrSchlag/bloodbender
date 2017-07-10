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
using System.Threading;
using Bloodbender.PathFinding;

namespace Bloodbender
{
    public class FollowBehaviorComponent : IComponent
    {
        PhysicObj owner;
        PhysicObj target;

        float escapeZoneRadius;

        float pathRequestRate = 0.2f;
        float pathRequestRateCounter = 0f;

        //float ignoreNodeRadiusOffset = 5 * Bloodbender.pixelToMeter;

        PathFinderNode nextNode;
        List<PathFinderNode> path;
        
        float maxVertexDistance;
        float maxVertexDistanceOffset = 10 * Bloodbender.pixelToMeter;

        public FollowBehaviorComponent(PhysicObj obj, PhysicObj target, float escapeZoneRadius)
        {
            this.owner = obj;
            this.target = target;
            this.escapeZoneRadius = escapeZoneRadius;
            maxVertexDistance = maxLenghtCentroidVertex();
            nextNode = null;
            target.getPosNode().TriangleChangedEvent += FollowBehaviorComponent_TriangleChangedEvent;
        }

        private void FollowBehaviorComponent_TriangleChangedEvent()
        {
            path = Bloodbender.ptr.pFinder.pathRequest(owner, target);
        }

        bool IComponent.Update(float elapsed)
        {
            pathRequestRateCounter += elapsed;
            if (pathRequestRateCounter > pathRequestRate)
            {
                pathRequestRateCounter = 0f;
                Bloodbender.ptr.pFinder.UpdateTriangleForObj(target);
            }

            if (path == null)
            {
                owner.body.LinearVelocity = Vector2.Zero;
                return false;
            }

            if (path.Count() < 2) //fixe temporaire
                return true;

            var nextNode = path[1];

            Vector2 posToNode = nextNode.position - owner.getPosNode().position;
            Vector2 posToTarget = target.body.Position - owner.body.Position;

            if (posToNode.Length() < 0.3)
            {
                path.RemoveAt(1);
                posToNode = nextNode.position - owner.getPosNode().position;
                posToTarget = target.body.Position - owner.body.Position;
            }

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

            return true;
        }

        private bool velocityCorrection()
        {
            if (((AdditionalFixtureData)owner.getBoundsFixture().UserData).isTouching)
                Console.WriteLine("is Touching");
            if (owner.body.LinearVelocity == Vector2.Zero)
                return false;
            if (((AdditionalFixtureData)owner.getBoundsFixture().UserData).isTouching)
            {
                Vector2 velocityVec = owner.body.LinearVelocity;
                Console.WriteLine("veclocity correction raw " + velocityVec);

                velocityVec.Normalize();

                Console.WriteLine("veclocity correction normalize " + velocityVec);

                velocityVec *= owner.velocity * Bloodbender.pixelToMeter;
                Console.WriteLine("veclocity correction to " + velocityVec + " speed : " + velocityVec.Length());
                owner.body.LinearVelocity = velocityVec;

                return true;
            }
            return false;
        }

        private bool ignoreThisNodeForWidth(PathFinderNode nextNode)
        {
            float maxLength = maxLenghtCentroidVertex();
           
            return false;
        }

        private Vector2 correctNodePositionForBodyWidth(PathFinderNode node)
        {
            if (node.owner == null)
                return node.position;

            if (node.offset == Vector2.Zero)
                return node.position;

            Vector2 centerToVertexOffset = node.offset;
            centerToVertexOffset *= new Vector2(maxVertexDistance / centerToVertexOffset.Length());
            return node.position + centerToVertexOffset;
        }

        private float maxLenghtCentroidVertex()
        {
            float maxLenght = 0f;
            float lenght;
            Vertices vertices = ((PolygonShape)owner.getBoundsFixture().Shape).Vertices;

            if (vertices == null)
                return 0f;

            foreach (Vector2 vertex in vertices)
            {
                lenght = (vertex - vertices.GetCentroid()).Length();
                maxLenght = lenght > maxLenght ? lenght : maxLenght;
            }

            return maxLenght;
        }

        private float distanceWithPhysicObj(PhysicObj obj)
        {
            if (obj == null)
                return -1;
            Fixture proxyAfix = owner.getBoundsFixture();
            Fixture proxyBfix = obj.getBoundsFixture();
            if (proxyAfix == null || proxyBfix == null)
                return -1;

            DistanceProxy proxyA = new DistanceProxy();
            proxyA.Set(proxyAfix.Shape, 0);
            DistanceProxy proxyB = new DistanceProxy();
            proxyB.Set(proxyBfix.Shape, 1);
            DistanceInput distInput = new DistanceInput();
            distInput.ProxyA = proxyA;
            distInput.ProxyB = proxyB;
            Transform transformA;
            owner.body.GetTransform(out transformA);
            Transform transformB;
            obj.body.GetTransform(out transformB);
            distInput.TransformA = transformA;
            distInput.TransformB = transformB;

            DistanceOutput distout = new DistanceOutput();
            SimplexCache simplexCache = new SimplexCache();

            Distance.ComputeDistance(out distout, out simplexCache, distInput);

            return (distout.Distance);
        }

        public void changeTarget(PhysicObj target)
        {
            this.target = target;
        }
    }
}
