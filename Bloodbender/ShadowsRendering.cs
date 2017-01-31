using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodbender
{
    public class ShadowsRendering
    {
        private List<Shadow> listShadows;
        public RenderTarget2D targetShadows = null;

        public ShadowsRendering()
        {
            listShadows = new List<Shadow>();
            targetShadows = new RenderTarget2D(Bloodbender.ptr.GraphicsDevice, Bloodbender.ptr.graphics.PreferredBackBufferWidth, Bloodbender.ptr.graphics.PreferredBackBufferHeight);
        }

        public void Update(float elapsed)
        {
            foreach (Shadow obj in listShadows)
            {
                if (!obj.Update(elapsed))
                {
                    //supprimer la shadow ici
                }
            }
        }

        public void LoadContent()
        {
        }
        public void addShadow(Shadow shadow)
        {
            listShadows.Add(shadow);
        }

        public void removeShadow()
        {

        }

        public void renderShadowsOnTarget()
        {
            Bloodbender.ptr.GraphicsDevice.SetRenderTarget(targetShadows);
            Bloodbender.ptr.GraphicsDevice.Clear(Color.Transparent);

            Bloodbender.ptr.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Bloodbender.ptr.camera.View);
            foreach (GraphicObj obj in listShadows)
                obj.Draw(Bloodbender.ptr.spriteBatch);
            Bloodbender.ptr.spriteBatch.End();

            Bloodbender.ptr.GraphicsDevice.SetRenderTarget(null);
        }

        public RenderTarget2D getTarget()
        {
            return targetShadows;
        }
    }

    public class Shadow : GraphicObj
    {
        protected GraphicObj graphicObj;

        public Shadow(GraphicObj graphicObj) : this(graphicObj, Vector2.One) // il faudrait pouvoir parametre avec la texture que l'on veut, et faire en sorte que la largeur/hauteur tend vers 0
        { }
        public Shadow(GraphicObj graphicObj, Vector2 scale) : base(OffSet.Center)
        {
            this.graphicObj = graphicObj;

            addAnimation(new Animation(Bloodbender.ptr.Content.Load<Texture2D>("shadow")));
        }

        public override bool Update(float elapsed)
        {
            // lire la position et le scale du graphique obj ici et non pas ds le draw

            return base.Update(elapsed);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            position = graphicObj.position;

            base.Draw(spriteBatch);
        }
    }
}
