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

        private List<TreeSeed> seeds;
        private Random rnd;

        public TreePlanter(float minX, float maxX, float minY, float maxY)
        {
            rnd = new Random();
            seeds = new List<TreeSeed>();

            _minX = minX - 5;
            _maxX = maxX + 5;
            _minY = minY - 5;
            _maxY = maxY + 5;

            _center.X = _maxX - _minX;
            _center.Y = _maxY - _minY;
            SetInitialSeedPosition(1.3f);
            //Run();
        }

        private void SetInitialSeedPosition(float step)
        {

            for (float y = _minY; y < _maxY; y += step)
            {
                for (float x = _minX; x < _maxX; x += step)
                {
                    AddTree(x + rnd.Next(-30, 30) / 10f, y + rnd.Next(-60, 60) / 10f);
                }
            }


            /*
            for (float y = _minY; y < _maxY; y += step)
            {
                var seed = new TreeSeed(new Vector2(_minX, y), new Vector2(1, 0), 0);
                seeds.Add(seed);
                seed = new TreeSeed(new Vector2(_maxX, y), new Vector2(-1, 0), 0);
                seeds.Add(seed);
            }
            
            for (float x = _minX + step; x < _maxX - step; x += step)
            {
                var seed = new TreeSeed(new Vector2(x, _minY), new Vector2(0, 1), 0);
                seeds.Add(seed);
                seed = new TreeSeed(new Vector2(x, _maxY), new Vector2(0, -1), 50);
                seeds.Add(seed);
            }*/

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

        private void Run()
        {
            foreach (var seed in seeds)
            {
                seed.Run();
            }
        }
    }
}