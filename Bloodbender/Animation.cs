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
        public bool isRunning { get; set; } = false; // bool to know if the animation is running
        public bool isDepthForce = false;

        private Texture2D texture;
        private Rectangle rectangleSource; // rectangle used to get a different part of the texture at each draw
        public Vector2 origin = Vector2.Zero;
        public Vector2 offSet = Vector2.Zero;
        public float depth { get; set; } = 0.0f;

        protected int framesNumber; // nombre de frame dans l'animation
        public float[] framesLength { get; set; } // array to represent the lenght in second of each frame
        public int frameWidth { get; set; } = 0;// largeur d'une frame
        public int frameHeight { get; set; } = 0;

        protected int currentFrame = 0;
        protected float totalElapsed = 0.0f;

        private int row = 0, column = 0;

        public Animation(Texture2D texture, int frameWidth = 0, int frameHeight = 0, int row = 0, int column = 0) : this(texture, 1, 0.0f, frameWidth, frameHeight, row, column)
        { }
        
        public Animation(Texture2D texture, int framesNumber, float frameLength, int frameWidth, int frameHeight, int row, int column) // precise the frameWidth if the number of frame indicated does not match the witdh of the texture (ex: a texture with 3frame with a width of 100pixels each, texture width = 3 * 100 = 300, if the number of frame indicated is 2, precise the width of a frame, 100 in this case)
        {
            this.texture = texture;

            this.row = row;
            this.column = column;

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

            if (frameHeight != 0)
                this.frameHeight = frameHeight;
            else
                this.frameHeight = texture.Height;

            rectangleSource = new Rectangle(column * this.frameWidth, row * this.frameHeight, this.frameWidth, this.frameHeight);
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

        public void Draw(SpriteBatch spriteBatch, Vector2 position, float rotation, SpriteEffects spriteEffect, Vector2 scale, Color color, float height = 0.0f)
        {
            rectangleSource.X = frameWidth * currentFrame + frameWidth * column;

            if (!isDepthForce)
                depth = ((Bloodbender.ptr.camera.ConvertWorldToScreen(position).Y) / 10000.0f) + 0.1f;

            position.Y -= height;// peut pose des problemes?

            // permet d'évite l'effet escalier sur les sprites
            position.X = (int)(position.X);
            position.Y = (int)(position.Y);

            position += offSet;

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

        public Vector2 getFrameDimensions()
        {
            return new Vector2(frameWidth, texture.Height);
        }
    }
}
