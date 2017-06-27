using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using FarseerPhysics.Common.Decomposition;
using System;
using Triangulator.Geometry;
using System.Linq;

namespace Bloodbender.PathFinding
{
    public class NavMesh
    {
        public List<PathFinderNode> Nodes { get; set; }
        public float RadiusOffset { get; set; }
        private List<NodeTriangle> allTriangle { get; set; }

        public NavMesh(List<PathFinderNode> nodes)
        {
            allTriangle = new List<NodeTriangle>();
            Nodes = nodes;
        }

        public NavMesh(float radiusOffset)
        {
            allTriangle = new List<NodeTriangle>();
            Nodes = new List<PathFinderNode>();
            RadiusOffset = radiusOffset;
        }

        public void ResetNodes()
        {
            foreach (PathFinderNode node in Nodes)
            {
                node.reset();
            }
        }

        public void AddNode(PathFinderNode nodePosition)
        {
            Nodes.Add(nodePosition);
        }


        public void GenerateNavMesh()
        {
            List<Triangulator.Geometry.Point> vertices = new List<Triangulator.Geometry.Point>();
            Nodes.ForEach(i => vertices.Add(new Triangulator.Geometry.Point(i.position.X * Bloodbender.meterToPixel, i.position.Y * Bloodbender.meterToPixel)));
            var list = Triangulator.Delauney.Triangulate(vertices);

            foreach (var triangle in list)
            {
                PathFinderNode node1 = GetNodeFromPosition(vertices[triangle.p1]);
                PathFinderNode node2 = GetNodeFromPosition(vertices[triangle.p2]);
                PathFinderNode node3 = GetNodeFromPosition(vertices[triangle.p3]);

                node1.neighbors.Add(node2);
                node1.neighbors.Add(node3);

                node2.neighbors.Add(node1);
                node2.neighbors.Add(node3);
                
                node3.neighbors.Add(node1);
                node3.neighbors.Add(node2);

                NodeTriangle nodeTriangle = new NodeTriangle();
                nodeTriangle.p1 = node1;
                nodeTriangle.p2 = node2;
                nodeTriangle.p3 = node3;

                allTriangle.Add(nodeTriangle);
            }
            DeleteInvalidLink();
            ThickenessCorrection();
        }

        private void ThickenessCorrection()
        {
            foreach (var node in Nodes)
            {
                Vector2 nodeToCentroid = node.position - node.DivergencePoint;
                nodeToCentroid.Normalize();
                nodeToCentroid *= RadiusOffset * Bloodbender.pixelToMeter;
                node.position += nodeToCentroid;
            }
        }

        private void DeleteInvalidLink()
        {
            List<PathFinderNode> neighboorToRemove = new List<PathFinderNode>();

            foreach (var node in Nodes)
            {
                neighboorToRemove.Clear();
                foreach (var neighboor in node.neighbors)
                {
                    if (!NodeToNodeRayCast(node, neighboor))
                        neighboorToRemove.Add(neighboor);
                }
                neighboorToRemove.ForEach(i => node.neighbors.Remove(i));
            }
        }

        private bool NodeToNodeRayCast(PathFinderNode node1, PathFinderNode node2)
        {
            bool isVisible = true;

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
            }, node2.position, node1.position);

            if (isVisible == false)
                return false;

            return true;
        }

        public PathFinderNode GetNodeFromPosition(Triangulator.Geometry.Point pos)
        {
            foreach (var node in Nodes)
            {
                if ((int)(node.position * Bloodbender.meterToPixel).X == (int)pos.X &&
                    (int)(node.position * Bloodbender.meterToPixel).Y == (int)pos.Y)
                    return node;
            }
            return null;
        }

        public PathFinderNode GetEquivalentNode(PathFinderNode node)
        {
            foreach (var n in Nodes)
            {
                if (n.NavId == node.NavId)
                    return n;
            }
            return null;
        }


        private float sign(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
        }

        private bool PointInTriangle(Vector2 pt, Vector2 v1, Vector2 v2, Vector2 v3)
        {
            bool b1, b2, b3;

            b1 = sign(pt, v1, v2) < 0.0f;
            b2 = sign(pt, v2, v3) < 0.0f;
            b3 = sign(pt, v3, v1) < 0.0f;

            return ((b1 == b2) && (b2 == b3));
        }

        public NodeTriangle GetNodeTriangle(PathFinderNode node)
        {
            foreach (var triangle in allTriangle)
            {
                if (PointInTriangle(node.position, triangle.p1.position, triangle.p2.position, triangle.p3.position))
                    return triangle;
            }
            return new NodeTriangle();
        }
    }
}
