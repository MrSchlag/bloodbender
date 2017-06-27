using System.Collections.Generic;
using System.Linq;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;


namespace Bloodbender.PathFinding
{

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

        public PathFinderNode pathRequest(GraphicObj startObj, PathFinderNode startNode, PathFinderNode endNode, List<PathFinderNode> ignoredNodes = null)
        {
            resultPath.Clear();
            openList.Clear();
            closedList.Clear();
            resetAllNodes();

            //findNodeToIgnore();

            setIgnoredNodes(findNodeToIgnore(startObj));//ignoredNodes);

            //runAstar(startNode, endNode, (PhysicObj)startObj);
            runDjikstra(startNode, endNode);

            

            if (pathDict.ContainsKey(startObj))
            {
                pathDict[startObj].Clear();
                pathDict[startObj].AddRange(resultPath);
            }
            else
            {
                pathDict[startObj] = new List<PathFinderNode>(resultPath);
            }

            if (resultPath == null || resultPath.Count < 2)
                return null;

            if ((startObj is PhysicObj) == false)
                return resultPath[1];
            return resultPath[1].pathNodePositionCorrectedForWidth((PhysicObj)startObj);
        }

        private List<PathFinderNode> findNodeToIgnore(GraphicObj obj)
        {
            if ((obj is PhysicObj) == false)
                return null;
            if (pathDict.ContainsKey(obj) == false || pathDict[obj].Count <= 1)
                return null;
            if (pathDict[obj][1].isReached((PhysicObj)obj))
            {
                List<PathFinderNode> listIgnored = new List<PathFinderNode>();
                listIgnored.Add(pathDict[obj][1]);
                return listIgnored;
            }
            return null;
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

        private void mandatoryNodeControl(GraphicObj startObj)
        {
            if (resultPath == null || resultPath.Count <= 1 || pathDict.ContainsKey(startObj) == false)
            {
                //Console.WriteLine("[mandatory waypoint] : null error");
                return;
            }  
            if (resultPath[1].isMandatory() == false)
            {
                //Console.WriteLine("[mandatory waypoint] : mandatory false");
                return;
            }
            if (resultPath[1].isReached((PhysicObj)startObj))
            {
                resultPath.RemoveAt(1);
                //Console.WriteLine("[mandatory waypoint] : is reached");
                return;
            }
            if (pathDict[startObj][1].isMandatory() == false)
            {
                return;
            }
            //Console.WriteLine("[mandatory waypoint] : replace old path");
            resultPath.Clear();
            resultPath.AddRange(pathDict[startObj]);
            //Console.WriteLine("[mandatory waypoint] : count " + resultPath.Count);
        }

        private void runAstar(PathFinderNode startNode, PathFinderNode endNode, PhysicObj startObj)
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
                neighborsProcessing(currentNode, endNode, startObj);
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

        private void runDjikstra(PathFinderNode startNode, PathFinderNode endNode)
        {
            startNode.weight = 0;
            PathFinderNode currentNode = startNode;
            PathFinderNode bestNeighbour;

            while (currentNode != null && currentNode != endNode)
            {
                currentNode.used = true;
                bestNeighbour = null;
                foreach (PathFinderNode neighbour in currentNode.neighbors)
                {
                    if (neighbour.used == false)
                    {
                        uint linkWeight;
                        
                        /*
                        if (neighbour.Equals(endNode) && isWayClearToTarget(startNode.owner, endNode) == false)
                            linkWeight = uint.MaxValue;*/
                        
                        if (currentNode.Equals(startNode) && isWayClearToNode(startNode.owner, neighbour) == false)
                            linkWeight = uint.MaxValue;
                        else
                            linkWeight = getDjikstraWeight(currentNode, neighbour);

                        if (currentNode.weight + linkWeight < neighbour.weight)
                        {
                            neighbour.weight = currentNode.weight + linkWeight;
                            neighbour.parent = currentNode;
                        }

                        if (bestNeighbour == null || neighbour.weight < bestNeighbour.weight)
                            bestNeighbour = neighbour;
                    }
                }
                currentNode = bestNeighbour;
            }
            if (endNode != null)
                createPath(endNode, startNode);
        }

        private uint getDjikstraWeight(PathFinderNode current, PathFinderNode neighbour)
        {
            int manathanDistX = ((int)current.position.X * 100) - ((int)neighbour.position.X * 100);
            int manathanDistY = ((int)current.position.Y * 100) - ((int)neighbour.position.Y * 100);
            if (manathanDistX < 0)
                manathanDistX *= -1;
            if (manathanDistY < 0)
                manathanDistY *= -1;
            uint manathanDist = (uint)(manathanDistY + manathanDistX);
            return manathanDist;
        }

        private void resetAllNodes()
        {
            foreach (PathFinderNode node in nodes)
            {
                node.reset();
            }
        }

        private void neighborsProcessing(PathFinderNode currentNode, PathFinderNode endNode, PhysicObj startObj)
        {
            float score = 0;

            foreach (PathFinderNode neighbour in currentNode.neighbors)
            {
                //bool objOverlapNode = startObj.isPointInside(neighbour.pathNodePositionCorrectedForWidth(startObj).position);
                //bool test = isNeighbourWayClear(neighbour, startObj, endNode);
                //Console.WriteLine("[reached] overlap : " + objOverlapNode);


               // bool objOverlapNodeZone = isInOverlapZone();
                score = getNodeScore(currentNode, neighbour, endNode);

                if (closedList.Contains(neighbour) == false && neighbour.free == true /*&& objOverlapNode == false /*&& test == true*/)
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

        private bool isWayClearToTarget(PhysicObj startObj, PathFinderNode endNode)
        {
            Vertices objVertices = ((PolygonShape)startObj.getBoundsFixture().Shape).Vertices;
            bool isVisible = true;
            PhysicObj endObj = endNode.owner;

            foreach (Vector2 vertex in objVertices)
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
                    if (((AdditionalFixtureData)fixture.UserData).physicParent.Equals(startObj))
                        return -1;
                    if (((AdditionalFixtureData)fixture.UserData).physicParent.Equals(endObj))
                    {
                        //Console.WriteLine("oscarzzaijdizjdizajdiazjdiazj");
                        return 0;
                    }
                    isVisible = false;
                    return 0;
                }, vertex + startObj.body.Position, endNode.position);

                if (isVisible == false)
                    return false;
            }

            return true;
        }

        private bool isWayClearToNode(PhysicObj startObj, PathFinderNode node)
        {
            Vertices objVertices = ((PolygonShape)startObj.getBoundsFixture().Shape).Vertices;
            bool isVisible = true;

            foreach (Vector2 vertex in objVertices)
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
                    if (((AdditionalFixtureData)fixture.UserData).physicParent.Equals(startObj))
                        return -1;
                    isVisible = false;
                    return 0;
                }, vertex + startObj.body.Position, node.position);

                if (isVisible == false)
                    return false;
            }

            return true;
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
                if (backPathNode == null)
                    return;
            }

            resultPath.Add(startNode);
            resultPath.Reverse();
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
