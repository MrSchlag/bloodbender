using FarseerPhysics;
using FarseerPhysics.DebugView;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodbender
{
    public class DebugView : DebugViewXNA
    {
        public DebugView() : base(Bloodbender.ptr.world)
        {
            AppendFlags(DebugViewFlags.PathFinding);
        }

        protected override void DrawDebugData()
        {
            base.DrawDebugData();

            if ((Flags & DebugViewFlags.PathFinding) == DebugViewFlags.PathFinding)
            {
                List<PathFinderNode> listNodes = Bloodbender.ptr.pathFinder.getPathFinderNodes();

                foreach (PathFinderNode n in listNodes)
                {
                    DrawSolidCircle(n.position, 0.1f, new Vector2(0, 0), Color.White);

                    foreach (PathFinderNode nl in n.neighbors)
                    {
                        DrawSegment(n.position, nl.position, Color.Gray);
                    }
                }
            }
        }
    }
}
