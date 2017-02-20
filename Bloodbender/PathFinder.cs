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
        public bool free;
        public int score;

        public PathFinderNode(Vector2 position)
        {
            neighbors = new List<PathFinderNode>();
            free = false;
            score = 0;

            this.position = position * Bloodbender.pixelToMeter;

            GraphicObj debug = new GraphicObj();
            debug.position = position;
            debug.addAnimation(new Animation(Bloodbender.ptr.debugNodeFree));
            Bloodbender.ptr.listGraphicObj.Add(debug);
        }

        public void reset()
        {
            score = 0;
            parent = null;
        }
    }

    public class PathFinder
    { //TODO : mettre a jour les nodes dans un autre thread
        MapBound mapBounds;
        List<List<PathFinderNode>> allNodes;

        List<PathFinderNode> nodes;

        List<PathFinderNode> openList;
        List<PathFinderNode> closedList;

        int pathStep;
        List<Fixture> mapShapeFixtures;
        Body body;

        public PathFinder(int pathStep)
        {
            body = BodyFactory.CreateBody(Bloodbender.ptr.world);
            body.BodyType = BodyType.Static;
            allNodes = new List<List<PathFinderNode>>();
            openList = new List<PathFinderNode>();
            closedList = new List<PathFinderNode>();
            mapShapeFixtures = new List<Fixture>();
            nodes = new List<PathFinderNode>();
            this.pathStep = pathStep;

            //createMapShape();
            //createGridOverlay();
            //destroyMapShape();
            //createNodeLinks();
        }

        public void addNode(PathFinderNode node)
        {
            nodes.Add(node);
        }

        private void createNodeLinks()
        {
            for (int i = 0; i < allNodes.Count(); i++)
            {
                for (int j = 0; j < allNodes[i].Count(); j++)
                {
                    nodeLink(allNodes[i][j], i - 1, j - 1);
                    nodeLink(allNodes[i][j], i, j - 1);
                    nodeLink(allNodes[i][j], i + 1, j - 1);
                    nodeLink(allNodes[i][j], i + 1, j);
                    nodeLink(allNodes[i][j], i + 1, j + 1);
                    nodeLink(allNodes[i][j], i, j + 1);
                    nodeLink(allNodes[i][j], i - 1, j + 1);
                    nodeLink(allNodes[i][j], i - 1, j);
                }
            }
        }

        private void nodeLink(PathFinderNode currentNode, int xPosNodeToLink, int yPosNodeToLink)
        {
            if (xPosNodeToLink < 0 || yPosNodeToLink < 0 ||
                xPosNodeToLink >= allNodes.Count() ||
                yPosNodeToLink >= allNodes[0].Count())
                return;
            foreach (PathFinderNode neighbour in currentNode.neighbors)
            {
                if (neighbour.Equals(allNodes[xPosNodeToLink][yPosNodeToLink]))
                    return;
            }
            currentNode.neighbors.Add(allNodes[xPosNodeToLink][yPosNodeToLink]);
        }

        public float pathRequest(GraphicObj objRequest, GraphicObj objTarget)//, GraphicObj objTarget)
        {
            PathFinderNode startNode = findClosestNode(objRequest.position);
            PathFinderNode targetNode = findClosestNode(objTarget.position);

            resetAllNodes();
            runAstar(startNode, targetNode);

            return 0;
        }

        public float pathRequest(Vector2 startPos, Vector2 endPos)
        {
            PathFinderNode startNode = findClosestNode(startPos);
            PathFinderNode targetNode = findClosestNode(endPos);

            Console.WriteLine("[PathFinder] StartNode pos : " + startNode.position * Bloodbender.meterToPixel + " EndNode pos : " + targetNode.position * Bloodbender.meterToPixel);
            Console.WriteLine("[PathFinder] StartNode free : " + startNode.free + " EndNode free : " + targetNode.free);

            resetAllNodes();
            runAstar(startNode, targetNode);

            return 0;
        }

        private PathFinderNode findClosestNode(Vector2 position)
        {
            int nodeX = ((int)position.X - (int)mapBounds.minX) / pathStep;
            int nodeY = ((int)position.Y - (int)mapBounds.minY) / pathStep;

            nodeX = nodeX < 0 ? 0 : nodeX;
            nodeY = nodeY < 0 ? 0 : nodeY;

            nodeX = nodeX > allNodes.Count() - 1 ? allNodes.Count() - 1 : nodeX;
            nodeY = nodeY > allNodes.Count() - 1 ? allNodes.Count() - 1 : nodeY;

            return allNodes[nodeX][nodeY];
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
                neighborsProcessing(currentNode);
                closedList.Add(currentNode);
                openList.Remove(currentNode);
                currentNode = findBestInOpenList();
            }
            Console.WriteLine("[PathFinder] : Astar End");
        }

        private void resetAllNodes()
        {
            foreach (List<PathFinderNode> listNode in allNodes)
                foreach (PathFinderNode node in listNode)
                    node.reset();
        }

        private void neighborsProcessing(PathFinderNode currentNode)
        {
            int score = 0;

            foreach (PathFinderNode neighbour in currentNode.neighbors)
            {
                if (neighbour.free == false || closedList.Contains(neighbour))
                    continue;
                score = getNodeScore(currentNode, neighbour);
                if (openList.Contains(neighbour) == false)
                {
                    neighbour.parent = currentNode;
                    neighbour.score = score;
                    openList.Add(neighbour);
                }
                else if (score > neighbour.score)
                {
                    neighbour.score = score;
                    neighbour.parent = currentNode;
                }
            }
        }

        private int getNodeScore(PathFinderNode currentNode, PathFinderNode neighbour)
        {
            int disty = (int)neighbour.position.Y - (int)currentNode.position.Y;
            int distx = (int)neighbour.position.X - (int)currentNode.position.X;
            distx *= distx;
            disty *= disty;

            return disty + distx;
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
            List<PathFinderNode> path = new List<PathFinderNode>();
            PathFinderNode backPathNode = currentNode;

            while (backPathNode.Equals(startNode) == false)
            {
                path.Add(backPathNode);
                backPathNode = backPathNode.parent;
            }
            path.Add(startNode);
            path.Reverse();
            foreach (PathFinderNode node in path)
            {
                Console.WriteLine("[PathFinder] PathNode : " + node.position * Bloodbender.meterToPixel);
            }
            Console.WriteLine("Path Found");
        }

        public List<PathFinderNode> getPathFinderNodes()
        {
            List<PathFinderNode> straightListNode = new List<PathFinderNode>();

            foreach (List<PathFinderNode> nodeList in allNodes)
            {
                straightListNode.AddRange(nodeList);
            }

            return straightListNode;
        }

    }
}
