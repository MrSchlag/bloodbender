using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Bloodbender.PathFinding
{
    public class PathProcessor
    {
        List<PathFinderNode> openList;
        List<PathFinderNode> closedList;
        //List<PathFinderNode> resultPath;

        public PathProcessor()
        {
            openList = new List<PathFinderNode>();
            closedList = new List<PathFinderNode>();
            //resultPath = new List<PathFinderNode>();
        }

        private void Clear()
        {
            openList.Clear();
            closedList.Clear();
            //resultPath.Clear();
        }

        public List<PathFinderNode> runDjikstra(PathFinderNode startNode, PathFinderNode endNode)
        {
            Clear();
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

                        //if (currentNode.Equals(startNode))
                          //  linkWeight = uint.MaxValue;
                        //else
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
                return createPath(endNode, startNode);
            return null;
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

        private List<PathFinderNode> createPath(PathFinderNode currentNode, PathFinderNode startNode)
        {
            var resultPath = new List<PathFinderNode>();
            PathFinderNode backPathNode = currentNode;

            while (backPathNode.Equals(startNode) == false)
            {
                resultPath.Add(backPathNode);
                backPathNode = backPathNode.parent;
                if (backPathNode == null)
                    break;
            }

            resultPath.Add(startNode);
            resultPath.Reverse();
            return resultPath;
        }

        public static bool isWayClearToNode(PhysicObj startObj, PathFinderNode node)
        {
            Vertices objVertices = ((PolygonShape)startObj.getBoundsFixture()?.Shape)?.Vertices;
            if (((AdditionalFixtureData)startObj.body.FixtureList[0].UserData).isTouching == true)
                return false;
            if (objVertices == null)
                return false;
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
    }
}
