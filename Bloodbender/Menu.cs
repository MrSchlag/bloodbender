using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodbender
{
    public class Menu
    {
        public bool showing = false;
        public string bigMessage = "PAUSED";
        public Color bigMessageColor = Color.Black;

        GraphicObj arrow;

        SpriteFont spriteFont;

        int counterOption = 0;

        List<string> options;

        public Menu(SpriteFont font)
        {
            spriteFont = font;


            arrow = new GraphicObj();
            Animation anim = new Animation(Bloodbender.ptr.Content.Load<Texture2D>("arrow"));
            anim.forceDepth(1);
            arrow.addAnimation(anim);
            arrow.checkIsInView = false;
            arrow.scale /= 1.5f;

            options = new List<string>();

            options.Add("Quit");
            options.Add("Restart");
        }
        public bool Update(float elapsed)
        {
            if (!showing)
                return false;

            if (Bloodbender.ptr.inputHelper.IsNewKeyPress(Keys.Z) || Bloodbender.ptr.inputHelper.IsNewKeyPress(Keys.Up) || Bloodbender.ptr.inputHelper.IsNewButtonPress(Buttons.DPadUp))
            {
                counterOption--;
            }
            else if (Bloodbender.ptr.inputHelper.IsNewKeyPress(Keys.S) || Bloodbender.ptr.inputHelper.IsNewKeyPress(Keys.Down) || Bloodbender.ptr.inputHelper.IsNewButtonPress(Buttons.DPadDown))
            {
                counterOption++;
            }
            else if (Bloodbender.ptr.inputHelper.IsNewKeyPress(Keys.Enter) || Bloodbender.ptr.inputHelper.IsNewButtonPress(Buttons.A))
            {
                if (counterOption == 0)
                    Bloodbender.ptr.Exit();
                else if (counterOption == 1)
                {
                    bigMessage = "PAUSED";
                    bigMessageColor = Color.Black;
                    Bloodbender.ptr.reload = true;
                }
            }


            if (counterOption < 0)
                counterOption = options.Count - 1;
            else if (counterOption >= options.Count)
                counterOption = 0;



            return showing;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (showing)
            {
                Vector2 resRelative = new Vector2(Bloodbender.ptr.resolutionIndependence.VirtualWidth / 2, Bloodbender.ptr.resolutionIndependence.VirtualHeight / 2);

                Vector2 position = new Vector2(100, 150) + Bloodbender.ptr.camera.Position - resRelative;
                spriteBatch.DrawString(spriteFont, bigMessage, position, bigMessageColor, 0, Vector2.Zero, 5, SpriteEffects.None, 1);
                //spriteBatch.DrawString(spriteFont, bigMessage, position, bigMessageColor, 0, spriteFont.MeasureString(bigMessage) / 2, 5, SpriteEffects.None, 1);
                //spriteBatch.DrawString(spriteFont, bigMessage, new Vector2(72, 72), Color.White, 0, Vector2.Zero, 7, SpriteEffects.None, 1);

                position.X += 50;
                position.Y += 125;

                arrow.position = position;
                arrow.position.X -= 50;
                arrow.position.Y += (10 * (counterOption + 1)) + (counterOption * spriteFont.MeasureString(options[0]).Y * 1.75f);

                arrow.Draw(spriteBatch);

                foreach (string option in options)
                {
                    spriteBatch.DrawString(spriteFont, option, position, Color.Black, 0, Vector2.Zero, 1.75f, SpriteEffects.None, 1);
                    position.Y += 50;
                }
            }
        }
    }
}
