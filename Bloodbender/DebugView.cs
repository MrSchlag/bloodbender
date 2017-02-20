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

        }


        public override void RenderDebugData(ref Matrix projection, ref Matrix view)
        {
            if (!Enabled)
                return;

            //Nothing is enabled - don't draw the debug view.
            if (Flags == 0)
                return;

            _device.RasterizerState = RasterizerState.CullNone;
            _device.DepthStencilState = DepthStencilState.Default;

            _primitiveBatch.Begin(ref projection, ref view);

            DrawDebugData();

            List<PathFinderNode> listNodes = Bloodbender.ptr.pathFinder.getPathFinderNodes();

            foreach (PathFinderNode n in listNodes)
            {
                DrawSolidCircle(n.position, 0.1f, new Vector2(0,0), Color.White);

                foreach (PathFinderNode nl in n.neighbors)
                {
                    DrawSegment(n.position, nl.position, Color.Gray);
                }
            }

            _primitiveBatch.End();

            if ((Flags & DebugViewFlags.PerformanceGraph) == DebugViewFlags.PerformanceGraph)
            {
                _primitiveBatch.Begin(ref _localProjection, ref _localView);
                DrawPerformanceGraph();
                _primitiveBatch.End();
            }

            // begin the sprite batch effect
            _batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            // draw any strings we have
            for (int i = 0; i < _stringData.Count; i++)
            {
                _batch.DrawString(_font, _stringData[i].Text, _stringData[i].Position, _stringData[i].Color);
            }

            // end the sprite batch effect
            _batch.End();

            _stringData.Clear();
        }
    }
}
