using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics;

namespace Bloodbender
{
    public class Bloodbender : Game
    {
        public static Bloodbender ptr { get; set; }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Camera camera;

        public World world;

        public float elapsed = 0.0f;

        public List<GraphicObj> listGraphicObj;
        //RenderTarget2D targetShadows;

        public float meterToPixel;
        public float pixelToMeter;


        public Bloodbender()
        {
            ptr = this;

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            listGraphicObj = new List<GraphicObj>();

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            world = new World(new Vector2(0, 0));
            meterToPixel = 32;
            pixelToMeter = 1 / meterToPixel;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.

            camera = new Camera();
            camera.zoom = new Vector2((float)GraphicsDevice.Viewport.Width / camera.width, (float)GraphicsDevice.Viewport.Height / camera.height);

            spriteBatch = new SpriteBatch(GraphicsDevice);

            Texture2D textureCarre = Content.Load<Texture2D>("carre");
            /*
            GraphicObj gobj = new GraphicObj();
            gobj.animations[0] = new Animation(textureCarre);
            */
            PhysicObj pobj = new PhysicObj(BodyFactory.CreateRectangle(world, 32 * pixelToMeter, 32 * pixelToMeter, 1),
                new Vector2(0 * pixelToMeter, 0 * pixelToMeter));
            pobj.animations[0] = new Animation(textureCarre);
            pobj.animations[0].origin = new Vector2(16, 16);
            pobj.canRotate(false);
            pobj.setLinearDamping(0);

            Player player = new Player(new Vector2(100, 100));
            player.animations[0] = new Animation(textureCarre);
            player.animations[0].origin = new Vector2(16, 16);
            player.setLinearDamping(10);

            camera.requestFocus(player);

            listGraphicObj.Add(player);
            listGraphicObj.Add(pobj);
            //listGraphicObj.Add(gobj);
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            foreach (GraphicObj obj in listGraphicObj)
                obj.Update(elapsed);

            world.Step(elapsed);
            // TODO: Add your update logic here
            camera.Update();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.get_transformation(GraphicsDevice));

            foreach (GraphicObj obj in listGraphicObj)
                obj.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
