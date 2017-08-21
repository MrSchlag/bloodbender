using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using FarseerPhysics.DebugView;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics;
using System;
using Bloodbender.PathFinding;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Bloodbender.Scene;
using Bloodbender.Enemies.Scenario3;
using Bloodbender.Enemies.Scenario1;
using Bloodbender.ParticuleEngine;
using Bloodbender.ParticuleEngine.ParticuleSpawners;
using SharpNoise.Builders;
using SharpNoise.Utilities.Imaging;
using SharpNoise;
using SharpNoise.Modules;
using System.Drawing.Imaging;

namespace Bloodbender
{
    public class Bloodbender : Game
    {
        public static Bloodbender ptr { get; set; }

        public Random rdn;

        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        public MouseState mouse;

        public ResolutionIndependentRenderer resolutionIndependence;
        public Camera camera;

        public World world;
        public const float meterToPixel = 32;
        public const float pixelToMeter = 1 / meterToPixel;
        public DebugView debugView;


        public InputHelper inputHelper;

        public ShadowsRenderer shadowsRendering;
        public List<GraphicObj> listGraphicObj;

        public PathFinder pFinder;
        public Player player;

        public float elapsed = 0.0f;

        public Microsoft.Xna.Framework.Color BackgroundColor = Microsoft.Xna.Framework.Color.CornflowerBlue;

        public Texture2D bouleRouge;
        public Texture2D blood1;
        public Texture2D blood2;
        public Texture2D blood3;
        public Texture2D debugNodeFree;
        public Texture2D debugNodeBusy;

        private TotemScene totemscene;

        private bool WindowSizeIsBeingChanged = false;

        SpriteFont font;

        FrameRateCounter frameRateCounter;

        EventHandler<EventArgs> eventResize;

        int width = 1280, height = 720, savedWidth = 0, savedHeight = 0;


        public ParticuleSystem particuleSystem;
        public SnowMarkSpawner snowMarkSpawner;

        ImageRenderer imageRendererNoise;
        SharpNoise.Utilities.Imaging.Image textureImageNoise;
        Texture2D renderedmap;

        public MapFactory mapFactory;
        int minNoiseX;
        int maxNoiseX;
        int minNoiseY;
        int maxNoiseY;

        Menu menu;

        public bool reload = false;

        public Bloodbender()
        {
            ptr = this;

            rdn = new Random();

            mouse = Mouse.GetState();
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

            ConvertUnits.SetDisplayUnitToSimUnitRatio(meterToPixel);

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
            world = new World(new Vector2(0, 0));

            debugView = new DebugView();
            debugView.LoadContent(GraphicsDevice, Content);

            inputHelper = new InputHelper(resolutionIndependence);
            inputHelper.ShowCursor = true;

            shadowsRendering = new ShadowsRenderer();

            listGraphicObj = new List<GraphicObj>();

            inputHelper.LoadContent();

            font = Content.Load<SpriteFont>("pixelSpriteFont");

            Texture2D textureCarre = Content.Load<Texture2D>("carre");
            Texture2D textureCarre2 = Content.Load<Texture2D>("carre2");
            Texture2D textureTotem = Content.Load<Texture2D>("Totem");
            bouleRouge = Content.Load<Texture2D>("blood");
            blood1 = Content.Load<Texture2D>("blood1");
            blood2 = Content.Load<Texture2D>("blood2");
            blood3 = Content.Load<Texture2D>("blood3");
            debugNodeBusy = Content.Load<Texture2D>("debugPathFinderNode");
            debugNodeFree = Content.Load<Texture2D>("debugPathFinderNode2");

            particuleSystem = new ParticuleSystem();

            //pathFinder = new PathFinder(2);
            pFinder = new PathFinder();
            pFinder.BuildtNavMeshes(6, 10);

            mapFactory = new MapFactory();
            mapFactory.newMap(listGraphicObj);
            var treeplanter = new TreePlanter(mapFactory.minX * pixelToMeter, mapFactory.maxX * pixelToMeter, mapFactory.minY * pixelToMeter, mapFactory.maxY * pixelToMeter, mapFactory.mGen.rand);

            frameRateCounter = new FrameRateCounter(font);

            particuleSystem.addParticuleSpawner(new SnowSpawner(new Vector2(100, 100), 0, player, new Vector2(350,-350)));

            snowMarkSpawner = new SnowMarkSpawner(new Vector2(0, 0), 0, player, new Vector2(0, 0));

            var noiseSource = new Perlin
            {
                Seed = new Random().Next()
            };
            var noiseMap = new NoiseMap();
            var noiseMapBuilder = new PlaneNoiseMapBuilder
            {
                DestNoiseMap = noiseMap,
                SourceModule = noiseSource,
                EnableSeamless = true
            };

            //noiseMapBuilder.SetDestSize(300, 300);
            //noiseMapBuilder.SetBounds(-4.5, 4.5, -4.5, 4.5);
            noiseMapBuilder.SetDestSize(256, 256);
            noiseMapBuilder.SetBounds(2, 6, 1, 5);

            noiseMapBuilder.Build();

            textureImageNoise = new SharpNoise.Utilities.Imaging.Image();

            imageRendererNoise = new ImageRenderer
            {
                SourceNoiseMap = noiseMap,
                DestinationImage = textureImageNoise,
                EnableLight = true,
                LightContrast = 1.0,
                LightBrightness = 2.3,
                EnableWrap = true
            };

            imageRendererNoise.ClearGradient();

            imageRendererNoise.AddGradientPoint(0.0000, new SharpNoise.Utilities.Imaging.Color(170, 180, 240, 255));
            imageRendererNoise.AddGradientPoint(1.000, new SharpNoise.Utilities.Imaging.Color(170, 180, 240, 255));

            imageRendererNoise.Render();

            renderedmap = createTexture(imageRendererNoise);

            minNoiseX = (int)mapFactory.minX - textureImageNoise.Width * 2;
            maxNoiseX = (int)mapFactory.maxX + textureImageNoise.Width * 2;
            minNoiseY = (int)mapFactory.minY - textureImageNoise.Height * 2;
            maxNoiseY = (int)mapFactory.maxY + textureImageNoise.Height * 2;

            menu = new Menu(font);
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
            mouse = Mouse.GetState();
            elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            inputHelper.Update(elapsed);



            if (inputHelper.IsNewButtonPress(Buttons.Back) || inputHelper.IsNewKeyPress(Keys.Escape))
                menu.showing = !menu.showing ;

            if (menu.Update(elapsed))
            {
                if (reload)
                {
                    LoadContent();
                    reload = false;
                }

                return;
            }

            world.Step(1f / 30f);

            camera.Update(elapsed);

            //totemscene.Update(elapsed);

            for (int i = 0; i < listGraphicObj.Count; ++i)
            {
                listGraphicObj[i].Update(elapsed);
            }

            for (int i = 0; i < listGraphicObj.Count; ++i)
            {
                if (listGraphicObj[i].shouldDie == true)
                {

                    listGraphicObj[i].Dispose();
                    listGraphicObj.RemoveAt(i);
                    --i;
                }
            }

            particuleSystem.Update(elapsed);
            snowMarkSpawner.Update(elapsed);

            pFinder.Update(elapsed);
            shadowsRendering.Update(elapsed);

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
            if (inputHelper.IsNewKeyPress(Keys.F9))
                debugView.EnableOrDisableFlag(DebugViewFlags.PathFinding);


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

            debugView.Update(elapsed);

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

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.GetView());

            for (int i = minNoiseY; i < maxNoiseY; i += textureImageNoise.Height)
                for (int j = minNoiseX; j < maxNoiseX; j += textureImageNoise.Width)
                    spriteBatch.Draw(renderedmap, new Vector2(j, i), Microsoft.Xna.Framework.Color.White);
            
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, camera.GetView());
            snowMarkSpawner.Draw(spriteBatch);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            spriteBatch.Draw(shadowsRendering.getTarget(), Vector2.Zero, null, new Microsoft.Xna.Framework.Color(255, 255, 255, 100), 0.0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.00001f);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, camera.GetView());
            foreach (GraphicObj obj in listGraphicObj)
                obj.Draw(spriteBatch);
            particuleSystem.Draw(spriteBatch);
            spriteBatch.End();


            debugView.RenderDebugData(ref camera.SimProjection, ref camera.SimView);

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.GetViewWithoutZoom());
            inputHelper.Draw();
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.GetViewWithoutZoom());
            //spriteBatch.Begin(samplerState:SamplerState.PointClamp);
            menu.Draw(spriteBatch);
            spriteBatch.End();

            resolutionIndependence.SetupFullViewport();

            frameRateCounter.Draw(spriteBatch);

            base.Draw(gameTime);
        }

        public Vector2 getMousePosition()
        {
            return new Vector2(mouse.X, mouse.Y);
        }

        private Texture2D createTexture(ImageRenderer Renderer)
        {
            Texture2D texture = new Texture2D(GraphicsDevice, Renderer.DestinationImage.Width, Renderer.DestinationImage.Height);
            Microsoft.Xna.Framework.Color[] imageColorData = new Microsoft.Xna.Framework.Color[Renderer.DestinationImage.Width * Renderer.DestinationImage.Height];

            for (int i = 0; i < imageColorData.Length; ++i)
                imageColorData[i] = new Microsoft.Xna.Framework.Color(Renderer.DestinationImage.Data[i].Red, Renderer.DestinationImage.Data[i].Green, Renderer.DestinationImage.Data[i].Blue);

            texture.SetData(imageColorData);

            return texture;
        }
    }
}
