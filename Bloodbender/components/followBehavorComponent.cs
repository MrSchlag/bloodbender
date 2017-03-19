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

        //float ignoreNodeRadiusOffset = 5 * Bloodbender.pixelToMeter;
        
        PathFinderNode nextNode;
        
        //PathFinderNode switchingPathControl;

        float maxVertexDistance;
        float maxVertexDistanceOffset = 10 * Bloodbender.pixelToMeter;

        public FollowBehaviorComponent(PhysicObj obj, PhysicObj target, float escapeZoneRadius)
        {
            this.owner = obj;
            this.target = target;
            this.escapeZoneRadius = escapeZoneRadius;
            maxVertexDistance = maxLenghtCentroidVertex();
            nextNode = null;
        }

        bool IComponent.Update(float elapsed)
        {
            pathRequestRateCounter += elapsed;
            if (pathRequestRateCounter > pathRequestRate)
            {
                pathRequestRateCounter = 0f;
                //Console.WriteLine("[FollowBehaviorComponent] asking path");

                /*
                if (ignoreThisNodeForWidth(nextNode))
                {
                    List<PathFinderNode> ignoreNodes = new List<PathFinderNode>();
                    ignoreNodes.Add(nextNode);
                    nextNode = Bloodbender.ptr.pathFinder.pathRequest(owner, owner.getPosNode(), target.getPosNode(), ignoreNodes);
                }
                else*/
                nextNode = Bloodbender.ptr.pathFinder.pathRequest(owner, owner.getPosNode(), target.getPosNode());
                //Console.WriteLine("[mandatory waypoint] nextnode : " + nextNode.position);
                //pathRequestAdjusted();
                //Vector2 newNodePosCorrected = correctNodePositionForBodyWidth(nextNode);

                Vector2 posToNode = nextNode.position - owner.getPosNode().position;
                //Vector2 posToNode = newNodePosCorrected - owner.body.Position;
                //Vector2 posToNode = shapeAvoidTrajectoryCorrection();
                Vector2 posToTarget = target.body.Position - owner.body.Position;
                
                //Console.WriteLine("speed : " + owner.body.LinearVelocity.Length());

                /*
                if (velocityCorrection() == true)
                {
                    return true;
                }*/

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

                //velocityCorrection();
                
                //Console.WriteLine("pos : " + owner.body.Position);
            }

            return true;
        }

        /*
        private Vector2 shapeAvoidTrajectoryCorrection()
        {
            List<PathFinderNode> path = Bloodbender.ptr.pathFinder.pathRequest(owner, owner.getPosNode(), target.getPosNode());

            nextNode = path[1];
            if (path.Count < 3 || path[1].owner == null || path[2].owner == null)
            {
                return nextNode.position - owner.getPosNode().position;
                //return Vector2.Zero;
            }

            nextNode = path[1];
            if (path[1].owner.pathNodeType == PathFinderNodeType.OUTLINE && path[2].owner.pathNodeType == PathFinderNodeType.OUTLINE)
            {
                if (distanceWithPhysicObj(path[1].owner) <= 4)//maxVertexDistance + maxVertexDistanceOffset + 0.5)
                {
                    Console.WriteLine("OOOKOKOKKO");
                    return path[2].position - path[1].position;
                    //owner.body.ve
                    //Vector2 correctedLinearVelocity = path[2].position - path[1].position;
                    //correctedLinearVelocity.Normalize();
                }
            }
            
            return nextNode.position - owner.getPosNode().position;
        }*/
        /*
        private void pathRequestAdjusted()
        {
            PathFinderNode posNode = owner.getPosNode();
            posNode.offset = Vector2.Zero;
            if (nextNode == null || nextNode.owner == null || nextNode.owner.pathNodeType != PathFinderNodeType.OUTLINE)
            {
                nextNode = Bloodbender.ptr.pathFinder.pathRequest(owner, owner.getPosNode(), target.getPosNode())[1];
                return;
            }
            
            Vector2 centroid = owner.getBoundsFixture().Shape.MassData.Centroid + owner.body.Position;
            //Vector2 centerToNextNodeVec = nextNode.position - centroid;//new Vector2(nextNode.position.X - centroid.X, nextNode.position.Y - centroid.Y);
            Vector2 centerToNextNodeVec = owner.body.LinearVelocity;
            centerToNextNodeVec.Normalize();
            centerToNextNodeVec *= maxVertexDistance;

            /*
            Vector2 rot1ToNextNode = nextNode.position - centerToNextNodeVec.Rotate((float)Math.PI / 2);
            Vector2 rot2ToNextNode = nextNode.position - centerToNextNodeVec.Rotate((float)Math.PI / -2);

            if (rot1ToNextNode.Length() > rot2ToNextNode.Length())
                posNode.offset = centerToNextNodeVec.Rotate((float)Math.PI / -2);
            else
                posNode.offset = centerToNextNodeVec.Rotate((float)Math.PI / 2);
            
            posNode.offset = centerToNextNodeVec;
            
            //DistanceOutput distanceToNextNodeOwner = distanceWithPhysicObj(nextNode.owner);
            //posNode.offset = distanceToNextNodeOwner.PointA - posNode.position;

            nextNode = Bloodbender.ptr.pathFinder.pathRequest(owner, owner.getPosNode(), target.getPosNode())[1];
            
        }*/

        private bool velocityCorrection()
        {
            if (((AdditionalFixtureData)owner.getBoundsFixture().UserData).isTouching)
                Console.WriteLine("is TOuching");
            if (owner.body.LinearVelocity == Vector2.Zero)
                return false;
            if (((AdditionalFixtureData)owner.getBoundsFixture().UserData).isTouching)//owner.isTouching(nextNode.owner))
            {
                //owner.body.
                
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
            /*
            if (nextNode != null && maxLength + ignoreNodeRadiusOffset >= (nextNode.position - owner.body.Position).Length())
            {
                Console.WriteLine("escape");
                return true;
            }*/
            return false;
        }

        private Vector2 correctNodePositionForBodyWidth(PathFinderNode node)
        {
            if (node.owner == null)
                return node.position;

            if (node.offset == Vector2.Zero)
                return node.position;

            //float distanceWithObj = distanceWithPhysicObj(node.owner);

            //distanceWithObj += 20 * Bloodbender.pixelToMeter;

            //if (distanceWithObj <= maxVertexDistance)
            //{
                Vector2 centerToVertexOffset = node.offset;
                centerToVertexOffset *= new Vector2(maxVertexDistance / centerToVertexOffset.Length());
                return node.position + centerToVertexOffset;
            //}

            return node.position;

            //Console.WriteLine("[followComponenet] corrected : " + (node.position + centerToVertexOffset));
            //Console.WriteLine("[followComponenet] initial  : " + node.position);

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
