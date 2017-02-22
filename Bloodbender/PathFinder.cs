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
        public bool free;
        public float score;

        public PathFinderNode(Vector2 position)
        {
            neighbors = new List<PathFinderNode>();
            free = true;
            score = 0f;
            this.offset = new Vector2(0);
            this.position = position * Bloodbender.pixelToMeter;

            findNeighbors();
        }

        public PathFinderNode(Vector2 position, Vector2 offset)
        {
            neighbors = new List<PathFinderNode>();
            free = false;
            score = 0f;
            this.offset = offset;
            this.position = position * Bloodbender.pixelToMeter;
            this.position += offset;

            findNeighbors();
        }

        public void reset()
        {
            score = 0;
            parent = null;
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
                if (!node.Equals(this))
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

        public PathFinderNode pathRequest(GraphicObj startObj, PathFinderNode startNode, PathFinderNode endNode)
        {
            //Console.WriteLine("[PathFinder][Request] Start Request");
            //Console.WriteLine("[PathFinder][Request] StartNode pos : " + startNode.position * Bloodbender.meterToPixel + " EndNode pos : " + endNode.position * Bloodbender.meterToPixel);

            resultPath.Clear();
            openList.Clear();
            closedList.Clear();
            resetAllNodes();
            runAstar(startNode, endNode);

            pathDict[startObj] = new List<PathFinderNode>(resultPath);

            return resultPath[1];
        }

        private void runAstar(PathFinderNode startNode, PathFinderNode endNode)
        {
            PathFinderNode currentNode = startNode;

            openList.Add(startNode);
            while (openList.Any())
            {
                //Console.WriteLine("[PathFinder][Request] OpenList Loop");
                if (currentNode == endNode)
                {
                    createPath(currentNode, startNode);
                    break;
                }
                neighborsProcessing(currentNode, endNode);
                closedList.Add(currentNode);
                openList.Remove(currentNode);
                currentNode = findBestInOpenList();
                //Console.WriteLine("[PathFinder][Request] find best score : " + openList.Count);
            }
            //Console.WriteLine("[PathFinder][Request] Astar End");
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
                if (closedList.Contains(neighbour) == false)
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
                if (bestNode == null)
                    bestNode = node;
                else if (node.score < bestNode.score)
                    bestNode = node;
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

        public void removeNode(PathFinderNode nodeToRemove)
        {
            nodes.Remove(nodeToRemove);
        }

    }
}
