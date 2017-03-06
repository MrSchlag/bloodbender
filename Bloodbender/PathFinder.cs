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
using FarseerPhysics.Common.Decomposition;

namespace Bloodbender
{

    public class PathFinderNode
    {
        public List<PathFinderNode> neighbors;
        public PathFinderNode parent;
        public Vector2 position; //position en metre (Farseer Units)
        public Vector2 offset;
        public PhysicObj owner;
        public bool free;
        public float score;

        public PathFinderNode(Vector2 position, PhysicObj owner = null)
        {
            neighbors = new List<PathFinderNode>();
            free = true;
            score = 0f;
            offset = new Vector2(0);
            this.position = position * Bloodbender.pixelToMeter;
            this.owner = owner;

            findNeighbors();
        }

        public PathFinderNode(Vector2 position, Vector2 offset, PhysicObj owner = null)
        {
            neighbors = new List<PathFinderNode>();
            free = true;
            score = 0f;
            this.offset = offset;
            this.position = position * Bloodbender.pixelToMeter;
            this.position += offset;
            this.owner = owner;

            findNeighbors();
        }

        public void reset()
        {
            score = 0;
            parent = null;
            free = true;
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
    }

    public class PathFinder
    { 
        List<PathFinderNode> nodes;

        List<PathFinderNode> openList;
        List<PathFinderNode> closedList;
        List<PathFinderNode> resultPath;

        public int pathStep;
        List<Fixture> mapShapeFixtures;
        Body body;

        Dictionary<GraphicObj, List<PathFinderNode>> pathDict = new Dictionary<GraphicObj, List<PathFinderNode>>();

        public PathFinder(int pathStep)
        {
            body = BodyFactory.CreateBody(Bloodbender.ptr.world);
            body.BodyType = BodyType.Static;

            openList = new List<PathFinderNode>();
            closedList = new List<PathFinderNode>();
            mapShapeFixtures = new List<Fixture>();
            nodes = new List<PathFinderNode>();
            resultPath = new List<PathFinderNode>();

            this.pathStep = pathStep;
        }

        public void addNode(PathFinderNode node)
        {
            nodes.Add(node);
        }

        public List<PathFinderNode> pathRequest(GraphicObj startObj, PathFinderNode startNode, PathFinderNode endNode, List<PathFinderNode> ignoredNodes = null)
        {
            resultPath.Clear();
            openList.Clear();
            closedList.Clear();
            resetAllNodes();
            setIgnoredNodes(ignoredNodes);

            runAstar(startNode, endNode);

            if (pathDict.ContainsKey(startObj))
            {
                pathDict[startObj].Clear();
                pathDict[startObj].AddRange(resultPath);
            }
            else
            {
                pathDict[startObj] = new List<PathFinderNode>(resultPath);
            }

            return resultPath;
        }

        private void setIgnoredNodes(List<PathFinderNode> ignoredNodes)
        {
            if (ignoredNodes == null)
                return;
            foreach (PathFinderNode node in ignoredNodes)
            {
                node.free = false;
            }
        }

        private void runAstar(PathFinderNode startNode, PathFinderNode endNode)
        {
            PathFinderNode currentNode = startNode;

            openList.Add(startNode);
            while (openList.Any())
            {
                if (currentNode == endNode)
                {
                    createPath(currentNode, startNode);
                    break;
                }
                neighborsProcessing(currentNode, endNode);
                closedList.Add(currentNode);
                openList.Remove(currentNode);
                currentNode = findBestInOpenList();
                if (currentNode == null)
                {
                    resultPath = null;
                    break;
                }
            }
        }

        private void resetAllNodes()
        {
            foreach (PathFinderNode node in nodes)
            {
                node.reset();
            }
        }

        private void neighborsProcessing(PathFinderNode currentNode, PathFinderNode endNode)
        {
            float score = 0;

            foreach (PathFinderNode neighbour in currentNode.neighbors)
            {
                score = getNodeScore(currentNode, neighbour, endNode);
                if (closedList.Contains(neighbour) == false && neighbour.free == true)
                {
                    if (openList.Contains(neighbour) == false)
                    {
                        neighbour.parent = currentNode;
                        neighbour.score = score;
                        openList.Add(neighbour);
                    }
                    else if (score < neighbour.score)
                    {
                        neighbour.score = score;
                        neighbour.parent = currentNode;
                    }
                }
            }
        }

        private float getNodeScore(PathFinderNode currentNode, PathFinderNode neighbour, PathFinderNode endNode)
        {
            Vector2 curToNeigh = new Vector2(neighbour.position.X - currentNode.position.X, neighbour.position.Y - currentNode.position.Y);
            Vector2 neighToEnd = new Vector2(endNode.position.X - neighbour.position.X, endNode.position.Y - neighbour.position.Y);

            return curToNeigh.Length() + neighToEnd.Length();
        }

        private PathFinderNode findBestInOpenList()
        {
            PathFinderNode bestNode = null;

            foreach (PathFinderNode node in openList)
            {
                if (node.free == true)
                {
                    if (bestNode == null)
                        bestNode = node;
                    else if (node.score < bestNode.score)
                        bestNode = node;
                }
            }
            return bestNode;
        }

        private void createPath(PathFinderNode currentNode, PathFinderNode startNode)
        {
            PathFinderNode backPathNode = currentNode;

            while (backPathNode.Equals(startNode) == false)
            {
                resultPath.Add(backPathNode);
                backPathNode = backPathNode.parent;
            }

            resultPath.Add(startNode);
            resultPath.Reverse();

            foreach (PathFinderNode node in resultPath)
            {
                //Console.WriteLine("[PathFinder][Request][Result] PathNode : " + node.position * Bloodbender.meterToPixel);
            }
            //Console.WriteLine("[PathFinder][Request] Path Found");
        }

        public List<PathFinderNode> getPathFinderNodes()
        {
            return nodes;
        }

        public Dictionary<GraphicObj, List<PathFinderNode>> getCurrentPaths()
        {
            return pathDict;
        }

        public void removeNode(PathFinderNode nodeToRemove)
        {
            nodes.Remove(nodeToRemove);
        }

    }
}
