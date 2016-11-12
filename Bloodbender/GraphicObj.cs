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
        public Animation[] animations;
        public Vector2 position = Vector2.Zero;
        protected float rotation = 0.0f;
        protected uint currentAnimation = 0;
        public float height = 0.0f;
        protected SpriteEffects spriteEffect = SpriteEffects.None;
        public Vector2 scale = Vector2.One;

        public GraphicObj(uint animationsNumber = 1) // faire en sorte que le constructeur ne prenne pas en parametre le nombre d'animation, et juste faire une fonction addAnimation
        {
            animations = new Animation[animationsNumber];
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
                animations[currentAnimation].Draw(spriteBatch, new Vector2(position.X, position.Y), rotation, spriteEffect, scale, height);
        }

        /*
        public void addAnimation(Animation animation)
        {
            Animation[] tmpAnimations = new Animation[animations.Length + 1];

            for (int i = 0; i < animations.Length; ++i)
                tmpAnimations[i] = animations[i];
            tmpAnimations[animations.Length] = animation;
            animations = tmpAnimations;
        }
        */

        public Vector2 getBottomCenter() // return the coordinate of the bottom center of sprite, don't take scale in parameter 
        {
            return new Vector2(position.X + (animations[currentAnimation].rectangleSource.Width / 2) * scale.X,
                                position.Y + animations[currentAnimation].rectangleSource.Height * scale.Y);
        }

        public Vector2 getCenter() // return the center of current sprite
        {
            return new Vector2(position.X + (animations[currentAnimation].rectangleSource.Width / 2) * scale.X,
                                position.Y + (animations[currentAnimation].rectangleSource.Height / 2) * scale.Y);
        }

        public Vector2 getSize()
        {
            return new Vector2(animations[currentAnimation].rectangleSource.Width * scale.X, animations[currentAnimation].rectangleSource.Height * scale.Y);
        }

        public void runAnimation(uint animationNbr, bool forceAnimation = false) // set force to true if you want an animation already running to re-run itself
        {
            if (currentAnimation != animationNbr || forceAnimation)
            {
                foreach (Animation animation in animations)
                    animation.reset();
            }

            currentAnimation = animationNbr;
        }

        public float angleWithMouse() // changer position avec les coordonné du centre du sprite
        {
            MouseState mouseState = Mouse.GetState();
            Vector2 centerPos = getCenter();
            //float deltaY = mouseState.Y - centerPos.Y + Bloodbender.ptr.camera.Position.Y - (Bloodbender.ptr.camera.height / 2);
            //float deltaX = mouseState.X - centerPos.X + Bloodbender.ptr.camera.P.X - (Bloodbender.ptr.camera.width / 2);
            //float angle = (float)(Math.Atan2(deltaY, deltaX));

            //return angle;
            return 0.0f;
        }
    }
}
