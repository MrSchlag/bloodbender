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
        ParticuleSystem ps;

        Dictionary<GraphicObj, List<PathFinderNode>> paths = new Dictionary<GraphicObj, List<PathFinderNode>>();
        public DebugView() : base(Bloodbender.ptr.world)
        {
            ps = new ParticuleSystem();

            AppendFlags(DebugViewFlags.PathFinding);
        }

        public void Update(float elapsed)
        {
            Dictionary<GraphicObj, List <PathFinderNode>> pathstmp = (Bloodbender.ptr.pathFinder.getCurrentPaths());

            if (paths.Equals(pathstmp))
            {
                paths = new Dictionary<GraphicObj, List<PathFinderNode>>(Bloodbender.ptr.pathFinder.getCurrentPaths());

                //ajout de tout les pathdebug
                foreach (KeyValuePair<GraphicObj, List<PathFinderNode>> key in paths)
                {
                    setPathParticuleSpawer(key);
                }
            }

            bool reset = false;

            foreach (KeyValuePair<GraphicObj, List<PathFinderNode>> key in pathstmp)
            {
                //key

                if (!paths.ContainsKey(key.Key))
                {
                    reset = true;

                    setPathParticuleSpawer(key);
                }
            }

            if (reset == true)
                paths = pathstmp;

            ps.Update(elapsed);
        }

        private void setPathParticuleSpawer(KeyValuePair<GraphicObj, List<PathFinderNode>> key)
        {
            ParticuleSpawner particuleSpawner = new ParticuleSpawner(60, key.Key.position);

            MoveTo comp = new MoveTo(particuleSpawner, key.Value, 450);
            particuleSpawner.addComponent(comp);

            ps.particuleSpawners.Add(particuleSpawner);
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


            //draw selected paths
            Bloodbender.ptr.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Bloodbender.ptr.camera.GetView());

            ps.Draw(Bloodbender.ptr.spriteBatch);

            Bloodbender.ptr.spriteBatch.End();

            _stringData.Clear();
        }
    }
}
