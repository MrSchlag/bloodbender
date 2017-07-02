﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Bloodbender.PathFinding
{
    public class PathFinder2
    {
        private List<NavMesh> navMeshes;
        private PathProcessor pathProc;
        private Dictionary<PhysicObj, NavMesh> objNavMeshMapping;

        public PathFinder2()
        {
            objNavMeshMapping = new Dictionary<PhysicObj, NavMesh>();
            navMeshes = new List<NavMesh>();
            pathProc = new PathProcessor();
        }
        
        public void BuildtNavMeshes(int navMeshNumber, int stepLenght)
        {
            int initialRadiusOffset = 2;
            for (int i = 0; i < navMeshNumber; i++)
            {
                navMeshes.Add(new NavMesh(initialRadiusOffset + (stepLenght * i)));
            }
        }

        public void RegisterObj(PhysicObj obj)
        {
            objNavMeshMapping[obj] = navMeshes[0];

            foreach (var navMesh in navMeshes)
            {
                if (navMesh.RadiusOffset > obj.Radius)
                {
                    objNavMeshMapping[obj] = navMesh;
                    break;
                }
            }
        }

        public PathFinderNode pathRequest(PhysicObj startObj, PhysicObj endObj)
        {
            startObj.getPosNode().neighbors.Remove(endObj.getPosNode());
            endObj.getPosNode().neighbors.Remove(startObj.getPosNode());

            RemoveNodeFromNavMesh(GetNavMesh(startObj), startObj.getPosNode());
            RemoveNodeFromNavMesh(GetNavMesh(startObj), endObj.getPosNode());

            AddNodeToNavMesh(GetNavMesh(startObj), startObj.getPosNode());
            AddNodeToNavMesh(GetNavMesh(startObj), endObj.getPosNode());

            //if (NavMesh.NodeToNodeRayCast(startObj.getPosNode(), endObj.getPosNode()))
            if (PathProcessor.isWayClearToNode(startObj, endObj.getPosNode()))
            {
                startObj.getPosNode().neighbors.Add(endObj.getPosNode());
                endObj.getPosNode().neighbors.Add(startObj.getPosNode());
            }

            startObj.getPosNode().reset();
            endObj.getPosNode().reset();
            navMeshes.ForEach(nav => nav.Nodes.ForEach(node => node.reset()));

            var resultPath = pathProc.runDjikstra(startObj.getPosNode(), endObj.getPosNode());
            //var resultPath = pathProc.RunAstar(startObj.getPosNode(), endObj.getPosNode());

            if (resultPath != null)
                return resultPath[1];
            return null;
        }

        private void AddNodeToNavMesh(NavMesh nav, PathFinderNode node)
        {
            NodeTriangle triangle;

            triangle = nav.GetNodeTriangle(node);

            if (NavMesh.NodeToNodeRayCast(node, navMeshes[0].GetEquivalentNode(triangle.p1)))
            {
                triangle.p1.neighbors.Add(node);
                node.neighbors.Add(triangle.p1);
            }

            if (NavMesh.NodeToNodeRayCast(node, navMeshes[0].GetEquivalentNode(triangle.p2)))
            {
                triangle.p2.neighbors.Add(node);
                node.neighbors.Add(triangle.p2);
            }

            if (NavMesh.NodeToNodeRayCast(node, navMeshes[0].GetEquivalentNode(triangle.p3)))
            {
                triangle.p3.neighbors.Add(node);
                node.neighbors.Add(triangle.p3);
            }

            /*
            triangle.p2.neighbors.Add(node);
            triangle.p3.neighbors.Add(node);

            node.neighbors.Add(triangle.p2);
            node.neighbors.Add(triangle.p3);*/
        }

        private void RemoveNodeFromNavMesh(NavMesh nav, PathFinderNode node)
        {
            foreach (var neighbors in node.neighbors)
            {
                neighbors.neighbors.Remove(node);
            }
            node.neighbors.Clear();
        }

        public void AddNode(PathFinderNode node)
        {
            navMeshes.ForEach(item => item.AddNode(node.Copy()));
        }

        public void GeneratesMeshes() 
        {
            navMeshes.ForEach(i => i.GenerateNavMesh());
        }

        public NavMesh GetNavMesh(PhysicObj obj)
        {
            return objNavMeshMapping[obj];
        }

        public List<PathFinderNode> GetFirstMesh()
        {
            if (!navMeshes.Any())
                return new List<PathFinderNode>();
            return navMeshes[4].Nodes;
        }

        //private bool TestPointsRayCast()
    }
}
