using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using static Bloodbender.GraphicObj;

namespace Bloodbender
{
    public class TreePlanter
    {
        private float _minX;
        private float _maxX;
        private float _minY;
        private float _maxY;

        private Vector2 _center;

        private Random rnd;

        public TreePlanter(float minX, float maxX, float minY, float maxY)
        {
            rnd = new Random();

            _minX = minX - 5;
            _maxX = maxX + 5;
            _minY = minY - 5;
            _maxY = maxY + 5;

            _center.X = _maxX - _minX;
            _center.Y = _maxY - _minY;
            SetInitialSeedPosition(2f);
        }

        private void SetInitialSeedPosition(float step)
        {
            for (float y = _minY; y < _maxY; y += step)
            {
                for (float x = _minX; x < _maxX; x += step)
                {
                    AddTree(x + rnd.Next(-100, 100) / 10f, y + rnd.Next(-100, 100) / 10f);
                }
            }
        }

        private void AddTree(float x, float y)
        {
            if (IsPointOutside(x, y))
            {
                var tree = new GraphicObj(OffSet.BottomCenterHorizontal);
                tree.position = new Vector2(x, y) * Bloodbender.meterToPixel;
                tree.addAnimation(new Animation(Bloodbender.ptr.Content.Load<Texture2D>("tree1")));
                Bloodbender.ptr.listGraphicObj.Add(tree);
            }
        }

        private bool IsPointOutside(float x, float y)
        {
            var pt1 = new Vector2(x, y);
            int intersectionCount = 0;
            Bloodbender.ptr.world.RayCast((fixture, point, normal, fraction) =>
            {
                intersectionCount++;
                return -1;
            }, pt1, new Vector2(500, 0));

            if (intersectionCount % 2 == 0)
                return true;
            return false;
        }

    }
}