using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using FarseerPhysics.DebugView;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics;
using System;

namespace Bloodbender
{
    public class Bloodbender : Game
    {
        public static Bloodbender ptr { get; set; }

        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;

        public ResolutionIndependentRenderer resolutionIndependence;
        public Camera camera;

        public World world;
        public const float meterToPixel = 32;
        public const float pixelToMeter = 1 / meterToPixel;
        public DebugViewXNA debugView;


        public InputHelper inputHelper;

        public ShadowsRendering shadowsRendering;
        public List<GraphicObj> listGraphicObj;

        public float elapsed = 0.0f;

        public Color BackgroundColor = Color.CornflowerBlue;

        public Texture2D bouleRouge;

        private bool WindowSizeIsBeingChanged = false;

        SpriteFont font;

        FrameRateCounter frameRateCounter;

        EventHandler<EventArgs> eventResize;

        int width = 1280, height = 720, savedWidth = 0, savedHeight = 0;

        public Bloodbender()
        {
            ptr = this;

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

          
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;

            //graphics.HardwareModeSwitch = false;

            //graphics.SynchronizeWithVerticalRetrace = false;
            //IsFixedTimeStep = false;

            //graphics.IsFullScreen = true;


            resolutionIndependence = new ResolutionIndependentRenderer(this);            
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

            ConvertUnits.SetDisplayUnitToSimUnitRatio(meterToPixel);

            debugView = new DebugViewXNA(world);
            debugView.RemoveFlags(DebugViewFlags.Shape);
            debugView.RemoveFlags(DebugViewFlags.Joint);
            debugView.DefaultShapeColor = Color.White;
            debugView.SleepingShapeColor = Color.LightGray;
            debugView.LoadContent(GraphicsDevice, Content);

            inputHelper = new InputHelper(resolutionIndependence);
            inputHelper.ShowCursor = true;

            shadowsRendering = new ShadowsRendering();

            listGraphicObj = new List<GraphicObj>();

            InitializeResolutionIndependence(graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);

            camera = new Camera(GraphicsDevice, resolutionIndependence);

            spriteBatch = new SpriteBatch(GraphicsDevice);

            this.Window.AllowUserResizing = true;
            eventResize = new EventHandler<EventArgs>(Window_ClientSizeChanged);
            this.Window.ClientSizeChanged += eventResize;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            inputHelper.LoadContent();

            font = Content.Load<SpriteFont>("pixelSpriteFont");

            Texture2D textureCarre = Content.Load<Texture2D>("carre");
            Texture2D textureCarre2 = Content.Load<Texture2D>("carre2");
            Texture2D textureTotem = Content.Load<Texture2D>("Totem");
            bouleRouge = Content.Load<Texture2D>("bouleRouge");

            
            MapBound mapBounds = new MapBound();
            mapBounds.addVertex(new Vector2(0, 0));
            mapBounds.addVertex(new Vector2(120, -50));
            mapBounds.addVertex(new Vector2(2000, 0));
            mapBounds.addVertex(new Vector2(1900, 500));
            mapBounds.addVertex(new Vector2(0, 500));
            mapBounds.finiliezMap();
            

            PhysicObj pobj = new PhysicObj(BodyFactory.CreateRectangle(world, 32 * pixelToMeter, 32 * pixelToMeter, 1), // meterTopixel a la place de 32?
                new Vector2(1280, 200));
            shadowsRendering.addShadow(new Shadow(pobj));
            pobj.addAnimation(new Animation(textureCarre2));
            pobj.isRotationFixed(true);

            Player player = new Player(new Vector2(100, 100));
            player.addAnimation(new Animation(textureCarre));
            player.setLinearDamping(10);

            Totem totem = new Totem(new Vector2(300, 300));
            totem.addAnimation(new Animation(textureTotem));

            listGraphicObj.Add(totem);
            listGraphicObj.Add(player);
            listGraphicObj.Add(pobj);

            frameRateCounter = new FrameRateCounter(font);
        }

        
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private void InitializeResolutionIndependence(int realScreenWidth, int realScreenHeight)
        {
            resolutionIndependence.ScreenWidth = realScreenWidth;
            resolutionIndependence.ScreenHeight = realScreenHeight;
            resolutionIndependence.Initialize();
        }
        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            WindowSizeIsBeingChanged = !WindowSizeIsBeingChanged;
            if (WindowSizeIsBeingChanged && Window.ClientBounds.Width > 0 && Window.ClientBounds.Height > 0)
            {
                graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
                graphics.ApplyChanges();

                setAllOnResize();
            }
        }

        void setAllOnResize()
        {
            InitializeResolutionIndependence(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            camera.ResetMatrice();

            inputHelper._viewport = graphics.GraphicsDevice.Viewport;

            shadowsRendering.targetShadows = new RenderTarget2D(graphics.GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
        }
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            inputHelper.Update(elapsed);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            world.Step(1f / 30f);


            for (int i = 0; i < listGraphicObj.Count; ++i)
                listGraphicObj[i].Update(elapsed);

            shadowsRendering.Update(elapsed);

            camera.Update(elapsed);


            if (inputHelper.IsNewKeyPress(Keys.F1))
                debugView.EnableOrDisableFlag(DebugViewFlags.Shape);
            if (inputHelper.IsNewKeyPress(Keys.F2))
            {
                debugView.EnableOrDisableFlag(DebugViewFlags.DebugPanel);
                debugView.EnableOrDisableFlag(DebugViewFlags.PerformanceGraph);
            }
            if (inputHelper.IsNewKeyPress(Keys.F3))
                debugView.EnableOrDisableFlag(DebugViewFlags.Joint);
            if (inputHelper.IsNewKeyPress(Keys.F4))
            {
                debugView.EnableOrDisableFlag(DebugViewFlags.ContactPoints);
                debugView.EnableOrDisableFlag(DebugViewFlags.ContactNormals);
            }
            if (inputHelper.IsNewKeyPress(Keys.F5))
                debugView.EnableOrDisableFlag(DebugViewFlags.PolygonPoints);
            if (inputHelper.IsNewKeyPress(Keys.F6))
                debugView.EnableOrDisableFlag(DebugViewFlags.Controllers);
            if (inputHelper.IsNewKeyPress(Keys.F7))
                debugView.EnableOrDisableFlag(DebugViewFlags.CenterOfMass);
            if (inputHelper.IsNewKeyPress(Keys.F8))
                debugView.EnableOrDisableFlag(DebugViewFlags.AABB);

            
            if (inputHelper.IsNewKeyPress(Keys.F12))
            {
                this.Window.ClientSizeChanged -= eventResize;

                if (!graphics.IsFullScreen)
                {
                    savedWidth = graphics.PreferredBackBufferWidth;
                    savedHeight = graphics.PreferredBackBufferHeight;

                    graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                    graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                }
                else
                {
                    graphics.PreferredBackBufferWidth = savedWidth;
                    graphics.PreferredBackBufferHeight = savedHeight;
                }

                graphics.ToggleFullScreen();

                setAllOnResize();

                this.Window.ClientSizeChanged += eventResize;
            }

            frameRateCounter.Update(elapsed);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            shadowsRendering.renderShadowsOnTarget();

            resolutionIndependence.BeginDraw();
            //camera.SetView();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            spriteBatch.Draw(shadowsRendering.getTarget(), Vector2.Zero, null, new Color(255, 255, 255, 100), 0.0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.00001f);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.View);

            foreach (GraphicObj obj in listGraphicObj)
                obj.Draw(spriteBatch);

            inputHelper.Draw();

            spriteBatch.End();


            debugView.RenderDebugData(ref camera.SimProjection, ref camera.SimView);


            resolutionIndependence.SetupFullViewport();
            frameRateCounter.Draw(spriteBatch);

            base.Draw(gameTime);
        }
    }
}
