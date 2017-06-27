using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodbender.PathFinding
{
    public class PathProcessor
    {
        List<PathFinderNode> nodes;

        List<PathFinderNode> openList;
        List<PathFinderNode> closedList;
        List<PathFinderNode> resultPath;

        public PathProcessor()
        {
            nodes = new List<PathFinderNode>();
            openList = new List<PathFinderNode>();
            closedList = new List<PathFinderNode>();
            resultPath = new List<PathFinderNode>();
        }

        private void Clear()
        {
            openList.Clear();
            closedList.Clear();
            resultPath.Clear();
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

                        if (currentNode.Equals(startNode))
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
            PathFinderNode backPathNode = currentNode;

            while (backPathNode.Equals(startNode) == false)
            {
                resultPath.Add(backPathNode);
                backPathNode = backPathNode.parent;
                if (backPathNode == null)
                    return null;
            }

            resultPath.Add(startNode);
            resultPath.Reverse();
            return resultPath;
        }
    }
}
