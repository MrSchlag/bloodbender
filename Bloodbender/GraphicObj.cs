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
    public class GraphicObj : IDisposable
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

        public bool shouldDie = false;

        private List<IComponent> components;

        public GraphicObj(OffSet offSet = OffSet.None)
        {
            animations = new List<Animation>();
            components = new List<IComponent>();
            this.offSet = offSet;
        }
        public virtual bool Update(float elapsed)
        {
            if (animations.Count > 0)
                if (!animations[currentAnimation].Update(elapsed))
                    return false;

            foreach (IComponent component in components)
                component.Update(elapsed);

            return true;
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (Bloodbender.ptr.camera.isInView(position) && animations.Count > 0) // Permet de draw que les elements present ds la vue
                animations[currentAnimation].Draw(spriteBatch, position, rotation, spriteEffect, scale, height);
        }

        public void addComponent(IComponent component)
        {
            components.Add(component); //TODO : Faire la vérification des composants
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
                    animations[animationNbr].reset();

                currentAnimation = animationNbr;
            }
        }

        public float angleWithMouse()
        {
            Vector2 positionInScreen = Bloodbender.ptr.camera.ConvertWorldToScreen(position);
            MouseState mouse = Mouse.GetState();

            float deltaY = mouse.Y - positionInScreen.Y;
            float deltaX = mouse.X - positionInScreen.X;

            float angle = (float)(Math.Atan2(deltaY, deltaX));

            return angle;
        }

        public void setRotation(float rotation)
        {
            this.rotation = rotation;
        }

        public void setOffSet(OffSet off)
        {
            if (off == OffSet.Center)
            {
                foreach (Animation animation in animations)
                {
                    animation.origin = animation.getFrameDimensions();
                    animation.origin /= 2;
                }
            }
            else if (off == OffSet.BottomCenterHorizontal)
            {
                foreach (Animation animation in animations)
                {
                    animation.origin = animation.getFrameDimensions();
                    animation.origin.X /= 2;
                }
            }
        }

        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public double distanceWith(Vector2 position2)
        {
            return Math.Sqrt(Math.Pow((position.X - position2.X), 2) + Math.Pow((position.Y - position2.Y), 2));
        }
    }

    public static class Vector2Extension
    {

        public static Vector2 Rotate(this Vector2 v, float rad)
        {
            float sin = (float)Math.Sin(rad);
            float cos = (float)Math.Cos(rad);

            float tx = v.X;
            float ty = v.Y;
            /*
            v.X = (cos * tx) - (sin * ty);
            v.Y = (sin * tx) + (cos * ty);
            */        
            return (new Vector2((cos * tx) - (sin * ty), (sin * tx) + (cos * ty)));
        }
    }
}
