using System.Collections.Generic;
using System.Linq;
using QuickGraph.Algorithms;
using QuickGraph;

namespace Bloodbender.PathFinding
{
    public class PathFinder
    {
        public static float PathStep = 2f;
        private List<NavMesh> navMeshes;
        private PathProcessor pathProc;
        private Dictionary<PhysicObj, NavMesh> objNavMeshMapping;
        public Dictionary<GraphicObj, List<PathFinderNode>> PathDict { get; set; }

        private float timerEventDuration = 0.2f;
        private float timerEvent = 0;

        public PathFinder()
        {
            PathDict = new Dictionary<GraphicObj, List<PathFinderNode>>();
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

        public void Update(float elapsed)
        {
            if (timerEvent > timerEventDuration)
            {
                foreach (var pair in objNavMeshMapping)
                {
                    AddNodeToNavMesh(pair.Value, pair.Key.getPosNode(), true);
                }
                timerEvent = 0;
            }
            timerEvent += elapsed;
        }

        public void UpdateTriangleForNode(PathFinderNode node)
        {

        }

        public List<PathFinderNode> pathRequest(PhysicObj startObj, PhysicObj endObj)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            startObj.getPosNode().neighbors.Remove(endObj.getPosNode());
            endObj.getPosNode().neighbors.Remove(startObj.getPosNode());

            RemoveNodeFromNavMesh(GetNavMesh(startObj), startObj.getPosNode());
            RemoveNodeFromNavMesh(GetNavMesh(startObj), endObj.getPosNode());

            AddNodeToNavMesh(GetNavMesh(startObj), startObj.getPosNode());
            AddNodeToNavMesh(GetNavMesh(startObj), endObj.getPosNode());
              
            if (PathProcessor.isWayClearToNode(startObj, endObj.getPosNode()))
            {
                //startObj.getPosNode().neighbors.Add(endObj.getPosNode());
                //endObj.getPosNode().neighbors.Add(startObj.getPosNode());
                return new List<PathFinderNode>() { startObj.getPosNode(), endObj.getPosNode() };
                //GetNavMesh(startObj).graph.AddEdge(new Edge<PathFinderNode>(startObj.getPosNode(), endObj.getPosNode()));
            }

            //startObj.getPosNode().reset();
            //endObj.getPosNode().reset();
            //navMeshes.ForEach(nav => nav.Nodes.ForEach(node => node.reset()));

            //var resultPath = pathProc.runDjikstra(startObj.getPosNode(), endObj.getPosNode());
            //var res = GetNavMesh(startObj).graph.ShortestPathsDijkstra(GetNavMesh(startObj).verticesDistance, startObj.getPosNode());
            if (GetNavMesh(startObj).graph.ContainsVertex(startObj.getPosNode()))
            {
                
                stopwatch.Start();
                var res = GetNavMesh(startObj).graph.ShortestPathsDijkstra(GetNavMesh(startObj).verticesDistance, startObj.getPosNode());

                IEnumerable<Edge<PathFinderNode>> path;
                if (res(endObj.getPosNode(), out path))
                {
                    var list = path.ToList();
                    var list2 = new List<PathFinderNode>() { startObj.getPosNode() };

                    foreach (var edge in list)
                    {
                        if (list2[list2.Count - 1] == edge.Source)
                            list2.Add(edge.Target);
                        else
                            list2.Add(edge.Source);
                    }
                    
                    stopwatch.Stop();
                    //Console.WriteLine(stopwatch.ElapsedMilliseconds);
                    return list2;
                }
            }

            //if (resultPath != null)
            //{
            //    PathDict[startObj] = resultPath;
            //    return resultPath;
            //}
            return null;
        }

        public void RemoveObjFromNavMesh(PhysicObj obj)
        {
            obj.getPosNode().neighbors.Remove(obj.getPosNode());
            RemoveNodeFromNavMesh(GetNavMesh(obj), obj.getPosNode());

            objNavMeshMapping.Remove(obj);
            PathDict.Remove(obj);
        }

        public Dictionary<GraphicObj, List<PathFinderNode>> GetCurrentPaths()
        {
            return PathDict;
        }

        private bool PathComparator(List<PathFinderNode> path1, List<PathFinderNode> path2)
        {
            if (path1 == null || path2 == null)
                return false;
            if (path1.Count != path2.Count)
                return false;

            for (int i = 0; i < path1.Count; i++)
            {
                if (path1[i] != path2[i])
                    return false;
            }

            return true;
        }

        public void UpdateTriangleForObj(PhysicObj obj)
        {
            AddNodeToNavMesh(GetNavMesh(obj), obj.getPosNode()); 
        }

        private bool AddNodeToNavMesh(NavMesh nav, PathFinderNode node, bool throwEvent = false)
        {
            NodeTriangle triangle;

            triangle = nav.GetNodeTriangle(node);

            if (throwEvent == true)
                node.NodeTriangleEvent = triangle;
            else
                node.NodeTriangle = triangle;

            if (triangle.p1 == null || triangle.p2 == null || triangle.p3 == null)
                return false;

            nav.graph.AddVertex(node);

            if (NavMesh.NodeToNodeRayCast(node, navMeshes[0].GetEquivalentNode(triangle.p1)))
            {
                triangle.p1.neighbors.Add(node);
                node.neighbors.Add(triangle.p1);
                nav.graph.AddEdge(new Edge<PathFinderNode>(node, triangle.p1));
            }

            if (NavMesh.NodeToNodeRayCast(node, navMeshes[0].GetEquivalentNode(triangle.p2)))
            {
                triangle.p2.neighbors.Add(node);
                node.neighbors.Add(triangle.p2);
                nav.graph.AddEdge(new Edge<PathFinderNode>(node, triangle.p2));
            }

            if (NavMesh.NodeToNodeRayCast(node, navMeshes[0].GetEquivalentNode(triangle.p3)))
            {
                triangle.p3.neighbors.Add(node);
                node.neighbors.Add(triangle.p3);
                nav.graph.AddEdge(new Edge<PathFinderNode>(node, triangle.p3));
            }

            return true;
        }

        private void RemoveNodeFromNavMesh(NavMesh nav, PathFinderNode node)
        {
            if (!nav.graph.ContainsVertex(node))
                return;
            foreach (var neighbor in node.neighbors)
            {
                Edge<PathFinderNode> edge = null;
                if (nav.graph.TryGetEdge(node, neighbor, out edge))
                {
                    nav.graph.RemoveEdge(edge);  
                }
                neighbor.neighbors.Remove(node);
            }
            if (nav.graph.ContainsVertex(node)) 
                nav.graph.RemoveVertex(node);
            node.neighbors.Clear();
        }

        public void AddNode(PathFinderNode node)
        {
            navMeshes.ForEach(item => item.AddNode(node.Copy()));
        }

        public void Clear()
        {
            foreach (var nav in navMeshes)
            {
                nav.graph.Clear();
                nav.allTriangle.Clear();
                nav.Nodes.Clear();
            }
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

    }
}
