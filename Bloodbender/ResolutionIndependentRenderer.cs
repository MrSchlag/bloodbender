using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodbender
{
    public class ResolutionIndependentRenderer
    {
        private readonly Game _game;
        private Viewport _viewport;
        private float _ratioX;
        private float _ratioY;
        private Vector2 _virtualMousePosition = new Vector2();

        public Color BackgroundColor = Color.CornflowerBlue;

        private double _scale;

        public ResolutionIndependentRenderer(Game game)
        {
            _game = game;
            VirtualWidth = 1280;
            VirtualHeight = 720;

            ScreenWidth = 1280;
            ScreenHeight = 720;
        }

        public int VirtualHeight;

        public int VirtualWidth;

        public int ScreenWidth;
        public int ScreenHeight;

        public void Initialize()
        {
            SetupVirtualScreenViewport();

            _ratioX = (float)_viewport.Width / VirtualWidth;
            _ratioY = (float)_viewport.Height / VirtualHeight;

            _dirtyMatrix = true;
        }

        public void SetupFullViewport()
        {
            Viewport vp = new Viewport();
            vp.X = vp.Y = 0;
            vp.Width = ScreenWidth;
            vp.Height = ScreenHeight;
            vp.MaxDepth = 1.0f;
            _game.GraphicsDevice.Viewport = vp;
            _dirtyMatrix = true;
        }

        public void BeginDraw()
        {
            SetupFullViewport();

            _game.GraphicsDevice.Clear(BackgroundColor);

            SetupVirtualScreenViewport();
        }

        public bool RenderingToScreenIsFinished;
        private static Matrix _scaleMatrix;
        private bool _dirtyMatrix = true;

        public Matrix GetTransformationMatrix()
        {
            if (_dirtyMatrix)
                RecreateScaleMatrix();

            return _scaleMatrix;
        }

        private void RecreateScaleMatrix()
        {
            Matrix.CreateScale((float)_scale, (float)_scale, 1f, out _scaleMatrix);

            _dirtyMatrix = false;
        }

        public Vector2 ScaleMouseToScreenCoordinates(Vector2 screenPosition)
        {
            float realX = screenPosition.X - _viewport.X;
            float realY = screenPosition.Y - _viewport.Y;

            _virtualMousePosition.X = realX;// / _ratioX;
            _virtualMousePosition.Y = realY;// / _ratioY;

            return _virtualMousePosition;
        }

        public void SetupVirtualScreenViewport()
        {
            int height, width;
            double widthScale = 0, heightScale = 0;
            widthScale = (double)ScreenWidth / (double)VirtualWidth;
            heightScale = (double)ScreenHeight / (double)VirtualHeight;

            _scale = Math.Min(widthScale, heightScale);

            width = (int)(VirtualWidth * _scale);
            height = (int)(VirtualHeight * _scale);

            _viewport = new Viewport
            {
                X = (ScreenWidth - width) / 2,
                Y = (ScreenHeight - height) / 2,
                Width = width,
                Height = height,
            };

            _viewport.MaxDepth = 1.0f;

            _game.GraphicsDevice.Viewport = _viewport;
        }
    }
}
