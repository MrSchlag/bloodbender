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


        public void SetPosition(Vector2 position)
        {
            this.position = position;
        }

        public void remove()
        {
            foreach (PathFinderNode neighbour in neighbors)
            {
                neighbour.neighbors.Remove(this);
            }
            neighbors.Clear();
        }

        public void OnTriangleChanged()
        {
            TriangleChangedEvent?.Invoke();
        }

        public event TargetChangeTriangle TriangleChangedEvent;

    }
}
