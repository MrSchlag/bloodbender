﻿using System;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics;
using Microsoft.Xna.Framework.Input;

namespace Bloodbender
{
    public class Camera
    {
        public static GraphicsDevice _graphics;

        private const float MinZoom = 0.02f;
        private const float MaxZoom = 20f;

        private Vector2 _currentPosition;
        private float _currentRotation;
        private float _currentZoom;
        private Vector2 _maxPosition;
        private float _maxRotation;
        private Vector2 _minPosition;
        private float _minRotation;
        private bool _positionTracking;
        private bool _rotationTracking;
        private Vector2 _targetPosition;
        private float _targetRotation;
        private Body _trackingBody;
        public Vector2 _translateCenter;

        public Matrix SimProjection;
        public Matrix SimView;
        public Matrix View;

        private int scrollWheelValue = 0;
        private float zoomStep = 0.15f;

        ResolutionIndependentRenderer resolutionIndependence;
        /// <summary>
        /// The constructor for the Camera2D class.
        /// </summary>
        /// <param name="graphics"></param>
        public Camera(GraphicsDevice graphics, ResolutionIndependentRenderer resolutionIndependence)
        {
            _graphics = graphics;

            this.resolutionIndependence = resolutionIndependence;

            ResetMatrice();

            ResetCamera();
        }

        public void ResetMatrice()
        {
            SimProjection = Matrix.CreateOrthographicOffCenter(0f, ConvertUnits.ToSimUnits(_graphics.Viewport.Width), ConvertUnits.ToSimUnits(_graphics.Viewport.Height), 0f, 0f, 1f);
            SimView = Matrix.Identity;
            View = Matrix.Identity;

            _translateCenter = new Vector2(ConvertUnits.ToSimUnits(resolutionIndependence.VirtualWidth / 2f), ConvertUnits.ToSimUnits(resolutionIndependence.VirtualHeight / 2f));
        }
        /// <summary>
        /// The current position of the camera.
        /// </summary>
        public Vector2 Position
        {
            get { return ConvertUnits.ToDisplayUnits(_currentPosition); }
            set
            {
                _targetPosition = ConvertUnits.ToSimUnits(value);
                if (_minPosition != _maxPosition)
                {
                    Vector2.Clamp(ref _targetPosition, ref _minPosition, ref _maxPosition, out _targetPosition);
                }
            }
        }

        /// <summary>
        /// The furthest up, and the furthest left the camera can go.
        /// if this value equals maxPosition, then no clamping will be 
        /// applied (unless you override that function).
        /// </summary>
        public Vector2 MinPosition
        {
            get { return ConvertUnits.ToDisplayUnits(_minPosition); }
            set { _minPosition = ConvertUnits.ToSimUnits(value); }
        }

        /// <summary>
        /// the furthest down, and the furthest right the camera will go.
        /// if this value equals minPosition, then no clamping will be 
        /// applied (unless you override that function).
        /// </summary>
        public Vector2 MaxPosition
        {
            get { return ConvertUnits.ToDisplayUnits(_maxPosition); }
            set { _maxPosition = ConvertUnits.ToSimUnits(value); }
        }

        /// <summary>
        /// The current rotation of the camera in radians.
        /// </summary>
        public float Rotation
        {
            get { return _currentRotation; }
            set
            {
                _targetRotation = value % MathHelper.TwoPi;
                if (_minRotation != _maxRotation)
                {
                    _targetRotation = MathHelper.Clamp(_targetRotation, _minRotation, _maxRotation);
                }
            }
        }

        /// <summary>
        /// Gets or sets the minimum rotation in radians.
        /// </summary>
        /// <value>The min rotation.</value>
        public float MinRotation
        {
            get { return _minRotation; }
            set { _minRotation = MathHelper.Clamp(value, -MathHelper.Pi, 0f); }
        }

        /// <summary>
        /// Gets or sets the maximum rotation in radians.
        /// </summary>
        /// <value>The max rotation.</value>
        public float MaxRotation
        {
            get { return _maxRotation; }
            set { _maxRotation = MathHelper.Clamp(value, 0f, MathHelper.Pi); }
        }

        /// <summary>
        /// The current rotation of the camera in radians.
        /// </summary>
        public float Zoom
        {
            get { return _currentZoom; }
            set
            {
                _currentZoom = value;
                _currentZoom = MathHelper.Clamp(_currentZoom, MinZoom, MaxZoom);
            }
        }

        /// <summary>
        /// the body that this camera is currently tracking. 
        /// Null if not tracking any.
        /// </summary>
        public Body TrackingBody
        {
            get { return _trackingBody; }
            set
            {
                _trackingBody = value;
                if (_trackingBody != null)
                {
                    _positionTracking = true;
                }
            }
        }

        public bool EnablePositionTracking
        {
            get { return _positionTracking; }
            set
            {
                if (value && _trackingBody != null)
                {
                    _positionTracking = true;
                }
                else
                {
                    _positionTracking = false;
                }
            }
        }

        public bool EnableRotationTracking
        {
            get { return _rotationTracking; }
            set
            {
                if (value && _trackingBody != null)
                {
                    _rotationTracking = true;
                }
                else
                {
                    _rotationTracking = false;
                }
            }
        }

        public bool EnableTracking
        {
            set
            {
                EnablePositionTracking = value;
                EnableRotationTracking = value;
            }
        }

        public void MoveCamera(Vector2 amount)
        {
            _currentPosition += amount;
            if (_minPosition != _maxPosition)
            {
                Vector2.Clamp(ref _currentPosition, ref _minPosition, ref _maxPosition, out _currentPosition);
            }
            _targetPosition = _currentPosition;
            _positionTracking = false;
            _rotationTracking = false;
        }

        public void RotateCamera(float amount)
        {
            _currentRotation += amount;
            if (_minRotation != _maxRotation)
            {
                _currentRotation = MathHelper.Clamp(_currentRotation, _minRotation, _maxRotation);
            }
            _targetRotation = _currentRotation;
            _positionTracking = false;
            _rotationTracking = false;
        }

        /// <summary>
        /// Resets the camera to default values.
        /// </summary>
        public void ResetCamera()
        {
            _currentPosition = Vector2.Zero;
            _targetPosition = Vector2.Zero;
            _minPosition = Vector2.Zero;
            _maxPosition = Vector2.Zero;

            _currentRotation = 0f;
            _targetRotation = 0f;
            _minRotation = -MathHelper.Pi;
            _maxRotation = MathHelper.Pi;

            _positionTracking = false;
            _rotationTracking = false;

            _currentZoom = 1f;

            SetView();
        }

        public void Jump2Target()
        {
            _currentPosition = _targetPosition;
            _currentRotation = _targetRotation;

            SetView();
        }

        public void SetView()
        {
            Matrix matRotation = Matrix.CreateRotationZ(_currentRotation);
            Matrix matZoom = Matrix.CreateScale(_currentZoom, _currentZoom, 1.0f);
            Vector3 translateCenter = new Vector3(_translateCenter, 0f);
            Vector3 translateBody = new Vector3(-_currentPosition, 0f);

            SimView = Matrix.CreateTranslation(translateBody) * matRotation * matZoom * Matrix.CreateTranslation(translateCenter) * resolutionIndependence.GetTransformationMatrix();

            translateCenter = ConvertUnits.ToDisplayUnits(translateCenter);
            translateBody = ConvertUnits.ToDisplayUnits(translateBody);

            View = Matrix.CreateTranslation(translateBody) * matRotation * matZoom * Matrix.CreateTranslation(translateCenter) * resolutionIndependence.GetTransformationMatrix();
        }

        public Matrix GetView()
        {
            SetView();

            return View;
        }

        public Matrix GetViewWithoutZoom()
        {
            Matrix matRotation = Matrix.CreateRotationZ(_currentRotation);

            Vector3 translateCenter = new Vector3(_translateCenter, 0f);
            Vector3 translateBody = new Vector3(-_currentPosition, 0f);

            SimView = Matrix.CreateTranslation(translateBody) * matRotation * Matrix.CreateTranslation(translateCenter) * resolutionIndependence.GetTransformationMatrix();

            translateCenter = ConvertUnits.ToDisplayUnits(translateCenter);
            translateBody = ConvertUnits.ToDisplayUnits(translateBody);

            View = Matrix.CreateTranslation(translateBody) * matRotation * Matrix.CreateTranslation(translateCenter) * resolutionIndependence.GetTransformationMatrix();

            return View;
        }

        /// <summary>
        /// Moves the camera forward one timestep.
        /// </summary>
        public void Update(float elapsed)
        {
            //Console.WriteLine(_currentZoom);
            if (Bloodbender.ptr.inputHelper.IsNewMouseButtonPress(MouseButtons.MiddleButton))
                _currentZoom = 1f;

            if (scrollWheelValue > Mouse.GetState().ScrollWheelValue)
            {
                Zoom -= zoomStep;
            }
            else if (scrollWheelValue < Mouse.GetState().ScrollWheelValue)
            {
                Zoom += zoomStep;
            }

            scrollWheelValue = Mouse.GetState().ScrollWheelValue;

            if (_trackingBody != null)
            {
                if (_positionTracking)
                {
                    _targetPosition = _trackingBody.Position;
                    if (_minPosition != _maxPosition)
                    {
                        Vector2.Clamp(ref _targetPosition, ref _minPosition, ref _maxPosition, out _targetPosition);
                    }
                }
                if (_rotationTracking)
                {
                    _targetRotation = -_trackingBody.Rotation % MathHelper.TwoPi;
                    if (_minRotation != _maxRotation)
                    {
                        _targetRotation = MathHelper.Clamp(_targetRotation, _minRotation, _maxRotation);
                    }
                }
            }

            _currentPosition = _targetPosition;

            /*
            Vector2 delta = _targetPosition - _currentPosition;
            float distance = delta.Length();

            if (distance > 0f)
            {
                delta /= distance;
            }

            float inertia;

            if (distance < 10f)
            {
                inertia = (float)Math.Pow(distance / 10.0, 2.0);
            }
            else
            {
                inertia = 1f;
            }

            float rotDelta = _targetRotation - _currentRotation;

            float rotInertia;
            if (Math.Abs(rotDelta) < 5f)
            {
                rotInertia = (float)Math.Pow(rotDelta / 5.0, 2.0);
            }
            else
            {
                rotInertia = 1f;
            }
            if (Math.Abs(rotDelta) > 0f)
            {
                rotDelta /= Math.Abs(rotDelta);
            }


            // permet d'évite l'effet escalier sur les sprite, à paufiner
            /*
            Vector2 dep;

            dep.X = (int)Math.Ceiling(100f * delta.X * inertia * elapsed * Bloodbender.meterToPixel);
            dep.Y = (int)Math.Ceiling(100f * delta.Y * inertia * elapsed * Bloodbender.meterToPixel);

            Console.WriteLine(dep);

            dep /= Bloodbender.meterToPixel;

            _currentPosition += dep;
            */

            //_currentPosition += 100f * delta * inertia * elapsed;
            //_currentRotation += 80f * rotDelta * rotInertia * elapsed;

            
            SetView();
        }

        public Vector2 ConvertScreenToWorld(Vector2 location)
        {
            Vector3 t = new Vector3(location, 0);
            t = Bloodbender.ptr.graphics.GraphicsDevice.Viewport.Unproject(t, SimProjection, SimView, Matrix.Identity);

            return new Vector2(t.X, t.Y) * Bloodbender.meterToPixel;
        }

        public Vector2 ConvertWorldToScreen(Vector2 location)
        {
            Vector3 t = new Vector3(location * Bloodbender.pixelToMeter, 0);
            t = _graphics.Viewport.Project(t, SimProjection, SimView, Matrix.Identity);

            return new Vector2(t.X, t.Y);
        }

        public bool isInView(Vector2 position)
        {
            int margin = 200;

            position = ConvertWorldToScreen(position);

            if (position.X >= -margin && position.X <= Bloodbender.ptr.graphics.PreferredBackBufferWidth + margin
                &&
                position.Y >= -margin && position.Y <= Bloodbender.ptr.graphics.PreferredBackBufferHeight + margin)
                return true;

            return false;
        }
    }
}