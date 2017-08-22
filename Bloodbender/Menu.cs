using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public bool loadClicked = false;

        public int counterOption = 0;
        public int counterSave = 0;

        GraphicObj arrow;

        SpriteFont spriteFont;

        List<string> options;
        List<string> saves;


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
            saves = new List<string>();

            options.Add("Quit");
            options.Add("New Game");
            if (Bloodbender.ptr.savedSeeds != null)
            {
                options.Add("Load Game");
                foreach (var savedSeed in Bloodbender.ptr.savedSeeds)
                {
                    DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(savedSeed).ToLocalTime();
                    string formattedDate = dt.ToString("HH:mm:ss - dd-MM-yyyy");
                    saves.Add(formattedDate);
                }
            }
        }
        public bool Update(float elapsed)
        {
            if (!showing)
                return false;

            if (Bloodbender.ptr.inputHelper.IsNewKeyPress(Keys.Z) || Bloodbender.ptr.inputHelper.IsNewKeyPress(Keys.Up) || Bloodbender.ptr.inputHelper.IsNewButtonPress(Buttons.DPadUp))
            {
                if (!loadClicked)
                    counterOption--;
                else
                    counterSave--;
            }
            else if (Bloodbender.ptr.inputHelper.IsNewKeyPress(Keys.S) || Bloodbender.ptr.inputHelper.IsNewKeyPress(Keys.Down) || Bloodbender.ptr.inputHelper.IsNewButtonPress(Buttons.DPadDown))
            {
                if (!loadClicked)
                    counterOption++;
                else
                    counterSave++;
            }
            else if (Bloodbender.ptr.inputHelper.IsNewKeyPress(Keys.Enter) || Bloodbender.ptr.inputHelper.IsNewButtonPress(Buttons.A))
            {
                if (!loadClicked)
                {
                    if (counterOption == 0)
                        Bloodbender.ptr.Exit();
                    else if (counterOption == 1)
                    {
                        bigMessage = "PAUSED";
                        bigMessageColor = Color.Black;
                        Bloodbender.ptr.reload = true;
                        loadClicked = false;
                    }
                    else if (counterOption == 2)
                    {
                        loadClicked = true;
                        counterSave = 0;
                    }
                } else
                {
                    //Debug.WriteLine(counterSave + " " + saves[counterSave]);
                    loadClicked = false;
                    Bloodbender.ptr.seedIndexToLoad = counterSave;
                    Bloodbender.ptr.reload = true;

                }
            }
            if (!loadClicked)
            {
                if (counterOption < 0)
                    counterOption = options.Count - 1;
                else if (counterOption >= options.Count)
                    counterOption = 0;
            } else {
                if (counterSave < 0)
                    counterSave = saves.Count - 1;
                else if (counterSave >= saves.Count)
                    counterSave = 0;
            }
            return showing;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (showing)
            {
                Vector2 resRelative = new Vector2(Bloodbender.ptr.resolutionIndependence.VirtualWidth / 2, Bloodbender.ptr.resolutionIndependence.VirtualHeight / 2);
                Vector2 position = new Vector2(100, 50) + Bloodbender.ptr.camera.Position - resRelative;
                spriteBatch.DrawString(spriteFont, bigMessage, position, bigMessageColor, 0, Vector2.Zero, 5, SpriteEffects.None, 1);
                //spriteBatch.DrawString(spriteFont, bigMessage, position, bigMessageColor, 0, spriteFont.MeasureString(bigMessage) / 2, 5, SpriteEffects.None, 1);
                //spriteBatch.DrawString(spriteFont, bigMessage, new Vector2(72, 72), Color.White, 0, Vector2.Zero, 7, SpriteEffects.None, 1);
                if (!loadClicked)
                {
                    position.X += 50;
                    position.Y += 125;

                    arrow.position = position;
                    arrow.position.X -= 50;
                    arrow.position.Y += (10 * (counterOption + 1)) + (counterOption * spriteFont.MeasureString(options[0]).Y * 1.9f);

                    arrow.Draw(spriteBatch);

                    foreach (string option in options)
                    {
                        spriteBatch.DrawString(spriteFont, option, position, Color.Black, 0, Vector2.Zero, 1.75f, SpriteEffects.None, 1);
                        position.Y += 50;
                    }
                } else {
                    position.X += 50;
                    position.Y += 125;

                    arrow.position = position;
                    arrow.position.X -= 50;
                    arrow.position.Y += (10 * (counterSave + 1)) + (counterSave * spriteFont.MeasureString(saves[0]).Y * 1.9f);

                    arrow.Draw(spriteBatch);

                    foreach (string save in saves)
                    {
                        spriteBatch.DrawString(spriteFont, save, position, Color.Black, 0, Vector2.Zero, 1.75f, SpriteEffects.None, 1);
                        position.Y += 50;
                    }
                }
            }
        }
    }
}
