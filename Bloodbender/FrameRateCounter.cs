using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodbender
{
    public class FrameRateCounter : GraphicObj
    {
        int frameRate = 0;
        int frameCounter = 0;
        float elapsedTime = 0.0f;

        SpriteFont spriteFont;

        public FrameRateCounter(SpriteFont font)
        {
            spriteFont = font;
        }

        public override bool Update(float elapsed)
        {
            elapsedTime += elapsed;

            if (elapsedTime > 1)
            {
                elapsedTime -= 1;
                frameRate = frameCounter;
                frameCounter = 0;
            }

            return true;
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            frameCounter++;

            string fps = string.Format("{0}", frameRate);

            spriteBatch.Begin();

            spriteBatch.DrawString(spriteFont, fps, new Vector2(6, 6), Color.Black);
            spriteBatch.DrawString(spriteFont, fps, new Vector2(5, 5), Color.White);

            spriteBatch.End();
        }
    }
}
