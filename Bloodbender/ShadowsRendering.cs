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
        private RenderTarget2D targetShadows = null;

        public ShadowsRendering()
        {
            listShadows = new List<Shadow>();
            targetShadows = new RenderTarget2D(Bloodbender.ptr.GraphicsDevice, Bloodbender.ptr.GraphicsDevice.PresentationParameters.BackBufferWidth, Bloodbender.ptr.GraphicsDevice.PresentationParameters.BackBufferHeight);
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
        //Vector2 offSet;

        public Shadow(GraphicObj graphicObj) : this(graphicObj, Vector2.One) // il faudrait pouvoir parametre avec la texture que l'on veut, et faire en sorte que la largeur/hauteur tend vers 0
        { }
        public Shadow(GraphicObj graphicObj, Vector2 scale)
        {
            this.graphicObj = graphicObj;

            addAnimation(new Animation(Bloodbender.ptr.Content.Load<Texture2D>("shadow")));
            //animations[0].forceDepth(0.00001f);
        }

        public override bool Update(float elapsed)
        {
            // lire la position et le scale du graphique obj ici et non pas ds le draw

            return base.Update(elapsed);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            position = graphicObj.getBottomCenter();

            position.X -= getAnimation(currentAnimation).getFrameDimensions().X / 2 ;
            position.Y -= getAnimation(currentAnimation).getFrameDimensions().Y / 2 ;

            position -= graphicObj.getAnimation(0).origin;

            base.Draw(spriteBatch);
        }
    }
}
