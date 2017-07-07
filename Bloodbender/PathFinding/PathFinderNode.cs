using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Bloodbender;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarseerPhysics.Dynamics;

namespace Bloodbender.PathFinding
{
    public delegate void TargetChangeTriangle();

    public class PathFinderNode
    {

        /* identity */
        public static int IdInc = 0;
        public int Id;
        public int NavId { get; set; }

        public List<PathFinderNode> neighbors;
        public PathFinderNode parent;
        public Vector2 position; //position en metre (Farseer Units)
        public Vector2 offset;
        public PhysicObj owner;
        public Vector2 DivergencePoint { get; set; }

        /* Astar */
        public bool free;
        public float score;
        private bool mandatoryWaypoint;

        /* Djikstra */
        public uint weight;
        public bool used;

        private NodeTriangle _nodeTriangle;
        public NodeTriangle NodeTriangle {
            get { return _nodeTriangle; }
            set
            {
                if (_nodeTriangle != value)
                {
                    _nodeTriangle = value;
                    TriangleChangedEvent?.Invoke();
                }
            }
        }

        public PathFinderNode(Vector2 position, PhysicObj owner = null, bool notConnected = false)
        {
            Id = IdInc;
            IdInc++;
            neighbors = new List<PathFinderNode>();
            free = true;
            score = 0f;
            offset = new Vector2(0);
            this.position = position * Bloodbender.pixelToMeter;
            this.owner = owner;
            reset();

        }

        public PathFinderNode(Vector2 position, Vector2 offset, PhysicObj owner = null)
        {
            Id = IdInc;
            IdInc++;
            neighbors = new List<PathFinderNode>();
            free = true;
            score = 0f;
            this.offset = offset;
            this.position = position * Bloodbender.pixelToMeter;
            this.position += offset;
            this.owner = owner;
            reset();
        }

        public PathFinderNode Copy()
        {
            NavId = Id;
            return new PathFinderNode(position * Bloodbender.meterToPixel) { DivergencePoint = DivergencePoint, NavId = Id };
        }

        public void reset()
        {
            score = 0;
            parent = null;
            free = true;
            weight = uint.MaxValue;
            used = false;
        }

        public void setPosition(Vector2 position)
        {
            this.position = position;
            this.position += offset;
            foreach (PathFinderNode neighbour in neighbors)
            {
                neighbour.neighbors.Remove(this);
            }
            neighbors.Clear();
            findNeighbors();
        }

        public void SetPosition(Vector2 position)
        {
            this.position = position;
            /*
            foreach (PathFinderNode neighbour in neighbors)
            {
                neighbour.neighbors.Remove(this);
            }*/
            //neighbors.Clear();
            //findNeighbors2(navMesh);
        }

        public void FindNeighbors(NavMesh rayCastNavmesh, NavMesh navmesh)
        {
            List<PathFinderNode> allNodes = navmesh.Nodes;
            bool isVisible;

            foreach (PathFinderNode node in rayCastNavmesh.Nodes)
            {
                isVisible = true;
                if (!node.Equals(this) && node.position != position)
                {
                    Bloodbender.ptr.world.RayCast((fixture, point, normal, fraction) =>
                    {
                        if (fixture.UserData == null)
                        {
                            isVisible = false;
                            return 0;
                        }
                        if (fixture.IsSensor || ((AdditionalFixtureData)fixture.UserData).physicParent.pathNodeType == PathFinderNodeType.CENTER)
                            return -1;
                        isVisible = false;
                        return 0;
                    }, position, node.position);

                    if (isVisible == true)
                    {
                        var nodeEquivalent = navmesh.GetEquivalentNode(node);
                        if (!neighbors.Contains(nodeEquivalent))
                            neighbors.Add(nodeEquivalent);
                        if (!nodeEquivalent.neighbors.Contains(nodeEquivalent))
                            nodeEquivalent.neighbors.Add(this);
                    }
                }
            }
        }

        private void findNeighbors()
        {
            List<PathFinderNode> allNodes = Bloodbender.ptr.pathFinder.getPathFinderNodes();
            bool isVisible;

            foreach (PathFinderNode node in allNodes)
            {
                isVisible = true;
                if (!node.Equals(this) && node.position != position)
                {
                    Bloodbender.ptr.world.RayCast((fixture, point, normal, fraction) =>
                    {
                        if (fixture.UserData == null)
                        {
                            isVisible = false;
                            return 0;
                        }
                        if (fixture.IsSensor || ((AdditionalFixtureData)fixture.UserData).physicParent.pathNodeType == PathFinderNodeType.CENTER)
                            return -1;
                        isVisible = false;
                        return 0;
                    }, position, node.position);

                    if (isVisible == true)
                    {
                        if (!neighbors.Contains(node))
                            neighbors.Add(node);
                        if (!node.neighbors.Contains(node))
                            node.neighbors.Add(this);
                    }
                }
            }
        }

        private void setMandatoryWaypoint()
        {
            if (owner == null)
            {
                mandatoryWaypoint = false;
            }
            else if (owner.pathNodeType == PathFinderNodeType.OUTLINE)
            {
                mandatoryWaypoint = true;
            }
            else
            {
                mandatoryWaypoint = false;
            }
        }

        public bool isReached(PhysicObj obj)
        {
            Vector2 objPos = obj.position * Bloodbender.pixelToMeter;
            /*
            Vector2 objPosNodeCorrectedCenterVec = pathNodePositionCorrectedForWidth(obj).position - objPos;
            Vector2 objPosNodeCenterVec = position - objPos;
            */
            Vector2 correctedPos = pathNodePositionCorrectedForWidth(obj).position;
            Vector2 midPoint = new Vector2((position.X + correctedPos.X) / 2, (position.Y + correctedPos.Y) / 2);

            Vector2 objPosNodeMidPoint = midPoint - objPos;

            //Console.WriteLine("[Corrected node] lenght control : " + objPosNodeMidPoint.Length() + " " + obj.maxLenghtCentroidVertex());

            if (objPosNodeMidPoint.Length() <= obj.maxLenghtCentroidVertex())
                return true;

            /*
            if (objPosNodeCorrectedCenterVec.Length() < obj.maxLenghtCentroidVertex())
            {
                Console.WriteLine("[reached] reached");
                return true;
            }

            if (objPosNodeCenterVec.Length() < obj.maxLenghtCentroidVertex())
            {
                return true;
            }*/



            return false;
        }

        private float pathNodeRayCastCallback(Fixture arg1, Vector2 arg2, Vector2 arg3, float arg4)
        {
            throw new NotImplementedException();
        }

        private int pathNodeRayCastCallback(Fixture fix, Vector2 vec1, Vector2 vec2, float fl1, float fl2)
        {
            return 0;
        }

        public void remove()
        {
            foreach (PathFinderNode neighbour in neighbors)
            {
                neighbour.neighbors.Remove(this);
            }
            neighbors.Clear();
            Bloodbender.ptr.pathFinder.removeNode(this);
        }

        public bool isMandatory()
        {
            return mandatoryWaypoint;
        }

        public PathFinderNode pathNodePositionCorrectedForWidth(PhysicObj startObj)
        {
            if (owner == null)
                return this;

            if (offset == Vector2.Zero)
                return this;

            Vector2 centerToVertexOffset = new Vector2(offset.X, offset.Y);
            centerToVertexOffset *= new Vector2((float)Math.Sqrt(startObj.maxLenghtCentroidVertex() * 2) / centerToVertexOffset.Length());

            PathFinderNode correctedNode = new PathFinderNode((position + centerToVertexOffset) * Bloodbender.meterToPixel, null, true);

            return correctedNode;
        }

        public event TargetChangeTriangle TriangleChangedEvent;

    }
}
