using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bloodbender
{
    public class GraphicObj
    {
        private List<Animation> animations;
        public Vector2 position = Vector2.Zero;
        protected float rotation = 0.0f;
        protected int currentAnimation = 0;
        public float height = 0.0f;
        protected SpriteEffects spriteEffect = SpriteEffects.None;
        public Vector2 scale = Vector2.One;
        private OffSet offSet;
        public enum OffSet { Center, BottomCenterHorizontal, None };

        public GraphicObj(OffSet offSet = OffSet.None)
        {
            animations = new List<Animation>();
            this.offSet = offSet;
        }
        public virtual bool Update(float elapsed)
        {
            if (!animations[currentAnimation].Update(elapsed))
                return false;

            return true;
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            //if (Bloodbender.ptr.camera.isInView(this)) // Permet de draw que les elements present ds la vue
                animations[currentAnimation].Draw(spriteBatch, position, rotation, spriteEffect, scale, height);
        }

        public void addAnimation(Animation animation)
        {

            if (offSet == OffSet.BottomCenterHorizontal)
            {
                animation.origin = animation.getFrameDimensions();
                animation.origin.X /= 2;
            }
            else if (offSet == OffSet.Center)
            {
                animation.origin = animation.getFrameDimensions();
                animation.origin /= 2;
            }

            animations.Add(animation);
        }

        public Animation getAnimation(int i)
        {
            return animations[i];
        }

        public void runAnimation(int animationNbr, bool forceAnimation = false) // set force to true if you want an animation already running to re-run itself
        {
            if (animationNbr >= 0)
            {
                if (currentAnimation != animationNbr || forceAnimation)
                {
                    //foreach (Animation animation in animations)
                      //  animation.reset();

                    animations[animationNbr].reset();
                }

                currentAnimation = animationNbr;
            }
        }

        public float angleWithMouse()
        {
            Vector2 vec = Bloodbender.ptr.resolutionIndependence.ScaleMouseToScreenCoordinates(Bloodbender.ptr.inputHelper.Cursor);

            float deltaY = vec.Y - position.Y + Bloodbender.ptr.camera.Position.Y - (Bloodbender.ptr.resolutionIndependence.VirtualHeight / 2);
            float deltaX = vec.X - position.X + Bloodbender.ptr.camera.Position.X - (Bloodbender.ptr.resolutionIndependence.VirtualWidth / 2);
            float angle = (float)(Math.Atan2(deltaY, deltaX));

            return angle;
        }
    }
}
