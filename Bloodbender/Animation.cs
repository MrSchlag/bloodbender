using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bloodbender
{
    public class Animation
    {
        public bool isLooping { get; set; } = true; // bool to know if the animation loop forever
        public bool isRunning { get; set; } = true; // bool to know if the animation is running
        public bool isDepthForce = false;

        protected Texture2D texture;
        public Rectangle rectangleSource; // rectangle used to get a different part of the texture at each draw
        public Vector2 origin = Vector2.Zero;
        public float depth { get; set; } = 0.0f;
        public Color color = Color.White;

        protected int framesNumber; // nombre de frame dans l'animation
        public float[] framesLength { get; set; } // array to represent the lenght in second of each frame
        public int frameWidth { get; set; } // largeur d'une frame
        protected int currentFrame = 0;
        protected float totalElapsed = 0.0f;

        public Animation(Texture2D texture, int frameWidth = 0) : this(texture, 1, 0.0f, frameWidth) // use this constructor if their only is 1 frame in your animation
        { }
        public Animation(Texture2D texture, int framesNumber, float frameLength, int frameWidth = 0) // precise the frameWidth if the number of frame indicated does not match the witdh of the texture (ex: a texture with 3frame with a width of 100pixels each, texture width = 3 * 100 = 300, if the number of frame indicated is 2, precise the width of a frame, 100 in this case)
        {
            this.texture = texture;

            if (framesNumber <= 0)
                framesNumber = 1;
            if (framesNumber == 1)
                isRunning = false;
            this.framesNumber = framesNumber;

            framesLength = new float[framesNumber];
            for (int i = 0; i < framesNumber; ++i)
                framesLength[i] = frameLength;

            if (frameWidth != 0)
                this.frameWidth = frameWidth;
            else
                this.frameWidth = texture.Width / framesNumber;

            rectangleSource = new Rectangle(0, 0, this.frameWidth, texture.Height);
        }

        public bool Update(float elapsed)
        {
            if (isRunning)
            {
                totalElapsed += elapsed;
                if (totalElapsed > framesLength[currentFrame])
                {
                    totalElapsed -= framesLength[currentFrame];

                    currentFrame++;

                    if (isLooping)
                        currentFrame = currentFrame % framesNumber;
                }

                if (currentFrame == framesNumber)
                {
                    currentFrame--;
                    isRunning = false;
                    return false;
                }
            }
            return true;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, float rotation, SpriteEffects spriteEffect, Vector2 scale, float height = 0.0f)
        {
            rectangleSource.X = frameWidth * currentFrame;

            if (!isDepthForce)
                 depth = ((position.Y + texture.Height * scale.Y) - (Bloodbender.ptr.camera.Position.X - (Bloodbender.ptr.GraphicsDevice.Viewport.Height / 2))) / 10000.0f; // ATTENTION A CORRIGER SI MANIPULATION SUR LE SCALE, A CORRIGER QUAND PhysicObj FINI, depth responsable du non  affichage de sprite sur les negatif

            position.Y -= height;

            spriteBatch.Draw(texture, position, rectangleSource, color,
                rotation, origin, scale, spriteEffect, depth);
        }

        public void reset()
        {
            totalElapsed = 0.0f;
            currentFrame = 0;
            isRunning = true;
        }

        public void forceDepth(float depth) // permet de forcer la profondeur d'affichage du sprite
        {
            this.depth = depth;
            isDepthForce = true;
        }

        public Texture2D getTexture() // methode à supprimer car fausse
        {
            return texture;
        }
    }
}
